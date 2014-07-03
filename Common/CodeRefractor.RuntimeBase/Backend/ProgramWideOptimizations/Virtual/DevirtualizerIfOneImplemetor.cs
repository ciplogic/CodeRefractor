#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations.Virtual
{
    public class DevirtualizerIfOneImplemetor : ResultingProgramOptimizationBase
    {
        protected override void DoOptimize(ClosureEntities closure)
        {
            var methodInterpreters = closure.MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.Default)
                .ToList();
            foreach (var interpreter in methodInterpreters)
            {
                HandleInterpreterInstructions(interpreter, closure.MappedTypes.Values.ToList());
            }
        }

        private void HandleInterpreterInstructions(MethodInterpreter interpreter, List<Type> usedTypes)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.CallVirtual).ToList();
            var allOps = useDef.GetLocalOperations();
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (MethodData) op;
                var callingInterpreterKey = methodData.Interpreter.ToKey();
                var declaringType = callingInterpreterKey.Interpreter.DeclaringType;
                var implementors = declaringType.ClrType.ImplementorsOfT(usedTypes);
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