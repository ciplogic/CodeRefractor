#region Usings

using CodeRefractor.ClosureCompute;

#endregion



namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public abstract class ProgramOptimizationBase
    {
        public abstract bool Optimize(ClosureEntities closure);
    }
}