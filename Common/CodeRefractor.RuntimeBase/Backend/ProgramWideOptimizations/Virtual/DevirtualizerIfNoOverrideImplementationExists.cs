#region Uses

using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.ClosureCompute.Steps;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    public class DevirtualizerIfNoOverrideImplementationExists : ProgramOptimizationBase
    {
        public override bool Optimize(ClosureEntities closure)
        {
            //Interfaces cannot be devirtualized this way
            var methodInterpreters = closure.MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.CilInstructions)
                .Select(mth => (CilMethodInterpreter) mth)
                .ToArray();
            var result = false;
            foreach (var interpreter in methodInterpreters)
            {
                result |= HandleInterpreterInstructions(interpreter, closure);
            }
            return result;
        }

        private static bool HandleInterpreterInstructions(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.CallVirtual);
            var allOps = useDef.GetLocalOperations();
            var result = false;

            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (CallMethodStatic)op;
                var thisParameter = (LocalVariable)methodData.Parameters.First();
                var clrType = thisParameter.FixedType.GetClrType(closure);

                var overridenTypes = clrType.ImplementorsOfT(closure);
                overridenTypes.Remove(clrType);

                //Check for NewSlot

                if (clrType.BaseType != null && !methodData.Info.IsVirtual)
                {
                    if (
                        clrType.BaseType.GetMethods(ClosureEntitiesBuilder.AllFlags)
                            .Select(m => m.MethodMatches(methodData.Info))
                            .Any())
                        continue;
                }

                if (overridenTypes.Count >= 1)
                    continue;

                //TODO: map correct method
                var resolvedMethod = AddVirtualMethodImplementations.GetImplementingMethod(clrType,
                    (MethodInfo)methodData.Info);
                methodData.Interpreter = closure.ResolveMethod(resolvedMethod);
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