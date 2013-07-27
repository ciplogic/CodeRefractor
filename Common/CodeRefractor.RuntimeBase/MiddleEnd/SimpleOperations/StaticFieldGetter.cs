#region Usings

using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class StaticFieldGetter : IdentifierValue
    {
        public TypeData DeclaringType;
        public string FieldName;
    }
}