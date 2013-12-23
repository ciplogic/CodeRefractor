﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    class InFunctionLoweringVars : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var candidateVariables = new HashSet<LocalVariable>();
            var toAdd = intermediateCode.Vars.LocalVars.Where(varId => !varId.ComputedType().ClrType.IsPrimitive);
            candidateVariables.AddRange(toAdd);
            toAdd = intermediateCode.Vars.VirtRegs.Where(varId => !varId.ComputedType().ClrType.IsPrimitive);
            candidateVariables.AddRange(toAdd);
            var localOp = intermediateCode.LocalOperations;
            foreach (var op in localOp)
            {
                var usages = op.GetUsages();
                foreach (var localVariable in usages.Where(candidateVariables.Contains))
                {
                    RemoveCandidatesIfEscapes(localVariable, candidateVariables, op);
                }
            }
            if (candidateVariables.Count == 0)
                return;
            foreach (var variable in candidateVariables)
                variable.NonEscaping = NonEscapingMode.Pointer;
            AllocateVariablesOnStack(localOp, candidateVariables);
            PropagateEscapign(localOp);
        }

        private void PropagateEscapign(List<LocalOperation> localOp)
        {
            foreach (var op in localOp)
            {
                if (op.Kind != OperationKind.Assignment) continue;
                var assign = op.GetAssignment();
                var right = assign.Right as LocalVariable;
                if(right==null)
                    continue;
                if (assign.AssignedTo.NonEscaping == NonEscapingMode.Smart
                    && right.NonEscaping != NonEscapingMode.Smart)
                    right.NonEscaping = NonEscapingMode.Smart;
            }
        }

        private static void AllocateVariablesOnStack(List<LocalOperation> localOp, HashSet<LocalVariable> candidateVariables)
        {
            foreach (var op in localOp)
            {
                switch (op.Kind)
                {
                    case OperationKind.NewArray:
                    case OperationKind.NewObject:

                        var definition = op.GetUseDefinition();
                        if (definition != null && candidateVariables.Contains(definition))
                            definition.NonEscaping = NonEscapingMode.Stack;
                        break;
                }
            }
        }

        public static void RemoveCandidatesIfEscapes(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            switch (op.Kind)
            {
                case OperationKind.Assignment:
                    HandleAssign(localVariable, candidateVariables, op);
                    break;
                case OperationKind.Return:
                    HandleReturn(localVariable, candidateVariables, op);
                    break;
                
                case OperationKind.Call:
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
                default:
                    throw new NotImplementedException();
            }
        }

        private static void HandleReturn(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            candidateVariables.Remove(localVariable);
        }

        private static void HandleAssign(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            var assignData = op.GetAssignment();
            candidateVariables.Remove(assignData.AssignedTo);
        }

        private static void HandleCall(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            var methodData = (MethodData)op.Value;
            var escapeData = AnalyzeParametersAreEscaping.GetEscapingParameterData(methodData);
            if (escapeData == null)
            {
                candidateVariables.Remove(localVariable);
                return;
            }
            for (int index = 0; index < methodData.Parameters.Count; index++)
            {
                var parameter = methodData.Parameters[index];
                bool isEscaping;
                if (!escapeData.TryGetValue(index, out isEscaping) || !parameter.Equals(localVariable)) 
                    continue;
                if (isEscaping)
                    candidateVariables.Remove(localVariable);
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
