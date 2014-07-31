#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase;
using CodeRefractor.Util;

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

            WriteParametersToSb(operationData, sb, interpreter);

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
            WriteParametersToSb(operationData, sb, interpreter);

            sbCode.Append(sb);
        }

        public static void HandleCallVirtual(LocalOperation operation, StringBuilder sbCode, MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            var operationData = (CallMethodStatic)operation;
            var sb = new StringBuilder();
            var methodInfo = operationData.Info.GetReversedMethod(crRuntime);
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (!isVoidMethod && operationData.Result != null)
            {
                sb.AppendFormat("{0} = ", operationData.Result.Name);
            }

            var mappedType = methodInfo.DeclaringType.GetReversedMappedType(crRuntime);
            while ((mappedType.BaseType != typeof(Object))) // Match top level virtual dispatch
            {
                if (mappedType.BaseType == null) break;
                mappedType = mappedType.BaseType;
            }

            
            sb.AppendFormat("{0}_vcall", methodInfo.ClangMethodSignature(crRuntime, mappedType));


            //TODO: the intermediate representation should remove the testinf of final methods and such
            //the virtual call is always a virtual call
            //Added DevirtualizeFinalMethods

            //Virtual Method Dispatch Table is on base class only
            //Also we need to take care of the call if this is not a virtual call 
            // C# compiler seems to use virtual calls when derived class uses new operator on non-virtual base class method
            //Added special case for interface calls

            /*
            if (methodInfo.IsVirtual)
            {
                var @params = operationData.Parameters.Select(h => h.FixedType.ClrType).Skip(1).ToArray(); // Skip first parameter for virtual dispatch

                if ((methodInfo.DeclaringType.GetMethod(methodInfo.Name, @params) != null && methodInfo.DeclaringType.GetMethod(methodInfo.Name, @params).DeclaringType == methodInfo.DeclaringType))
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
            */
            WriteParametersToSb(operationData, sb, interpreter);

            sbCode.Append(sb);
        }

        public static void WriteParametersToSb(CallMethodStatic operationStatic, StringBuilder sb,
            MethodInterpreter interpreter)
        {
            var fullEscapeData = operationStatic.Interpreter.BuildEscapeModes();
            var parameters = operationStatic.Parameters;
            var parametersData = new List<EscapingMode>();
            for (int index = 0; index < parameters.Count; index++)
            {
                var identifierValue = parameters[index];
                if (identifierValue is LocalVariable)
                {
                    var callingParameterData =
                        interpreter.AnalyzeProperties.GetVariableData((LocalVariable) identifierValue);
                    parametersData.Add(callingParameterData);
                }
                else
                {
                    parametersData.Add(EscapingMode.Smart);
                }
            }

            BuildCallString(sb, parameters, fullEscapeData, parametersData.ToArray());
        }

        public static void BuildCallString(StringBuilder sb, List<IdentifierValue> parameters, EscapingMode[] fullEscapeData,
            EscapingMode[] parametersData)
        {
            var parameterStrings = new List<string>();
            for (int index = 0; index < parameters.Count; index++)
            {
                var identifierValue = parameters[index];
                var escapeParameterData = fullEscapeData[index];
                var callingParameterData = parametersData[index];

                if (escapeParameterData == EscapingMode.Unused)
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
            sb.AppendFormat("({0});", argumentsJoin);
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