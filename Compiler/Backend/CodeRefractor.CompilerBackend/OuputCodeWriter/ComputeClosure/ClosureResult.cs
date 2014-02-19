using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.MiddleEnd;

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

        public List<Type> ImplementorsOfT(Type t)
        {
            var result = UsedTypes.Where(usedType => usedType.IsSubclassOf(t)).ToList();
            return result;
        } 

    }
}