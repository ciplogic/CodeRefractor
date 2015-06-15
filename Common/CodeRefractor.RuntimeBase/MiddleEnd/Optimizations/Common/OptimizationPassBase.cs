#region Uses

using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Common
{
    public abstract class OptimizationPassBase
    {
        protected OptimizationPassBase(OptimizationKind kind)
        {
            Kind = kind;
        }

        public OptimizationKind Kind { get; set; }
        public abstract bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure);

        public virtual bool CheckPreconditions(CilMethodInterpreter midRepresentation, ClosureEntities closure)
        {
            return true;
        }
    }
}