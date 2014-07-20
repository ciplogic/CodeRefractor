#region Usings

using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.SimpleDce
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    internal class DeleteCallToConstructorOfObject : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var callIndices = useDef.GetOperationsOfKind(OperationKind.Call);
            foreach (var index in callIndices)
            {
                var op = operations[index];
                var methodData = (CallMethodStatic) op;
                var info = methodData.Info;
                if (!info.IsConstructor) continue;
                if (info.DeclaringType != typeof (object) &&
                    info.DeclaringType != interpreter.Method.DeclaringType) continue;
				midRepresentation.LocalOperations.RemoveAt(index);
                Result = true;
                return;
            }
        }
    }
}