#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    public class DevirtualizerIfOneImplementor : ProgramOptimizationBase
    {
        public override bool Optimize(ClosureEntities closure)
        {
            var methodInterpreters = closure.MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.CilInstructions)
                .Select(mth => (CilMethodInterpreter) mth)
                .ToArray();

            var result = false;
            foreach (var interpreter in methodInterpreters)
            {
                result |= HandleInterpreterInstructions(
                    interpreter,
                    closure.MappedTypes.Values.ToList(),
                    closure);
            }
            return result;
        }

        private static bool HandleInterpreterInstructions(CilMethodInterpreter interpreter, List<Type> usedTypes,
            ClosureEntities closure)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.CallVirtual).ToArray();
            if (calls.Length == 0)
                return false;
            var allOps = useDef.GetLocalOperations();
            var result = false;
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (CallMethodStatic)op;
                var callingInterpreterKey = methodData.Interpreter.Method.ToKey(closure);
                var declaringType = callingInterpreterKey.Interpreter.Method.DeclaringType;
                var implementors = callingInterpreterKey.DeclaringType.ImplementorsOfT(usedTypes);

                implementors.Remove(declaringType);
                if (implementors.Count > 0)
                    continue;
                //TODO: map correct method
                interpreter.MidRepresentation.LocalOperations[callOp] = new CallMethodStatic(methodData.Interpreter)
                {
                    Result = methodData.Result,
                    Parameters = methodData.Parameters
                };
                result = true;
            }
            if (result)
            {
                interpreter.MidRepresentation.UpdateUseDef();
            }
            return result;
        }
    }
}