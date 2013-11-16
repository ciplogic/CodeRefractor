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
            var closure = MetaLinkerClosureComputing.GetMethodClosure(linker.Interpreter);
            var toOptimizeList = closure
                .Where(c=>c.Kind==MethodKind.Default
                    && c.MidRepresentation.LocalOperations.Count>1)
                .Select(cl => cl.MidRepresentation)
                .ToList();
            MetaLinkerOptimizer.ApplyOptimizations(false, toOptimizeList);
            closure = MetaLinkerClosureComputing.GetMethodClosure(linker.Interpreter);
            var typeClosure = TypesClosureLinker.GetTypesClosure(closure);
            var sb = new StringBuilder();
            LinkingData.Includes.Clear();

            sb.AppendLine("#include \"sloth.h\"");
            CrRuntimeLibrary.Instance.RemapUsedTypes();

            WriteClosureStructBodies(typeClosure.ToArray(), sb);
            WriteClosureDelegateBodies(closure, sb);
            WriteClosureHeaders(closure, sb);

            sb.AppendLine("#include \"runtime_base.partcpp\"");


            WriteClosureMethods(closure, sb);

            WriteMainBody(linker, sb);
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
                sb.AppendFormat("struct {0}; ", type.ToCppMangling()).AppendLine();
            }
            foreach (var typeData in typeDatas)
            {
                if (DelegateManager.IsTypeDelegate(typeData))
                    continue;
                var type = CrRuntimeLibrary.Instance.GetReverseType(typeData) ?? typeData;
                var mappedType = CrRuntimeLibrary.Instance.GetMappedType(type) ?? typeData;
                var ns = type.Namespace??"";
                
                sb.AppendFormat("struct {0} {{", type.ToCppMangling()).AppendLine();
                WriteClassFieldsBody(sb, mappedType);
                sb.AppendFormat("}};").AppendLine();

                var staticFields = mappedType.GetFields(BindingFlags.Static).ToList();
                staticFields.AddRange(mappedType.GetFields(BindingFlags.Static | BindingFlags.NonPublic));
                foreach (var fieldData in staticFields.Where(field => field.IsStatic))
                {
					if (!fieldData.IsLiteral) {
						sb.AppendFormat (" /* static*/ {0} {3}::{1} = {2};", 
							fieldData.FieldType.ToCppName (), 
							fieldData.Name.ValidName (), 
							Activator.CreateInstance (fieldData.FieldType), 
							type.ToCppMangling ())
								.AppendLine ();
					} else
						continue;
                }
            }
        }

        private static void WriteClassFieldsBody(StringBuilder sb, Type mappedType)
        {
            var fieldInfos = mappedType.GetFields().ToList();
            fieldInfos.AddRange(mappedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var fieldData in fieldInfos)
            {
                if (fieldData.IsLiteral)
                {
                    var constFieldValue = fieldData.GetValue(null);
                    sb.AppendFormat("// {0} {1} = {2};",
                        fieldData.FieldType.ToCppName(), fieldData.Name, constFieldValue)
                        .AppendLine();
                }else
                sb.AppendFormat(" {0} {1};", fieldData.FieldType.ToCppName(), 
                    fieldData.Name.ValidName()).AppendLine();
            }
            var staticFields = mappedType.GetFields(BindingFlags.Static).ToList();
            staticFields.AddRange(mappedType.GetFields(BindingFlags.Static | BindingFlags.NonPublic));
            foreach (var fieldData in staticFields.Where(field => field.IsStatic))
            {
                if (fieldData.IsLiteral)
                {
                    var constFieldValue = fieldData.GetValue(null);
                    sb.AppendFormat("// static {0} {1} = {2};", 
                        fieldData.FieldType.ToCppName(), fieldData.Name, constFieldValue)
                        .AppendLine();
                }
                else
                {

                    sb.AppendFormat(" static {0} {1};", 
                            fieldData.FieldType.ToCppName(), 
                            fieldData.Name.ValidName())
                        .AppendLine();
                } 
            }
        }

        private static void WriteClosureDelegateBodies(List<MethodInterpreter> closure, StringBuilder sb)
        {
            foreach (var interpreter in closure)
            {
                var codeWriter = new MethodInterpreterCodeWriter
                {
                    Interpreter = interpreter
                };

                if (interpreter.Kind != MethodKind.Delegate)
                    continue;
                sb.AppendLine(codeWriter.WriteDelegateCallCode());
            }

            sb.AppendLine(DelegateManager.Instance.BuildDelegateContent());

        }

        private static void WriteClosureBodies(List<MethodInterpreter> closure, StringBuilder sb)
        {
            foreach (var methodBodyAttribute in CrRuntimeLibrary.Instance.UsedCppMethods)
            {
                WriteUsedCppRuntimeMethod(methodBodyAttribute, sb);
            }

            foreach (var interpreter in closure)
            {
                var codeWriter = new MethodInterpreterCodeWriter
                {
                    Interpreter = interpreter
                };

                if (interpreter.Kind != MethodKind.PlatformInvoke)
                    continue;
                sb.AppendLine(codeWriter.WritePInvokeMethodCode());
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