#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.Backend.ComputeClosure
{
    public class ClosureResult
    {
        public ClosureResult()
        {
            MethodInterpreters = new Dictionary<MethodInterpreterKey, MethodInterpreter>();
            UsedTypes = new HashSet<Type>();
        }

        public Dictionary<MethodInterpreterKey, MethodInterpreter> MethodInterpreters { get; set; }

        public HashSet<Type> UsedTypes { get; set; }
    }
}