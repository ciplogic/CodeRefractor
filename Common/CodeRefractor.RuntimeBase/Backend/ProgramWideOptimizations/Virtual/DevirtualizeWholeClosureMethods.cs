using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    /// <summary>
    /// removes abstract unused vcalls
    /// </summary>
    class DevirtualizeWholeClosureMethods : ProgramOptimizationBase
    {
        public override bool Optimize(ClosureEntities closure)
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

            var result = usedMethods.Count != closure.AbstractMethods.Count;
            if (result)
            {
                closure.AbstractMethods = usedMethods;
            }
            return result;
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
