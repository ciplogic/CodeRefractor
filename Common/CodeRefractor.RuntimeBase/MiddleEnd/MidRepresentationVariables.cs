#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MidRepresentationVariables
    {
        public readonly List<LocalVariableInfo> VariableInfos = new List<LocalVariableInfo>();
        public readonly List<ArgumentVariable> Arguments = new List<ArgumentVariable>();
        public List<LocalVariable> VirtRegs = new List<LocalVariable>();
        public readonly List<LocalVariable> LocalVars = new List<LocalVariable>();

        public void SetupArguments(MethodBase _method)
        {
            var pos = 0;
            var isConstructor = _method is ConstructorInfo;
            if (isConstructor || !_method.IsStatic)
            {
                Arguments.Add(
                    new ArgumentVariable("_this")
                    {
                        FixedType = UsedTypeList.Set(_method.DeclaringType)
                    });
            }
            var argumentVariables = _method.GetParameters()
                .Select(param => new ArgumentVariable(param.Name)
                {
                    FixedType = UsedTypeList.Set(param.ParameterType),
                    Id = pos++
                })
                .ToArray();
            Arguments.AddRange(argumentVariables);
        }



        public void SetupLocalVariables(MethodBase value)
        {
            VariableInfos.Clear();
            VariableInfos.AddRange(value.GetMethodBody().LocalVariables);

            var varsToAdd = VariableInfos.Select((v, index) => new LocalVariable
            {
                FixedType = UsedTypeList.Set(v.LocalType),
                Id = index,
                Kind = VariableKind.Local
            }).ToList();
            LocalVars.Clear();
            LocalVars.AddRange(varsToAdd);
        }


    }
}