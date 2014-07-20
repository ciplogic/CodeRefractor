using System;
using CodeRefractor.Runtime.Annotations;

[ExtensionsImplementation(typeof(Math))]
public static class MathImpl
{

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "return sin(a);")]
    public static double Sin(double a)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "return cos(a);")]
    public static double Cos(double a)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "return sqrt(d);")]
    public static double Sqrt(double d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Acos(Double d)
    {
        return 0;
    }

     [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Asin(Double d)
    {
        return 0;
    }

     [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Atan(Double d)
    {
        return 0;
    }

     [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Atan2(Double y, Double x)
    {
        return 0;
    }

     [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Ceiling(Decimal d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Ceiling(Double a)
    {
        return 0;
    }


    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Cosh(Double value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Floor(Decimal d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Floor(Double d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Tan(Double a)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Sinh(Double value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Tanh(Double value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Round(Double a)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Round(Double value, Int32 digits)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Round(Double value, MidpointRounding mode)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Round(Double value, Int32 digits, MidpointRounding mode)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Round(Decimal d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Round(Decimal d, Int32 decimals)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Round(Decimal d, MidpointRounding mode)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Round(Decimal d, Int32 decimals, MidpointRounding mode)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Truncate(Decimal d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Truncate(Double d)
    {
        return 0;
    }


    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Log(Double d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Log10(Double d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Exp(Double d)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Pow(Double x, Double y)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double IEEERemainder(Double x, Double y)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static SByte Abs(SByte value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int16 Abs(Int16 value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Abs(Int32 value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int64 Abs(Int64 value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Single Abs(Single value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Abs(Double value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Abs(Decimal value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static SByte Max(SByte val1, SByte val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Byte Max(Byte val1, Byte val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int16 Max(Int16 val1, Int16 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static UInt16 Max(UInt16 val1, UInt16 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Max(Int32 val1, Int32 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static UInt32 Max(UInt32 val1, UInt32 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int64 Max(Int64 val1, Int64 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static UInt64 Max(UInt64 val1, UInt64 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Single Max(Single val1, Single val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Max(Double val1, Double val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Max(Decimal val1, Decimal val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static SByte Min(SByte val1, SByte val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Byte Min(Byte val1, Byte val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int16 Min(Int16 val1, Int16 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static UInt16 Min(UInt16 val1, UInt16 val2)
    {
        return 0;
    }

    public static Int32 Min(Int32 val1, Int32 val2)
    {
        return val1 < val2 ? val1 : val2;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static UInt32 Min(UInt32 val1, UInt32 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int64 Min(Int64 val1, Int64 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static UInt64 Min(UInt64 val1, UInt64 val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Single Min(Single val1, Single val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Min(Double val1, Double val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Decimal Min(Decimal val1, Decimal val2)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Double Log(Double a, Double newBase)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Sign(SByte value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Sign(Int16 value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Sign(Int32 value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Sign(Int64 value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Sign(Single value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Sign(Double value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 Sign(Decimal value)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int64 BigMul(Int32 a, Int32 b)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int32 DivRem(Int32 a, Int32 b, ref Int32 result)
    {
        return 0;
    }

    [MapMethod(IsStatic = true)]
    [CppMethodBody(Header = "math.h", Code = "")]
    public static Int64 DivRem(Int64 a, Int64 b, ref Int64 result)
    {
        return 0;
    }
}