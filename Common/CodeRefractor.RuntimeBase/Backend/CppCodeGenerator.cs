#region Usings

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
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Backend
{
    public static class CppCodeGenerator
    {
        public static StringBuilder GenerateSourceStringBuilder(MethodInterpreter interpreter, TypeDescriptionTable table, List<MethodInterpreter> closure, ClosureEntities closureEntities)
        {
            var headerSb = new StringBuilder();

            headerSb.AppendLine("#include \"sloth.h\"");
            headerSb.AppendLine("#include <functional>");


            var initializersSb = new StringBuilder();
            TypeBodiesCodeGenerator.WriteClosureStructBodies(initializersSb, closureEntities);
            WriteClosureDelegateBodies(closure, initializersSb);
            WriteClosureHeaders(closure, initializersSb, closureEntities);

            initializersSb.AppendLine("#include \"runtime_base.hpp\"");


            var bodySb = new StringBuilder();
            bodySb.AppendLine(VirtualMethodTableCodeWriter.GenerateTypeTableCode(table, closureEntities)); // We need to use this type table to generate missing jumps for subclasses  that dont override a base method
            WriteCppMethods(closure, bodySb, closureEntities);
            WriteClosureMethods(closure, bodySb, table, closureEntities);

            WriteMainBody(interpreter, bodySb, closureEntities);
            bodySb.AppendLine(PlatformInvokeCodeWriter.LoadDllMethods());
            bodySb.AppendLine(ConstByteArrayList.BuildConstantTable());

            LinkingData.Instance.IsInstTable.BuildTypeMatchingTable(table, closureEntities);
            bodySb.AppendLine(LinkingData.Instance.Strings.BuildStringTable());

            //Add Logic to Automatically include std class features that are needed ...
            if (closureEntities.Features.Count > 0)
            {
                foreach (var runtimeFeature in closureEntities.Features)
                {
                    if (runtimeFeature.IsUsed)
                    {
                        if (runtimeFeature.Headers.Count > 0)
                            headerSb.AppendLine("//Headers For Feature: " + runtimeFeature.Name + "\n" +
                                runtimeFeature.Headers.Select(g => "#include<" + g + ">").Aggregate((a, b) => a + "\n" + b) + "\n//End Of Headers For Feature: " + runtimeFeature.Name + "\n");
                        if (runtimeFeature.Declarations.Count > 0)
                            initializersSb.AppendLine("//Initializers For: " + runtimeFeature.Name + "\n" + runtimeFeature.Declarations.Aggregate((a, b) => a + "\n" + b) + "\n//End OF Initializers For: " + runtimeFeature.Name + "\n");
                        if (!String.IsNullOrEmpty(runtimeFeature.Functions))
                            bodySb.AppendLine("//Functions For Feature: " + runtimeFeature.Name + "\n" + runtimeFeature.Functions + "\n//End Of Functions For Feature: " + runtimeFeature.Name + "\n");
                    }
                }
            }

            return headerSb.Append(initializersSb).Append(bodySb);
        }

        private static void WriteCppMethods(List<MethodInterpreter> closure, StringBuilder sb, ClosureEntities crRuntime)
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
                    sb.AppendFormat("#include \"{0}\"", runtimeLibrary.Header).AppendLine();
                var sbDeclaration = CppWriteSignature.WriteSignature(interpreter, crRuntime, false);
                sb.Append(sbDeclaration);
                sb.AppendFormat("{{\n{0}\n}}", runtimeLibrary.Source).AppendLine();
            }
        }

        private static void WriteClosureMethods(List<MethodInterpreter> closure, StringBuilder sb, TypeDescriptionTable typeTable, ClosureEntities closureEntities)
        {
            WriteClosureBodies(closure, sb, typeTable, closureEntities);
        }

        private static void WriteClosureHeaders(IEnumerable<MethodInterpreter> closure, StringBuilder sb, ClosureEntities closureEntities)
        {
            var methodInterpreters =
                closure
                .Where(interpreter => !interpreter.Method.IsAbstract)
                .ToArray();
            foreach (var interpreter in methodInterpreters)
            {
                sb.AppendLine(MethodInterpreterCodeWriter.WriteMethodSignature(interpreter, closureEntities));
            }
        }




        private static void WriteClassFieldsBody(CodeOutput sb, Type mappedType, ClosureEntities crRuntime)
        {
            var typeDesc = UsedTypeList.Set(mappedType, crRuntime);
            typeDesc.WriteLayout(sb);
        }

        private static void WriteClosureDelegateBodies(List<MethodInterpreter> closure, StringBuilder sb)
        {
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.Delegate)
                    continue;
                sb.AppendLine(MethodInterpreterCodeWriter.WriteDelegateCallCode(interpreter));
            }

            sb.AppendLine(DelegateManager.Instance.BuildDelegateContent());
        }

        private static void WriteClosureBodies(List<MethodInterpreter> closure, StringBuilder sb, TypeDescriptionTable typeTable, ClosureEntities closureEntities)
        {
            sb.AppendLine("///--- PInvoke code --- ");
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.PlatformInvoke)
                    continue;
                sb.AppendLine(MethodInterpreterCodeWriter.WritePInvokeMethodCode((PlatformInvokeMethod)interpreter, closureEntities));
            }

            sb.AppendLine("///---Begin closure code --- ");
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.CilInstructions)
                    continue;

                if (interpreter.Method.IsAbstract)
                    continue;
                sb.AppendLine(MethodInterpreterCodeWriter.WriteMethodCode((CilMethodInterpreter)interpreter, typeTable, closureEntities));
            }
            sb.AppendLine("///---End closure code --- ");
        }

        private static void WriteUsedCppRuntimeMethod(KeyValuePair<string, MethodBase> methodBodyAttribute, StringBuilder sb, ClosureEntities crRuntime)
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
            var methodHeaderText = method.WriteHeaderMethod(crRuntime, writeEndColon: false);
            sb.Append(methodHeaderText);
            sb.AppendFormat("{{ {0} }}", methodNativeDescription.Code).AppendLine();
        }

        private static void WriteMainBody(MethodInterpreter interpreter, StringBuilder sb, ClosureEntities crRuntime)
        {
            sb.AppendLine("System_Void initializeRuntime();");
            sb.AppendFormat("int main(int argc, char**argv) {{").AppendLine();
            sb.AppendFormat("auto argsAsList = System_getArgumentsAsList(argc, argv);").AppendLine();
            sb.AppendLine("initializeRuntime();");

            if (crRuntime.Features.Count > 0)
            {
                foreach (var runtimeFeature in crRuntime.Features)
                {
                    sb.AppendLine(runtimeFeature.Initializer + "\n");
                }
            }
            var entryPoint = interpreter.Method as MethodInfo;
            if (entryPoint.ReturnType != typeof(void))
                sb.Append("return ");
            var parameterInfos = entryPoint.GetParameters();
            var args = String.Empty;
            if (parameterInfos.Length != 0)
            {
                args = "argsAsList";
            }
            sb.AppendFormat("{0}({1});", entryPoint.ClangMethodSignature(crRuntime), args).AppendLine();
            sb.AppendLine("return 0;");
            sb.AppendFormat("}}").AppendLine();
        }
    }
}