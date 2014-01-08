namespace CodeRefractor.RuntimeBase.MiddleEnd
{
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