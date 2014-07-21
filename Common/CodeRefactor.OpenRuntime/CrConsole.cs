#region Usings

using System;
using CodeRefractor.Runtime.Annotations;

#endregion

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof(Console))]
    public class CrConsole
    {
        [CppMethodBody(Header = "stdio.h", Code = "printf(\"\\n\");")]
        public static void WriteLine()
        {
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%d\", value);")]
        public static void Write(int value)
        {
        }


        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%d\\n\", value);")]
        public static void WriteLine(int value)
        {
        }

      

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%d\", value);")]
        public static void Write(long value)
        {
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%d\\n\", value);")]
        public static void WriteLine(long value)
        {
        }

      


        [CppMethodBody(Header = "stdio.h",
           Code = @"
                    if(value) 
                        printf(""True"");
                    else
                        printf(""False"");")]
        public static void Write(bool value)
        {
        }

        [CppMethodBody(Header = "stdio.h", 
            Code = @"
                    if(value) 
                        printf(""True\"""");
                    else
                        printf(""False\"""");")]
        public static void WriteLine(bool value)
        {
        }


       

        public static void WriteLine(float value)
        {
            WriteLine((double)value);
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%ls\", value.get()->Text->Items);")]
        public static void Write(string value)
        {
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%ls\\n\", value.get()->Text->Items);")]
        public static void WriteLine(string value)
        {
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%lf\", value);")]
        public static void Write(double value)
        {
        }

        [CppMethodBody(Header = "stdio.h", Code = "printf(\"%lf\\n\", value);")]
        public static void WriteLine(double value)
        {
        }
    }
}