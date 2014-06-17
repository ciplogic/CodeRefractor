using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Boxing
    {
        public IdentifierValue Value;
        public LocalVariable AssignedTo;

        public override string ToString()
        {
            return string.Format("{0} = box( {1})", 
                Value.Name, 
                AssignedTo.Name);
        }
    }
    public class Unboxing
    {
        public IdentifierValue Value;
        public LocalVariable AssignedTo;

        public override string ToString()
        {
            return string.Format("{0} = unbox( {1})",
                Value.Name,
                AssignedTo.Name);
        }
    }
}