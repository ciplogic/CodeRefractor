using CodeRefractor.RuntimeBase.DataBase.SerializeXml;

namespace ClassOpenRuntimeCodeGenerator
{
    [XNode]
    public class Form1ViewModelData
    {
        public bool IsCoreLibraries;
        public string AssemblyPath;
        public string SourceType;
        public string TargetType;
        public bool IsPure;
        public bool IsCppMethod;
        public string Header;
    }
}