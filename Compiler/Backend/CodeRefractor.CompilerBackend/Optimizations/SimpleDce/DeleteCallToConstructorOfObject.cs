using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    class DeleteCallToConstructorOfObject : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var midRepresentation = methodInterpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var localOps = midRepresentation.LocalOperations;
            var callIndices = useDef.GetOperations(OperationKind.Call);
            foreach (var index in callIndices)
            {
                var op = operations[index];
                var methodData = (MethodData) op.Value;
                var info = methodData.Info;
                if (info.DeclaringType != typeof (object))
                    continue;
                if (!info.IsConstructor) continue;
                localOps.RemoveAt(index);
                Result = true;
                return;
            }
        }
    }
}
