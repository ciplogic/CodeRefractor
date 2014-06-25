#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class FieldSetter : LocalVariable
    {
        public IdentifierValue Instance;
        public string FieldName;

        public override IdentifierValue Clone()
        {
            return new FieldSetter
            {
                Instance = Instance.Clone(),
                FieldName = FieldName
            };
        }

        public override string ToString()
        {
            return string.Format("{0}->{1}", Instance.Name, FieldName);
        }
    }
}