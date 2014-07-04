#region Usings

using CodeRefractor.ClosureCompute;

#endregion



namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public abstract class ResultingProgramOptimizationBase : ProgramOptimizationBase
    {
        public override bool Optimize(ClosureEntities closure)
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

        protected abstract void DoOptimize(ClosureEntities closure);

        public bool Result { get; protected set; }
    }
}