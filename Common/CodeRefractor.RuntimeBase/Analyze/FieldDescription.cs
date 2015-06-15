#region Uses

using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.Analyze
{
    public class FieldDescription
    {
        public string Name { get; set; }
        public TypeDescription TypeDescription { get; set; }
        public bool IsStatic { get; set; }
        public int? Offset { get; set; }

        public override string ToString()
        {
            var result = $"{Name}: {TypeDescription}";
            if (IsStatic)
            {
                result = result + " (static)";
            }
            return result;
        }
    }
}