#region Usings

using System;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class TypeData
    {
        public string Namespace;
        public string Name;
        public bool IsClass;
        public bool IsArray;
        public Type Info;

        public override string ToString()
        {
            return string.Format("MethodName='{0}' Namespace='{1}'", Name, Namespace);
        }

        public static TypeData GetTypeData(Type type)
        {
            var assembly = AssemblyData.GetAssemblyData(type.Assembly);
            return ExtractData(type, assembly);
        }

        public static TypeData ExtractData(Type type, AssemblyData assemblyData)
        {
            var programData = ProgramData.Instance;
            var fullName = ComputeFullName(type.Namespace, type.Name);
            var result = programData.LocateType(fullName);
            if (result != null)
                return result;
            if ((type.IsClass || type.IsValueType) &&!type.IsSubclassOf(typeof(Enum)))
            {
                result = new ClassTypeData();
            }
            else
            {
                result = new TypeData();
            }
            PopulateTypeDataFields(type, result);

            assemblyData.Types[fullName] = result;
            if (type.IsClass)
            {
                ClassTypeData.PopulateFields(type, (ClassTypeData) result);
            }
            return result;
        }

        private static void PopulateTypeDataFields(Type type, TypeData result)
        {
            result.Namespace = type.Namespace;
            result.Name = type.Name;
            result.IsClass = type.IsClass;
            result.IsArray = type.IsArray;
            result.Info = type;
        }


        public string FullName
        {
            get
            {
                var typeNs = Namespace;
                var name = Name;
                return ComputeFullName(typeNs, name);
            }
        }

        public static string ComputeFullName(string typeNs, string name)
        {
            return string.IsNullOrEmpty(typeNs)
                       ? name
                       : string.Format("{0}.{1}", typeNs, name);
        }
    }
}