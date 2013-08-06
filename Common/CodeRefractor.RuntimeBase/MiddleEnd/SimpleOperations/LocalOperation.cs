namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
        public enum Kinds
        {
            Call,

            Return,
            BranchOperator,
            AlwaysBranch,
            Label,

            NewObject,
            CallRuntime,
            Switch,

            GetField,
            SetField,
            GetStaticField,
            SetStaticField,

            GetArrayItem,
            SetArrayItem,
            NewArray,
            CopyArrayInitializer,

            BinaryOperator,
            UnaryOperator,


            Assignment,

            RefAssignment,
            DerefAssignment,

            LoadFunction
        }

        public Kinds Kind;
        public object Value;

        public override string ToString()
        {
            return string.Format("{0}: {1}", Kind, Value ?? "");
        }

        public LocalOperation Clone()
        {
            return new LocalOperation
                       {
                           Kind = Kind,
                           Value = Value
                       };
        }
    }
}