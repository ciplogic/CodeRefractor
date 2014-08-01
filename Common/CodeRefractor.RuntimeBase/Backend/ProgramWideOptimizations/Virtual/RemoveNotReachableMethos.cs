using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    public class RemoveNotReachableMethos : ResultingProgramOptimizationBase
    {
        protected override void DoOptimize(ClosureEntities closure)
        {
            var methodInterpreters = closure.MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.CilInstructions)
                .Select(mth => (CilMethodInterpreter)mth)
                .ToArray();

            var entryPoint = closure.EntryPoint;
            var candidateMethods = new HashSet<MethodInterpreterKey>
            {
                GetKeyFromMethod(closure, entryPoint)
            };

            foreach (var interpreter in methodInterpreters)
            {
                HandleInterpreterInstructions(interpreter, candidateMethods, closure);
            }
            var removeList = new List<MethodBaseKey>();
            foreach (var cilMethodInterpreter in methodInterpreters)
            {
                var key = cilMethodInterpreter.ToKey();
                if (!candidateMethods.Contains(key))
                    removeList.Add(key.Interpreter.Method.ToKey());
            }
            foreach (var key in removeList)
            {
                closure.MethodImplementations.Remove(key);
            }

            Result = removeList.Count>0;
        }

        private static MethodInterpreterKey GetKeyFromMethod(ClosureEntities closure, MethodBase entryPoint)
        {
            return closure.ResolveMethod(entryPoint).ToKey();
        }

        private static void HandleInterpreterInstructions(CilMethodInterpreter interpreter, HashSet<MethodInterpreterKey> candidateMethods, ClosureEntities closure)
        {
            var clrMethod = interpreter.Method;
            if (IsPossibleOverride(closure.AbstractMethods, clrMethod))
                candidateMethods.Add(GetKeyFromMethod(closure, clrMethod));

            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.Call).ToList();
            var allOps = useDef.GetLocalOperations();
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (CallMethodStatic)op;
                var callingInterpreterKey = methodData.Interpreter.ToKey();
                candidateMethods.Add(callingInterpreterKey);
            }
        }

        private static bool IsPossibleOverride(HashSet<MethodInfo> abstractMethods, MethodBase clrMethod)
        {
            //TODO: make the match to be as precise as possible
            var clrParams = clrMethod.GetParameters();
            foreach (var abstractMethod in abstractMethods)
            {
                if (clrMethod.Name != abstractMethod.Name) continue;
                var abstractParams = abstractMethod.GetParameters();
                if (clrParams.Length == abstractParams.Length)
                {
                    return true;
                }
            }
            return false;
        }
    }
}