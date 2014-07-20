#region Usings

using System;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    public static class CppWriteSignature
    {
        public static string GetArgumentsAsTextWithEscaping(this MethodInterpreter interpreter, ClosureEntities closureEntities)
        {
            var method = interpreter.Method;

            var parameterInfos = method.GetParameters();
            var escapingBools = method.BuildEscapingBools(closureEntities);
            var sb = new StringBuilder();
            var index = 0;
            var analyze = interpreter.AnalyzeProperties;
            if (!method.IsStatic)
            {
                var parameterData = analyze.GetVariableData(new LocalVariable
                {
                    VarName = "_this",
                    Kind = VariableKind.Argument,
                    Id = 0
                }); 
                TypeDescription argumentTypeDescription = UsedTypeList.Set(method.DeclaringType.GetReversedMappedType(closureEntities) ?? method.DeclaringType.GetMappedType(closureEntities), closureEntities);
                    
                switch (parameterData )
                {
                    case EscapingMode.Unused:
                        break;
                    case EscapingMode.Smart:
                        sb.AppendFormat("const {0}& _this", argumentTypeDescription.ClrType.ToCppName(true));
                        break;
                    case EscapingMode.Pointer:
                        sb.AppendFormat("const {0} _this", argumentTypeDescription.ClrType.ToCppName(true, isSmartPtr:EscapingMode.Pointer));
                        break;
                }
                index++;
            }
            var isFirst = index == 0;
            for (index = 0; index < parameterInfos.Length; index++)
            {
                var parameterInfo = parameterInfos[index];
                var parameterData = analyze.GetVariableData(new LocalVariable()
                {
                    Kind = VariableKind.Argument,
                    VarName = parameterInfo.Name
                });
                if (parameterData == EscapingMode.Unused)
                    continue;

                if (isFirst)
                    isFirst = false;
                else
                {
                    sb.Append(", ");
                }
                var isSmartPtr = escapingBools[index];
                var nonEscapingMode = isSmartPtr ? EscapingMode.Smart : EscapingMode.Pointer;
                var parameterType = parameterInfo.ParameterType.GetReversedMappedType(closureEntities);
                var argumentTypeDescription = UsedTypeList.Set(parameterType, closureEntities);
                sb.AppendFormat("{0} {1}",
                     argumentTypeDescription.ClrType.ToCppName(true, nonEscapingMode), //Handle byref
                    parameterInfo.Name);
            }
            return sb.ToString();
        }


        public static string WriteHeaderMethodWithEscaping(this MethodInterpreter interpreter, ClosureEntities closureEntities, bool writeEndColon = true)
        {
            var methodBase = interpreter.Method;
            var sb = new StringBuilder();
            sb.Append(methodBase.GetReturnType().ToCppName(true));

            sb.Append(" ");

            sb.Append(interpreter.ClangMethodSignature(closureEntities));
            var arguments = interpreter.GetArgumentsAsTextWithEscaping(closureEntities);

            sb.AppendFormat("({0})", arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }

        public static StringBuilder WriteSignature(MethodInterpreter interpreter, ClosureEntities closureEntities, bool writeEndColon = false)
        {
            var sb = new StringBuilder();
            if (interpreter == null)
                return sb;
            var text = interpreter.WriteHeaderMethodWithEscaping(closureEntities, writeEndColon);
            sb.Append(text);
            return sb;
        }
    }
}