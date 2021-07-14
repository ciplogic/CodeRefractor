#region Uses

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;

#endregion

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    public class RemoveNotReachableMethos : ProgramOptimizationBase
    {
        public override bool Optimize(ClosureEntities closure)
        {
            // the problem is that since some methods are mapped methods, we need to
            // keep the map of <MethodBaseKey, MethodInterpreter>, since deriving from
            // the MethodInterpreter will lead us to the mapped method instead.

            var methodInterpreters = closure.MethodImplementations
                .Where(m => m.Value.Kind == MethodKind.CilInstructions)
                .Select(m2 => GetKeyFromMethod(closure, m2.Key.Method))
                .ToArray();
            var entryPoint = closure.EntryPoint;
            var candidateMethods = new HashSet<MethodInterpreterKey>
            {
                GetKeyFromMethod(closure, entryPoint)
            };

            foreach (var interpreter in methodInterpreters)
            {
                HandleInterpreterInstructions((CilMethodInterpreter) interpreter.Interpreter, candidateMethods, closure);
            }

            var removeList = new List<MethodInterpreterKey>();

            foreach (var cilMethodInterpreter in methodInterpreters)
            {
                //var methodInterpreterKey = GetKeyFromMethod(closure, cilMethodInterpreter.Key.Method);
                if (!candidateMethods.Contains(cilMethodInterpreter))
                {
                    removeList.Add(cilMethodInterpreter.Interpreter.ToKey());
                }
            }

            foreach (var key in removeList)
            {
                var resultItem = closure.MethodImplementations.FirstOrDefault(m => m.Value.ToKey().Equals(key));
                closure.MethodImplementations.Remove(resultItem.Key);
            }

            return removeList.Count > 0;
        }

        private static MethodInterpreterKey GetKeyFromMethod(ClosureEntities closure, MethodBase entryPoint)
        {
            return entryPoint.ToKey(closure);
        }

        private static void HandleInterpreterInstructions(CilMethodInterpreter interpreter,
            HashSet<MethodInterpreterKey> candidateMethods, ClosureEntities closure)
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