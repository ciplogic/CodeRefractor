#region Uses

using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Common
{
    public abstract class ResultingInFunctionOptimizationPass : ResultingOptimizationPass
    {
        public ResultingInFunctionOptimizationPass()
            : base(OptimizationKind.InFunction)
        {
        }
    }
}