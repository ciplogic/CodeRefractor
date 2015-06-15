#region Uses

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor
{
    public static class CommonExtensions
    {
        public static IEnumerable<Type> GetObjectsByInterface(this Assembly assembly, Type T)
        {
            if (!T.IsInterface)
                throw new ArgumentException("T must be an interface");

            return assembly.GetTypes()
                .Where(x => x.GetInterface(T.Name) != null)
                .Select(x => x)
                .ToArray();
        }

        public static TypeCode ExtractTypeCode(this Type fieldInfo)
        {
            return Type.GetTypeCode(fieldInfo);
        }

        public static string GetFullNameOfFile(this string fileName)
        {
            var info = new FileInfo(fileName);
            var fullName = info.FullName;
            return fullName;
        }

        public static void AddRange<T>(this HashSet<T> collection, IEnumerable<T> toAdd)
        {
            foreach (var item in toAdd)
            {
                collection.Add(item);
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            var result = new HashSet<T>();
            foreach (var item in items)
            {
                result.Add(item);
            }
            return result;
        }

        public static void AddRange<T>(this SortedSet<T> collection, IEnumerable<T> toAdd)
        {
            foreach (var item in toAdd)
            {
                collection.Add(item);
            }
        }

        public static T GetCustomAttributeT<T>(this MethodInfo method, bool inherited = false)
        {
            var attributes = method.GetCustomAttributes(inherited);
            if (attributes.Length == 0)
                return default(T);
            foreach (var attribute in attributes)
            {
                if (attribute is T)
                    return (T) attribute;
            }
            return default(T);
        }

        public static TypeCode ComputeTypeCode(this object instance)
        {
            var type = instance.GetType();
            return Type.GetTypeCode(type);
        }

        public static int Int(this string value)
        {
            return int.Parse(value);
        }

        public static string Str(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static byte[] ToByteArray(this object value, int maxLength = 0)
        {
            var rawsize = Marshal.SizeOf(value);

            maxLength = maxLength == 0 ? rawsize : maxLength;
            var rawdata = new byte[rawsize];
            var handle =
                GCHandle.Alloc(rawdata,
                    GCHandleType.Pinned);
            Marshal.StructureToPtr(value,
                handle.AddrOfPinnedObject(),
                false);
            handle.Free();
            if (maxLength >= rawdata.Length)
                return rawdata;
            var temp = new byte[maxLength];
            Array.Copy(rawdata, temp, maxLength);
            return temp;
        }

        public static int CorrectedSizeOf(this Type type)
        {
            if (type == typeof (char))
                return sizeof (char);
            if (type.IsEnum)
                return Marshal.SizeOf(Enum.GetUnderlyingType(type));
            return Marshal.SizeOf(type);
        }

        public static T[] FromByteArray<T>(this byte[] rawValue) where T : struct
        {
            var type = typeof (T);
            var sizeOfT = type.CorrectedSizeOf();
            var result = new T[rawValue.Length/sizeOfT];
            if (type == typeof (char))
            {
                var sourceArray = Encoding.Unicode.GetString(rawValue).ToCharArray();

                Array.Copy(sourceArray, 0, result, 0, result.Length);
                return result;
            }
            Array.Copy(rawValue, 0, result, 0, result.Length);
            return result;
        }

        public static Type GetMappedType(this Type type, ClosureEntities crRuntime)
        {
            var mappedType = crRuntime.ResolveType(type);
            return mappedType ?? type;
        }

        public static Type GetReversedMappedType(this Type type, ClosureEntities crRuntime)
        {
            foreach (var mappedType in crRuntime.MappedTypes)
            {
                if (mappedType.Value == type)
                    return mappedType.Key;
            }
            return type;
        }

        public static IEnumerable<string> GetParamAsPrettyList(ParameterInfo[] parameterInfos, bool pinvoke = false)
        {
            return parameterInfos.Select(
                param =>
                    $"{param.ParameterType.ToCppName(isPInvoke: pinvoke)} {param.Name}");
        }

        public static bool IsBranchOperation(this LocalOperation operation, bool andLabels = true)
        {
            if (andLabels && operation.Kind == OperationKind.Label)
                return true;
            return operation.Kind == OperationKind.AlwaysBranch ||
                   operation.Kind == OperationKind.BranchOperator;
        }

        public static Type GetReturnType(this MethodBase methodBase)
        {
            var method = methodBase as MethodInfo;
            if (method == null)
                return typeof (void);
            return method.ReturnType;
        }

        public static bool GetIsStatic(this MethodBase methodBase)
        {
            var method = methodBase as MethodInfo;
            if (method == null)
                return false; //if constructor
            return method.IsStatic;
        }

        public static int ToInt(this string value)
        {
            if (value.ToLower() == "m1") //
                return -1;
            int result;
            if (!int.TryParse(value, out result))
                throw new InvalidDataException("Integer not well formatted: " + value);
            return result;
        }

        public static long ToLong(this string value)
        {
            long result;
            long.TryParse(value, out result);
            return result;
        }

        public static FieldInfo LocateField(this Type type, string fieldName, bool isStatic = false)
        {
            var result = type.GetField(fieldName);
            if (result != null)
                return result;
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Public;
            if (!isStatic)
                bindingFlags |= BindingFlags.Instance;
            else
                bindingFlags |= BindingFlags.Static;
            var fields = type.GetFields(bindingFlags);
            result = fields.FirstOrDefault(field => field.Name == fieldName);
            return result;
        }

        public static string ToEscapedString(this string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        public static string ExecuteCommand(this string pathToExe, string arguments = "", string workingDirectory = "")
        {
            if (workingDirectory == "")
            {
                workingDirectory = Path.GetDirectoryName(pathToExe);
            }

            var process = new Process
            {
                StartInfo =
                {
                    FileName = pathToExe,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden | ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                }
            };

            var output = new StringBuilder();
            var error = new StringBuilder();

            using (var outputWaitHandle = new AutoResetEvent(false))
            using (var errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };


                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (process.WaitForExit(20000) &&
                    outputWaitHandle.WaitOne(20000) &&
                    errorWaitHandle.WaitOne(20000))
                {
                    // Process completed. Check process.ExitCode here.
                    var standardOutput = output.ToString();
                    var standardError = error.ToString();
                    return string.IsNullOrWhiteSpace(standardOutput)
                        ? standardError
                        : string.IsNullOrWhiteSpace(standardError)
                            ? standardOutput
                            : standardOutput + Environment.NewLine + standardError;
                }
                // Timed out.
                return "Process terminated immaturely";
            }
        }

        public static void DeleteFile(this string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch (Exception)
            {
            }
        }

        public static void ToFile(this string text, string fileName)
        {
            File.WriteAllText(fileName, text);
        }

        public static void ToFile(this StringBuilder text, string fileName)
        {
            File.WriteAllText(fileName, text.ToString());
        }

        public static void ToFile(this CodeOutput text, string fileName)
        {
            File.WriteAllText(fileName, text.ToString());
        }
    }
}