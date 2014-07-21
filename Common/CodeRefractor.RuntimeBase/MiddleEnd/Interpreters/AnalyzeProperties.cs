#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public class AnalyzeProperties
    {
        public bool IsPure { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsSetter { get; set; }
        public bool IsGetter { get; set; }
        public bool IsReadOnly { get; set; }


        public readonly List<LocalVariable> Arguments = new List<LocalVariable>();
        public readonly Dictionary<LocalVariable, EscapingMode> LocalVarEscaping =
            new Dictionary<LocalVariable, EscapingMode>();

        public void SetupArguments(MethodBase method)
        {
            var pos = 0;
            var isConstructor = method is ConstructorInfo;
            if (isConstructor || !method.IsStatic)
            {
                Arguments.Add(
                    new LocalVariable
                    {
                        VarName = "_this",
                        Kind = VariableKind.Argument,
                        FixedType = new TypeDescription(method.DeclaringType)
                    });
            }
            var argumentVariables = method.GetParameters()
                .Select(param => new LocalVariable
                {
                    VarName = param.Name,
                    Kind = VariableKind.Argument,
                    FixedType = new TypeDescription(param.ParameterType),
                    Id = pos++
                })
                .ToArray();
            Arguments.AddRange(argumentVariables);

        }

        public void Setup(List<LocalVariable> virtRegs, List<LocalVariable> localVars)
        {
            LocalVarEscaping.Clear();
            foreach (var variable in Arguments)
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

        public bool SetVariableData(LocalVariable variable, EscapingMode escaping)
        {
            EscapingMode result;
            if (LocalVarEscaping.TryGetValue(variable, out result))
            {
                if (result == escaping)
                    return false;
            }
            
            LocalVarEscaping[variable] = escaping;
            return true;
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