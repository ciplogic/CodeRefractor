#region Usings

using CodeRefractor.RuntimeBase.Optimizations;

#endregion

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