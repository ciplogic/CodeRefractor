#region Uses

using System.Collections.Generic;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters;

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

        public override string ToString()
        {
            return $"{base.ToString()} (vcall)";
        }
    }
}