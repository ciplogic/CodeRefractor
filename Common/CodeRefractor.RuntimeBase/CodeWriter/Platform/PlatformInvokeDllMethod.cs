#region Usings

using System;
using System.Runtime.InteropServices;

#endregion

namespace CodeRefractor.CodeWriter.Platform
{
    public class PlatformInvokeDllMethod
    {
        public string Name { get; set; }
        public CallingConvention? CallingConvention { get; set; }
        public string EntryPoint { get; set; }

        public int Id { get; set; }

        public PlatformInvokeDllMethod(string name, CallingConvention? callingConvention = null, string entryPoint = "")
        {
            Name = name;
            CallingConvention = callingConvention;
            EntryPoint = !String.IsNullOrEmpty(entryPoint) ? entryPoint : name;
        }


        public string FormattedName()
        {
            return String.Format("dll_method_{0}", Id);
        }
    }
}