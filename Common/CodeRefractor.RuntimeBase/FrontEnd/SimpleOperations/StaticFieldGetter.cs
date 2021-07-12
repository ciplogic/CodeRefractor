#region Uses

using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class StaticFieldGetter : IdentifierValue
    {
        public TypeDescription DeclaringType;
        public string FieldName;
    }
}