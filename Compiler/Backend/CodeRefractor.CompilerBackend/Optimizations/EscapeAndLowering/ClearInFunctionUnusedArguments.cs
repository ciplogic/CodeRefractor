using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    internal class ClearInFunctionUnusedArguments : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var argList = midRepresentation.Vars.Arguments.Select(a=>(LocalVariable)a).ToList();
            useDef.ComputeUnusedArguments(argList);
        }
    }
}