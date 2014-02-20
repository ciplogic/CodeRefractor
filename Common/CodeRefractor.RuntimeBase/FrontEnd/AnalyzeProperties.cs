using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.FrontEnd
{
    public class AnalyzeProperties
    {
        public bool IsPure { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsSetter { get; set; }
        public bool IsGetter { get; set; }
        public bool IsReadOnly { get; set; }


        public readonly Dictionary<LocalVariable, VariableData> LocalVarEscaping = new Dictionary<LocalVariable, VariableData>();
        public void Setup(List<ArgumentVariable> arguments,List<LocalVariable> virtRegs,List<LocalVariable> localVars)
        {
            LocalVarEscaping.Clear();
            foreach (var variable in arguments)
            {
                RegisterVariable(variable);
            }

            foreach (var variable in virtRegs)
            {
                RegisterVariable(variable);
            }
            foreach (var variable in localVars)
            {
                RegisterVariable(variable);
            }
        }

        public void RegisterVariable(LocalVariable variable)
        {
            LocalVarEscaping[variable] = new VariableData();
        }

        public VariableData GetVariableData(LocalVariable variable)
        {
            return LocalVarEscaping[variable];
        }
    }
}