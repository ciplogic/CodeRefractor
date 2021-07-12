#region Uses

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Methods
{
    public class CallMethodStatic : LocalOperation
    {
        public LocalVariable Result;

        public CallMethodStatic(MethodInterpreter interpreter)
            : base(OperationKind.Call)
        {
            Parameters = new List<IdentifierValue>();

            Interpreter = interpreter;
            Info = interpreter.Method;
        }

        public bool IsVoid => Interpreter.Method.GetReturnType() == typeof (void);

        public List<IdentifierValue> Parameters { get; set; }
        public MethodInterpreter Interpreter { get; set; }
        public MethodBase Info { get; set; }

        public void ExtractNeededValuesFromStack(EvaluatorStack evaluatorStack)
        {
            var methodParams = Info.GetParameters();
            if (Info.IsConstructor)
            {
                Parameters.Insert(0, evaluatorStack.Pop());
                foreach (var t in methodParams)
                    Parameters.Insert(1, evaluatorStack.Pop());
                return;
            }
            foreach (var t in methodParams)
                Parameters.Insert(0, evaluatorStack.Pop());
            if (!Info.IsStatic)
                Parameters.Insert(0, evaluatorStack.Pop());
        }

        public override string ToString()
        {
            var paramData = string.Join(", ",
                Parameters.Select(
                    par =>
                        $"{par.Name}:{par.ComputedType().Name}"));
            if (Result == null)
            {
                return $"{Info.Name}({paramData})";
            }
            return $"{Result.Name} = {Info.Name}({paramData})";
        }
    }
}