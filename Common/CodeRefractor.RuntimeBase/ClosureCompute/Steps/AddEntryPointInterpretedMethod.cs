namespace CodeRefractor.ClosureCompute.Steps
{
    public class AddEntryPointInterpretedMethod : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var resolveEntryPoint = closureEntities.ResolveMethod(closureEntities.EntryPoint);
            if (resolveEntryPoint != null) return false;
            AddMethodToClosure(closureEntities, closureEntities.EntryPoint);
            return true;
        }
    }
}