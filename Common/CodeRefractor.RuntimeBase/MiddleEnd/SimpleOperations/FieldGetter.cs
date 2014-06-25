#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class FieldGetter : LocalOperation
    {
        public LocalVariable AssignedTo { get; set; }
        public LocalVariable Instance { get; set; }
        public string FieldName { get; set; }

        public FieldGetter()
            : base(OperationKind.GetField)
        {
        }

        public override LocalOperation Clone()
        {
            return new FieldGetter
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