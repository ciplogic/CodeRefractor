#region Usings

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public static class CommonExtensions
    {
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

        public static T GetCustomAttribute<T>(this MethodInfo method, bool inherited = false)
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

        public static int Int(this string value)
        {
            return Int32.Parse(value);
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


        public static string GetMethodDescriptor(this MethodBase method)
        {
            var declaringType = method.DeclaringType;
            var mappedtypeAttr = declaringType.GetCustomAttribute<MapTypeAttribute>();
            var mappedType2 = mappedtypeAttr == null ? null : mappedtypeAttr.MappedType;
            var format = String.Format("{0}.{1}({2})",
                mappedType2 == null ? declaringType.FullName : mappedType2.FullName, method.Name,
                method.GetArgumentsAsText());
            return format;
        }

        public static string GetArgumentsAsText(this MethodBase method)
        {
            var parameterInfos = method.GetParameters();
            var arguments = String.Join(", ",
                parameterInfos.Select(
                    param => String.Format("{0} {1}", param.ParameterType.ToCppMangling(), param.Name)));
            if (!method.IsStatic)
            {
                var thisText = String.Format("const {0}& _this", method.DeclaringType.ToCppName());
                return parameterInfos.Length == 0
                    ? thisText
                    : String.Format("{0}, {1}", thisText, arguments);
            }
            return arguments;
        }

        public static string ClangMethodSignature(this MethodBase method, Type mappedType = null)
        {
            var declaringType = mappedType ?? method.DeclaringType;
            var typeNamespace = (declaringType.Namespace ?? string.Empty).Replace('.', '_');
            var typeName = declaringType.Name;
            var methodName = method.Name;
            if (method is ConstructorInfo)
                methodName = declaringType.Name + "_ctor";
            return String.Format("{0}_{1}__{2}", typeNamespace, typeName, methodName);
        }

        public static bool IsBranchOperation(this LocalOperation operation, bool andLabels = true)
        {
            if (andLabels && operation.Kind == LocalOperation.Kinds.Label)
                return true;
            return operation.Kind == LocalOperation.Kinds.AlwaysBranch ||
                   operation.Kind == LocalOperation.Kinds.BranchOperator;
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
            int result;
            Int32.TryParse(value, out result);
            return result;
        }

        public static FieldInfo LocateField(this Type type, string fieldName, bool isStatic = false)
        {
            var result = type.GetField(fieldName);
            if (result != null)
                return result;
            var fields = type.GetFields(BindingFlags.Instance |
                                        BindingFlags.NonPublic |
                                        BindingFlags.FlattenHierarchy |
                                        BindingFlags.Public);
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

        public static string ExecuteCommand(this string pathToGpp, string arguments = "")
        {
            var p = new Process
            {
                StartInfo =
                {
                    FileName = pathToGpp,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            p.Start();
            p.WaitForExit();

            var standardOutput = p.StandardOutput.ReadToEnd();
            var standardError = p.StandardError.ReadToEnd();
            return String.IsNullOrWhiteSpace(standardOutput)
                ? standardError
                : String.IsNullOrWhiteSpace(standardError)
                    ? standardOutput
                    : standardOutput + Environment.NewLine + standardError;
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

        public static string ToCppMangling(this Type type)
        {
            return IsVoid(type)
                ? "void"
                : type.FullName.ToCppMangling();
        }

        public static string ToCppMangling(this string s)
        {
            var fullName = s.Replace(".", "::");
            if (fullName.EndsWith("[]"))
            {
                fullName = fullName.Remove(fullName.Length - 2, 2);
                fullName = String.Format("std::shared_ptr <Array<{0} > > &", fullName);
            }
            return fullName;
        }

        public static string ToCppName(this Type type, bool isSmartPtr = true)
        {
            if (!type.IsClass || !isSmartPtr)
                return type.FullName.ToCppMangling();
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var fullTypeName = elementType.ToCppName();
                return String.Format("std::shared_ptr< Array < {0} > >", fullTypeName);
            }
            return String.Format("std::shared_ptr<{0}>", type.FullName.ToCppMangling());
        }

        public static bool IsVoid(this Type type)
        {
            return type == typeof (void);
        }

        public static void ToFile(this string text, string fileName)
        {
            File.WriteAllText(fileName, text);
        }

        public static void ToFile(this StringBuilder text, string fileName)
        {
            File.WriteAllText(fileName, text.ToString());
        }
    }
}