using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class SizeOfAssignment : IClonableOperation
    {
        public LocalVariable AssignedTo;
        public Type Right;

        public override string ToString()
        {
            return String.Format("{0} = {1}", AssignedTo.Name, Right);
        }

        public object Clone()
        {
            return new SizeOfAssignment
                {
                    AssignedTo = (LocalVariable) AssignedTo.Clone(),
                    Right = Right
                };
        }
    }
}