using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    class DevirtualizeWholeClosureMethods : ResultingProgramOptimizationBase
    {
        //removes abstract unused vcalls
        protected override void DoOptimize(ClosureEntities closure)
        {
            var methodInterpreters = closure.MethodImplementations.Values
                   .Where(m => m.Kind == MethodKind.CilInstructions)
                   .Select(mth => (CilMethodInterpreter)mth)
                   .ToArray();
            var usedMethods = new HashSet<MethodInfo>();
            foreach (var interpreter in methodInterpreters)
            {
                HandleInterpreterInstructions(interpreter, usedMethods);
            }

            Result = usedMethods.Count != closure.AbstractMethods.Count;
            if (Result)
            {
                closure.AbstractMethods = usedMethods;
            }
        }
        private static void HandleInterpreterInstructions(CilMethodInterpreter interpreter, HashSet<MethodInfo> usedMethods)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.CallVirtual).ToList();
            var allOps = useDef.GetLocalOperations();
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (CallMethodStatic)op;
                usedMethods.Add((MethodInfo) methodData.Info);
            }
        }
    }
}
