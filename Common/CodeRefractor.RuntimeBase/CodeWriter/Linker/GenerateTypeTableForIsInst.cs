﻿#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.FrontEnd.SimpleOperations.Casts;
using CodeRefractor.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.Linker
{
    public class GenerateTypeTableForIsInst
    {
        public GenerateTypeTableForIsInst()
        {
            TypesToCast = new HashSet<Type>();
        }

        public HashSet<Type> TypesToCast { get; }
        public ClosureEntities Closure { get; set; }

        public void GenerateInstructionCode(IsInstance isInstance, StringBuilder sb, ClosureEntities crRuntime)
        {
            AddToRuntime(crRuntime);
            //Needs improvement, how do i get the correct typeid at this point ? we cant just use zero :P
            //This is a stupid hack as usedtypes can probably change as closure is computed

            crRuntime.AddType(isInstance.CastTo);
            var usedTypes = crRuntime.MappedTypes.Values.ToList();
            var typeTable = new TypeDescriptionTable(usedTypes, crRuntime);
            TypesToCast.Add(isInstance.CastTo);
            sb
                .AppendFormat("{0} = IsInstanceOf({1}, {2});",
                    isInstance.AssignedTo.Name,
                    typeTable.GetTypeId(isInstance.CastTo),
                    isInstance.Right.Name + "->_typeId"
                );
        }

        static bool IsInstanceUsed { get; set; }

        static void AddToRuntime(ClosureEntities crRuntime)
        {
            IsInstanceUsed  = true;

            /*feature.Declarations = new List<string>
            {
                "bool IsInstanceOf(int typeSource, int typeImplementation);",
                "System_Void buildTypesTable();",
                "std::map<int, std::vector<int> > GlobalMappingType;"
            };
            feature.Initializer =
                "buildTypesTable();";
    */    
    }

        public void BuildTypeMatchingTable(TypeDescriptionTable table, ClosureEntities closure, StringBuilder featureBuilder)
        {
            var sb = new StringBuilder();


            sb.AppendLine("System_Void buildTypesTable() {");
            if (TypesToCast.Count > 0)
            {
                AddAllTypes(sb, table, closure);
            }
            sb.AppendLine("}");
            sb.AppendLine(@"
bool IsInstanceOf(int typeSource, int typeImplementation); 
System_Void buildTypesTable();
std::map<int, std::vector<int> > GlobalMappingType;

bool IsInstanceOf(int typeSource, int typeImplementation) {
    auto typeVector = GlobalMappingType[typeSource];
	auto begin = typeVector.begin();
	auto end = typeVector.end();
	return std::find(begin, end, typeImplementation)!= end;

}
");
            featureBuilder.AppendLine( sb.ToString());
        }

        void AddAllTypes(StringBuilder sb, TypeDescriptionTable table, ClosureEntities closure)
        {
            foreach (var type in TypesToCast)
            {
                sb.AppendFormat("// Type of {0}", type.FullName)
                    .AppendLine();

                var typeIdBase = table.GetTypeId(type);

                sb.AppendFormat("std::vector<int> typeTable{0};", typeIdBase)
                    .AppendLine();
                var implementingTypes = type.ImplementorsOfT(closure);
                foreach (var implementingType in implementingTypes)
                {
                    sb.AppendFormat("//Implementor {0}", implementingType.FullName)
                        .AppendLine();

                    var typeIdImpl = table.GetTypeId(implementingType);

                    sb
                        .AppendFormat("typeTable{0}.push_back({1});", typeIdBase, typeIdImpl)
                        .AppendLine();
                }

                sb.AppendFormat("GlobalMappingType[{0}] = typeTable{0};", typeIdBase)
                    .AppendLine();
            }
        }
    }
}