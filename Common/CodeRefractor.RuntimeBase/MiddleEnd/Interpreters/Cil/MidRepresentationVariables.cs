#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters.Cil
{
    public class MidRepresentationVariables
    {
        public List<LocalVariable> VirtRegs = new List<LocalVariable>();
        public readonly List<LocalVariable> LocalVars = new List<LocalVariable>(); //Seems we are not using this anymore


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