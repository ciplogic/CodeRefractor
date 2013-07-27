#region Usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.Compiler.Optimizations.SimpleDce
{
    public class DceVRegAssigned : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            var vregConstants = new HashSet<int>(intermediateCode.Vars.VirtRegs.Select(localVar => localVar.Id));

            RemoveCandidatesInLoadLocal(operations, vregConstants);
            RemoveCandidatesInOperators(operations, vregConstants);
            RemoveCandidatesInCalls(operations, vregConstants);

            RemoveCandidatesInGetFields(operations, vregConstants);
            RemoveCandidatesInSetFields(operations, vregConstants);
            RemoveCandidatesInReturn(operations, vregConstants);

            RemoveCandidatesInGetArrayItem(operations, vregConstants);
            RemoveCandidatesInSetArrayItem(operations, vregConstants);

            RemoveCandidatesInBranchOperators(operations, vregConstants);

            OptimizeUnusedVregs(intermediateCode, vregConstants, operations);
        }

        private void RemoveCandidatesInReturn(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.Return).ToArray();
            foreach (var operation in setFields)
            {
                RemoveCandidateVarIfVreg(vregConstants, operation.Value as LocalVariable);
            }
        }

        private void RemoveCandidatesInSetArrayItem(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.SetArrayItem).ToArray();
            foreach (var operation in setFields)
            {
                var operationData = (Assignment) operation.Value;
                var fieldSetter = (ArrayVariable) operationData.Left;
                RemoveCandidateVarIfVreg(vregConstants, operationData.Right as LocalVariable);
                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Parent);

                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Index);
            }
        }


        private void RemoveCandidatesInGetArrayItem(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.GetArrayItem).ToArray();
            foreach (var operation in setFields)
            {
                var assignment = (Assignment) operation.Value;
                var arrayVar = (ArrayVariable) assignment.Right;
                RemoveCandidateVarIfVreg(vregConstants, arrayVar.Index);
                RemoveCandidateVarIfVreg(vregConstants, arrayVar.Parent);
            }
        }

        private void RemoveCandidatesInGetFields(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.GetField).ToArray();
            foreach (var operation in setFields)
            {
                var assignment = (Assignment) operation.Value;
                var fieldSetter = (FieldGetter) assignment.Right;
                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Instance);
            }
        }

        private void RemoveCandidatesInSetFields(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.SetField).ToArray();
            foreach (var operation in setFields)
            {
                var operationData = (Assignment) operation.Value;
                var fieldSetter = (FieldSetter) operationData.Left;
                RemoveCandidateVarIfVreg(vregConstants, operationData.Right as LocalVariable);
                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Instance);
            }
        }

        #region Remove Candidates

        private void RemoveCandidatesInCalls(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var calls = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.Call).ToArray();
            foreach (var operation in calls)
            {
                var operationData = (MethodData) operation.Value;
                foreach (var vregConstant in operationData.Parameters)
                {
                    RemoveCandidateVarIfVreg(vregConstants, vregConstant as LocalVariable);
                }
            }
        }

        private void RemoveCandidatesInLoadLocal(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var assignments = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.Assignment).ToArray();
            foreach (var operation in assignments)
            {
                var localAssignment = (Assignment) operation.Value;

                RemoveCandidateVarIfVreg(vregConstants, localAssignment.Right);
            }
        }

        private static void RemoveCandidatesInBranchOperators(List<LocalOperation> operations,
            HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                operation.Kind == LocalOperation.Kinds.BranchOperator))
            {
                var rightBinaryAssignment = operation.Value as BranchOperator;

                if (rightBinaryAssignment == null)
                    throw new InvalidDataException("A branch operator has encoded other operation in its' place");
                var left = rightBinaryAssignment.CompareValue;
                var right = rightBinaryAssignment.SecondValue;
                RemoveCandidateVregIfInExpression(vregConstants, right, left);
            }
        }

        private static void RemoveCandidatesInOperators(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                operation.Kind == LocalOperation.Kinds.Operator))
            {
                var localVariable = (Assignment) operation.Value;

                var rightBinaryAssignment = localVariable.Right as BinaryOperator;
                var unaryAssignment = localVariable.Right as UnaryOperator;
                RemoveCandidateVarIfVreg(vregConstants, localVariable.Left);
                if (unaryAssignment != null)
                {
                    RemoveCandidateVarIfVreg(vregConstants, unaryAssignment.Left as LocalVariable);
                    continue;
                }

                if (rightBinaryAssignment == null) continue;
                var left = rightBinaryAssignment.Left as LocalVariable;
                var right = rightBinaryAssignment.Right as LocalVariable;
                RemoveCandidateVregIfInExpression(vregConstants, right, left);
            }
        }

        private static void RemoveCandidateVregIfInExpression(HashSet<int> vregConstants, IdentifierValue right,
            IdentifierValue left)
        {
            RemoveCandidateVarIfVreg(vregConstants, left);
            RemoveCandidateVarIfVreg(vregConstants, right);
        }

        private static void RemoveCandidateVarIfVreg(HashSet<int> vregConstants, IdentifierValue identifier)
        {
            var localVariable = identifier as LocalVariable;
            if (localVariable != null && localVariable.Kind == VariableKind.Vreg)
                vregConstants.Remove(localVariable.Id);
        }

        #endregion

        private void OptimizeUnusedVregs(MetaMidRepresentation intermediateCode, HashSet<int> vregConstants,
            List<LocalOperation> operations)
        {
            var liveVRegs =
                intermediateCode.Vars.VirtRegs.Where(
                    vreg => vreg.Kind != VariableKind.Vreg || !vregConstants.Contains(vreg.Id)).ToList();
            intermediateCode.Vars.VirtRegs = liveVRegs;

            var liveOperations = operations.Where(operation => IsLiveOperation(operation, vregConstants)).ToList();
            intermediateCode.LocalOperations = liveOperations;
        }

        private static bool IsLiveOperation(LocalOperation op, HashSet<int> vregConstants)
        {
            if (op.Kind != LocalOperation.Kinds.Assignment) return true;
            var assignment = (Assignment) op.Value;
            var localVariable = assignment.Left;
            var isRemovableVRegAssignment = localVariable.Kind == VariableKind.Vreg &&
                                            vregConstants.Contains(localVariable.Id);
            return !isRemovableVRegAssignment;
        }
    }
}