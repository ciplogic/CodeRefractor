namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class ArgumentVariable : LocalVariable
    {
        private readonly string _name;

        public ArgumentVariable(string name)
        {
            _name = name;
            Kind= VariableKind.Argument;
        }

        public override string FormatVar()
        {
            return _name;
        }
    }
}