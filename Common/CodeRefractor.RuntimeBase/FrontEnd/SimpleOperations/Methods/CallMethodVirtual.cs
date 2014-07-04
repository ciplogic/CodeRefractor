using System.Collections.Generic;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.MiddleEnd.SimpleOperations.Methods
{
    public class CallMethodVirtual : CallMethodStatic
    {
        public CallMethodVirtual(MethodInterpreter interpreter)
            : base(interpreter)
        {
            Parameters = new List<IdentifierValue>();

            Interpreter = interpreter;
            Info = interpreter.Method;
        }
    }
}