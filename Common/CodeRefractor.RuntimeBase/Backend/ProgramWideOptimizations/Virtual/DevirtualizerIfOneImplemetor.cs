#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
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
                .Select(mth=>(CilMethodInterpreter)mth)
                .ToArray();
            foreach (var interpreter in methodInterpreters)
            {
                Result |= HandleInterpreterInstructions(interpreter, closure.MappedTypes.Values.ToList());
            }
        }

        private static bool HandleInterpreterInstructions(CilMethodInterpreter interpreter, List<Type> usedTypes)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.CallVirtual).ToList();
            var allOps = useDef.GetLocalOperations();
            var result = false;
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (CallMethodStatic) op;
                var callingInterpreterKey = methodData.Interpreter.ToKey();
                var declaringType = callingInterpreterKey.Interpreter.Method.DeclaringType;
                var implementors = declaringType.ImplementorsOfT(usedTypes);
                if (implementors.Count > 0)
                    continue;
                //TODO: map correct method
                interpreter.MidRepresentation.LocalOperations[callOp] = new CallMethodStatic(methodData.Interpreter)
                {
                    Result = methodData.Result,
                    Parameters = methodData.Parameters
                };
                result= true;
            }
            if (result)
            {
                interpreter.MidRepresentation.UpdateUseDef();
            }
            return result;
        }
    }
}