#region Uses

using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment
{
    //[Optimization(Category = OptimizationCategories.Propagation)]
    public class AssignmentVregWithConstNextLineFolding : OptimizationPassBase
    {
        public AssignmentVregWithConstNextLineFolding()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
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
                var toFixInstruction = new ToFixAssignment
                {
                    Index = index + 1,
                    SourceAssignment = assignment.AssignedTo,
                    RightValue = constValue
                };
                toFix.Add(toFixInstruction);
            }
            if (toFix.Count == 0)
                return false;
            foreach (var toFixAssignment in toFix)
            {
                var destOperation = operations[toFixAssignment.Index];
                destOperation.SwitchUsageWithDefinition(toFixAssignment.SourceAssignment, toFixAssignment.RightValue);
            }
            return true;
        }

        private struct ToFixAssignment
        {
            public int Index;
            public IdentifierValue RightValue;
            public LocalVariable SourceAssignment;

            public override string ToString()
            {
                return string.Format("Line {2}: {0} -> {1}",
                    SourceAssignment.Name, RightValue.Name, Index);
            }
        }
    }
}