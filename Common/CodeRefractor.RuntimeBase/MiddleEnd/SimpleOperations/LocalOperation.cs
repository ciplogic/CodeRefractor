namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
        public enum Kinds
        {
            Operator,
            Call,
            Return,
            BranchOperator,
            Label,
            AlwaysBranch,
            SetField,
            NewObject,
            LoadArgument,
            GetField,
            Assignment,
            GetArrayItem,
            NewArray,
            SetArrayItem,
            CopyArrayInitializer,
            CallRuntime,
            Switch,
            GetStaticField,
            SetStaticField
        }

        public Kinds Kind;
        public object Value;

        public override string ToString()
        {
            return string.Format("{0}: {1}", Kind, Value ?? "");
        }
    }
}
