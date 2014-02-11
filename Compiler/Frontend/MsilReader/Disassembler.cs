#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace MsilReader
{
    public static class Disassembler
    {
        public static Instruction[] GetInstructions(this MethodBase self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            return MethodBodyReader.GetInstructions(self);
        }
    }
}