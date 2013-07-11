using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class FieldGetter : IdentifierValue
    {
        public IdentifierValue Instance;
        public string FieldName;
    }
}