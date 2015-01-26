#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class GetField : LocalOperation
    {
        public LocalVariable AssignedTo { get; set; }
        public LocalVariable Instance { get; set; }
        public string FieldName { get; set; }

        public GetField()
            : base(OperationKind.GetField)
        {
        }

        public override LocalOperation Clone()
        {
            return new GetField
            {
                AssignedTo = (LocalVariable) AssignedTo.Clone(),
                Instance = (LocalVariable) Instance.Clone(),
                FieldName = FieldName
            };
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}->{2}", AssignedTo.Name, Instance.Name, FieldName);
        }
    }
}