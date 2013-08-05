#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    internal class DeleteVregAssignedVariableAndUsedNextLine : ResultingOptimizationPass
    {
        private readonly HashSet<int> _instructionsToBeDeleted = new HashSet<int>();
        private readonly HashSet<int> _vregToBeDeleted = new HashSet<int>();
        private int _currentId;
        private int _currentRow;
        private MetaMidRepresentation _intermediateCode;
        private LocalVariable _leftVreg;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            _intermediateCode = intermediateCode;
            for (var i = 0; i < operations.Count - 1; i++)
            {
                var srcOperation = operations[i];
                _currentRow = i;
                if (srcOperation.Kind != LocalOperation.Kinds.Assignment)
                    continue;
                var assignment = srcOperation.GetAssignment();
                _leftVreg = assignment.Left;
                if (_leftVreg.Kind != VariableKind.Vreg) continue;
                var rightItem = assignment.Right as LocalVariable;
                if (rightItem == null)
                    continue;
                _currentId = _leftVreg.Id;

                var destOperation = operations[i + 1];
                ProcessOperation(destOperation, assignment);
            }
            if (_instructionsToBeDeleted.Count == 0)
                return;
            CleanVRegs(intermediateCode);
        }

        private void CleanVRegs(MetaMidRepresentation intermediateCode)
        {
            intermediateCode.DeleteInstructions(_instructionsToBeDeleted);

            _vregToBeDeleted.Clear();
        }

        private void ProcessOperation(LocalOperation destOperation, Assignment srcAssignment)
        {
            switch (destOperation.Kind)
            {
                case LocalOperation.Kinds.SetField:
                    HandleSetField(srcAssignment, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.GetField:
                    HandleGetField(srcAssignment.Right, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.SetArrayItem:
                    HandleSetArrayItem(srcAssignment, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.GetArrayItem:
                    HandleGetArrayItem(srcAssignment, (Assignment) destOperation.Value);
                    break;
                default:
                    return;
            }
        }

        private void HandleGetArrayItem(Assignment srcAssignment, Assignment assignment)
        {
            var right = srcAssignment.Right;
            var arrayVariable = (ArrayVariable) assignment.Right;
            if (VarMatchVreg(arrayVariable.Parent))
            {
                arrayVariable.Parent = right;
                return;
            }
            if (VarMatchVreg(arrayVariable.Index))
            {
                arrayVariable.Index = right;
                return;
            }

            if (right is LocalVariable && VarMatchVreg(assignment.Left))
            {
                assignment.Left = right as LocalVariable;
                return;
            }
        }

        private void HandleSetArrayItem(Assignment srcAssignment, Assignment assignment)
        {
            var right = srcAssignment.Right;
            var arrayVariable = (ArrayVariable) assignment.Left;
            if (VarMatchVreg(arrayVariable.Parent))
            {
                arrayVariable.Parent = right;
                return;
            }
            if (VarMatchVreg(assignment.Right))
            {
                assignment.Right = right;
                return;
            }
            if (VarMatchVreg(arrayVariable.Index))
            {
                arrayVariable.Index = right;
            }
        }

        private void HandleGetField(IdentifierValue right, Assignment assignment)
        {
            var fieldGetter = (FieldGetter) assignment.Right;
            if (VarMatchVreg(fieldGetter.Instance))
                fieldGetter.Instance = right;
        }

        private void HandleSetField(Assignment srcAssignment, Assignment assignment)
        {
            var right = srcAssignment.Right;
            var fieldSetter = (FieldSetter) assignment.Left;
            if (VarMatchVreg(fieldSetter.Instance))
                fieldSetter.Instance = right;
            if (VarMatchVreg(assignment.Right))
                assignment.Right = right;
        }

        private bool VarMatchVreg(IdentifierValue identifier)
        {
            var localVar = identifier as LocalVariable;
            if (localVar == null)
            {
                return false;
            }
            if (localVar.Kind != VariableKind.Vreg || localVar.Id != _currentId)
                return false;
            DeletVreg();
            return true;
        }

        private void DeletVreg()
        {
            Result = true;
            var operations = _intermediateCode.LocalOperations;
            for (var i = _currentRow + 1; i < operations.Count; i++)
            {
                var operation = operations[i];
                if (operation.IsBranchOperation())
                    break;
                if (operation.OperationUses(_leftVreg))
                    return;
            }
            _vregToBeDeleted.Add(_leftVreg.Id);
            _instructionsToBeDeleted.Add(_currentRow);
        }
    }
}