#region Usings

using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public abstract class ProgramOptimizationBase
    {
        public abstract bool Optimize(ProgramClosure closure);
    }
}