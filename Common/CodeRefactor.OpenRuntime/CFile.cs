using System;
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
        public static extern IntPtr fopen(string fileName, string mode);

        [CppLibMethod(
            Header = "stdio.h",
            Code = "return fclose(fileHandle);",
            Library = ""
            )]
        public static extern void fclose(IntPtr fileHandle);
    }
}