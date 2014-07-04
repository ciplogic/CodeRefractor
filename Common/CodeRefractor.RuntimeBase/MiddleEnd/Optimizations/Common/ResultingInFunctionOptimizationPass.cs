#region Usings

using CodeRefractor.ClosureCompute;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Common
{
    public abstract class ResultingInFunctionOptimizationPass : ResultingOptimizationPass
    {
        public static ClosureEntities Closure { get; set; }

        public ResultingInFunctionOptimizationPass()
            : base(OptimizationKind.InFunction)
        {
        }
    }
}