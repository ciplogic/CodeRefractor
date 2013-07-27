#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class AssemblyData
    {
        public string Name;
        public Dictionary<string, TypeData> Types { get; set; }

        public AssemblyData()
        {
            Types = new Dictionary<string, TypeData>();
        }

        public static AssemblyData GetAssemblyData(Assembly assembly)
        {
            var programData = ProgramData.Instance;

            AssemblyData result;
            if (programData.AssemblyDatas.TryGetValue(assembly.FullName, out result))
                return result;
            result = new AssemblyData() {Name = assembly.FullName,};
            programData.AssemblyDatas[result.Name] = result;
            return result;
        }

        public override string ToString()
        {
            return String.Format("Assembly: '{0}'", Name);
        }
    }
}