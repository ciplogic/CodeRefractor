#region Usings

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Runtime;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
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


        public static void HandleCall(LocalOperation operation, StringBuilder sbCode, MidRepresentationVariables vars,
            MethodInterpreter interpreter, CrRuntimeLibrary crRuntime)
        {
            var operationData = (MethodData) operation.Value;
            var sb = new StringBuilder();
            var methodInfo = operationData.Info.GetReversedMethod();
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (!isVoidMethod && operationData.Result != null)
            {
                sb.AppendFormat("{0} = ", operationData.Result.Name);
            }

            sb.AppendFormat("{0}", methodInfo.ClangMethodSignature());

            if (WriteParametersToSb(operationData, methodInfo, sb, interpreter, crRuntime)) return;

            sbCode.Append(sb);
        }

        public static void HandleCallInterface(LocalOperation operation, StringBuilder sbCode,
            MidRepresentationVariables vars, MethodInterpreter interpreter, CrRuntimeLibrary crRuntime)
        {
            var operationData = (MethodData) operation.Value;
            var sb = new StringBuilder();
            var methodInfo = operationData.Info.GetReversedMethod();
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (!isVoidMethod && operationData.Result != null)
            {
                sb.AppendFormat("{0} = ", operationData.Result.Name);
            }

            sb.AppendFormat("{0}_icall", methodInfo.ClangMethodSignature());

            if (WriteParametersToSb(operationData, methodInfo, sb, interpreter, crRuntime)) return;

            sbCode.Append(sb);
        }

        public static void HandleCallVirtual(LocalOperation operation, StringBuilder sbCode,
            MidRepresentationVariables vars, MethodInterpreter interpreter, CrRuntimeLibrary crRuntime)
        {
            var operationData = (MethodData) operation.Value;
            var sb = new StringBuilder();
            var methodInfo = operationData.Info.GetReversedMethod();
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (!isVoidMethod && operationData.Result != null)
            {
                sb.AppendFormat("{0} = ", operationData.Result.Name);
            }

            //Virtual Method Dispatch Table is on base class only
            //Also we need to take care of the call if this is not a virtual call 
            // C# compiler seems to use virtual calls when derived class uses new operator on non-virtual base class method

            if (methodInfo.IsVirtual)
            {
                if (methodInfo.DeclaringType.GetMethod(methodInfo.Name).DeclaringType == methodInfo.DeclaringType)
                {
                    
                    sb.AppendFormat("{0}_vcall", methodInfo.ClangMethodSignature(true));
                }
                else
                {
                    sb.AppendFormat("{0}", methodInfo.DeclaringType.BaseType.GetMethod(methodInfo.Name).ClangMethodSignature(false));
                }
            }
            else
            {
                sb.AppendFormat("{0}", methodInfo.ClangMethodSignature(false));
            }

            if (WriteParametersToSb(operationData, methodInfo, sb, interpreter, crRuntime)) return;

            sbCode.Append(sb);
        }

        private static bool WriteParametersToSb(MethodData operationData, MethodBase methodInfo, StringBuilder sb,
            MethodInterpreter interpreter, CrRuntimeLibrary crRuntime)
        {
            var identifierValues = operationData.Parameters;

            var escapingData = methodInfo.BuildEscapingBools(crRuntime);
            if (escapingData == null)
            {
                var argumentsCall = String.Join(", ", identifierValues.Select(p =>
                {
                    var computeValue = p.ComputedValue();

                    return computeValue;
                }));

                sb.AppendFormat("({0});", argumentsCall);
                return true;
            }

            #region Parameters

            sb.Append("(");
            var pos = 0;
            var isFirst = true;
            var argumentUsages =
                operationData.Interpreter.AnalyzeProperties.GetUsedArguments(
                    operationData.Interpreter.MidRepresentation.Vars.Arguments);

            var argumentTypes = operationData.Info.GetMethodArgumentTypes();
            for (var index = 0; index < identifierValues.Count; index++)
            {
                var value = identifierValues[index];
                if (!argumentUsages[index])
                    continue;
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(", ");
                var localValue = value as LocalVariable;
                var argumentData = argumentTypes[pos];
                var isEscaping = escapingData[pos];
                pos++;
                if (localValue == null)
                {
                    sb.Append(value.ComputedValue());
                    continue;
                }
                if (localValue.Kind == VariableKind.Argument)
                {
                }

                if (localValue.ComputedType().ClrType == typeof (IntPtr))
                {
                    var argumentTypeCast = argumentData.ToCppMangling();
                    sb.AppendFormat("({0}){1}", argumentTypeCast, localValue.Name);
                    continue;
                }

                var localValueData = interpreter.AnalyzeProperties.GetVariableData(localValue);
                switch (localValueData)
                {
                    case EscapingMode.Smart: // Want to use dynamic_casts instead
//                        if ((!isEscaping && localValue.ComputedType().ClrType.IsClass )|| (operationData.Info.IsVirtual && index==0))
//                        {
//                            sb.AppendFormat("{0}.get()", localValue.Name);
//                        }
//                        else
                        {
                            sb.AppendFormat("{0}", localValue.Name);
                        }
                        continue;
                    case EscapingMode.Stack:
                        sb.AppendFormat("&{0}", localValue.Name);
                        continue;

                    case EscapingMode.Pointer:
                        sb.AppendFormat(!isEscaping ? "{0}" : "{0}.get()", localValue.Name);
                        continue;
                }
            }

            sb.Append(");");

            #endregion

            return false;
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