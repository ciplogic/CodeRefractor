using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public class ClosureFromEntry
    {
        public ClosureFromEntry()
        {
            MethodInterpreters = new List<MethodInterpreter>();
        }

        public List<MethodInterpreter> MethodInterpreters { get; set; }

        public List<Type> UsedTypes { get; set; }

    }
}