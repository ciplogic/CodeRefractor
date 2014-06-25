using System;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase;

namespace CodeRefactor.OpenRuntime
{
    public static class CFile
    {
        [CppLibMethod(
            Header = "stdio.h",
            Code = "return fopen(fileName, mode);",
            Library = ""
            )]
        public static IntPtr fopen(string fileName, string mode)
        {
            return IntPtr.Zero;
        }

        [CppLibMethod(
            Header = "stdio.h",
            Code = "return fclose(fileHandle);",
            Library = ""
            )]
        public static void fclose(IntPtr fileHandle)
        {
        }
    }
}