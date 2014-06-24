#region Usings

using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Common
{
    public abstract class ResultingInFunctionOptimizationPass : ResultingOptimizationPass
    {
        public static CrRuntimeLibrary Runtime { get; set; }

        public ResultingInFunctionOptimizationPass()
            : base(OptimizationKind.InFunction)
        {
        }
    }
}