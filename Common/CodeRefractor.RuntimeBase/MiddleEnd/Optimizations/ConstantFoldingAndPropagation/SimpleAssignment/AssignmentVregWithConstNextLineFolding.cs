#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment
{
    //[Optimization(Category = OptimizationCategories.Propagation)]
    public class AssignmentVregWithConstNextLineFolding : ResultingInFunctionOptimizationPass
    {
        private class ToFixAssignment
        {
            public LocalVariable SourceAssignment;
            public IdentifierValue RightValue;
            public int Index;

            public override string ToString()
            {
                return String.Format("Line {2}: {0} -> {1}",
                    SourceAssignment.Name, RightValue.Name, Index);
            }
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var toFix = new List<ToFixAssignment>();
            for (var index = 0; index < operations.Length - 1; index++)
            {
                var localOperation = operations[index];
                if (localOperation.Kind != OperationKind.Assignment)
                    continue;

                var assignment = localOperation.GetAssignment();
                var constValue = assignment.Right;
                var destOperation = operations[index + 1];
                if (!destOperation.OperationUses(assignment.AssignedTo)) continue;
                var toFixInstruction = new ToFixAssignment()
                {
                    Index = index + 1,
                    SourceAssignment = assignment.AssignedTo,
                    RightValue = constValue
                };
                toFix.Add(toFixInstruction);
            }
            if (toFix.Count == 0)
                return;
            foreach (var toFixAssignment in toFix)
            {
                var destOperation = operations[toFixAssignment.Index];
                destOperation.SwitchUsageWithDefinition(toFixAssignment.SourceAssignment, toFixAssignment.RightValue);
            }
            Result = true;
        }
    }
}