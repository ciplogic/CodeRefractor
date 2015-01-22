using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.ClosureCompute.TypeSorter;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

namespace CodeRefractor.CodeWriter.Types
{
    static class TypeBodiesCodeGenerator
    {
        public static void WriteClosureStructBodies(StringBuilder sb, ClosureEntities crRuntime)
        {
            //Remove Mapped Types in favour of their resolved types
        
            var typesToMap = crRuntime.MappedTypes.Keys
                .Where(t=>!ShouldSkipType(t))
                .ToArray();
            var sorter = new ClosureTypeSorter(typesToMap, crRuntime);
            var sorted = sorter.DoSort();
            if (sorted.Contains(typeof(IntPtr)))
                sorted.Remove(typeof(IntPtr));
            var forwardTypesSorted = sorted.ToArray();
            GenerateForwardTypes(forwardTypesSorted, sb, crRuntime);

           

            var sortedTypeData = forwardTypesSorted;

           
            foreach (var type in sortedTypeData)
            {
                WriteStructWithFields(sb, crRuntime, type);
            }
        }

        private static void WriteStructWithFields(StringBuilder sb, ClosureEntities crRuntime, Type type)
        {
            if (DelegateManager.IsTypeDelegate(type))
                return;

            CodeOutput codeOutput = new CodeOutput();

            var mappedType = type.GetMappedType(crRuntime);
            type = mappedType.GetReversedMappedType(crRuntime);

            if (!type.IsValueType && type.BaseType != null)
            {
                //Not Necessary
                // codeOutput.AppendFormat("struct {0} : public {1} {2} {{", type.ToCppMangling(), type.BaseType.ToCppMangling(),type.GetInterfaces().Any()? " ,"+type.GetInterfaces().Select(j=>j.ToCppMangling()).Aggregate((a,b)=>a + " , " + b):"");
                codeOutput.AppendFormat(
                        "struct {0} : public {1}",
                        type.ToCppMangling(),
                        type.BaseType.ToCppMangling())
                    .BracketOpen();
            }
            else if (!type.IsValueType && type.IsInterface)
            {
                codeOutput.AppendFormat("struct {0} : public {1}",
                    type.ToCppMangling(),
                    typeof (object).ToCppMangling())
                   .BracketOpen();
            }
            else
            {
                codeOutput.AppendFormat("struct {0}", type.ToCppMangling())
                    .BracketOpen();
            }

            if (type == typeof (object))
            {
                codeOutput.Append("int _typeId;\n");
            }

            //String Support
            if (type == typeof(string))
            {
                crRuntime.AddType(typeof(string));
                List<Type> usedTypes = crRuntime.MappedTypes.Values.ToList();
                var typeTable = new TypeDescriptionTable(usedTypes, crRuntime);

                codeOutput.Append("System_String()")
                    .BracketOpen()
                    .AppendFormat("typeId = {0};\n", typeTable.GetTypeId(typeof (string)))
                    .BracketClose()
                    .BlankLine();
            }

            WriteClassFieldsBody(codeOutput, mappedType, crRuntime);
            codeOutput.BracketClose(true)
                .Append(";\n")
                .BlankLine();

            var typedesc = UsedTypeList.Set(type, crRuntime);
            typedesc.WriteStaticFieldInitialization(codeOutput);

            sb.Append(codeOutput.ToString());
        }

        private static void GenerateForwardTypes(Type[] typeDatas, StringBuilder sb, ClosureEntities crRuntime)
        {
            foreach (var typeData in typeDatas)
            {
                var mappedType = typeData.GetMappedType(crRuntime);
                if (ShouldSkipType(typeData))
                    continue;
                if (!mappedType.IsGenericType)
                    sb.AppendFormat("struct {0}; ", mappedType.ToCppMangling()).AppendLine();
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

        private static void WriteClassFieldsBody(CodeOutput codeOutput, Type mappedType, ClosureEntities crRuntime)
        {
            var typeDesc = UsedTypeList.Set(mappedType, crRuntime);
            typeDesc.WriteLayout(codeOutput);
        }
    }
}