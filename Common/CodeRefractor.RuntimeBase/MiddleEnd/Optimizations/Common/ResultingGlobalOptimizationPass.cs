#region Usings

using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    public abstract class ResultingGlobalOptimizationPass : ResultingOptimizationPass
    {
        public static CrRuntimeLibrary Runtime { get; set; }

        public ResultingGlobalOptimizationPass()
            : base(OptimizationKind.Global)
        {
        }
    }
}