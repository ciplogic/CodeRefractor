using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.RuntimeBase.Optimizations
{
    public abstract class OptimizationPass
    {
        //returns true if it succeed to apply any optimizations
        //Try to return false by default
        //If the code succeeded to optimize something that other optimizations may benefit, return true
        public abstract bool Optimize(MetaMidRepresentation intermediateCode);
    }
}
