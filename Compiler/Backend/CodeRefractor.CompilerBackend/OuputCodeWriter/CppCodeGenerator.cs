#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class CppCodeGenerator
    {

        public static StringBuilder BuildFullSourceCode(MetaLinker linker)
        {
            var closure = linker.GetMethodClosure(linker.Interpreter);
            var typeClosure = linker.GetTypesClosure(closure);
            var sb = new StringBuilder();
            LinkingData.Includes.Clear();

            sb.AppendLine("#include \"sloth.h\"");
            CrRuntimeLibrary.Instance.RemapUsedTypes();
            //WriteUsedRuntimeTypes(CrRuntimeLibrary.Instance, sb);

            WriteClosureStructBodies(typeClosure.ToArray(), sb);
            WriteClosureHeaders(closure, sb);

            sb.AppendLine("#include \"runtime_base.partcpp\"");


            WriteClosureMethods(closure, sb);

            sb.AppendLine(PlatformInvokeCodeWriter.LoadDllMethods());
            sb.AppendLine(ConstByteArrayList.BuildConstantTable());
            sb.AppendLine(LinkingData.Instance.Strings.BuildStringTable());

            return sb;
        }

        private static void WriteClosureMethods(List<MethodInterpreter> closure, StringBuilder sb)
        {
            WriteClosureBodies(closure, sb);
        }

        private static void WriteClosureHeaders(List<MethodInterpreter> closure, StringBuilder sb)
        {
            foreach (var interpreter in closure)
            {
                var codeWriter = new MethodInterpreterCodeWriter
                    {
                        Interpreter = interpreter
                    };
                if (interpreter.Kind != MethodKind.Default)
                    continue;
                sb.AppendLine(codeWriter.WriteMethodSignature());
            }

        }

        private static void WriteClosureStructBodies(Type[] typeDatas, StringBuilder sb)
        {
            foreach (var typeData in typeDatas)
            {
                var type = CrRuntimeLibrary.Instance.GetReverseType(typeData) ?? typeData;
                var ns = type.Namespace ?? "";
                sb.AppendFormat("namespace {0} {{ ", ns.Replace('.', '_'));
                sb.AppendFormat("struct {0}; }}", type.Name).AppendLine();
            }
            foreach (var typeData in typeDatas)
            {
                var type = CrRuntimeLibrary.Instance.GetReverseType(typeData) ?? typeData;
                var mappedType = CrRuntimeLibrary.Instance.GetMappedType(type) ?? typeData;
                var ns = type.Namespace??"";
                sb.AppendFormat("namespace {0} {{", ns.Replace('.', '_')).AppendLine();
                sb.AppendFormat("struct {0} {{", type.Name).AppendLine();
                var fieldInfos = mappedType.GetFields(BindingFlags.Instance).ToList();
                fieldInfos.AddRange(mappedType.GetFields(BindingFlags.NonPublic|BindingFlags.Instance));
                foreach (var fieldData in fieldInfos.Where(field => !field.IsStatic))
                {
                    sb.AppendFormat(" {0} {1};", fieldData.FieldType.ToCppName(), fieldData.Name).AppendLine();
                }
                var staticFields = mappedType.GetFields(BindingFlags.Static).ToList();
                staticFields.AddRange(mappedType.GetFields(BindingFlags.Static|BindingFlags.NonPublic));
                foreach (var fieldData in staticFields.Where(field => field.IsStatic))
                {
                    sb.AppendFormat(" static {0} {1};", fieldData.FieldType.ToCppName(), fieldData.Name)
                        .AppendLine();
                }
                sb.AppendFormat("}}; }}").AppendLine();
            }
        }

        private static void WriteClosureBodies(List<MethodInterpreter> closure, StringBuilder sb)
        {
            foreach (var methodBodyAttribute in CrRuntimeLibrary.Instance.UsedCppMethods)
            {
                WriteUsedCppRuntimeMethod(methodBodyAttribute, sb);
            }
            sb.AppendLine("///---Begin closure code --- ");
            foreach (var interpreter in closure)
            {
                var methodDesc = interpreter.Method.GetMethodDescriptor();
                if(CrRuntimeLibrary.Instance.UsedCppMethods.ContainsKey(methodDesc))
                    continue;
                var codeWriter = new MethodInterpreterCodeWriter
                    {
                        Interpreter = interpreter
                    };

                if (interpreter.Kind != MethodKind.Default)
                    continue;
                sb.AppendLine(codeWriter.WriteMethodCode());
            }
            sb.AppendLine("///---End closure code --- ");
        }

        private static void WriteUsedRuntimeTypes(CrRuntimeLibrary crCrRuntimeLibrary, StringBuilder sb)
        {
            var usedTypes = crCrRuntimeLibrary.UsedTypes;
            foreach (var usedType in usedTypes)
            {
                var typeData = usedType.Value;
                var runtimeType = usedType.Key;
                sb.AppendFormat("namespace {0} {{", typeData.Namespace.Replace('.', '_')).AppendLine();
                sb.AppendFormat("struct {0} {{", typeData.Name).AppendLine();
                var mappedAttribute = crCrRuntimeLibrary.TypeAttribute[runtimeType];
                //sb.AppendLine(mappedAttribute.Code);
                var fields = GetFieldsOfType(runtimeType);
                foreach (var fieldData in fields)
                {
                    sb.AppendFormat(" {0} {1};", fieldData.FieldType.ToCppName(), fieldData.Name).AppendLine();
                }
                foreach (var fieldData in typeData.GetFields(BindingFlags.Static).Where(field => field.IsStatic))
                {
                    sb.AppendFormat(" static {0} {1};", fieldData.FieldType.ToCppName(), fieldData.Name)
                        .AppendLine();
                }
                sb.AppendFormat("}}; }}").AppendLine();
            }
        }

        private static List<FieldInfo> GetFieldsOfType(Type runtimeType)
        {
            var fields = runtimeType.GetFields(BindingFlags.Instance|BindingFlags.Public).Where(field => !field.IsStatic).ToList();
            fields.AddRange(runtimeType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(field => !field.IsStatic));
            return fields;
        }

        private static void WriteClassHeaders(MetaLinker linker, StringBuilder sb)
        {
            var entryPoint = linker.MethodInfo;
            var assemblyData = AssemblyData.GetAssemblyData(entryPoint.DeclaringType.Assembly);
            var typeDatas =
                assemblyData.Types.Values.Where(t => t.IsClass && !t.IsArray).Cast<ClassTypeData>().ToArray();

            WriteForwardTypeDefinitions(sb, typeDatas);

            var enumTypeDatas = assemblyData.Types.Values.Where(t => t.Info.IsSubclassOf(typeof (Enum))).ToArray();
            WriteEnumBodies(sb, enumTypeDatas);
            WriteForwardStructBodies(typeDatas, sb);
            WriteStaticFieldDefinitions(sb, typeDatas);
        }

        private static void WriteMethodBodies(MetaLinker linker, StringBuilder sb, CrRuntimeLibrary crCrRuntimeLibrary)
        {
            foreach (var metaInterpreter in GlobalMethodPool.Instance.Interpreters
                .Where(m => m.Value.Kind == MethodKind.PlatformInvoke))
            {
                var interpreterCodeWriter = new MethodInterpreterCodeWriter
                {
                    Interpreter = metaInterpreter.Value
                };
                sb.AppendLine(interpreterCodeWriter.WritePInvokeMethodCode());
            }
            foreach (var metaInterpreter in GlobalMethodPool.Instance.Interpreters
                .Where(m => m.Value.Kind != MethodKind.PlatformInvoke))
            {
                var interpreterCodeWriter = new MethodInterpreterCodeWriter
                                                {
                                                    Interpreter = metaInterpreter.Value
                                                };
                sb.AppendLine(interpreterCodeWriter.WriteMethodSignature());
            } 
            foreach (var metaInterpreter in GlobalMethodPool.Instance.Interpreters
                .Where(m => m.Value.Kind != MethodKind.PlatformInvoke))
            {
                var interpreterCodeWriter = new MethodInterpreterCodeWriter
                {
                    Interpreter = metaInterpreter.Value
                };
                sb.AppendLine(interpreterCodeWriter.WriteMethodCode());
            } 
            WriteMainBody(linker, sb);
        }

        private static void WriteEnumBodies(StringBuilder sb, TypeData[] enumTypeDatas)
        {
            sb.Append("namespace Enums");
            sb.AppendLine("{");
            foreach (var enumTypeData in enumTypeDatas)
            {
                var enumTypeInfo = enumTypeData.Info;

                sb.AppendFormat("enum {0} {{", enumTypeInfo.Name);
                sb.AppendLine("};");
            }
            sb.AppendLine("}");
        }

        private static void WriteStaticFieldDefinitions(StringBuilder sb, ClassTypeData[] typeDatas)
        {
            foreach (var typeData in typeDatas)
            {
                foreach (var fieldData in typeData.Fields.Where(field => field.IsStatic))
                {
                    sb.AppendFormat("{2} {4}::{0}::{1} = {3};",
                                    typeData.Name,
                                    fieldData.Name,
                                    fieldData.TypeData.Info.ToCppName(),
                                    Activator.CreateInstance(fieldData.TypeData.Info),
                                    typeData.Namespace
                        ).AppendLine();
                }
            }
        }

        private static void WriteForwardStructBodies(ClassTypeData[] typeDatas, StringBuilder sb)
        {
            foreach (var typeData in typeDatas)
            {
                sb.AppendFormat("namespace {0} {{", typeData.Namespace).AppendLine();
                sb.AppendFormat("struct {0} {{", typeData.Name).AppendLine();
                foreach (var fieldData in typeData.Fields.Where(field => !field.IsStatic))
                {
                    sb.AppendFormat(" {0} {1};", fieldData.TypeData.Info.ToCppName(), fieldData.Name).AppendLine();
                }
                foreach (var fieldData in typeData.Fields.Where(field => field.IsStatic))
                {
                    sb.AppendFormat(" static {0} {1};", fieldData.TypeData.Info.ToCppName(), fieldData.Name)
                        .AppendLine();
                }
                sb.AppendFormat("}}; }}").AppendLine();
                foreach (var item in typeData.Interpreters)
                {
                    var interpreterCodeWriter = new MethodInterpreterCodeWriter
                                                    {
                                                        Interpreter = item
                                                    };
                    sb.Append(interpreterCodeWriter.WriteHeaderMethod());
                }
            }
        }

        private static void WriteForwardTypeDefinitions(StringBuilder sb, ClassTypeData[] typeDatas)
        {
            foreach (var typeData in typeDatas)
            {
                sb.AppendFormat("namespace {0} {{", typeData.Namespace).AppendLine();
                sb.AppendFormat("struct {0};", typeData.Name).AppendLine();
                sb.AppendFormat("}}").AppendLine();
            }
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

        private static void WriteMainBody(MetaLinker linker, StringBuilder sb)
        {
            sb.AppendLine("void initializeRuntime();");
            sb.AppendFormat("int main(int argc, char**argv) {{").AppendLine();
            sb.AppendFormat("auto argsAsList = System::getArgumentsAsList(argc, argv);").AppendLine();
            sb.AppendLine("initializeRuntime();");
            var entryPoint = linker.MethodInfo as MethodInfo;
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