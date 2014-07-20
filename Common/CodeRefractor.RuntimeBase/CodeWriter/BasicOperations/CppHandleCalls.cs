#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            if (WriteParametersToSb(operationData, methodInfo, sb, interpreter, crRuntime)) return;

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

            if (WriteParametersToSb(operationData, methodInfo, sb, interpreter, crRuntime)) return;

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

            if (WriteParametersToSb(operationData, methodInfo, sb, interpreter, crRuntime)) return;

            sbCode.Append(sb);
        }

        private static bool WriteParametersToSb(CallMethodStatic operationStatic, MethodBase methodInfo, StringBuilder sb,
            MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            var identifierValues = operationStatic.Parameters;

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
            var argumentUsages = MidRepresentationUtils.GetUsedArguments(operationStatic.Interpreter);

            var argumentTypes = operationStatic.Info.GetMethodArgumentTypes();
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
                    //We need to take care of strings here
                    if (value is ConstValue)
                    {
                        if ((value.FixedType.ClrType == typeof(string) && methodInfo.IsPinvoke()))
                        {
                            sb.Append("" + value.ComputedValue() + "->Text->Items");
                        }
                        else
                        if ((value.FixedType.ClrType == typeof(string) && !isEscaping && !methodInfo.IsSpecialName) ) //Why isnt escape analysis done for properties ?
                        {
                            sb.Append("(System_String*)" + value.ComputedValue() + "->Text.get()");
                        }
                        else
                        {
                            sb.Append(value.ComputedValue());
                        }
                    }
                    else
                    {
                        sb.Append(value.ComputedValue());
                    }

                    continue;
                }
                if (localValue.Kind == VariableKind.Argument)
                {
                }

                if (localValue.ComputedType().ClrType == typeof(IntPtr))
                {
                    var argumentTypeCast = argumentData.ToCppMangling();
                    sb.AppendFormat("({0}){1}", argumentTypeCast, localValue.Name);
                    continue;
                }

                var localValueData = interpreter.AnalyzeProperties.GetVariableData(localValue);

                switch (localValueData)
                {
                    case EscapingMode.Smart: // Want to use dynamic_casts instead
                        //                        if ((!isEscaping && localValue.ComputedType().ClrType.IsClass )|| (operationStatic.Info.IsVirtual && index==0))
                        //                        {
                        //                            sb.AppendFormat("{0}.get()", localValue.Name);
                        //                        }
                        //                        else
                        {
                            //TODO: this won't work we'll just declare interface instances as system_object
                            //                            if (index==0 && operationStatic.Info.DeclaringType.IsInterface)
                            //                            {
                            //                                sb.AppendFormat("std::static_pointer_cast<{0}>({1})", typeof(object).ToCppMangling(), localValue.Name);
                            //                            }
                            //                            else

                            //Handle ByRef

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