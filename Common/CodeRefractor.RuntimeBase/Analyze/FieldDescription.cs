namespace CodeRefractor.RuntimeBase.Analyze
{
    public class FieldDescription
    {
        public string Name { get; set; }
        public TypeDescription TypeDescription { get; set; }
        public bool IsStatic { get; set; }
    }
}