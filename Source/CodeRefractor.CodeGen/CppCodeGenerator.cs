﻿#region Uses

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.Backend.ComputeClosure;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.CodeWriter.Platform;
using CodeRefractor.CodeWriter.Types;
using CodeRefractor.FrontEnd.SimpleOperations.ConstTable;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Interpreters.NonCil;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Backend
{
    /// <summary>
    ///     Writes output.cpp file
    /// </summary>
    public class CppCodeGenerator
    {

        public static (string Src, string Header) BuildFullSourceCode(ClosureEntities closureEntities)
        {
            var entryInterpreter = closureEntities.ResolveMethod(closureEntities.EntryPoint);
            var usedTypes = closureEntities.MappedTypes.Values.ToList();
            var typeTable = new TypeDescriptionTable(usedTypes, closureEntities);

            return CppCodeGenerator.GenerateSourceCodeOutput(
                entryInterpreter,
                typeTable,
                closureEntities);
        }
        public static (string Src, string Header) GenerateSourceCodeOutput(
            MethodInterpreter interpreter,
            TypeDescriptionTable table,
            ClosureEntities closureEntities)
        {
            var closure = closureEntities.MethodImplementations.Values.ToArray();
            var headerSb = new StringBuilder();

            headerSb.Append("#include \"sloth.h\"\n");
            headerSb.Append("#include \"output.hpp\"\n");
            headerSb.Append("#include \"sloth_platform.h\"\n");
            
            if (!string.IsNullOrEmpty(TypeNamerUtils.SmartPtrHeader))
            {
                headerSb
                    .AppendFormat("#include {0}\n", TypeNamerUtils.SmartPtrHeader);
            }

            var initializersCodeOutput = new StringBuilder();
            var typeSignaturesSb = new StringBuilder("#pragma once\n");
            TypeBodiesCodeGenerator.WriteClosureStructBodies(typeSignaturesSb, closureEntities);
            WriteClosureDelegateBodies(closure, initializersCodeOutput);
            WriteClosureHeaders(closure, initializersCodeOutput, closureEntities);

            initializersCodeOutput.BlankLine();
            initializersCodeOutput.Append("#include \"runtime_base.hpp\"");
            initializersCodeOutput.BlankLine();

            var bodySb = new StringBuilder();
            bodySb.Append(VirtualMethodTableCodeWriter.GenerateTypeTableCode(table, closureEntities));
                // We need to use this type table to generate missing jumps for subclasses  that dont override a base method
            WriteCppMethods(closure, bodySb, closureEntities);
            WriteClosureMethods(closure, bodySb, table, closureEntities);

            WriteMainBody(interpreter, bodySb, closureEntities);
            bodySb.Append(PlatformInvokeCodeWriter.LoadDllMethods());
            bodySb.Append(ConstByteArrayList.BuildConstantTable());

            LinkingData.Instance.IsInstTable.BuildTypeMatchingTable(table, closureEntities, initializersCodeOutput);
            bodySb.Append(LinkingData.Instance.Strings.BuildStringTable());


             headerSb
                .Append(initializersCodeOutput.ToString())
                .Append(bodySb.ToString());
             var result = headerSb.ToString();
             return (result, typeSignaturesSb.ToString());
        }

        private static void WriteCppMethods(IList<MethodInterpreter> closure, StringBuilder sb, ClosureEntities crRuntime)
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

        private static void WriteClosureMethods(IList<MethodInterpreter> closure, StringBuilder sb,
            TypeDescriptionTable typeTable, ClosureEntities closureEntities)
        {
            WriteClosureBodies(closure, sb, typeTable, closureEntities);
        }

        private static void WriteClosureHeaders(IEnumerable<MethodInterpreter> closure, StringBuilder stringBuilder,
            ClosureEntities closureEntities)
        {
            closure.Where(interpreter => !interpreter.Method.IsAbstract)
                .Each(interpreter =>
                {
                    MethodInterpreterCodeWriter.WriteMethodSignature(stringBuilder, interpreter, closureEntities);
                    stringBuilder.Append("\n");
                });
        }

        private static void WriteClassFieldsBody(StringBuilder sb, Type mappedType, ClosureEntities crRuntime)
        {
            var typeDesc = UsedTypeList.Set(mappedType, crRuntime);
            typeDesc.WriteLayout(sb);
        }

        private static void WriteClosureDelegateBodies(IList<MethodInterpreter> closure, StringBuilder StringBuilder)
        {
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.Delegate)
                    continue;
                StringBuilder.Append(MethodInterpreterCodeWriter.WriteDelegateCallCode(interpreter));
            }

            StringBuilder.Append(DelegateManager.Instance.BuildDelegateContent());
        }

        private static void WriteClosureBodies(IList<MethodInterpreter> closure, StringBuilder sb,
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

        private static void WriteUsedCppRuntimeMethod(KeyValuePair<string, MethodBase> methodBodyAttribute,
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

        private static void WriteMainBody(MethodInterpreter interpreter, StringBuilder sb, ClosureEntities crRuntime)
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