#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class ProgramData
    {
        private static readonly ProgramData StaticInstance = new ProgramData();
        public Dictionary<string, AssemblyData> AssemblyDatas { get; set; }

        public static ProgramData Instance
        {
            get { return StaticInstance; }
        }

        private ProgramData()
        {
            AssemblyDatas = new Dictionary<string, AssemblyData>();
        }

        public AssemblyData CurrentAsembly { get; set; }

        public static TypeData LocateType(Type type)
        {
            var fullName = TypeData.ComputeFullName(type.Namespace, type.Name);
            return Instance.LocateType(fullName);
        }

        public TypeData LocateType(string fullTypeName)
        {
            foreach (var assemblyData in AssemblyDatas.Values)
            {
                TypeData result;
                if (assemblyData.Types.TryGetValue(fullTypeName, out result))
                    return result;
            }
            return null;
        }

        public static TypeData UpdateType(Type declaringType)
        {
            var fullName = TypeData.ComputeFullName(declaringType.Namespace, declaringType.Name);
            var locateType = Instance.LocateType(fullName);
            if (locateType != null)
                return locateType;

            var result = (ClassTypeData)TypeData.GetTypeData(declaringType);
            foreach (var fieldDataRef in result.Fields)
            {
                var info = fieldDataRef.TypeData.Info;
                UpdateType(info);
                if (info.IsArray)
                {
                    var elementType = info.GetElementType();
                    UpdateType(elementType);
                }
            }
            return result;
        }
    }
}