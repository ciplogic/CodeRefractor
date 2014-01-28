#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Runtime;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class CppFullFileMethodWriter
    {
        public static MethodInterpreter CreateLinkerFromEntryPoint(this MethodInfo definition)
        {
            var methodInterpreter = definition.Register();
            MetaLinker.Interpret(methodInterpreter);

            MetaLinkerOptimizer.OptimizeMethods();
            var foundMethodCount = 1;
            bool canContinue = true;
            while (canContinue)
            {
                var dependencies = methodInterpreter.GetMethodClosure();
                canContinue = foundMethodCount != dependencies.Count;
                foundMethodCount = dependencies.Count;
                foreach (var interpreter in dependencies)
                {
                    MetaLinker.Interpret(interpreter);
                }
                MetaLinkerOptimizer.OptimizeMethods();
            }

            return methodInterpreter;
        }

        public static string WriteHeaderMethodWithEscaping(this MethodBase methodBase, bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName(true);

            var sb = new StringBuilder();
            var declaringType = methodBase.DeclaringType;
            if (declaringType.IsGenericType)
            {
                var genericTypeCount = declaringType.GetGenericArguments().Length;

                if (genericTypeCount > 0)
                    sb.AppendLine(genericTypeCount.GetTypeTemplatePrefix());
            }

            var arguments = methodBase.GetArgumentsAsTextWithEscaping();

            sb.AppendFormat("{0} {1}({2})",
                            retType, methodBase.ClangMethodSignature(), arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }

        public static string GetMethodDescriptor(this MethodBase method)
        {
            return CrRuntimeLibrary.GetMethodDescription(method);
        }

        public static string GetArgumentsAsTextWithEscaping(this MethodBase method)
        {
            var parameterInfos = method.GetParameters();
            var escapingBools = BuildEscapingBools(method);
            var sb = new StringBuilder();
            var index = 0;
            if (!method.IsStatic)
            {
                var argumentTypeDescription = UsedTypeList.Set(method.DeclaringType.GetMappedType());
                var thisText = String.Format("const {0}& _this", argumentTypeDescription.ClrType.ToCppName(true));
                if(!escapingBools[0])
                {
                    thisText = String.Format("{0} _this", argumentTypeDescription.ClrType.ToCppName(true, EscapingMode.Pointer));
                }
                sb.Append(thisText);
                index++;
            }
            var isFirst = index==0;
            
            for (index=0; index < parameterInfos.Length; index++)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    sb.Append(", ");
                }
                var parameterInfo = parameterInfos[index];
                var isSmartPtr = escapingBools[index];
                var nonEscapingMode = isSmartPtr ? EscapingMode.Smart : EscapingMode.Pointer;
                var argumentTypeDescription = UsedTypeList.Set(method.DeclaringType.GetMappedType());
                sb.AppendFormat("{0} {1}",
                    argumentTypeDescription.ClrType.ToCppName(true, isSmartPtr: nonEscapingMode), 
                    parameterInfo.Name);
            }
            return sb.ToString();
        }

        public static bool[] BuildEscapingBools(MethodBase method)
        {
            var parameters = method.GetParameters();
            var escapingBools = new bool[parameters.Length + 1];
            
            var escapeData = AnalyzeParametersAreEscaping.EscapingParameterData(method);
            if (escapeData != null)
            {
                foreach (var escaping in escapeData)
                {
                    if (escaping.Value)
                        escapingBools[escaping.Key] = true;
                }
            }
            else
            {
                for (var index = 0; index <= parameters.Length; index++)
                {
                    escapingBools[index] = true;
                }
            }
            return escapingBools;
        }
    }
}