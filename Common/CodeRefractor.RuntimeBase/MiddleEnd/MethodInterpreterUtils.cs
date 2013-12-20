#region Usings

using System.Reflection;
using MsilReader;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public static class MethodInterpreterUtils
    {
        public static int GetIntOperand(this Instruction instruction)
        {
            var localVarInfo = instruction.Operand as LocalVariableInfo;
            if (localVarInfo != null)
                return localVarInfo.LocalIndex;
            return instruction.Operand.ToString().ToInt();
        }
        public static long GetLongOperand(this Instruction instruction)
        {
            var localVarInfo = instruction.Operand as LocalVariableInfo;
            if (localVarInfo != null)
                return localVarInfo.LocalIndex;
            return instruction.Operand.ToString().ToLong();
        }
    }
}