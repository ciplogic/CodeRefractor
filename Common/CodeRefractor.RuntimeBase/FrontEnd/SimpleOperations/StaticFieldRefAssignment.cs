using System;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class StaticFieldRefAssignment : LocalOperation
    {
        public LocalVariable Left;
        public FieldInfo Field;

        public StaticFieldRefAssignment()
            : base(OperationKind.StaticFieldRefAssignment)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name);
        }
    }
}