#region Usings

using System;
using System.Collections.Generic;
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
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.Platform
{
    public static class PlatformInvokeCodeWriter
    {
        private static string Import(string dll, string method, CallingConvention? callingConvention, string entryPoint)
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
            var sb = new StringBuilder();

            sb.AppendLine("System_Void mapLibs() {");
            var pos = 0;
            foreach (var library in LinkingData.Libraries)
            {
                sb.AppendFormat("auto lib_{0} = LoadNativeLibrary(L\"{1}\");", pos, library.DllName);
                sb.AppendLine();
                foreach (var method in library.Methods.Values)
                {
                    sb.AppendFormat("{0} = ({0}_type)LoadNativeMethod(lib_{2}, \"{1}\");", method.FormattedName(),
                        method.EntryPoint, pos);
                    sb.AppendLine();
                }
                pos++;
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string WritePInvokeDefinition(this MethodInterpreter methodBase, string methodDll)
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

        public static string WritePlatformInvokeMethod(this PlatformInvokeMethod platformInvoke, ClosureEntities crRuntime)
        {
            var methodId = Import(platformInvoke.LibraryName,
                platformInvoke.MethodName,
                platformInvoke.CallingConvention,
                platformInvoke.EntryPoint);

            CodeOutput codeOutput = new CodeOutput();

            codeOutput.AppendFormat(platformInvoke.WritePInvokeDefinition(methodId));
            codeOutput.BlankLine();
            codeOutput.Append(platformInvoke.Method.WriteHeaderMethod(crRuntime, writeEndColon: false));

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