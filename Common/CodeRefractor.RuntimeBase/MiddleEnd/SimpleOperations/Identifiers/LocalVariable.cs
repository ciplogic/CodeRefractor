namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers
{
    public class LocalVariable : IdentifierValue
    {
        public VariableKind Kind;
        public int Id;

        public override string ToString()
        {
            return string.Format("{0}:{1}", Name, FixedType != null ? FixedType.Name : "Unknown");
        }

        public LocalVariable Clone()
        {
            var result = new LocalVariable
                             {
                                 FixedType = FixedType,
                                 Id = Id,
                                 Kind = Kind
                             };
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LocalVariable)) return false;
            return Equals((LocalVariable) obj);
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
            return other.Kind.Equals(Kind) && other.Id == Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode();
            }
        }
    }
}