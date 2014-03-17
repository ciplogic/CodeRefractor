using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public abstract class ProgramOptimizationBase
    {
        public abstract bool Optimize(ProgramClosure closure);
    }
}
