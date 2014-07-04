#region Uses

using System;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
        public LocalOperation(OperationKind kind)
        {
            Kind = kind;
        }

        public OperationKind Kind { get; private set; }

        public virtual LocalOperation Clone()
        {
            var type = GetType();
            var result = (LocalOperation) Activator.CreateInstance(type);
            OperationUtils.PopulateInstance(result, this, type);
            return result;
        }

        public override string ToString()
        {
            return OperationUtils.BuildString(this);
        }
    }
}