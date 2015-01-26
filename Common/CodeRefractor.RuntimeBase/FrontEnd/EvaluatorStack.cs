#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd
{
    public class EvaluatorStack : List<IdentifierValue>
    {
        public Type[] GenericArguments { get; set; }

        public override string ToString()
        {
            var items = ToArray()
                .Select(id =>
                    string.Format(
                        id is ConstValue ? "'{0}'" : "{0}",
                        id.Name));


            return String.Join("; ", items);
        }

        private int _vRegId;

        public EvaluatorStack(Type[] genericArguments)
        {
            GenericArguments = genericArguments;
        }

        public LocalVariable SetNewVReg()
        {
            _vRegId++;
            var newLocal = new LocalVariable
            {
                Kind = VariableKind.Vreg,
                Id = _vRegId
            };
            newLocal.AutoName();
            Push(newLocal);
            return newLocal;
        }

        public void Push(IdentifierValue newLocal)
        {
            Add(newLocal);
        }

        public IdentifierValue Top
        {
            get { return this[Count - 1]; }
        }

        public IdentifierValue Pop()
        {
            var result = Top;
            RemoveAt(Count - 1);
            return result;
        }
    }
}