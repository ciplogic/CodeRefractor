using System.Collections.Generic;
using System.Runtime.InteropServices;
using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MethodDescription
    {
        public MethodDescription()
        {
            IsStatic = true;
        }

        public TypeDescription DeclaringType { get; set; }
        public bool IsStatic { get; set; }
        public CallingConvention CallingConvention { get; set; }
        public string MethodName { get; set; }

        public string EntryPoint { get; set; }
        public string LibraryName { get; set; }

        public TypeDescription ReturnType { get; set; }
        public List<FieldDescription> Arguments { get; set; }
    }
}