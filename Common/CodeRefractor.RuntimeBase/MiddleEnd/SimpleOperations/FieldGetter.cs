using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class FieldGetter : IClonableOperation
    {
        public IdentifierValue AssignedTo;
        public IdentifierValue Instance;
        public string FieldName;
        public object Clone()
        {
            return new FieldGetter
            {
                AssignedTo = AssignedTo.Clone(),
                Instance = Instance.Clone(),
                FieldName = FieldName
            };
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}->{2}", AssignedTo.Name, Instance.Name, FieldName);
        }
    }
}