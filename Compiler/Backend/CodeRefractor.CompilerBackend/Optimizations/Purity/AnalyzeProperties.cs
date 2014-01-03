namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeProperties
    {
        public bool IsPure { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsSetter { get; set; }
        public bool IsGetter { get; set; }
        public bool NoStaticSideEffects { get; set; }
    }
}