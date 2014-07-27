#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.Util;
using Microsoft.Win32.SafeHandles;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    internal static class CppHandleCalls
    {
        public static void HandleReturn(LocalOperation operation, StringBuilder bodySb, MethodInterpreter interpreter)
        {
            var returnValue = (Return)operation;


            if (returnValue.Returning == null)
                bodySb.Append("return;");
            else
            {

                //Need to expand this for more cases

                if (returnValue.Returning is ConstValue)
                {
                    var retType = interpreter.Method.GetReturnType();
                    if (retType == typeof (string))
                    {
                        bodySb.AppendFormat("return {0};", returnValue.Returning.ComputedValue());
                    }
                    else
                    {
                        bodySb.AppendFormat("return {0};", returnValue.Returning.Name);
                    }
                }
                else
                {
                    bodySb.AppendFormat("return {0};", returnValue.Returning.Name);
                }


            }
        }


        public static void HandleCall(LocalOperation operation, StringBuilder sbCode, MidRepresentationVariables vars,
            MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            var operationData = (CallMethodStatic)operation;
            var sb = new StringBuilder();
            var methodInfo = operationData.Info.GetReversedMethod(crRuntime);
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (!isVoidMethod && operationData.Result != null)
            {
                sb.AppendFormat("{0} = ", operationData.Result.Name);
            }

            sb.AppendFormat("{0}", methodInfo.ClangMethodSignature(crRuntime));

            if (WriteParametersToSb(operationData, sb, interpreter, crRuntime)) return;

            sbCode.Append(sb);
        }

        public static void HandleCallInterface(LocalOperation operation, StringBuilder sbCode,
            MidRepresentationVariables vars, MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            var operationData = (CallMethodStatic)operation;
            var sb = new StringBuilder();
            var methodInfo = operationData.Info.GetReversedMethod(crRuntime);
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (!isVoidMethod && operationData.Result != null)
            {
                sb.AppendFormat("{0} = ", operationData.Result.Name);
            }

            sb.AppendFormat("{0}_icall", methodInfo.ClangMethodSignature(crRuntime));

            if (WriteParametersToSb(operationData, sb, interpreter, crRuntime)) return;

            sbCode.Append(sb);
        }

        public static void HandleCallVirtual(LocalOperation operation, StringBuilder sbCode,
            MidRepresentationVariables vars, MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            var operationData = (CallMethodStatic)operation;
            var sb = new StringBuilder();
            var methodInfo = operationData.Info.GetReversedMethod(crRuntime);
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (!isVoidMethod && operationData.Result != null)
            {
                sb.AppendFormat("{0} = ", operationData.Result.Name);
            }

            //Virtual Method Dispatch Table is on base class only
            //Also we need to take care of the call if this is not a virtual call 
            // C# compiler seems to use virtual calls when derived class uses new operator on non-virtual base class method
            //Added special case for interface calls


            if (methodInfo.IsVirtual)
            {
                var @params = operationData.Parameters.Select(h => h.FixedType.ClrType).Skip(1).ToArray(); // Skip first parameter for virtual dispatch

                if (methodInfo.IsFinal)//(!operationStatic.Parameters[0].FixedType.ClrType.GetMethod(methodInfo.Name).IsVirtual)) || !operationStatic.Parameters[0].FixedType.ClrType.GetMethod(methodInfo.Name).))
                {
                    //Direct call
                    sb.AppendFormat("{0}", methodInfo.ClangMethodSignature(crRuntime, isvirtualmethod: false));
                }


                else if ((methodInfo.DeclaringType.GetMethod(methodInfo.Name, @params) != null && methodInfo.DeclaringType.GetMethod(methodInfo.Name, @params).DeclaringType == methodInfo.DeclaringType))
                {

                    sb.AppendFormat("{0}_vcall", methodInfo.ClangMethodSignature(crRuntime, isvirtualmethod: true));

                }
                else
                {
                    sb.AppendFormat("{0}", methodInfo.DeclaringType.BaseType.GetMethod(methodInfo.Name, operationData.Parameters.Select(h => h.FixedType.ClrType).ToArray()).ClangMethodSignature(crRuntime, isvirtualmethod: false));
                }
            }
            else
            {
                sb.AppendFormat("{0}", methodInfo.ClangMethodSignature(crRuntime, isvirtualmethod: false));
            }

            if (WriteParametersToSb(operationData, sb, interpreter, crRuntime)) return;

            sbCode.Append(sb);
        }

        private static bool WriteParametersToSb(CallMethodStatic operationStatic, StringBuilder sb,
            MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            var fullEscapeData = operationStatic.Interpreter.BuildEscapeModes();
            var parameters = operationStatic.Parameters;
            sb.Append("(");
            var isFirstParameter = true;
            var parameterStrings = new List<string>();
            for (int index = 0; index < parameters.Count; index++)
            {
                var identifierValue = parameters[index];
                var escapeParameterData = fullEscapeData[index];
                if(escapeParameterData==EscapingMode.Unused)
                    continue;
                var computedValue = identifierValue.ComputedValue();
                if (identifierValue is ConstValue)
                {
                    parameterStrings.Add(computedValue);
                    continue;
                }
                switch (escapeParameterData)
                {
                    case EscapingMode.Smart:
                        parameterStrings.Add(computedValue);
                        break;
                    case EscapingMode.Pointer:
                    {
                        var callingParameterData = interpreter.AnalyzeProperties.GetVariableData((LocalVariable) identifierValue);
                        switch (callingParameterData)
                        {
                            case EscapingMode.Pointer:
                                parameterStrings.Add(computedValue);
                                continue;
                            case EscapingMode.Smart:
                                parameterStrings.Add(String.Format("{0}.get()", computedValue));
                                continue;
                            case EscapingMode.Stack:
                                parameterStrings.Add(String.Format("&{0}", computedValue));
                                continue;
                        }
                    }
                    break;
                }
            }
            var argumentsJoin = String.Join(", ", parameterStrings);
            sb.AppendFormat("({0})", argumentsJoin);
            return true;
        }

        public static void HandleCallRuntime(LocalOperation operation, StringBuilder sb, ClosureEntities crRuntime)
        {
            var operationData = (CallMethodStatic)operation;

            var methodInfo = operationData.Info;
            if (methodInfo.IsConstructor)
                return; //don't call constructor for now
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (isVoidMethod)
            {
                sb.AppendFormat("{0}", methodInfo.ClangMethodSignature(crRuntime));
            }
            else
            {
                sb.AppendFormat("{1} = {0}", methodInfo.ClangMethodSignature(crRuntime),
                    operationData.Result.Name);
            }
            var identifierValues = operationData.Parameters;
            var argumentsCall = String.Join(", ", identifierValues.Select(p => p.Name));

            sb.AppendFormat("({0});", argumentsCall);
        }
    }
}