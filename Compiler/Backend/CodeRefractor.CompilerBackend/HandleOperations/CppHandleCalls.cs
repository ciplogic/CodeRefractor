#region Usings

using System;
using System.Linq;
using System.Text;
using CodeRefractor.CompilerBackend.Linker;
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
                if(ctorInterpreter==null)
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
            if(methodInfo.IsConstructor)
            {
                identifierValues.Insert(0, operationData.Result);
            }

            var argumentsCall = String.Join(", ", identifierValues.Select(p =>
                                                                              {
                                                                                  var computeValue = p.ComputedValue();
                                                                                  return computeValue;
                                                                              }));

            sb.AppendFormat("({0});", argumentsCall);
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