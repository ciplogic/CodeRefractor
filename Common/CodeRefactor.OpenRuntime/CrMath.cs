#region Usings

using System;
using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (Math))]
    public class CrMath
    {
        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "return sin(a);")]
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

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Acos(System.Double d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Asin(System.Double d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Atan(System.Double d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Atan2(System.Double y, System.Double x)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Ceiling(System.Decimal d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Ceiling(System.Double a)
        {
            return 0;
        }

     
        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Cosh(System.Double value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Floor(System.Decimal d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Floor(System.Double d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Tan(System.Double a)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Sinh(System.Double value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Tanh(System.Double value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Round(System.Double a)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Round(System.Double value, System.Int32 digits)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Round(System.Double value, System.MidpointRounding mode)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Round(System.Double value, System.Int32 digits, System.MidpointRounding mode)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Round(System.Decimal d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Round(System.Decimal d, System.Int32 decimals)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Round(System.Decimal d, System.MidpointRounding mode)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Round(System.Decimal d, System.Int32 decimals, System.MidpointRounding mode)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Truncate(System.Decimal d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Truncate(System.Double d)
        {
            return 0;
        }

     
        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Log(System.Double d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Log10(System.Double d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Exp(System.Double d)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Pow(System.Double x, System.Double y)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double IEEERemainder(System.Double x, System.Double y)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.SByte Abs(System.SByte value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int16 Abs(System.Int16 value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Abs(System.Int32 value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int64 Abs(System.Int64 value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Single Abs(System.Single value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Abs(System.Double value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Abs(System.Decimal value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.SByte Max(System.SByte val1, System.SByte val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Byte Max(System.Byte val1, System.Byte val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int16 Max(System.Int16 val1, System.Int16 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.UInt16 Max(System.UInt16 val1, System.UInt16 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Max(System.Int32 val1, System.Int32 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.UInt32 Max(System.UInt32 val1, System.UInt32 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int64 Max(System.Int64 val1, System.Int64 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.UInt64 Max(System.UInt64 val1, System.UInt64 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Single Max(System.Single val1, System.Single val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Max(System.Double val1, System.Double val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Max(System.Decimal val1, System.Decimal val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.SByte Min(System.SByte val1, System.SByte val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Byte Min(System.Byte val1, System.Byte val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int16 Min(System.Int16 val1, System.Int16 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.UInt16 Min(System.UInt16 val1, System.UInt16 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Min(System.Int32 val1, System.Int32 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.UInt32 Min(System.UInt32 val1, System.UInt32 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int64 Min(System.Int64 val1, System.Int64 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.UInt64 Min(System.UInt64 val1, System.UInt64 val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Single Min(System.Single val1, System.Single val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Min(System.Double val1, System.Double val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Decimal Min(System.Decimal val1, System.Decimal val2)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Double Log(System.Double a, System.Double newBase)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Sign(System.SByte value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Sign(System.Int16 value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Sign(System.Int32 value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Sign(System.Int64 value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Sign(System.Single value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Sign(System.Double value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 Sign(System.Decimal value)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int64 BigMul(System.Int32 a, System.Int32 b)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int32 DivRem(System.Int32 a, System.Int32 b, ref System.Int32 result)
        {
            return 0;
        }

        [PureMethod]
        [CppMethodBody(Header = "math.h", Code = "")]
        public static System.Int64 DivRem(System.Int64 a, System.Int64 b, ref System.Int64 result)
        {
            return 0;
        }
    }
}
