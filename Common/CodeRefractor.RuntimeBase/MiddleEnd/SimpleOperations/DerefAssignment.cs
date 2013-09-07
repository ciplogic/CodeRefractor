using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class DerefAssignment
    {
        public LocalVariable Left;
        public LocalVariable Right;

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }
}