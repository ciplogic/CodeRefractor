#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    internal class InFunctionLoweringVars : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var candidateVariables = new HashSet<LocalVariable>();
            var midRepresentation = methodInterpreter.MidRepresentation;
            var variables = midRepresentation.Vars;
            var toAdd = variables.LocalVars.Where(varId => !varId.ComputedType().ClrType.IsPrimitive);
            candidateVariables.AddRange(toAdd);
            toAdd = variables.VirtRegs.Where(varId => !varId.ComputedType().ClrType.IsPrimitive);
            candidateVariables.AddRange(toAdd);
            var useDef = midRepresentation.UseDef;
            var localOp = useDef.GetLocalOperations();
            for (var index = 0; index < localOp.Length; index++)
            {
                var op = localOp[index];
                var usages = useDef.GetUsages(index);
                foreach (var localVariable in usages.Where(candidateVariables.Contains))
                {
                    RemoveCandidatesIfEscapes(localVariable, candidateVariables, op);
                    if (candidateVariables.Count == 0)
                        return;
                }
            }
            if (candidateVariables.Count == 0)
                return;
            foreach (var variable in candidateVariables)
            {
                var variableData = variables.GetVariableData(variable.Name);
                variableData.Escaping = EscapingMode.Pointer;
            }
            AllocateVariablesOnStack(localOp, candidateVariables, variables);
        }

        private static void AllocateVariablesOnStack(LocalOperation[] localOp, HashSet<LocalVariable> candidateVariables,
            MidRepresentationVariables variables)
        {
            var newOps = localOp.Where(op =>
                op.Kind == OperationKind.NewArray
                || op.Kind == OperationKind.NewObject).ToArray();
            if (newOps.Length == 0)
                return;
            foreach (var op in newOps)
            {
                var variable = op.GetDefinition();
                if (variable == null)
                    continue;

                if (!candidateVariables.Contains(variable)) continue;
                var variableData = variables.GetVariableData(variable.Name);
                variableData.Escaping = EscapingMode.Stack;
            }
        }

        public static void RemoveCandidatesIfEscapes(LocalVariable localVariable,
            HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            switch (op.Kind)
            {
                case OperationKind.Assignment:
                    HandleAssign(candidateVariables, op);
                    break;
                case OperationKind.Return:
                    HandleReturn(localVariable, candidateVariables, op);
                    break;

                case OperationKind.Call:
                case OperationKind.CallVirtual:
                case OperationKind.CallInterface:
                    HandleCall(localVariable, candidateVariables, op);
                    break;
                case OperationKind.BinaryOperator:
                case OperationKind.UnaryOperator:
                case OperationKind.BranchOperator:
                case OperationKind.GetArrayItem:
                case OperationKind.GetField:
                    break;
                case OperationKind.SetArrayItem:
                    HandleSetArrayItem(candidateVariables, op);
                    break;
                case OperationKind.SetField:
                    HandleSetArrayItem(candidateVariables, op);
                    break;

                case OperationKind.RefAssignment:
                    HandleRefAssignment(localVariable, candidateVariables, op);
                    break;
                case OperationKind.FieldRefAssignment:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void HandleRefAssignment(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables,
            LocalOperation op)
        {
            var value = (RefAssignment)op.Value;
            candidateVariables.Remove(localVariable);
            candidateVariables.Remove(value.Right);
        }

        private static void HandleReturn(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables,
            LocalOperation op)
        {
            candidateVariables.Remove(localVariable);
        }

        private static void HandleAssign(HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            var assignData = op.GetAssignment();
            var right = assignData.Right as LocalVariable;
            if (right == null || candidateVariables.Contains(right))
                return;
            candidateVariables.Remove(assignData.AssignedTo);
        }

        private static void HandleCall(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables,
            LocalOperation op)
        {
            var methodData = (MethodData)op.Value;
            var escapeData = AnalyzeParametersAreEscaping.GetEscapingParameterData(methodData);
            if (escapeData == null)
            {
                candidateVariables.Remove(localVariable);
                return;
            }

            var escapingBools = LinkerUtils.BuildEscapingBools(methodData.Info);
            for (var index = 0; index < methodData.Parameters.Count; index++)
            {
                var parameter = methodData.Parameters[index];
                var variable = parameter as LocalVariable;
                if (variable == null)
                    continue;
                if (escapingBools[index])
                    candidateVariables.Remove(variable);
            }
        }

        private static void HandleSetArrayItem(ICollection<LocalVariable> candidateVariables, LocalOperation op)
        {
            var assignSetArray = (Assignment)op.Value;
            var right = assignSetArray.Right as LocalVariable;
            if (right != null)
            {
                candidateVariables.Remove(right);
            }
        }
    }
}