#region Uses

using System;
using CodeRefractor.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class LocalOperation
    {
        public LocalOperation(OperationKind kind)
        {
            Kind = kind;
        }

        public OperationKind Kind { get; internal set; }

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