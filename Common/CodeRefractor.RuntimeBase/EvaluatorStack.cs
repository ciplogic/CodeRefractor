#region Usings

using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public class EvaluatorStack
    {
        public readonly Stack<IdentifierValue> Stack = new Stack<IdentifierValue>();

        private int _vRegId;

        public LocalVariable SetNewVReg()
        {
            _vRegId++;
            var newLocal = new LocalVariable
                               {
                                   Kind = VariableKind.Vreg,
                                   Id = _vRegId
                               };
            Stack.Push(newLocal);
            return newLocal;
        }


        public IdentifierValue Top
        {
            get { return Stack.Peek(); }
        }

        public IdentifierValue Pop()
        {
            return Stack.Pop();
        }
    }
}