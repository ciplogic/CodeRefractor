using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class FieldGetter : IdentifierValue, IClonableOperation
    {
        public IdentifierValue Instance;
        public string FieldName;
        public object Clone()
        {
            return new FieldGetter
            {
                Instance = Instance.Clone(),
                FieldName = FieldName
            };
        }
    }
}