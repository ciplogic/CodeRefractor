using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    class DeleteCallToConstructorOfObject : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOps = intermediateCode.LocalOperations;
            for (var index = 0; index < localOps.Count; index++)
            {
                var op = localOps[index];
                if (op.Kind != OperationKind.Call)
                    continue;
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
