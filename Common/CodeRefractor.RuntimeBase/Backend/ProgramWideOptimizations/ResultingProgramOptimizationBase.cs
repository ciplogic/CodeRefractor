#region Usings

using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public abstract class ResultingProgramOptimizationBase : ProgramOptimizationBase
    {
        public override bool Optimize(ProgramClosure closure)
        {
            try
            {
                DoOptimize(closure);
            }
            catch
            {
                return false;
            }
            return Result;
        }

        protected abstract void DoOptimize(ProgramClosure closure);

        public bool Result { get; protected set; }
    }
}