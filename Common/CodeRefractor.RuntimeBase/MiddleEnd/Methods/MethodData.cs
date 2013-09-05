#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using System.Linq;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.Methods
{
    public class MethodData : IdentifierValue
    {
        public bool IsStatic;
        public bool IsVoid;
        public LocalVariable Result;
        public bool IsPure;
        public List<IdentifierValue> Parameters { get; set; }

        public MethodData(MethodBase info)
        {
            Info = info;
            IsStatic = info.IsStatic;
            Parameters = new List<IdentifierValue>();
            IsVoid = info.GetReturnType() == typeof (void);
            IsPure = PureMethodTable.ComputeMethodPurity(info.ToString());
        }

        public MethodBase Info { get; set; }

        public void ExtractNeededValuesFromStack(EvaluatorStack evaluatorStack)
        {
            var stack = evaluatorStack.Stack;
            var methodParams = Info.GetParameters();
            foreach (var t in methodParams)
            {
                Parameters.Insert(0, stack.Pop());
            }
            if (!IsStatic)
                Parameters.Insert(0, stack.Pop());
        }

        public override string ToString()
        {
            var paramData = string.Join(", ",
                                        Parameters.Select(
                                            par =>
                                            string.Format("{0}:{1}", par.Name, par.ComputedType().Name)));
            return String.Format("Call {0} = {1}({2});", Result!=null? Result.Name :"void", Info.Name, paramData);
        }
    }
}