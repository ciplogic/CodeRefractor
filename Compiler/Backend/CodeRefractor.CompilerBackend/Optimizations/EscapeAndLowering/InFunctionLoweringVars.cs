using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    class InFunctionLoweringVars : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var candidateVariables = new HashSet<LocalVariable>();
            var toAdd = intermediateCode.Vars.LocalVariables.Values.Where(varId =>!varId.ComputedType().IsPrimitive);
            candidateVariables.AddRange(toAdd);
            toAdd = intermediateCode.Vars.VirtRegs.Where(varId => !varId.ComputedType().IsPrimitive);
            candidateVariables.AddRange(toAdd);
            var localOp = intermediateCode.LocalOperations;
            foreach (var op in localOp)
            {
                var usages = op.GetUsages();
                foreach (var localVariable in usages)
                {
                    if (candidateVariables.Contains(localVariable))
                        RemoveCandidatesIfEscapes(localVariable, candidateVariables, op);
                }
            }
        }

        private static void RemoveCandidatesIfEscapes(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            switch (op.Kind)
            {
                case OperationKind.Assignment:                    
                case OperationKind.Return:
                case OperationKind.Call:
                    candidateVariables.Remove(localVariable);
                    break;
                case OperationKind.BinaryOperator:
                case OperationKind.UnaryOperator:
                case OperationKind.BranchOperator:
                case OperationKind.GetArrayItem:
                case OperationKind.GetField:
                    break;
                    //hard to handle, remove them for now
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
