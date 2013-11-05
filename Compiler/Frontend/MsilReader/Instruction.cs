#region Usings

using System.Reflection.Emit;
using System.Text;

#endregion

namespace MsilReader
{
    public sealed class Instruction
    {
        private int offset;
        private OpCode opcode;
        private object operand;

        public int Offset
        {
            get { return offset; }
        }

        public OpCode OpCode
        {
            get { return opcode; }
        }

        public object Operand
        {
            get { return operand; }
            internal set { operand = value; }
        }

        public Instruction Previous { get; internal set; }

        public Instruction Next { get; internal set; }

        public int Size
        {
            get
            {
                var size = opcode.Size;

                switch (opcode.OperandType)
                {
                    case OperandType.InlineSwitch:
                        size += (1 + ((int[]) operand).Length)*4;
                        break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        size += 8;
                        break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:
                        size += 4;
                        break;
                    case OperandType.InlineVar:
                        size += 2;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        size += 1;
                        break;
                }

                return size;
            }
        }

        internal Instruction(int offset, OpCode opcode)
        {
            this.offset = offset;
            this.opcode = opcode;
        }

        public override string ToString()
        {
            var instruction = new StringBuilder();

            AppendLabel(instruction, this);
            instruction.Append(':');
            instruction.Append(' ');
            instruction.Append(opcode.Name);

            if (operand == null)
                return instruction.ToString();

            instruction.Append(' ');

            switch (opcode.OperandType)
            {
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    AppendLabel(instruction, (Instruction) operand);
                    break;
                case OperandType.InlineSwitch:
                    var labels = (Instruction[]) operand;
                    for (var i = 0; i < labels.Length; i++)
                    {
                        if (i > 0)
                            instruction.Append(',');

                        AppendLabel(instruction, labels[i]);
                    }
                    break;
                case OperandType.InlineString:
                    instruction.Append('\"');
                    instruction.Append(operand);
                    instruction.Append('\"');
                    break;
                default:
                    instruction.Append(operand);
                    break;
            }

            return instruction.ToString();
        }

        private static void AppendLabel(StringBuilder builder, Instruction instruction)
        {
            builder.Append("IL_");
            builder.Append(instruction.offset.ToString("x4"));
        }
    }
}