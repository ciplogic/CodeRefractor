using System;
using CodeRefractor.RuntimeBase;

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof(Math))]
    public class CrMath
    {
        [CppMethodBody(Header="math.h", Code="return sin(a);", IsPure=true)]
        public static double Sin(double a)
        {
            return 0;
        }

        [CppMethodBody(Header = "math.h", Code = "return cos(a);", IsPure = true)]
        public static double Cos(double a)
        {
            return 0;
        }
        [CppMethodBody(Header = "math.h", Code = "return sqrt(d);", IsPure = true)]
        public static double Sqrt(double d)
        {
            return 0;
        }
    }
}
