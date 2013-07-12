using System;
using CodeRefractor.RuntimeBase;

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof(Math))]
    public class CrMath
    {
        [PureMethod]
        [CppMethodBody(Header="math.h", Code="return sin(a);")]
        public static double Sin(double a)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "return cos(a);")]
        public static double Cos(double a)
        {
            return 0;
        }
        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "return sqrt(d);")]
        public static double Sqrt(double d)
        {
            return 0;
        }
    }
}
