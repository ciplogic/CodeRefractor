#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.UseDefs;

#endregion

namespace CodeRefractor.MiddleEnd
{
    public class MetaMidRepresentation
    {
        public MidRepresentationVariables Vars = new MidRepresentationVariables();

        public List<LocalOperation> LocalOperations = new List<LocalOperation>();

        public readonly Dictionary<string, object> AuxiliaryObjects = new Dictionary<string, object>();
        private UseDefDescription _useDef;


        public UseDefDescription UseDef
        {
            get
            {
                if (_useDef.GetLocalOperations() == null)
                {
                    _useDef.Update(LocalOperations.ToArray());
                }
                return _useDef;
            }
            private set { _useDef = value; }
        }

        public MetaMidRepresentation()
        {
            UseDef = new UseDefDescription();
        }

        public void UpdateUseDef()
        {
            UseDef.Update(LocalOperations.ToArray());
        }

        public MethodBase Method { private get; set; }

        public override string ToString()
        {
            return String.Format("Interpreter for '{0}' - instruction count: {1}", Method, LocalOperations.Count);
        }

        public MethodBody GetMethodBody
        {
            get { return Method.GetMethodBody(); }
        }
    }
}