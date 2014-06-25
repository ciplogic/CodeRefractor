using System;

namespace CodeRefractor.Runtime.Annotations
{
    public class CppLibMethodAttribute : Attribute
    {
        public string Header { get; set; }
        public string Library { get; set; }

        public string Code { get; set; }
    }
}