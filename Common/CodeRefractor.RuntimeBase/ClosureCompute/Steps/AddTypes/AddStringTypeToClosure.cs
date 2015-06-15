namespace CodeRefractor.ClosureCompute.Steps.AddTypes
{
    public class AddStringTypeToClosure : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            return closureEntities.AddType(typeof (string))
                   && closureEntities.AddType(typeof (object))
                ;
        }
    }
}