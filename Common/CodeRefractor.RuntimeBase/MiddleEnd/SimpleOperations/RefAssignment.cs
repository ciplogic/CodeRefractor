#region Usings

using System;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class RefAssignment
    {
        public LocalVariable Left;
        public LocalVariable Right;

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }

    public class FieldRefAssignment
    {
        public LocalVariable Left;
        public LocalVariable Right;
        public FieldInfo Field;

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }
}