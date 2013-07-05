using System;
using CodeRefractor.RuntimeBase;

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof(Math))]
    public class CrMath
    {
        [CppMethodBody(Header="math.h", Code="return sin(d);")]
        public static double Sin(double d)
        {
            return 0;
        }

        [CppMethodBody(Header = "math.h", Code = "return cos(d);")]
        public static double Cos(double d)
        {
            return 0;
        }
        [CppMethodBody(Header = "math.h", Code = "return sqrt(d);")]
        public static double Sqrt(double d)
        {
            return 0;
        }
    }
}
