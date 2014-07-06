namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public enum CppKinds
    {
        RuntimeLibrary,
        StaticLibrary,
    }

    public class CppRepresentation
    {
        public CppKinds Kind { get; set; }
        public string Library { get; set; }
        public string EntryPoint { get; set; }

        public string Source { get; set; }
        public string Header { get; set; }
    }
}