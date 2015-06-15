#region Uses

using System;

#endregion

namespace CodeRefractor.Runtime.Annotations
{
    public class CppMethodBodyAttribute : Attribute
    {
        public string Header { get; set; }
        public string Code { get; set; }
        public string Libraries { get; set; }
    }
}