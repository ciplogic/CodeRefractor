#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public class EvaluatorStack
    {
        private readonly Stack<IdentifierValue> _stack = new Stack<IdentifierValue>();

        public override string ToString()
        {
            var items = _stack.ToArray()
                .Select(id =>
                    string.Format(
                        id is ConstValue ? "'{0}'" : "{0}",
                        id.Name));


            return String.Join("; ", items);
        }

        private int _vRegId;

        public LocalVariable SetNewVReg()
        {
            _vRegId++;
            var newLocal = new LocalVariable
            {
                Kind = VariableKind.Vreg,
                Id = _vRegId
            };
            Push(newLocal);
            return newLocal;
        }

        public void Push(IdentifierValue newLocal)
        {
            _stack.Push(newLocal);
        }

        public int Count
        {
            get { return _stack.Count; }
        }

        public IdentifierValue Top
        {
            get { return _stack.Peek(); }
        }

        public IdentifierValue Pop()
        {
            return _stack.Pop();
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}