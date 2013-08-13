#region Usings

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
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
        private void OptimizeUnusedLocals(HashSet<int> localConstants, List<LocalOperation> operations,
                                          MetaMidRepresentation intermediateCode)
        {
            if (localConstants.Count == 0)
                return;

            foreach (var localUnused in localConstants)
            {
                intermediateCode.Vars.LocalVars.RemoveAll(local => local.Id == localUnused);
                intermediateCode.Vars.LocalVariables.Remove(localUnused);
            }
        }


        private static void RemoveCandidatesInArrays(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var operation in operations.Where(operation =>
                                                       operation.Kind == LocalOperation.Kinds.SetArrayItem))
            {
                var assignment = (Assignment) operation.Value;
                var arrayItem = (ArrayVariable) assignment.AssignedTo;
                RemoveLocalVarIfLocal(vregConstants, arrayItem.Index);
                RemoveLocalVarIfLocal(vregConstants, arrayItem.Parent);
                RemoveLocalVarIfLocal(vregConstants, assignment.Right);
            }
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

                RemoveLocalVarIfLocal(vregConstants, operationData.AssignedTo);
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
                                                       operation.Kind == LocalOperation.Kinds.BinaryOperator ||
                                                       operation.Kind == LocalOperation.Kinds.UnaryOperator))
            {
                var localVariable = (OperatorBase) operation.Value;

                var rightBinaryAssignment = localVariable as BinaryOperator;
                var unaryAssignment = localVariable as UnaryOperator;
                RemoveLocalVarIfLocal(vregConstants, localVariable.AssignedTo);
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