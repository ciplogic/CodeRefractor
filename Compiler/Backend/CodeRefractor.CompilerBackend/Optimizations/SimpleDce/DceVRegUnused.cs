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
    public class DceVRegUnused : ResultingInFunctionOptimizationPass
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


            RemoveCandidatesInCreateObject(operations, vregConstants);

            RemoveCandidatesInCreateArray(operations, vregConstants);
            RemoveCandidatesInGetArrayItem(operations, vregConstants);
            RemoveCandidatesInSetArrayItem(operations, vregConstants);

            RemoveCandidatesInBranchOperators(operations, vregConstants);

            OptimizeUnusedVregs(intermediateCode, vregConstants);
        }


        private static void OptimizeUnusedVregs(MetaMidRepresentation intermediateCode, HashSet<int> vregConstants)
        {
            if (vregConstants.Count == 0)
                return;
            var liveVRegs =
            intermediateCode.Vars.VirtRegs.Where(
                vreg => vreg.Kind != VariableKind.Vreg || !vregConstants.Contains(vreg.Id)).ToList();
            intermediateCode.Vars.VirtRegs = liveVRegs;
        }
        #region Remove candidates
        private static void RemoveCandidatesInReturn(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.Return).ToArray();
            foreach (var operation in setFields)
            {
                RemoveCandidateVarIfVreg(vregConstants, operation.Value as LocalVariable);
            }
        }

        private static void RemoveCandidatesInCreateObject(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var newObjectOps = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.NewObject).ToArray();
            if(newObjectOps.Length==0)
                return;
            foreach (var operation in newObjectOps)
            {
                var assignment = (Assignment)operation.Value;
                RemoveCandidateVarIfVreg(vregConstants, assignment.AssignedTo);
            }
        }

        private static void RemoveCandidatesInCreateArray(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.NewArray).ToArray();
            foreach (var operation in setFields)
            {
                var assignment = (Assignment)operation.Value;
                var arrayVar = (NewArrayObject)assignment.Right;
                RemoveCandidateVarIfVreg(vregConstants, assignment.AssignedTo);
                RemoveCandidateVarIfVreg(vregConstants, arrayVar.ArrayLength);
            }
        }


        private static void RemoveCandidatesInSetArrayItem(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.SetArrayItem).ToArray();
            foreach (var operation in setFields)
            {
                var operationData = (Assignment)operation.Value;
                var fieldSetter = (ArrayVariable)operationData.AssignedTo;
                RemoveCandidateVarIfVreg(vregConstants, operationData.Right as LocalVariable);
                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Parent);

                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Index);
            }
        }


        private static void RemoveCandidatesInGetArrayItem(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.GetArrayItem).ToArray();
            foreach (var operation in setFields)
            {
                var assignment = (Assignment)operation.Value;
                var arrayVar = (ArrayVariable)assignment.Right;
                RemoveCandidateVarIfVreg(vregConstants, arrayVar.Index);
                RemoveCandidateVarIfVreg(vregConstants, arrayVar.Parent);
            }
        }

        private static void RemoveCandidatesInGetFields(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.GetField).ToArray();
            if(setFields.Length==0)
                return;
            foreach (var operation in setFields)
            {
                var assignment = (Assignment)operation.Value;
                var fieldSetter = (FieldGetter)assignment.Right;
                RemoveCandidateVarIfVreg(vregConstants, assignment.AssignedTo);
                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Instance);
            }
        }

        private static void RemoveCandidatesInSetFields(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var setFields = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.SetField).ToArray();
            foreach (var operation in setFields)
            {
                var operationData = (Assignment)operation.Value;
                var fieldSetter = (FieldSetter)operationData.AssignedTo;
                RemoveCandidateVarIfVreg(vregConstants, operationData.Right as LocalVariable);
                RemoveCandidateVarIfVreg(vregConstants, fieldSetter.Instance);
            }
        }


        private static void RemoveCandidatesInCalls(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var calls = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.Call)
                .Select(operation=>(MethodData)operation.Value).ToArray();
            foreach (var operationData in calls)
            {
                foreach (var vregConstant in operationData.Parameters)
                {
                    RemoveCandidateVarIfVreg(vregConstants, vregConstant as LocalVariable);
                }
                RemoveCandidateVarIfVreg(vregConstants, operationData.Result);
            }
        }

        private static void RemoveCandidatesInLoadLocal(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            var assignments = operations
                .Where(operation => operation.Kind == LocalOperation.Kinds.Assignment).ToArray();
            foreach (var operation in assignments)
            {
                var localAssignment = (Assignment)operation.Value;

                RemoveCandidateVarIfVreg(vregConstants, localAssignment.AssignedTo);
                RemoveCandidateVarIfVreg(vregConstants, localAssignment.Right);
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
                var localVariable = (OperatorBase)operation.Value;

                var rightBinaryAssignment = localVariable as BinaryOperator;
                var unaryAssignment = localVariable as UnaryOperator;
                RemoveCandidateVarIfVreg(vregConstants, localVariable.AssignedTo);
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
    }
}