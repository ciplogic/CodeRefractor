using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class StaticFieldGetter : IdentifierValue
    {
        public TypeData DeclaringType;
        public string FieldName;
    }
}