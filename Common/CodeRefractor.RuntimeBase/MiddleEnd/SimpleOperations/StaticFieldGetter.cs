#region Usings

using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class StaticFieldGetter : IdentifierValue
    {
        public TypeDescription DeclaringType;
        public string FieldName;
    }
}