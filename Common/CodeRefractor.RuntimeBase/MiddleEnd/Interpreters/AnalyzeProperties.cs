#region Uses

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public class AnalyzeProperties
    {
        public readonly List<LocalVariable> Arguments = new List<LocalVariable>();
        public bool IsPure { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsSetter { get; set; }
        public bool IsGetter { get; set; }
        public bool IsReadOnly { get; set; }

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
        }

        public EscapingMode GetVariableData(LocalVariable variable)
        {
            var result = variable.Escaping;
            return result;
        }

        public bool SetVariableData(LocalVariable variable, EscapingMode escaping)
        {
            if (variable.Escaping == escaping)
                return false;
            variable.Escaping = escaping;
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