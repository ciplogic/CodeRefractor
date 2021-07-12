#region Uses

using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class SetField : LocalOperation
    {
        public SetField()
            : base(OperationKind.SetField)
        {
        }

        public IdentifierValue Instance { get; set; }
        public string FieldName { get; set; }
        public IdentifierValue Right { get; set; }
        public TypeDescription FixedType { get; set; }

        public override string ToString()
        {
            return $"{Instance.Name}.{FieldName}= {Right.Name}";
        }
    }
}