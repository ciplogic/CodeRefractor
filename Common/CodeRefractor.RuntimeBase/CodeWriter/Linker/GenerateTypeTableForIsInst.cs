using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations.Casts;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

namespace CodeRefractor.CodeWriter.Linker
{
    public class GenerateTypeTableForIsInst
    {
        public HashSet<Type> TypesToCast { get; private set; }
        public GenerateTypeTableForIsInst()
        {
            TypesToCast = new HashSet<Type>();
        }

        public ClosureEntities Closure { get; set; }

        public void GenerateInstructionCode(IsInstance isInstance, StringBuilder sb, ClosureEntities crRuntime)
        {
            AddToRuntime(crRuntime);
            //Needs improvement, how do i get the correct typeid at this point ? we cant just use zero :P
            //This is a stupid hack as usedtypes can probably change as closure is computed
            List<Type> usedTypes =crRuntime.MappedTypes.Values.ToList();
            var typeTable = new TypeDescriptionTable(usedTypes, crRuntime);
            TypesToCast.Add(isInstance.CastTo);
            sb
                .AppendFormat("{0} = IsInstanceOf({1}, {2});", 
                    isInstance.AssignedTo.Name,
                    typeTable.GetTypeId(isInstance.CastTo),
                    isInstance.Right.Name + "->_typeId"
                    )
                .AppendLine();
        }

        private static void AddToRuntime(ClosureEntities crRuntime)
        {
            
       
           var feature=crRuntime.FindFeature("IsInstance");

                feature.IsUsed = true;
                feature.Name = "IsInstance";
                
                feature.Headers = new List<string>() {"map", "algorithm"};
                feature.Declarations = new List<string>()
                {
                    "bool IsInstanceOf(int typeSource, int typeImplementation);",
                    "System_Void buildTypesTable();",
                    "std::map<int, std::vector<int> > GlobalMappingType;"
                };
            feature.Initializer =
                "buildTypesTable();";



        }

        public void BuildTypeMatchingTable(TypeDescriptionTable table, ClosureEntities closure)
        {
            var sb = new StringBuilder();
         
         
            
            sb.AppendLine("System_Void buildTypesTable() {");
            if (TypesToCast.Count > 0)
            {
                AddAllTypes(sb, table, closure);
            }
            sb.AppendLine("}");
            sb.AppendLine(@"
bool IsInstanceOf(int typeSource, int typeImplementation) {
    auto typeVector = GlobalMappingType[typeSource];
	auto begin = typeVector.begin();
	auto end = typeVector.end();
	return std::find(begin, end, typeImplementation)!= end;

}
");
            closure.FindFeature("IsInstance").Functions += sb.ToString();
           
        }

        private void AddAllTypes(StringBuilder sb, TypeDescriptionTable table, ClosureEntities closure)
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
