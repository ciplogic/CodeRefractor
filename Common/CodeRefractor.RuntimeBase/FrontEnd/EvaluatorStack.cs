#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd
{
    public class EvaluatorStack : List<IdentifierValue>
    {
        int _vRegId;

        public EvaluatorStack(Type[] genericArguments)
        {
            GenericArguments = genericArguments;
        }

        public Type[] GenericArguments { get; set; }

        public IdentifierValue Top => this[Count - 1];

        public override string ToString()
        {
            var items = ToArray()
                .Select(id =>
                    string.Format(
                        id is ConstValue ? "'{0}'" : "{0}",
                        id.Name));


            return string.Join("; ", items);
        }

        public LocalVariable SetNewVReg()
        {
            _vRegId++;

            var newLocal = new LocalVariable
            {
                Kind = VariableKind.Vreg,
                Id = _vRegId
            }.AutoName();

            Push(newLocal);

            return newLocal;
        }

        public EvaluatorStack Push(IdentifierValue newLocal)
        {
            Add(newLocal);

            return this;
        }

        public IdentifierValue Pop()
        {
            var result = Top;
            RemoveAt(Count - 1);
            return result;
        }
    }
}