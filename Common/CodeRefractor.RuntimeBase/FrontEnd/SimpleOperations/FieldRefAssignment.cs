#region Uses

using System;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using Mono.Cecil;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class FieldRefAssignment : LocalOperation
    {
        public LocalVariable Left;
        public LocalVariable Right;
        public FieldReference Field;

        public FieldRefAssignment() : base(OperationKind.FieldRefAssignment)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }
}