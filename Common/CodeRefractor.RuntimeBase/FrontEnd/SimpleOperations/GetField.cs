#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class GetField : LocalOperation
    {
        public GetField()
            : base(OperationKind.GetField)
        {
        }

        public LocalVariable AssignedTo { get; set; }
        public LocalVariable Instance { get; set; }
        public string FieldName { get; set; }

        public override LocalOperation Clone()
        {
            return new GetField
            {
                AssignedTo = AssignedTo,
                Instance = Instance,
                FieldName = FieldName
            };
        }

        public override string ToString()
        {
            return $"{AssignedTo.Name} = {Instance.Name}->{FieldName}";
        }
    }
}