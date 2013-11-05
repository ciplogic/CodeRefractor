#region Usings

using System;
using System.Reflection;
using System.Reflection.Emit;

#endregion

namespace MsilReader
{
    public static class BackingFieldResolver
    {
        private class FieldPattern : ILPattern
        {
            public static object FieldKey = new object();

            private ILPattern pattern;

            public FieldPattern(ILPattern pattern)
            {
                this.pattern = pattern;
            }

            public override void Match(MatchContext context)
            {
                pattern.Match(context);
                if (!context.success)
                    return;

                var match = GetLastMatchingInstruction(context);
                var field = (FieldInfo) match.Operand;
                context.AddData(FieldKey, field);
            }
        }

        private static ILPattern Field(OpCode opcode)
        {
            return new FieldPattern(ILPattern.OpCode(opcode));
        }

        private static ILPattern GetterPattern =
            ILPattern.Sequence(
                ILPattern.Optional(OpCodes.Nop),
                ILPattern.Either(
                    Field(OpCodes.Ldsfld),
                    ILPattern.Sequence(
                        ILPattern.OpCode(OpCodes.Ldarg_0),
                        Field(OpCodes.Ldfld))),
                ILPattern.Optional(
                    ILPattern.Sequence(
                        ILPattern.OpCode(OpCodes.Stloc_0),
                        ILPattern.OpCode(OpCodes.Br_S),
                        ILPattern.OpCode(OpCodes.Ldloc_0))),
                ILPattern.OpCode(OpCodes.Ret));

        private static ILPattern SetterPattern =
            ILPattern.Sequence(
                ILPattern.Optional(OpCodes.Nop),
                ILPattern.OpCode(OpCodes.Ldarg_0),
                ILPattern.Either(
                    Field(OpCodes.Stsfld),
                    ILPattern.Sequence(
                        ILPattern.OpCode(OpCodes.Ldarg_1),
                        Field(OpCodes.Stfld))),
                ILPattern.OpCode(OpCodes.Ret));

        private static FieldInfo GetBackingField(MethodInfo method, ILPattern pattern)
        {
            var result = ILPattern.Match(method, pattern);
            if (!result.success)
                throw new ArgumentException();

            object value;
            if (!result.TryGetData(FieldPattern.FieldKey, out value))
                throw new InvalidOperationException();

            return (FieldInfo) value;
        }

        public static FieldInfo GetBackingField(this PropertyInfo self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            var getter = self.GetGetMethod(true);
            if (getter != null)
                return GetBackingField(getter, GetterPattern);

            var setter = self.GetSetMethod(true);
            if (setter != null)
                return GetBackingField(setter, SetterPattern);

            throw new ArgumentException();
        }
    }
}