#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd
{
    public class MidRepresentationVariables
    {
        public readonly List<LocalVariable> Arguments = new List<LocalVariable>();
        public List<LocalVariable> VirtRegs = new List<LocalVariable>();
        public readonly List<LocalVariable> LocalVars = new List<LocalVariable>(); //Seems we are not using this anymore

        public void SetupArguments(MethodBase _method)
        {
            var pos = 0;
            var isConstructor = _method is ConstructorInfo;
            if (isConstructor || !_method.IsStatic)
            {
                Arguments.Add(
                    new LocalVariable()
                    {
                        VarName = "_this",
                        Kind = VariableKind.Argument,
                        FixedType = new TypeDescription(_method.DeclaringType)
                    });
            }
            var argumentVariables = _method.GetParameters()
                .Select(param => new LocalVariable()
                {
                    VarName = param.Name,
                    Kind = VariableKind.Argument,
                    FixedType = new TypeDescription(param.ParameterType),
                    Id = pos++
                })
                .ToArray();
            Arguments.AddRange(argumentVariables);

        }


        public void SetupLocalVariables(MethodBase value)
        {
            var methodBody = value.GetMethodBody();
            if (methodBody != null)
            {
                var localVariableInfos = methodBody.LocalVariables;

                var varsToAdd = localVariableInfos.Select((v, index) => new LocalVariable
                {
                    FixedType = new TypeDescription(v.LocalType),
                    Id = index,
                    Kind = VariableKind.Local
                }).ToList();
                foreach (var localVariable in varsToAdd)
                {
                    localVariable.AutoName();
                }
                LocalVars.Clear();
                LocalVars.AddRange(varsToAdd);
            }
        }
    }
}