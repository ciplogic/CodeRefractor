#region Uses

using System.Runtime.InteropServices;

#endregion

namespace CodeRefractor.CodeWriter.Platform
{
    public class PlatformInvokeDllMethod
    {
        public PlatformInvokeDllMethod(string name, CallingConvention? callingConvention = null, string entryPoint = "")
        {
            Name = name;
            CallingConvention = callingConvention;
            EntryPoint = !string.IsNullOrEmpty(entryPoint) ? entryPoint : name;
        }

        public string Name { get; set; }
        public CallingConvention? CallingConvention { get; set; }
        public string EntryPoint { get; set; }
        public int Id { get; set; }

        public string FormattedName()
        {
            return $"dll_method_{Id}";
        }
    }
}