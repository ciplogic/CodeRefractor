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
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class CppCodeGenerator
    {

        public static StringBuilder BuildFullSourceCode(MetaLinker linker, CrRuntimeLibrary crCrRuntimeLibrary)
        {
            var sb = new StringBuilder();
            LinkingData.Includes.Clear();

            sb.AppendLine("#include \"sloth.h\"");
            crCrRuntimeLibrary.RemapUsedTypes();
            WriteUsedRuntimeTypes(crCrRuntimeLibrary, sb);

            sb.AppendLine("#include \"runtime_base.partcpp\"");

            foreach (var methodBodyAttribute in crCrRuntimeLibrary.UsedCppMethods)
            {
                WriteUsedCppRuntimeMethod(crCrRuntimeLibrary, methodBodyAttribute, sb);
            }

            WriteClassHeaders(linker, sb);

            WriteMethodBodies(linker, sb, crCrRuntimeLibrary);

            sb.AppendLine(PlatformInvokeCodeWriter.LoadDllMethods());
            sb.AppendLine(ConstByteArrayList.BuildConstantTable());
            sb.AppendLine(LinkingData.Instance.Strings.BuildStringTable());

            return sb;
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
                sb.AppendLine(mappedAttribute.Code);
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
            var fields = runtimeType.GetFields(BindingFlags.Instance).Where(field => !field.IsStatic).ToList();
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
                sb.AppendLine(interpreterCodeWriter.WriteMethodSignature(crCrRuntimeLibrary));
            } 
            foreach (var metaInterpreter in GlobalMethodPool.Instance.Interpreters
                .Where(m => m.Value.Kind != MethodKind.PlatformInvoke))
            {
                var interpreterCodeWriter = new MethodInterpreterCodeWriter
                {
                    Interpreter = metaInterpreter.Value
                };
                sb.AppendLine(interpreterCodeWriter.WriteMethodCode(crCrRuntimeLibrary));
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

        private static void WriteUsedCppRuntimeMethod(CrRuntimeLibrary crCrRuntimeLibrary,
                                                      KeyValuePair<string, MethodBase> methodBodyAttribute,
                                                      StringBuilder sb)
        {
            var method = methodBodyAttribute.Value;
            var typeData = method.DeclaringType;
            if (typeData == null)
                throw new InvalidDataException("Method's declaring type should be valid");
            var mappedType = crCrRuntimeLibrary.ReverseMappedTypes[typeData];
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