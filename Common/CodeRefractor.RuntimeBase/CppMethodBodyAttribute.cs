using System;

namespace CodeRefractor.RuntimeBase
{
    public class CppMethodBodyAttribute : Attribute
    {
        public string Header { get; set; }

        public string Code { get; set; }
    }
}