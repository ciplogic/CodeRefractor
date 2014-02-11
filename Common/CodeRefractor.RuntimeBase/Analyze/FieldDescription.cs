namespace CodeRefractor.RuntimeBase.Analyze
{
    public class FieldDescription
    {
        public string Name { get; set; }
        public TypeDescription TypeDescription { get; set; }
        public bool IsStatic { get; set; }
        public int? Offset { get; set; }

        public override string ToString()
        {
            var result = string.Format("{0}: {1}", Name, TypeDescription);
            if (IsStatic)
            {
                result = result + " (static)";
            }
            return result;
        }
    }
}