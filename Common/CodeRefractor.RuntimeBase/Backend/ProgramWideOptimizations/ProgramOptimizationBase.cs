using CodeRefractor.CompilerBackend.OuputCodeWriter;

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public abstract class ProgramOptimizationBase
    {
        public abstract bool Optimize(ProgramClosure closure);
    }
}
