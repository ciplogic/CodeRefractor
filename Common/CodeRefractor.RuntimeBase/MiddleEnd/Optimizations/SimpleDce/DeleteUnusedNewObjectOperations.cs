#region Uses

using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.SimpleDce
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
class DeleteUnusedNewObjectOperations : OptimizationPassBase
    {
        public DeleteUnusedNewObjectOperations()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var useDef = interpreter.MidRepresentation.UseDef;

            List<int> newOperations;
            var newObjOperations = FindAllNewOperators(useDef, out newOperations);
            if (newObjOperations.Length == 0)
                return false;
            var getAllUsages = useDef.GetAllUsedVariables();
            var unusedNewObj = new List<int>();
            foreach (var newOperation in newOperations)
            {
                var definition = useDef.GetDefinition(newOperation);
                if (getAllUsages.Contains(definition))
                    continue;
                unusedNewObj.Add(newOperation);
            }
            if (unusedNewObj.Count == 0)
                return false;
            interpreter.DeleteInstructions(unusedNewObj);
            return true;
        }

        static int[] FindAllNewOperators(UseDefDescription useDef, out List<int> newOperations)
        {
            var newObjOperations = useDef.GetOperationsOfKind(OperationKind.NewObject);
            var newArrayOperations = useDef.GetOperationsOfKind(OperationKind.NewArray);
            newOperations = new List<int>();
            newOperations.AddRange(newObjOperations);
            newOperations.AddRange(newArrayOperations);

            return newOperations.ToArray();
        }
    }
}