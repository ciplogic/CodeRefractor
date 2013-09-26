using CodeRefractor.RuntimeBase.Optimizations;

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    public abstract class ResultingGlobalOptimizationPass : ResultingOptimizationPass
    {
        public ResultingGlobalOptimizationPass()
            : base(OptimizationKind.Global)
        {
        }
    }
}