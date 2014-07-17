using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.ClosureCompute.TypeSorter;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.Util;

namespace CodeRefractor.CodeWriter.Types
{
    class TypeBodiesCodeGenerator
    {
        public static void WriteClosureStructBodies(Type[] typeDatas, StringBuilder sb, ClosureEntities crRuntime)
        {
            //Remove Mapped Types in favour of their resolved types
            typeDatas = typeDatas.Where(type => !typeDatas.Any(t => CommonExtensions.GetMappedType(t, crRuntime) == type && t != type)).Except(new[] { (typeof(Int32)) }).ToArray();// typeDatas.ToList().Except(typeDatas.Where(y => y.Name == "String")).ToArray(); // This appears sometimes, why ?

            var typesToMap = crRuntime.MappedTypes.Keys
                .Where(t=>!ShouldSkipType(t))
                .ToArray();
            var sorter = new ClosureTypeSorter(typesToMap, crRuntime);
            var forwardTypesSorted = sorter.DoSort().ToArray();
            GenerateForwardTypes(forwardTypesSorted, sb, crRuntime);

            //Better Algorithm for sorting typedefs so that parents are declared before children, seems sometimes compiler complains of invalid use and forward declarations

            //start with base classes

            var sortedTypeData = forwardTypesSorted;

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

        private static void GenerateForwardTypes(Type[] typeDatas, StringBuilder sb, ClosureEntities crRuntime)
        {
            foreach (var typeData in typeDatas)
            {
                var mappedType = typeData.GetMappedType(crRuntime);
                if (ShouldSkipType(typeData))
                    continue;
                if (!mappedType.IsGenericType)
                    sb.AppendFormat("struct {0}; ", mappedType.ToCppMangling()).AppendLine();

                //Lets not redeclare 
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
    }
}