namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers
{
    public class ArgumentVariable : LocalVariable
    {
        private readonly string _name;

        public ArgumentVariable(string name)
        {
            _name = name;
            Kind = VariableKind.Argument;
        }

        public override string FormatVar()
        {
            return _name;
        }

        public override IdentifierValue Clone()
        {
            return new ArgumentVariable(Name)
            {
                Id = Id
            };
        }
    }
}