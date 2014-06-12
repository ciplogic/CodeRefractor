namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers
{
    public class LocalVariable : IdentifierValue
    {
        public VariableKind Kind;
        public int Id;
        public object CustomData;

        public override string ToString()
        {
            return string.Format("{0}:{1}", Name, FixedType != null ? FixedType.Name : "Unknown");
        }

        public override IdentifierValue Clone()
        {
            var result = new LocalVariable
            {
                FixedType = FixedType,
                Id = Id,
                Kind = Kind,
                CustomData = CustomData
            };
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var variable = obj as LocalVariable;
            return variable != null && Equals(variable);
        }

        public override string FormatVar()
        {
            var varKind = Kind == VariableKind.Vreg ? "vreg" : "local";
            var formatVar = string.Format("{0}_{1}", varKind, Id);
            return formatVar;
        }

        public bool Equals(LocalVariable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Kind == Kind && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return (int) Kind + 8*Id;
        }
    }
}