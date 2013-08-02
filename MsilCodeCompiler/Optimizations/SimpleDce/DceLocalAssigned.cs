#region Usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.Compiler.Optimizations.SimpleDce
{
    public class DceLocalAssigned : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            var vregConstants = new HashSet<int>();
            foreach (var localVariable in intermediateCode.Vars.LocalVars)
            {
                vregConstants.Add(localVariable.Id);
            }

            RemoveCandidatesInLoadLocal(operations, vregConstants);
            RemoveCandidatesInOperators(operations, vregConstants);
            RemoveCandidatesInArrays(operations, vregConstants);

            RemoveCandidatesInCalls(operations, vregConstants);
            RemoveCandidatesInAssign(operations, vregConstants);
            RemoveCandidatesInBranchOperators(operations, vregConstants);

            OptimizeUnusedLocals(vregConstants, operations, intermediateCode);
        }

        private static void RemoveCandidatesInArrays(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                                                       operation.Kind == LocalOperation.Kinds.SetArrayItem))
            {
                var assignment = (Assignment) operation.Value;
                var arrayItem = (ArrayVariable) assignment.Left;
                RemoveLocalVarIfLocal(vregConstants, arrayItem.Index);
                RemoveLocalVarIfLocal(vregConstants, arrayItem.Parent);
                RemoveLocalVarIfLocal(vregConstants, assignment.Right);
            }
        }

        private void OptimizeUnusedLocals(HashSet<int> localConstants, List<LocalOperation> operations,
                                          MetaMidRepresentation intermediateCode)
        {
            if (localConstants.Count == 0)
                return;

            foreach (var localUnused in localConstants)
            {
                operations.RemoveAll(op =>
                                         {
                                             var canRemove = IsRemovableVRegAssignment(localUnused, op);
                                             Result |= canRemove;
                                             return canRemove;
                                         });
                intermediateCode.Vars.LocalVars.RemoveAll(local => local.Id == localUnused);
                intermediateCode.Vars.LocalVariables.Remove(localUnused);
            }
        }

        private static bool IsRemovableVRegAssignment(int vregConstant, LocalOperation op)
        {
            if (op.Kind != LocalOperation.Kinds.Assignment) return false;
            var assignment = (Assignment) op.Value;
            var localVariable = assignment.Left;
            var isRemovableVRegAssignment = localVariable.Kind != VariableKind.Vreg && localVariable.Id == vregConstant;
            return isRemovableVRegAssignment;
        }

        #region Remove Candidates

        private static void RemoveCandidatesInAssign(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                                                       operation.Kind == LocalOperation.Kinds.Assignment))
            {
                var operationData = (Assignment) operation.Value;
                RemoveLocalVarIfLocal(vregConstants, operationData.Right);
            }
            foreach (var operation in operations.Where(op => op.Value is Assignment))
            {
                var operationData = (Assignment) operation.Value;

                RemoveLocalVarIfLocal(vregConstants, operationData.Left);
            }
        }

        private static void RemoveCandidatesInCalls(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                                                       operation.Kind == LocalOperation.Kinds.Call))
            {
                var operationData = (MethodData) operation.Value;
                foreach (var vregConstant in operationData.Parameters)
                {
                    RemoveLocalVarIfLocal(vregConstants, vregConstant as LocalVariable);
                }
            }
        }

        private static void RemoveCandidatesInLoadLocal(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                                                       operation.Kind == LocalOperation.Kinds.Assignment))
            {
                var localAssignment = (Assignment) operation.Value;

                RemoveLocalVarIfLocal(vregConstants, localAssignment.Right);
            }
        }


        private static void RemoveCandidatesInBranchOperators(List<LocalOperation> operations,
                                                              HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                                                       operation.Kind == LocalOperation.Kinds.BranchOperator)
                )
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
                RemoveLocalVarIfLocal(vregConstants, localVariable.Left);
                if (unaryAssignment != null)
                {
                    RemoveLocalVarIfLocal(vregConstants, unaryAssignment.Left as LocalVariable);
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
            RemoveLocalVarIfLocal(vregConstants, left);
            RemoveLocalVarIfLocal(vregConstants, right);
        }

        private static void RemoveLocalVarIfLocal(HashSet<int> vregConstants, IdentifierValue identifier)
        {
            var localVariable = identifier as LocalVariable;
            if (localVariable != null && localVariable.Kind == VariableKind.Local)
                vregConstants.Remove(localVariable.Id);
        }

        #endregion
    }
}