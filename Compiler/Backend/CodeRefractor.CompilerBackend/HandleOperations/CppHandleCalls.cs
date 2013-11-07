#region Usings

using System;
using System.Linq;
using System.Text;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.HandleOperations
{
    internal static class CppHandleCalls
    {
        public static void HandleReturn(LocalOperation operation, StringBuilder bodySb)
        {
            var returnValue = operation.Value as IdentifierValue;

            if (returnValue == null)
                bodySb.Append("return;");
            else
                bodySb.AppendFormat("return {0};", returnValue.Name);
        }

        public static void HandleCall(LocalOperation operation, StringBuilder sb)
        {
            var operationData = (MethodData) operation.Value;

            var methodInfo = operationData.Info;
            if (methodInfo.IsConstructor)
            {
                var ctorInterpreter = methodInfo.GetInterpreter();
                if (ctorInterpreter == null)
                    return;
            }
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (isVoidMethod)
            {
                sb.AppendFormat("{0}", methodInfo.ClangMethodSignature());
            }
            else
            {
                sb.AppendFormat("{1} = {0}", methodInfo.ClangMethodSignature(),
                                operationData.Result.Name);
            }
            var identifierValues = operationData.Parameters;

            var escapingData = CppFullFileMethodWriter.BuildEscapingBools(methodInfo, methodInfo.GetParameters());
            if (escapingData == null)
            {
                var argumentsCall = String.Join(", ", identifierValues.Select(p =>
                    {
                        var computeValue = p.ComputedValue();
                        return computeValue;
                    }));

                sb.AppendFormat("({0});", argumentsCall);
                return;
            }
            sb.Append("(");

            var pos = 0;
            bool isFirst = true;
            var arguments = operationData.Info.GetParameters();
            foreach (var value in identifierValues)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(", ");
                var localValue = value as LocalVariable;
                var argumentData = arguments[pos];
                bool isEscaping = escapingData[pos];
                pos++;
                if(localValue==null)
                {
                    sb.Append(value.ComputedValue());
                    continue;
                }

                if (localValue.ComputedType() == typeof (IntPtr))
                {
                    var argumentTypeCast = argumentData.ParameterType.ToCppMangling();
                    sb.AppendFormat("({0}){1}", argumentTypeCast, localValue.Name);
                    continue;
                }

                switch (localValue.NonEscaping)
                {
                    case NonEscapingMode.Smart:
                        if (!isEscaping && localValue.ComputedType().IsClass)
                            sb.AppendFormat("{0}.get()", localValue.Name);
                        else
                            sb.Append(localValue.Name);
                        continue;
                    case NonEscapingMode.Stack:
                        sb.AppendFormat("&{0}", localValue.Name);
                        continue;

                    case NonEscapingMode.Pointer:
                        sb.AppendFormat("{0}.get()", localValue.Name);
                        continue;
                }
            }
            sb.Append(");");
        }

        public static void HandleCallRuntime(LocalOperation operation, StringBuilder sb)
        {
            var operationData = (MethodData) operation.Value;

            var methodInfo = operationData.Info;
            if (methodInfo.IsConstructor)
                return; //don't call constructor for now
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (isVoidMethod)
            {
                sb.AppendFormat("{0}", methodInfo.ClangMethodSignature());
            }
            else
            {
                sb.AppendFormat("{1} = {0}", methodInfo.ClangMethodSignature(),
                                operationData.Result.Name);
            }
            var identifierValues = operationData.Parameters;
            var argumentsCall = String.Join(", ", identifierValues.Select(p => p.Name));

            sb.AppendFormat("({0});", argumentsCall);
        }
    }
}