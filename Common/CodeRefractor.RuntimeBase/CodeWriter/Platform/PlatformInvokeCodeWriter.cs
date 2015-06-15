#region Uses

using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.CodeWriter.Types;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.Platform
{
    public static class PlatformInvokeCodeWriter
    {
        static string Import(string dll, string method, CallingConvention? callingConvention, string entryPoint)
        {
            LinkingData.LibraryMethodCount++;
            var id = LinkingData.LibraryMethodCount;
            var findItem = LinkingData.Libraries.FirstOrDefault(lib => lib.DllName == dll);
            if (findItem == null)
            {
                findItem = new PlatformInvokeDllImports(dll);
                LinkingData.Libraries.Add(findItem);
            }

            var dllId = new PlatformInvokeDllMethod(method, callingConvention, entryPoint)
            {
                Id = id
            };

            if (!findItem.Methods.ContainsKey(method))
                findItem.Methods.Add(method, dllId);
            return dllId.FormattedName();
        }

        public static string LoadDllMethods()
        {
            var sb = new CodeOutput();

            sb.BlankLine()
                .Append("System_Void mapLibs()")
                .BracketOpen();

            var pos = 0;
            foreach (var library in LinkingData.Libraries)
            {
                sb.Append("//---------------------------------------------------------\n")
                    .AppendFormat("// {0} methods\n", library.DllName)
                    .Append("//---------------------------------------------------------\n")
                    .AppendFormat("auto lib_{0} = LoadNativeLibrary(L\"{1}\");\n", pos, library.DllName)
                    .BlankLine();
                foreach (var method in library.Methods.Values)
                {
                    sb.AppendFormat("{0} = ({0}_type)LoadNativeMethod(lib_{2}, \"{1}\");\n", method.FormattedName(),
                        method.EntryPoint, pos);
                }
                pos++;
            }

            return sb.BracketClose()
                .BlankLine()
                .ToString();
        }

        static string WritePInvokeDefinition(this MethodInterpreter methodBase, string methodDll)
        {
            var platformInterpreter = (PlatformInvokeMethod)methodBase;
            var retType = platformInterpreter.Method.GetReturnType().ToCppMangling();
            var sb = new StringBuilder();
            var arguments = methodBase.Method.GetArgumentsAsText(true);
            var callConvention = platformInterpreter.CallingConvention;
            var callConventionStr = string.Empty;
            switch (callConvention)
            {
                case CallingConvention.StdCall:
                    callConventionStr = "__stdcall";
                    break;
                case CallingConvention.Cdecl:
                    callConventionStr = "__cdecl";
                    break;
            }

            sb.AppendFormat("typedef {0} ({3} *{1}_type)({2})",
                retType,
                methodDll,
                arguments,
                callConventionStr);

            sb.AppendLine(";");
            sb.AppendFormat("{0}_type {0};", methodDll);
            sb.AppendLine();
            return sb.ToString();
        }

        public static string WriteDelegateCallCode(this MethodInterpreter delegateInvoke)
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }

        public static string WritePlatformInvokeMethod(this PlatformInvokeMethod platformInvoke,
            ClosureEntities crRuntime)
        {
            var methodId = Import(platformInvoke.LibraryName,
                platformInvoke.MethodName,
                platformInvoke.CallingConvention,
                platformInvoke.EntryPoint);

            var codeOutput = new CodeOutput();

            codeOutput.AppendFormat(platformInvoke.WritePInvokeDefinition(methodId));
            codeOutput.BlankLine();
            codeOutput.Append(platformInvoke.Method.WriteHeaderMethod(crRuntime, false));

            // write PInvoke implementation
            codeOutput.BracketOpen();

            var argumentsCall = platformInvoke.Method.GetParameters()
                .Select(CallMarshallerFactory.CreateMarshaller)
                .Each(marshaller => { codeOutput.Append(marshaller.GetTransformationCode()); })
                .Once(marshallers => { codeOutput.BlankLine(); })
                .Select(p => p.GetParameterString())
                .Join(", ");

            if (!platformInvoke.Method.GetReturnType().IsVoid())
            {
                codeOutput.Append("return ");
            }
            codeOutput.AppendFormat("{0}({1});", methodId, argumentsCall);
            codeOutput.BracketClose();

            return codeOutput.ToString();
        }
    }
}