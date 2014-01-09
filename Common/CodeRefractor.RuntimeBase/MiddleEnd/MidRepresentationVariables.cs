#region Usings

using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MidRepresentationVariables
    {
        public readonly List<LocalVariableInfo> Variables = new List<LocalVariableInfo>();
        public readonly List<ArgumentVariable> Arguments = new List<ArgumentVariable>();
        public List<LocalVariable> VirtRegs = new List<LocalVariable>();
        public readonly List<LocalVariable> LocalVars = new List<LocalVariable>();

        public readonly Dictionary<string, VariableData> LocalVarEscaping = new Dictionary<string, VariableData>();

        public void Setup()
        {
            LocalVarEscaping.Clear();
            foreach (var variable in Arguments)
            {
                RegisterVariable(variable);
            }

            foreach (var variable in VirtRegs)
            {
                RegisterVariable(variable);
            }
            foreach (var variable in LocalVars)
            {
                RegisterVariable(variable);
            }
        }

        public void RegisterVariable(LocalVariable variable)
        {
            LocalVarEscaping[variable.Name] = new VariableData();
        }


        public VariableData GetVariableData(string name)
        {
            return LocalVarEscaping[name];
        }

        public VariableData GetVariableData(LocalVariable variable)
        {
            return GetVariableData(variable.Name);
        }
    }
}