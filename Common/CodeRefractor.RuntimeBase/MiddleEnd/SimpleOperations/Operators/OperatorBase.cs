using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
{
    public class OperatorBase
    {
        public LocalVariable AssignedTo { get; set; }

        private string _name;

        public OperatorBase(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string FormatVar()
        {
            return _name;
        }
    }
}