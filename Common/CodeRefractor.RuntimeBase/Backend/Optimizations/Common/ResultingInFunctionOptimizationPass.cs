#region Usings

using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    public abstract class ResultingInFunctionOptimizationPass : ResultingOptimizationPass
    {
        public ResultingInFunctionOptimizationPass()
            : base(OptimizationKind.InFunction)
        {
        }
    }
}