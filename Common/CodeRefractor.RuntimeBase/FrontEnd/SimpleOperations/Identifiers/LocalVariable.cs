#region Uses

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Identifiers
{
    public class LocalVariable : IdentifierValue
    {
        public LocalVariable()
        {
            Escaping = EscapingMode.Smart;
        }

        public VariableKind Kind { get; set; }
        public int Id { get; set; }
        public string VarName { get; set; }
        public EscapingMode Escaping { get; set; }

        public override string ToString()
        {
            return $"{Name}:{(FixedType != null ? FixedType.Name : "Unknown")}";
        }

        public override IdentifierValue Clone()
        {
            var result = new LocalVariable
            {
                FixedType = FixedType,
                Id = Id,
                Kind = Kind,
                VarName = VarName
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
            if (!string.IsNullOrEmpty(VarName)) return VarName;
            return GetAutoName();
        }

        string GetAutoName()
        {
            var varKind = Kind == VariableKind.Vreg ? "vreg" : "local";
            var formatVar = $"{varKind}_{Id}";
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

        public LocalVariable AutoName()
        {
            VarName = GetAutoName();

            return this;
        }
    }
}