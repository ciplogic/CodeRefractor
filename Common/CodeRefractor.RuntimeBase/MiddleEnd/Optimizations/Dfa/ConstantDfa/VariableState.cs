#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Dfa.ConstantDfa
{
    class VariableState
    {
        #region ConstantState enum

        public enum ConstantState
        {
            NotDefined,
            Constant,
            NotConstant
        }

        #endregion

        public ConstValue Constant;
        public ConstantState State; //0 == not yet defined, 1 = constant, 2 = non constant

        public override string ToString()
        {
            return string.Format("{0}: {1}", State, Constant);
        }

        public bool Compare(VariableState value)
        {
            if (State != value.State)
                return false;
            if (State != ConstantState.Constant)
                return true;
            return Constant.Value.Equals(value.Constant.Value);
        }
    }
}