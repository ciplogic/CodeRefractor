#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter.ComputeClosure
{
    public class ClosureResult
    {
        public ClosureResult()
        {
            MethodInterpreters = new Dictionary<MethodInterpreterKey, MethodInterpreter>();
        }

        public Dictionary<MethodInterpreterKey, MethodInterpreter> MethodInterpreters { get; set; }

        public HashSet<Type> UsedTypes { get; set; }
    }
}