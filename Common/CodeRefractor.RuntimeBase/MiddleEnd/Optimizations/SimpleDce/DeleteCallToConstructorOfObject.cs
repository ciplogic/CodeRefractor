#region Usings

using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.SimpleDce
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    internal class DeleteCallToConstructorOfObject : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var callIndices = useDef.GetOperationsOfKind(OperationKind.Call);
            foreach (var index in callIndices)
            {
                var op = operations[index];
                var methodData = (MethodData) op;
                var info = methodData.Info;
                if (!info.IsConstructor) continue;
                if (info.DeclaringType != typeof (object) &&
                    info.DeclaringType != interpreter.Method.DeclaringType) continue;
                if (methodData.Interpreter.MidRepresentation.LocalOperations.Count > 1)
					continue;
				var localOps = midRepresentation.LocalOperations;
                localOps.RemoveAt(index);
                Result = true;
                return;
            }
        }
    }
}