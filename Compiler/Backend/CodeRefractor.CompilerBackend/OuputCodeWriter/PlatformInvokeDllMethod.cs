using System;
using System.Runtime.InteropServices;

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    internal class PlatformInvokeDllMethod
    {
        public string Name { get; set; }
        public CallingConvention? CallingConvention { get; set; }
        public string EntryPoint { get; set; }

        public int Id { get; set; }

        public PlatformInvokeDllMethod(string name, CallingConvention? callingConvention = null, string entryPoint = "")
        {
            Name = name;
            CallingConvention = callingConvention;
            EntryPoint = !String.IsNullOrEmpty(entryPoint)? entryPoint:name;
        }


        public string FormattedName()
        {
            return String.Format("dll_method_{0}", Id);
        }
    }
}