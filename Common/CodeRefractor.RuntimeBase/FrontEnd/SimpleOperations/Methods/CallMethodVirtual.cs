#region Uses

using System.Collections.Generic;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.Methods
{
    public class CallMethodVirtual : CallMethodStatic
    {
        public CallMethodVirtual(MethodInterpreter interpreter)
            : base(interpreter)
        {
            Kind = OperationKind.CallVirtual;
            Parameters = new List<IdentifierValue>();

            Interpreter = interpreter;
            Info = interpreter.Method;
        }
    }
}