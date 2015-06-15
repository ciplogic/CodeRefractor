#region Uses

using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.UseDefs;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters.Cil
{
    public class MetaMidRepresentation
    {
        public readonly Dictionary<string, object> AuxiliaryObjects = new Dictionary<string, object>();
        UseDefDescription _useDef;
        public List<LocalOperation> LocalOperations = new List<LocalOperation>();
        public MidRepresentationVariables Vars = new MidRepresentationVariables();

        public MetaMidRepresentation()
        {
            UseDef = new UseDefDescription();
        }

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

        public MethodBase Method { private get; set; }

        public MethodBody GetMethodBody 
            => Method.GetMethodBody();

        public void UpdateUseDef()
        {
            UseDef.Update(LocalOperations.ToArray());
        }

        public override string ToString()
        {
            return string.Format("Interpreter for '{0}' - instruction count: {1}", Method, LocalOperations.Count);
        }
    }
}