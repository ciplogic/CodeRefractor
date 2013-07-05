#region Usings

using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    internal class DeleteVregAsLocalAssignedAndUsedPreviousLine : ResultingOptimizationPass
    {
        private MetaMidRepresentation _intermediateCode;
        private int _currentRow;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            _intermediateCode = intermediateCode;
            var operations = intermediateCode.LocalOperations;
            for (var i = 1; i < operations.Count - 1; i++)
            {
                var srcOperation = operations[i];
                if (srcOperation.Kind != LocalOperation.Kinds.Assignment)
                    continue;
                var srcAssign = (Assignment) srcOperation.Value;
                var leftLocal = srcAssign.Left;
                if (srcAssign.Left.Kind == VariableKind.Vreg) continue;
                var rightVreg = srcAssign.Right as LocalVariable;
                if (rightVreg == null) continue;

                var destOperation = operations[i - 1];
                _currentRow = i;
                var destAssignment = destOperation.Value as Assignment;
                if (destAssignment == null)
                    continue;
                if (destAssignment.Left.Kind != VariableKind.Vreg || rightVreg.Id != destAssignment.Left.Id) continue;
                destAssignment.Left = leftLocal;
                DeletVreg();
            }
        }

        private void DeletVreg()
        {
            Result = true;
            _intermediateCode.LocalOperations.RemoveAt(_currentRow);
        }
    }
}