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
    public class CppLibMethodInvokeRepresentation
    {
        public string LibraryName { get; set; }
        public string HeaderName { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(HeaderName) && string.IsNullOrEmpty(LibraryName))
                return string.Empty;
            return string.Format("#include <{0}> - Lib: <{1}>", HeaderName, LibraryName);
        }
    }
}