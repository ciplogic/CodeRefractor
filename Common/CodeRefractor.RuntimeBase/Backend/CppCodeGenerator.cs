#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Platform;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.ComputeClosure;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Backend
{
    public static class CppCodeGenerator
    {
        public static StringBuilder GenerateSourceStringBuilder(MethodInterpreter interpreter, List<Type> typeClosure, List<MethodInterpreter> closure, VirtualMethodTable typeTable, ClosureEntities closureEntities)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#include \"sloth.h\"");
            sb.AppendLine("#include <functional>");

            var virtualMethodTableCodeWriter = new VirtualMethodTableCodeWriter(typeTable, closure);

            WriteClosureStructBodies(typeClosure.ToArray(), sb, closureEntities);
            WriteClosureDelegateBodies(closure, sb);
            WriteClosureHeaders(closure, sb, closureEntities);

            sb.AppendLine("#include \"runtime_base.hpp\"");

            sb.AppendLine(virtualMethodTableCodeWriter.GenerateTypeTableCode(typeClosure.ToArray(), closureEntities)); // We need to use this type table to generate missing jumps for subclasses  that dont override a base method
            WriteCppMethods(closure, sb, closureEntities);
            WriteClosureMethods(closure, sb, typeTable.TypeTable, closureEntities);

            WriteMainBody(interpreter, sb, closureEntities);
            sb.AppendLine(PlatformInvokeCodeWriter.LoadDllMethods());
            sb.AppendLine(ConstByteArrayList.BuildConstantTable());
            sb.AppendLine(LinkingData.Instance.Strings.BuildStringTable());

            return sb;
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
                var cppInterpreter = (CppMethodInterpreter) interpreter;
                var runtimeLibrary = cppInterpreter.CppRepresentation;
                var methodDeclaration = interpreter.Method.GenerateKey(crRuntime);
                if (LinkingData.SetInclude(runtimeLibrary.Header))
                    sb.AppendFormat("#include \"{0}\"", runtimeLibrary.Header).AppendLine();

                sb.Append(methodDeclaration);
                sb.AppendFormat("{{ {0} }}", runtimeLibrary.Source).AppendLine();
            }
        }

        private static void WriteClosureMethods(List<MethodInterpreter> closure, StringBuilder sb, TypeDescriptionTable typeTable, ClosureEntities closureEntities)
        {
            WriteClosureBodies(closure, sb, typeTable, closureEntities);
        }

        private static void WriteClosureHeaders(List<MethodInterpreter> closure, StringBuilder sb, ClosureEntities closureEntities)
        {
            foreach (var interpreter in closure)
            {
                if (interpreter.Kind != MethodKind.CilInstructions)
                    continue;
                if (interpreter.Method.IsAbstract)
                    continue;
                sb.AppendLine(MethodInterpreterCodeWriter.WriteMethodSignature(interpreter, closureEntities));
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

        private static void WriteClosureStructBodies(Type[] typeDatas, StringBuilder sb, ClosureEntities crRuntime)
        {
            //Remove Mapped Types in favour of their resolved types
            typeDatas = typeDatas.Where(type => !typeDatas.Any(t => t.GetMappedType(crRuntime) == type && t != type)).Except(new[] { (typeof(Int32)) }).ToArray();// typeDatas.ToList().Except(typeDatas.Where(y => y.Name == "String")).ToArray(); // This appears sometimes, why ?

            foreach (var typeData in typeDatas)
            {
                var mappedType = typeData.GetMappedType(crRuntime);
                if (ShouldSkipType(typeData))
                    continue;
                if (!mappedType.IsGenericType)
                    sb.AppendFormat("struct {0}; ", mappedType.ToCppMangling()).AppendLine();

                //Lets not redeclare 
            }

            //Better Algorithm for sorting typedefs so that parents are declared before children, seems sometimes compiler complains of invalid use and forward declarations

            var typeDataList = typeDatas.ToList();
            //start with base classes
            var sortedTypeData = new List<Type>();
            sortedTypeData.Add(typeof(System.Object));


            typeDataList.Remove(typeof(Object));

            sortedTypeData.Add(typeof(System.ValueType));


            if (typeDataList.Contains(typeof(System.ValueType)))
                typeDataList.Remove(typeof(System.ValueType));

           


            /*
            while (typeDataList.Count > 0)
            {
                foreach (var typeData in typeDatas)
                {
                    

                    if (sortedTypeData.Contains(typeData)) // Prevent repeats
                        continue;

                    if (sortedTypeData.Contains(typeData.BaseType) || typeData.IsInterface)
                    {
                        sortedTypeData.Add(typeData);
                        typeDataList.Remove(typeData);
                    }
                    if (typeDataList.Count == 0)
                        break;
                  
                }

            }
            */

            //Add these empty interfaces for strings  
            //TODO:Fix this use actual implementations
            /*
            sb.Append(
                new string[]
                {
                    "System_IComparable" , "System_ICloneable" , "System_IConvertible" , "System_IComparable_1" , "System_Collections_Generic_IEnumerable_1" , "System_Collections_IEnumerable" , "System_IEquatable_1"
                }.Select(r => "struct " + r + "{};\n").Aggregate((a, b) => a + "\n" + b));
            */
            foreach (var typeData in sortedTypeData)
            {
                var typeCode = Type.GetTypeCode(typeData);

                if (DelegateManager.IsTypeDelegate(typeData))
                    continue;
                var type = typeData.GetReversedMappedType(crRuntime);
                var mappedType = typeData.GetMappedType(crRuntime);
                if (ShouldSkipType(typeData)) continue;

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
                if (!type.IsValueType && type.BaseType != null)
                {
                    //Not Necessary
                    // sb.AppendFormat("struct {0} : public {1} {2} {{", type.ToCppMangling(), type.BaseType.ToCppMangling(),type.GetInterfaces().Any()? " ,"+type.GetInterfaces().Select(j=>j.ToCppMangling()).Aggregate((a,b)=>a + " , " + b):"");
                    sb.AppendFormat("struct {0} : public {1} {{", type.ToCppMangling(), type.BaseType.ToCppMangling());
                }
                else if (!type.IsValueType && type.IsInterface)
                {
                    sb.AppendFormat("struct {0} : public {1} {{", type.ToCppMangling(), typeof(object).ToCppMangling());
                }
                else
                {
                    sb.AppendFormat("struct {0} {{", type.ToCppMangling());
                }
                sb.AppendLine();
                if (type == typeof(object))
                {
                    sb.AppendLine("int _typeId;");
                }
                WriteClassFieldsBody(sb, mappedType, crRuntime);
                sb.AppendFormat("}};").AppendLine();

                var typedesc = UsedTypeList.Set(type, crRuntime);
                typedesc.WriteStaticFieldInitialization(sb);
            }
        }

        private static bool ShouldSkipType(Type mappedType)
        {
            var typeCode = mappedType.ExtractTypeCode();
            switch (typeCode)
            {
                case TypeCode.Object:
                case TypeCode.String:
                    return false;
            }
            return true;
        }

        private static void WriteClassFieldsBody(StringBuilder sb, Type mappedType, ClosureEntities crRuntime)
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