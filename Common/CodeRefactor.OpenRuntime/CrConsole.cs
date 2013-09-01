#region Usings

using System;
using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (Console))]
    public class CrConsole
    {
        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%lf\\n\", value);")]
        public static void WriteLine(double value)
        {
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%d\\n\", value);")]
        public static void WriteLine(int value)
        {
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%d\", value);")]
        public static void Write(int value)
        {
        }

        [CilMethod]
        public static void WriteLine(float value)
        {
            WriteLine((double) value);
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%ls\", value.get());")]
        public static void Write(string value)
        {
        }
        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%ls\\n\", value.get());")]
        public static void WriteLine(string value)
        {
        }

    }
}