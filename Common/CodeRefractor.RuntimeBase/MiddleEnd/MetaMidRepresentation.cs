#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MetaMidRepresentation
    {
        public readonly MidRepresentationVariables Vars = new MidRepresentationVariables();

        public List<LocalOperation> LocalOperations = new List<LocalOperation>();

        public readonly Dictionary<string, object> AuxiliaryObjects =new Dictionary<string, object>();
        private MethodBase _method;

        public MethodBase Method
        {
            get { return _method; }
            set
            {
                _method = value;
                Vars.Variables.Clear();
                Vars.Variables.AddRange(GetMethodBody.LocalVariables);
                var pos = 0;
                var isConstructor = _method is ConstructorInfo;
                if (isConstructor || !Method.IsStatic)
                {
                    Vars.Arguments.Add(new ArgumentVariable("_this")
                                           {
                                               FixedType = UsedTypeList.Set(Method.DeclaringType)
                                           });
                }
                Vars.Arguments.AddRange(_method.GetParameters().Select(param => new ArgumentVariable(param.Name)
                                                                                    {
                                                                                        FixedType = UsedTypeList.Set(param.ParameterType),
                                                                                        Id = pos++
                                                                                    }));

                var varsToAdd = Vars.Variables.Select((v, index) => new LocalVariable
                {
                    FixedType = UsedTypeList.Set(v.LocalType),
                    Id = index,
                    Kind = VariableKind.Local
                }).ToList();
                Vars.LocalVars.AddRange(varsToAdd);
            }
        }

        public override string ToString()
        {
            return String.Format("Interpreter for '{0}'", _method);
        }

        public MethodBody GetMethodBody
        {
            get { return _method.GetMethodBody(); }
        }
    }
}