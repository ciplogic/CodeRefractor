using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;

namespace CodeRefractor.Backend.ProgramWideOptimizations.Virtual
{
    public class DevirtualizerFinalMethods : ProgramOptimizationBase
    {

        public override bool Optimize(ClosureEntities closure)
        {
            var methodInterpreters = closure.MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.CilInstructions)
                .Select(mth => (CilMethodInterpreter)mth)
                .ToArray();
            var result = false;
            foreach (var interpreter in methodInterpreters)
            {
                result |= HandleInterpreterInstructions(interpreter, closure.MappedTypes.Values.ToList());
            }
            return result;
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
                var methodData = (CallMethodStatic)op;
                var callingInterpreterKey = methodData.Interpreter.ToKey();
                var methodBase = callingInterpreterKey.Interpreter.Method;
                if(!methodBase.IsFinal)
                    continue;
                
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