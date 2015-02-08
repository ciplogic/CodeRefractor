#region Usings

using CodeRefractor.ClosureCompute;

#endregion

namespace CodeRefractor.Backend.ProgramWideOptimizations
{
    public abstract class ProgramOptimizationBase
    {
        public abstract bool Optimize(ClosureEntities closure);
    }
}