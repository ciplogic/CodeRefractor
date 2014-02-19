#region Usings

using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public class EvaluatorStack
    {
        private readonly Stack<IdentifierValue> _stack = new Stack<IdentifierValue>();

        public override string ToString()
        {
            return _stack.ToArray().ToString();
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