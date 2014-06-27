#region Usings

using System.Collections.Generic;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class AnalyzeProperties
    {
        public bool IsPure { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsSetter { get; set; }
        public bool IsGetter { get; set; }
        public bool IsReadOnly { get; set; }


        public readonly Dictionary<LocalVariable, EscapingMode> LocalVarEscaping =
            new Dictionary<LocalVariable, EscapingMode>();

        public void Setup(List<LocalVariable> arguments, List<LocalVariable> virtRegs, List<LocalVariable> localVars)
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
            LocalVarEscaping[variable] = EscapingMode.Smart;
        }

        public EscapingMode GetVariableData(LocalVariable variable)
        {
            EscapingMode result;
            if (!LocalVarEscaping.TryGetValue(variable, out result))
                return EscapingMode.Smart;
            return LocalVarEscaping[variable];
        }

        public void SetVariableData(LocalVariable variable, EscapingMode escaping)
        {
            LocalVarEscaping[variable] = escaping;
        }

        public bool[] GetUsedArguments(List<LocalVariable> arguments)
        {
            var result = new bool[arguments.Count];
            for (var index = 0; index < arguments.Count; index++)
            {
                var argument = arguments[index];
                result[index] = GetVariableData(argument) != EscapingMode.Unused;
            }
            return result;
        }
    }
}