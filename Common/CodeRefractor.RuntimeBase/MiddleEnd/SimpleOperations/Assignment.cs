using System;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Assignment
    {
        public LocalVariable Left;
        public IdentifierValue Right;

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }
}