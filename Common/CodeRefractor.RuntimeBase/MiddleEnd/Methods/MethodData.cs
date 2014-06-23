#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.Methods
{
    public class MethodData : LocalOperation
    {
        public bool IsVoid
        {
            get { return _interpreter.Method.GetReturnType() == typeof (void); }
        }

        public LocalVariable Result;
        private MethodInterpreter _interpreter;
        public List<IdentifierValue> Parameters { get; set; }

        public MethodInterpreter Interpreter
        {
            get { return _interpreter; }
            set { _interpreter = value; }
        }

        public MethodData(MethodInterpreter interpreter, OperationKind kind) : base(kind)
        {
            Parameters = new List<IdentifierValue>();

            Interpreter = interpreter;
            Info = interpreter.Method;
        }

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
                        string.Format("{0}:{1}", par.Name, par.ComputedType().Name)));
            return String.Format("Call {0} = {1}({2});", Result != null ? Result.Name : "System_Void", Info.Name, paramData);
        }
    }
}