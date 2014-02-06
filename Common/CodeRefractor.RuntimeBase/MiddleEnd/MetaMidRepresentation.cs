#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MetaMidRepresentation
    {
        public MidRepresentationVariables Vars = new MidRepresentationVariables();

        public List<LocalOperation> LocalOperations = new List<LocalOperation>();

        public readonly Dictionary<string, object> AuxiliaryObjects =new Dictionary<string, object>();
        private MethodBase _method;

        public UseDefDescription UseDef { get; private set; }

        public MetaMidRepresentation()
        {
            UseDef = new UseDefDescription();
        }

        public void UpdateUseDef()
        {
            UseDef.Update(LocalOperations.ToArray());
        }

        public MethodBase Method
        {
            set
            {
                _method = value;
                EvaluateVariableData(value);
            }
        }

        private void EvaluateVariableData(MethodBase value)
        {
            Vars = new MidRepresentationVariables();
           
            Vars.Variables.AddRange(GetMethodBody.LocalVariables);
            var pos = 0;
            var isConstructor = _method is ConstructorInfo;
            if (isConstructor || !value.IsStatic)
            {
                Vars.Arguments.Add(
                    new ArgumentVariable("_this")
                    {
                        FixedType = UsedTypeList.Set(value.DeclaringType)
                    });
            }
            var argumentVariables = _method.GetParameters()
                .Select(param => new ArgumentVariable(param.Name)
                {
                    FixedType = UsedTypeList.Set(param.ParameterType),
                    Id = pos++
                })
                .ToArray();
            Vars.Arguments.AddRange(argumentVariables);

            var varsToAdd = Vars.Variables.Select((v, index) => new LocalVariable
            {
                FixedType = UsedTypeList.Set(v.LocalType),
                Id = index,
                Kind = VariableKind.Local
            }).ToList();
            Vars.LocalVars.Clear();
            Vars.LocalVars.AddRange(varsToAdd);
        }

        public override string ToString()
        {
            return String.Format("Interpreter for '{0}' - instruction count: {1}", _method, LocalOperations.Count);
        }

        public MethodBody GetMethodBody
        {
            get { return _method.GetMethodBody(); }
        }
    }
}