#region Uses

using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Common
{
    public abstract class ResultingOptimizationPass : OptimizationPassBase
    {
        protected ResultingOptimizationPass(OptimizationKind kind)
            : base(kind)
        {
        }

        //returns true if it succeed to apply any optimizations
        //Try to return false by default
        //If the code succeeded to optimize something that other optimizations may benefit, return true

        public static ClosureEntities Closure { get; set; }
        public bool Result { get; set; }
        public abstract void OptimizeOperations(CilMethodInterpreter interpreter);
        //returns true if it succeed to apply any optimizations
        //Try to return false by default
        //If the code succeeded to optimize something that other optimizations may benefit, return true
        public bool Optimize(CilMethodInterpreter intermediateCode)
        {
            Result = false;
            try
            {
                OptimizeOperations(intermediateCode);
            }
            catch
            {
            }
            return Result;
        }

        public override bool ApplyOptimization(CilMethodInterpreter intermediateCode, ClosureEntities closure)
        {
            return Optimize(intermediateCode);
        }
    }
}