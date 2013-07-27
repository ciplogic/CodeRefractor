#region Usings

using System.Reflection;
using Mono.Reflection;

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
    }
}