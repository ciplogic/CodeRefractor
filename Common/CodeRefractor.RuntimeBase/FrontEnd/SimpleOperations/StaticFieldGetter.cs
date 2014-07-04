#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class StaticFieldGetter : IdentifierValue
    {
        public TypeDescription DeclaringType;
        public string FieldName;
    }
}