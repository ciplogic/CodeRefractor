#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
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
            return string.Format("{0} = {1}->{2}", AssignedTo.Name, Instance.Name, FieldName);
        }
    }
}