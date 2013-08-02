#region Usings

using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    internal class DeleteGappingVregAssignment : ResultingOptimizationPass
    {
        private int _currentId;
        private int _currentRow;
        private MetaMidRepresentation _intermediateCode;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            _intermediateCode = intermediateCode;


            var operations = intermediateCode.LocalOperations;
            for (var i = 0; i < operations.Count; i++)
            {
                _currentRow = i;
                var srcOperation = operations[i];
                if (srcOperation.Kind != LocalOperation.Kinds.Assignment)
                    continue;
                var srcAssign = (Assignment) srcOperation.Value;
                var leftLocal = srcAssign.Left;
                if (srcAssign.Left.Kind != VariableKind.Vreg) continue;
                var rightVreg = srcAssign.Right as LocalVariable;
                if (rightVreg == null) continue;

                int pos;
                var nextUsage = operations.GetNextUsage(leftLocal, i + 1, out pos);
                if (nextUsage == null)
                    continue;
                _currentId = leftLocal.Id;

                if (i + 1 != pos)
                {
                    var destOperation = operations[pos];
                    ProcessOperation(destOperation, srcAssign);
                }
                return;
            }
        }

        private void ProcessOperation(LocalOperation destOperation, Assignment assignment)
        {
            switch (destOperation.Kind)
            {
                case LocalOperation.Kinds.Assignment:
                    HandleAssignment(assignment.Right, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.Call:
                    HandleCall(assignment.Right, (MethodData) destOperation.Value);
                    break;
                case LocalOperation.Kinds.SetField:
                    HandleSetField(assignment.Right, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.GetField:
                    HandleGetField(assignment.Right, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.Operator:
                    HandleOperator(assignment.Right, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.BranchOperator:
                    HandleBranchOperator(assignment.Right, (BranchOperator) destOperation.Value);
                    break;
                case LocalOperation.Kinds.SetArrayItem:
                    HandleSetArrayItem(assignment.Right, (Assignment) destOperation.Value);
                    break;
                case LocalOperation.Kinds.GetArrayItem:
                    HandleGetArrayItem(assignment.Right, (Assignment) destOperation.Value);
                    break;
                default:
                    return;
            }
        }

        private void HandleGetArrayItem(IdentifierValue right, Assignment assignment)
        {
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
            }
        }

        private void HandleSetArrayItem(IdentifierValue right, Assignment assignment)
        {
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

        private void HandleBranchOperator(IdentifierValue right, BranchOperator value)
        {
            if (VarMatchVreg(value.CompareValue))
                value.CompareValue = right;
        }

        private void HandleOperator(IdentifierValue right, Assignment value)
        {
            var binaryOperator = value.Right as BinaryOperator;
            var unaryOperator = value.Right as UnaryOperator;
            if (unaryOperator != null)
            {
                if (VarMatchVreg(unaryOperator.Left))
                    unaryOperator.Left = right;
                return;
            }
            if (binaryOperator == null) return;
            if (VarMatchVreg(binaryOperator.Left))
            {
                binaryOperator.Left = right;
            }
            if (VarMatchVreg(binaryOperator.Right))
            {
                binaryOperator.Right = right;
            }
        }

        private void HandleSetField(IdentifierValue right, Assignment assignment)
        {
            var fieldSetter = (FieldSetter) assignment.Left;
            if (VarMatchVreg(fieldSetter.Instance))
                fieldSetter.Instance = right;
        }

        private void HandleCall(IdentifierValue right, MethodData methodData)
        {
            for (var index = 0; index < methodData.Parameters.Count; index++)
            {
                var parameter = methodData.Parameters[index];
                if (VarMatchVreg(parameter))
                    methodData.Parameters[index] = right;
            }
        }

        private void HandleAssignment(IdentifierValue right, Assignment assignment)
        {
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
            operations.RemoveAt(_currentRow);
        }
    }
}