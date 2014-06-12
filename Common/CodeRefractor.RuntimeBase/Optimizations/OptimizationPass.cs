#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.RuntimeBase.Optimizations
{
    public abstract class OptimizationPass
    {
        //returns true if it succeed to apply any optimizations
        //Try to return false by default
        //If the code succeeded to optimize something that other optimizations may benefit, return true
        public abstract bool Optimize(MethodInterpreter intermediateCode);

        public OptimizationPass(OptimizationKind kind)
        {
            Kind = kind;
        }

        public OptimizationKind Kind { get; set; }

        public virtual bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            return true;
        }
    }
}