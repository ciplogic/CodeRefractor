namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class RuntimeLibraryDescription
    {
        public string Source { get; set; }
        public string HeaderName { get; set; }

        public override string ToString()
        {
            return Source;
        }
    }
}