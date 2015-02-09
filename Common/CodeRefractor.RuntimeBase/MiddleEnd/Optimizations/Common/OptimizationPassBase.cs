using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.RuntimeBase.Optimizations;

namespace CodeRefractor.MiddleEnd.Optimizations.Common
{
    public abstract class OptimizationPassBase
    {
        public abstract bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure);

        public virtual bool CheckPreconditions(CilMethodInterpreter midRepresentation, ClosureEntities closure)
        {
            return true;
        }
        public OptimizationKind Kind { get; set; }
        protected OptimizationPassBase(OptimizationKind kind)
        {
            Kind = kind;
        }

    }
}