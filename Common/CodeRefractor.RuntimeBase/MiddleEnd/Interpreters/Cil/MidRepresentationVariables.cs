#region Uses

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters.Cil
{
    public class MidRepresentationVariables
    {
        public LocalVariable[] LocalVars = new LocalVariable[0]; //Seems we are not using this anymore
        public List<LocalVariable> VirtRegs = new List<LocalVariable>();

        public void SetupLocalVariables(MethodBase value)
        {
            var methodBody = value.GetMethodBody();
            if (methodBody == null) return;
            var localVariableInfos = methodBody.LocalVariables;

            var varsToAdd = localVariableInfos.Select((v, index) => new LocalVariable
            {
                FixedType = new TypeDescription(v.LocalType),
                Id = index,
                Kind = VariableKind.Local
            }).ToArray();
            foreach (var localVariable in varsToAdd)
            {
                localVariable.AutoName();
            }
            LocalVars = varsToAdd;
        }
    }
}