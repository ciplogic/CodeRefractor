#region Uses

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.Backend.ComputeClosure;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.CodeWriter.Platform;
using CodeRefractor.CodeWriter.Types;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Backend
{
    /// <summary>
    ///     Writes output.cpp file
    /// </summary>
    public class CppCodeGenerator
    {
        readonly Func<CodeOutput> _createCodeOutput;

        public CppCodeGenerator(Func<CodeOutput> createCodeOutput)
        {
            _createCodeOutput = createCodeOutput;
        }

        public CodeOutput GenerateSourceCodeOutput(
            MethodInterpreter interpreter,
            TypeDescriptionTable table,
            List<MethodInterpreter> closure,
            ClosureEntities closureEntities)
        {
            var headerSb = _createCodeOutput();

            headerSb.Append("#include \"sloth.h\"")
                .BlankLine();
            if (!string.IsNullOrEmpty(TypeNamerUtils.SmartPtrHeader))
            {
                headerSb
                    .AppendFormat("#include {0}", TypeNamerUtils.SmartPtrHeader)
                    .BlankLine();
            }

            var initializersCodeOutput = _createCodeOutput();
            TypeBodiesCodeGenerator.WriteClosureStructBodies(initializersCodeOutput, closureEntities);
            WriteClosureDelegateBodies(closure, initializersCodeOutput);
            WriteClosureHeaders(closure, initializersCodeOutput, closureEntities);

            initializersCodeOutput.BlankLine();
            initializersCodeOutput.Append("#include \"runtime_base.hpp\"");
            initializersCodeOutput.BlankLine();

            var bodySb = _createCodeOutput();
            bodySb.Append(VirtualMethodTableCodeWriter.GenerateTypeTableCode(table, closureEntities));
                // We need to use this type table to generate missing jumps for subclasses  that dont override a base method
            WriteCppMethods(closure, bodySb, closureEntities);
            WriteClosureMethods(closure, bodySb, table, closureEntities);

            WriteMainBody(interpreter, bodySb, closureEntities);
            bodySb.Append(PlatformInvokeCodeWriter.LoadDllMethods());
            bodySb.Append(ConstByteArrayList.BuildConstantTable());

            LinkingData.Instance.IsInstTable.BuildTypeMatchingTable(table, closureEntities, initializersCodeOutput.StringBuilderOutput);
            bodySb.Append(LinkingData.Instance.Strings.BuildStringTable());


            return headerSb
                .Append(initializersCodeOutput.ToString())
                .Append(bodySb.ToString());
        }

        static void WriteCppMethods(List<MethodInterpreter> closure, CodeOutput sb, ClosureEntities crRuntime)
        {
            var cppMethods = closure
                .Where(m => m.Kind == MethodKind.RuntimeCppMethod)
                .ToArray();

            var methodInterpreter = cppMethods.FirstOrDefault();
            if (methodInterpreter == null) return;
            foreach (var interpreter in cppMethods)
            {
                var cppInterpreter = (CppMethodInterpreter)interpreter;
                var runtimeLibrary = cppInterpreter.CppRepresentation;
                if (LinkingData.SetInclude(runtimeLibrary.Header))
                    sb.AppendFormat("#include \"{0}\"\n", runtimeLibrary.Header);
                CppWriteSignature.WriteSignature(sb, interpreter, crRuntime, false);
                sb.BracketOpen()
                    .Append(runtimeLibrary.Source)
                    .BracketClose();
            }
        }

        static void WriteClosureMethods(List<MethodInterpreter> closure, CodeOutput sb,
            TypeDescriptionTable typeTable, ClosureEntities closureEntities)
        {
            WriteClosureBodies(closure, sb, typeTable, closureEntities);
        }

        static void WriteClosureHeaders(IEnumerable<MethodInterpreter> closure, CodeOutput codeOutput,
            ClosureEntities closureEntities)
        {
            closure.Where(interpreter => !interpreter.Method.IsAbstract)
                .Each(interpreter =>
                {
                    MethodInterpreterCodeWriter.WriteMethodSignature(codeOutput, interpreter, closureEntities);
                    codeOutput.Append("\n");
                });
        }

        static void WriteClassFieldsBody(CodeOutput sb, Type mappedType, ClosureEntities crRuntime)
        {
            var typeDesc = UsedTypeList.Set(mappedType, crRuntime);
            typeDesc.WriteLayout(sb);
        }

        static void WriteClosureDelegateBodies(List<MethodInterpreter> closure, CodeOutput codeOutput)
        {
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.Delegate)
                    continue;
                codeOutput.Append(MethodInterpreterCodeWriter.WriteDelegateCallCode(interpreter));
            }

            codeOutput.Append(DelegateManager.Instance.BuildDelegateContent());
        }

        static void WriteClosureBodies(List<MethodInterpreter> closure, CodeOutput sb,
            TypeDescriptionTable typeTable, ClosureEntities closureEntities)
        {
            sb.Append("///--- PInvoke code ---\n");
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.PlatformInvoke)
                    continue;
                sb.Append(MethodInterpreterCodeWriter.WritePInvokeMethodCode((PlatformInvokeMethod)interpreter,
                    closureEntities));
            }

            sb.Append("///---Begin closure code ---\n");
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.CilInstructions)
                    continue;

                if (interpreter.Method.IsAbstract)
                    continue;
                sb.Append(MethodInterpreterCodeWriter.WriteMethodCode((CilMethodInterpreter)interpreter, typeTable,
                    closureEntities));
            }
            sb.Append("///---End closure code ---\n");
        }

        static void WriteUsedCppRuntimeMethod(KeyValuePair<string, MethodBase> methodBodyAttribute,
            StringBuilder sb, ClosureEntities crRuntime)
        {
            var method = methodBodyAttribute.Value;
            var typeData = method.DeclaringType;
            if (typeData == null)
                throw new InvalidDataException("Method's declaring type should be valid");
            var methodNativeDescription = method.GetCustomAttribute<CppMethodBodyAttribute>();
            if (methodNativeDescription == null)
                throw new InvalidDataException(
                    "Cpp runtime method is called but is not marked with CppMethodBody attribute");
            if (LinkingData.SetInclude(methodNativeDescription.Header))
                sb.AppendFormat("#include \"{0}\"", methodNativeDescription.Header).AppendLine();
            var methodHeaderText = method.WriteHeaderMethod(crRuntime, false);
            sb.Append(methodHeaderText);
            sb.AppendFormat("{{ {0} }}", methodNativeDescription.Code).AppendLine();
        }

        static void WriteMainBody(MethodInterpreter interpreter, CodeOutput sb, ClosureEntities crRuntime)
        {
            sb.Append("System_Void initializeRuntime();\n");
            sb.Append("int main(int argc, char**argv)").BracketOpen();
            sb.Append("auto argsAsList = System_getArgumentsAsList(argc, argv);\n");
            sb.Append("initializeRuntime();\n");

    
            var entryPoint = interpreter.Method as MethodInfo;
            if (entryPoint.ReturnType != typeof(void))
                sb.Append("return ");
            var parameterInfos = entryPoint.GetParameters();
            var args = string.Empty;
            if (parameterInfos.Length != 0)
            {
                args = "argsAsList";
            }
            sb.AppendFormat("{0}({1});\n", entryPoint.ClangMethodSignature(crRuntime), args);
            sb.BlankLine();
            sb.Append("return 0;");
            sb.BracketClose();
        }
    }
}