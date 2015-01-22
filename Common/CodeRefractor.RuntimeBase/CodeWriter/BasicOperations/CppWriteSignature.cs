#region Usings

using System;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Output;
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
                if (parameterData != EscapingMode.Unused)
                {
                    TypeDescription argumentTypeDescription = UsedTypeList.Set(method.DeclaringType.GetReversedMappedType(closureEntities) ?? method.DeclaringType.GetMappedType(closureEntities), closureEntities);
                    var thisText = String.Format("const {0}& _this", argumentTypeDescription.GetClrType(closureEntities).ToCppName()); // all "_this" should be smart pointers
                    //For some reason at three Virtual Test 4 fails this, is something wrong with the escaping ?
//                    if ((!escapingBools[0]))
//                    {
//                        thisText = String.Format("{0} _this",
//                            argumentTypeDescription.ClrType.ToCppName(true, EscapingMode.Pointer));
//                    }
                    sb.Append(thisText);
                    index++;
                }
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
                     argumentTypeDescription.GetClrType(closureEntities).ToCppName(nonEscapingMode, isPInvoke: method.IsPinvoke()), //Handle byref
                    parameterInfo.Name);
            }
            return sb.ToString();
        }


        public static void WriteHeaderMethodWithEscaping(this MethodInterpreter interpreter,
            CodeOutput codeOutput,
            ClosureEntities closureEntities,
            bool writeEndColon = true)
        {
            var methodBase = interpreter.Method;

            codeOutput.Append(methodBase.GetReturnType().ToCppName())
                .Append(" ")
                .Append(interpreter.ClangMethodSignature(closureEntities));

            var arguments = interpreter.GetArgumentsAsTextWithEscaping(closureEntities);

            codeOutput.AppendFormat("({0})", arguments);
            if (writeEndColon)
                codeOutput.Append(";");

            codeOutput.Append("\n");
        }

        public static void WriteSignature(CodeOutput codeOutput, MethodInterpreter interpreter, ClosureEntities closureEntities, bool writeEndColon = false)
        {
            if (interpreter == null)
                return;
            
            interpreter.WriteHeaderMethodWithEscaping(codeOutput, closureEntities, writeEndColon);
        }
    }
}