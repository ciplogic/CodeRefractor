#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Runtime;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    internal class InFunctionLoweringVars : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var candidateVariables = SetAllCandidateVariables(interpreter);
            var useDef = interpreter.MidRepresentation.UseDef;
            var localOp = useDef.GetLocalOperations();
            if (RemoveAllEscaping(candidateVariables, localOp, useDef))
                return;

            if (candidateVariables.Count == 0)
                return;
            foreach (var variable in candidateVariables)
            {
                interpreter.AnalyzeProperties.SetVariableData(variable, EscapingMode.Pointer);
            }
            AllocateVariablesOnStack(localOp, candidateVariables, interpreter);
        }

        private static bool RemoveAllEscaping(HashSet<LocalVariable> candidateVariables, LocalOperation[] localOp,
            UseDefDescription useDef)
        {
            int candidatesCount;

            do
            {
                candidatesCount = candidateVariables.Count;
                for (var index = 0; index < localOp.Length; index++)
                {
                    var op = localOp[index];
                    var usages = useDef.GetUsages(index);
                    foreach (var localVariable in usages.Where(candidateVariables.Contains))
                    {
                        RemoveCandidatesIfEscapes(localVariable, candidateVariables, op, Runtime);
                    }
                }
                if (candidateVariables.Count == 0)
                    return true;
            } while (candidatesCount != candidateVariables.Count);
            return false;
        }

        private static HashSet<LocalVariable> SetAllCandidateVariables(MethodInterpreter interpreter)
        {
            var candidateVariables = new HashSet<LocalVariable>();
            var midRepresentation = interpreter.MidRepresentation;
            var variables = midRepresentation.Vars;
            var toAdd = variables.LocalVars.Where(varId => !varId.ComputedType().ClrType.IsPrimitive);
            candidateVariables.AddRange(toAdd);
            toAdd = variables.VirtRegs.Where(varId => !varId.ComputedType().ClrType.IsPrimitive);
            candidateVariables.AddRange(toAdd);
            return candidateVariables;
        }

        private static void AllocateVariablesOnStack(LocalOperation[] localOp, HashSet<LocalVariable> candidateVariables,
            MethodInterpreter interpreter)
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
                var variableData = interpreter.AnalyzeProperties.GetVariableData(variable);

                interpreter.AnalyzeProperties.SetVariableData(variable, EscapingMode.Stack);
            }
        }

        public static void RemoveCandidatesIfEscapes(LocalVariable localVariable,
            HashSet<LocalVariable> candidateVariables, LocalOperation op, CrRuntimeLibrary crRuntime)
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
                    HandleCall(localVariable, candidateVariables, op, crRuntime);
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
            var value = (RefAssignment) op.Value;
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
            if (right == null) return;
            if (!candidateVariables.Contains(assignData.AssignedTo))
            {
                candidateVariables.Remove(right);
            }

            if (!candidateVariables.Contains(right))
            {
                candidateVariables.Remove(assignData.AssignedTo);
            }
        }

        private static void HandleCall(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables,
            LocalOperation op, CrRuntimeLibrary crRuntime)
        {
            var methodData = (MethodData) op.Value;
            var escapeData = AnalyzeParametersAreEscaping.GetEscapingParameterData(methodData);
            if (escapeData == null)
            {
                candidateVariables.Remove(localVariable);
                return;
            }

            var escapingBools = LinkerUtils.BuildEscapingBools(methodData.Info, crRuntime);
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
            var assignSetArray = (Assignment) op.Value;
            var right = assignSetArray.Right as LocalVariable;
            if (right != null)
            {
                candidateVariables.Remove(right);
            }
        }
    }
}