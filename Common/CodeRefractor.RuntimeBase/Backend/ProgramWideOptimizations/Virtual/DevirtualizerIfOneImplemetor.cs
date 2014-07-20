#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    public class DevirtualizerIfOneImplemetor : ResultingProgramOptimizationBase
    {
        protected override void DoOptimize(ClosureEntities closure)
        {
            var methodInterpreters = closure.MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.CilInstructions)
                .ToList();
            foreach (var interpreter in methodInterpreters)
            {
                HandleInterpreterInstructions((CilMethodInterpreter)interpreter, closure.MappedTypes.Values.ToList());
            }
        }

        private void HandleInterpreterInstructions(CilMethodInterpreter interpreter, List<Type> usedTypes)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.CallVirtual).ToList();
            var allOps = useDef.GetLocalOperations();
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (CallMethodStatic) op;
                var callingInterpreterKey = methodData.Interpreter.ToKey();
                var declaringType = callingInterpreterKey.Interpreter.Method.DeclaringType;
                var implementors = declaringType.ImplementorsOfT(usedTypes);
                if (implementors.Count > 0)
                    continue;
                Result = true;
            }
            if (Result)
            {
                interpreter.MidRepresentation.UpdateUseDef();
            }
        }
    }
}