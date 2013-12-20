namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public enum OperationKind
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

        LoadFunction,
        FieldRefAssignment,
        SizeOf,
        AddressOfArrayItem,


        Comment,

    }
}