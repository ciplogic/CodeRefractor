#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.OuputCodeWriter.Platform;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class CppCodeGenerator
    {
        public static StringBuilder BuildFullSourceCode(MethodInterpreter interpreter)
        {
            var closure = interpreter.GetMethodClosure();
            var toOptimizeList = closure
                .Where(c=>c.Kind==MethodKind.Default
                    && c.MidRepresentation.LocalOperations.Count>1)
                .ToList();
            MetaLinkerOptimizer.ApplyOptimizations(toOptimizeList);
            closure = interpreter.GetMethodClosure();
            var typeClosure = TypesClosureLinker.GetTypesClosure(closure);
            LinkingData.Includes.Clear();
            var sb = GenerateSourceStringBuilder(interpreter, typeClosure, closure);

            return sb;
        }

        private static StringBuilder GenerateSourceStringBuilder(MethodInterpreter interpreter, List<Type> typeClosure, List<MethodInterpreter> closure)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#include \"sloth.h\"");

            WriteClosureStructBodies(typeClosure.ToArray(), sb);
            WriteClosureDelegateBodies(closure, sb);
            WriteClosureHeaders(closure, sb);

            sb.AppendLine("#include \"runtime_base.hpp\"");

            WriteCppMethods(closure, sb);
            WriteClosureMethods(closure, sb);

            WriteMainBody(interpreter, sb);
            sb.AppendLine(PlatformInvokeCodeWriter.LoadDllMethods());
            sb.AppendLine(ConstByteArrayList.BuildConstantTable());
            sb.AppendLine(LinkingData.Instance.Strings.BuildStringTable());
            return sb;
        }

        private static void WriteCppMethods(List<MethodInterpreter> closure, StringBuilder sb)
        {
            var cppMethods = closure
                .Where(m => m.Kind == MethodKind.RuntimeCppMethod)
                .ToArray();
            foreach (var interpreter in cppMethods)
            {
                var runtimeLibrary = interpreter.CppRepresentation;
                var methodDeclaration = interpreter.Method.GenerateKey();
                if (LinkingData.SetInclude(runtimeLibrary.Header))
                    sb.AppendFormat("#include \"{0}\"", runtimeLibrary.Header).AppendLine();

                sb.Append(methodDeclaration);
                sb.AppendFormat("{{ {0} }}", runtimeLibrary.Source).AppendLine();

            }
        }

        private static void WriteClosureMethods(List<MethodInterpreter> closure, StringBuilder sb)
        {
            WriteClosureBodies(closure, sb);
        }

        private static void WriteClosureHeaders(List<MethodInterpreter> closure, StringBuilder sb)
        {
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.Default)
                    continue;
                sb.AppendLine(MethodInterpreterCodeWriter.WriteMethodSignature(interpreter));
            }

        }
        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        private static void WriteClosureStructBodies(Type[] typeDatas, StringBuilder sb)
        {
            foreach (var typeData in typeDatas)
            {
                var mappedType = typeData.GetMappedType();
                if(!mappedType.IsGenericType)
                sb.AppendFormat("struct {0}; ", mappedType.ToCppMangling()).AppendLine();
            }
            foreach (var typeData in typeDatas)
            {
                if (DelegateManager.IsTypeDelegate(typeData))
                    continue;
                var type = typeData.GetMappedType();
                var mappedType = typeData;

                if (mappedType.IsGenericType)
                {
                    var genericTypeCount = mappedType.GetGenericArguments().Length;
                    var typeNames = new List<string>();
                    for (var i = 1; i <= genericTypeCount; i++)
                    {
                        typeNames.Add("class T" + i);
                    }
                    sb.AppendFormat("template <{0}> ", string.Join(", ", typeNames)).AppendLine();
                }
                sb.AppendFormat("struct {0} {{", type.ToCppMangling()).AppendLine();
                WriteClassFieldsBody(sb, mappedType);
                sb.AppendFormat("}};").AppendLine();

                var typedesc = UsedTypeList.Set(type);
                typedesc.WriteStaticFieldInitialization(sb);
               
            }
        }

        private static void WriteClassFieldsBody(StringBuilder sb, Type mappedType)
        {
            var typeDesc = UsedTypeList.Set(mappedType);
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

        private static void WriteClosureBodies(List<MethodInterpreter> closure, StringBuilder sb)
        {
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.PlatformInvoke)
                    continue;
                sb.AppendLine(MethodInterpreterCodeWriter.WritePInvokeMethodCode(interpreter));
            }

            sb.AppendLine("///---Begin closure code --- ");
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.Default)
                    continue;
                sb.AppendLine(MethodInterpreterCodeWriter.WriteMethodCode(interpreter));
            }
            sb.AppendLine("///---End closure code --- ");
        }

        private static void WriteUsedCppRuntimeMethod(KeyValuePair<string, MethodBase> methodBodyAttribute,
                                                      StringBuilder sb)
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
            var methodHeaderText = method.WriteHeaderMethod(false);
            sb.Append(methodHeaderText);
            sb.AppendFormat("{{ {0} }}", methodNativeDescription.Code).AppendLine();
        }

        private static void WriteMainBody(MethodInterpreter interpreter, StringBuilder sb)
        {
            sb.AppendLine("void initializeRuntime();");
            sb.AppendFormat("int main(int argc, char**argv) {{").AppendLine();
            sb.AppendFormat("auto argsAsList = System::getArgumentsAsList(argc, argv);").AppendLine();
            sb.AppendLine("initializeRuntime();");
            var entryPoint = interpreter.Method as MethodInfo;
            if (entryPoint.ReturnType != typeof (void))
                sb.Append("return ");
            var parameterInfos = entryPoint.GetParameters();
            var args = String.Empty;
            if (parameterInfos.Length != 0)
            {
                args = "argsAsList";
            }
            sb.AppendFormat("{0}({1});", entryPoint.ClangMethodSignature(), args).AppendLine();
            sb.AppendLine("return 0;");
            sb.AppendFormat("}}").AppendLine();
        }
    }
}