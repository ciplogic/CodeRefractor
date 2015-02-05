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
    /// <summary>
    /// This optimization should remove all unreferenced methods which:
    /// - are not the entry methods
    /// - are not called by transitive methods called by entry methods or any entry methods
    /// - are not called by used virtual methods
    /// </summary>
    public class RemoveNotReachableMethos : ResultingProgramOptimizationBase
    {
        protected override void DoOptimize(ClosureEntities closure)
        {

            //TODO: fix the logic 
            return;
            // the problem is that since some methods are mapped methods, we need to
            // keep the map of <MethodBaseKey, MethodInterpreter>, since deriving from
            // the MethodInterpreter will lead us to the mapped method instead.

            var methodInterpreters = closure.MethodImplementations
                .Where(m => m.Value.Kind == MethodKind.CilInstructions);

            var entryPoint = closure.EntryPoint;
            var candidateMethods = new HashSet<MethodInterpreterKey>
            {
                GetKeyFromMethod(closure, entryPoint)
            };

            foreach (var interpreter in methodInterpreters)
            {
                HandleInterpreterInstructions((CilMethodInterpreter) interpreter.Value, candidateMethods, closure);
            }
            
            var removeList = new List<MethodBaseKey>();

            foreach (var cilMethodInterpreter in methodInterpreters)
            {
                var methodInterpreterKey = cilMethodInterpreter.Value.ToKey(); 
                if (!candidateMethods.Contains(methodInterpreterKey))
                    removeList.Add(cilMethodInterpreter.Key);
            }

            foreach (var key in removeList)
            {
                closure.MethodImplementations.Remove(key);
            }

            Result = removeList.Count > 0;
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