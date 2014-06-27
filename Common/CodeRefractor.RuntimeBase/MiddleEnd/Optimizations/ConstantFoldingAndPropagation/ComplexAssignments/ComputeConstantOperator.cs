#region Usings

using System;
using System.IO;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    internal static class ComputeConstantOperator
    {
        public static object ComputeDouble(ConstValue constLeft)
        {
            return Convert.ToDouble(constLeft.Value);
        }

        public static object ComputeNeg(ConstValue constLeft)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int16:
                    return -(Int16)constLeft.Value;
                case TypeCode.Int64:
                    return -(long) constLeft.Value;
                case TypeCode.Int32:
                    return -(int) constLeft.Value;
                case TypeCode.Double:
                    return -(double) constLeft.Value;
                case TypeCode.Single:
                    return -(float) constLeft.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        public static object ComputeAdd(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int16:
                    return (Int16)constLeft.Value + (Int16)constRight.Value;
                case TypeCode.Int64:
                    return (long) constLeft.Value + (long) constRight.Value;
                case TypeCode.Int32:
                    return (int) constLeft.Value + (int) constRight.Value;
                case TypeCode.Double:
                    return (double) constLeft.Value + (double) constRight.Value;
                case TypeCode.Single:
                    return (float) constLeft.Value + (float) constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        public static object ComputeSub(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int64:
                    return (long) constLeft.Value - (long) constRight.Value;
                case TypeCode.Int32:
                    return (int) constLeft.Value - (int) constRight.Value;
                case TypeCode.Double:
                    return (double) constLeft.Value - (double) constRight.Value;
                case TypeCode.Single:
                    return (float) constLeft.Value - (float) constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        public static object ComputeMul(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int32:
                    return (int) constLeft.Value*(int) constRight.Value;
                case TypeCode.Double:
                    return (double) constLeft.Value*(double) constRight.Value;
                case TypeCode.Single:
                    return (float) constLeft.Value*(float) constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        public static object ComputeDiv(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int64:
                    return (long) constLeft.Value/(long) constRight.Value;
                case TypeCode.Int32:
                    return (int) constLeft.Value/(int) constRight.Value;
                case TypeCode.Double:
                    return (double) constLeft.Value/(double) constRight.Value;
                case TypeCode.Single:
                    return (float) constLeft.Value/(float) constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        public static object ComputeRem(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value%(int) constRight.Value;
        }


        public static object ComputeOr(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value | (int) constRight.Value;
        }

        public static object ComputeAnd(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value & (int) constRight.Value;
        }

        public static object ComputeXor(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value ^ (int) constRight.Value;
        }


        public static object ComputeFloat(ConstValue constLeft)
        {
            return Convert.ToSingle(constLeft.Value);
        }

        public static object ComputeCeq(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value == (int) constRight.Value ? 1 : 0;
        }

        public static object ComputeCgt(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value > (int) constRight.Value ? 1 : 0;
        }

        public static object ComputeClt(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value < (int) constRight.Value ? 1 : 0;
        }
    }
}