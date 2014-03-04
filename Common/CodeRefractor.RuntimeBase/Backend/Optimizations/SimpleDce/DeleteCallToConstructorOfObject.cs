#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    internal class DeleteCallToConstructorOfObject : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var localOps = midRepresentation.LocalOperations;
            var callIndices = useDef.GetOperationsOfKind(OperationKind.Call);
            foreach (var index in callIndices)
            {
                var op = operations[index];
                var methodData = (MethodData) op.Value;
                var info = methodData.Info;
                if (!info.IsConstructor) continue;
                if (info.DeclaringType != typeof (object) &&
                    info.DeclaringType != interpreter.Method.DeclaringType) continue;
                if(methodData.Interpreter.MidRepresentation.LocalOperations.Count>1)
                    continue;
                localOps.RemoveAt(index);
                Result = true;
                return;
            }
        }
    }
}