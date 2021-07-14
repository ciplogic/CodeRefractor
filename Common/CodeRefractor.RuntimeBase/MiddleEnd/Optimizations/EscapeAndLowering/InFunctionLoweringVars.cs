﻿#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.EscapeAndLowering
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    internal class InFunctionLoweringVars : OptimizationPassBase
    {
        public InFunctionLoweringVars()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var candidateVariables = SetAllCandidateVariables(interpreter, closure);
            var useDef = interpreter.MidRepresentation.UseDef;
            var localOp = useDef.GetLocalOperations();

            if (RemoveAllEscaping(candidateVariables, localOp, useDef))
                return false;


            if (candidateVariables.Count == 0)
                return false;
            var result = false;
            foreach (var variable in candidateVariables)
            {
                var getVariableData = interpreter.AnalyzeProperties.GetVariableData(variable);
                if (getVariableData != EscapingMode.Unused && getVariableData!=EscapingMode.Stack)
                {
                    result |= interpreter.AnalyzeProperties.SetVariableData(variable, EscapingMode.Pointer);
                }
            }
            AllocateVariablesOnStack(localOp, candidateVariables, interpreter);
            return result;
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
                        RemoveCandidatesIfEscapes(localVariable, candidateVariables, op);
                    }
                }
                if (candidateVariables.Count == 0)
                    return true;
            } while (candidatesCount != candidateVariables.Count);
            return false;
        }

        private static HashSet<LocalVariable> SetAllCandidateVariables(CilMethodInterpreter interpreter,
            ClosureEntities closure)
        {
            var candidateVariables = new HashSet<LocalVariable>();
            var midRepresentation = interpreter.MidRepresentation;
            var variables = midRepresentation.Vars;
            var toAdd =
                variables.LocalVars.Where(varId => !varId.ComputedType().GetClrType(closure).IsPrimitive).ToArray();
            candidateVariables.AddRange(toAdd);
            toAdd = variables.VirtRegs.Where(varId => !varId.ComputedType().GetClrType(closure).IsPrimitive).ToArray();
            candidateVariables.AddRange(toAdd);
            toAdd = interpreter.AnalyzeProperties.Arguments
                .Where(varId => !varId.ComputedType().GetClrType(closure).IsPrimitive).ToArray();
            foreach (var argumentVariable in toAdd)
            {
                if (argumentVariable.Escaping != EscapingMode.Unused)
                {
                    argumentVariable.Escaping = EscapingMode.Pointer;
                }
            }
            candidateVariables.AddRange(toAdd);
            return candidateVariables;
        }

        private static bool AllocateVariablesOnStack(LocalOperation[] localOp, HashSet<LocalVariable> candidateVariables,
            MethodInterpreter interpreter)
        {
            var newOps = localOp.Where(op =>
                op.Kind == OperationKind.NewArray
                || op.Kind == OperationKind.NewObject).ToArray();
            if (newOps.Length == 0)
                return false;
            var result = false;
            foreach (var op in newOps)
            {
                var variable = op.GetDefinition();
                if (variable == null)
                    continue;

                if (!candidateVariables.Contains(variable)) continue;
                result = true;
                var variableData = interpreter.AnalyzeProperties.GetVariableData(variable);
                if (variableData != EscapingMode.Stack)
                {
                    interpreter.AnalyzeProperties.SetVariableData(variable, EscapingMode.Stack);
                }
            }
            return result;
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
                    HandleCall(candidateVariables, op);
                    break;
                case OperationKind.CallVirtual:
                case OperationKind.CallInterface:
                    HandleCallVirtual(candidateVariables, op);
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
                    HandleSetField(candidateVariables, op);
                    break;

                case OperationKind.RefAssignment:
                    HandleRefAssignment(localVariable, candidateVariables, op);
                    break;
                case OperationKind.FieldRefAssignment:
                    break;
                case OperationKind.NewArray:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void HandleCallVirtual(HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            var methodData = (CallMethodStatic)op;
            foreach (var identifierValue in methodData.Parameters)
            {
                var localVariable = identifierValue as LocalVariable;
                if (localVariable == null)
                    continue;
                candidateVariables.Remove(localVariable);
            }
            if (methodData.Result != null)
            {
                candidateVariables.Remove(methodData.Result);
            }
        }

        private static void HandleRefAssignment(LocalVariable localVariable, HashSet<LocalVariable> candidateVariables,
            LocalOperation op)
        {
            var value = (RefAssignment)op;
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

        private static void HandleCall(HashSet<LocalVariable> candidateVariables,
            LocalOperation op)
        {
            var methodData = (CallMethodStatic)op;
            if (methodData.Result != null)
            {
                candidateVariables.Remove(methodData.Result);
            }
            var escapeFullData = methodData.Interpreter.BuildEscapeModes();
            for (var index = 0; index < methodData.Parameters.Count; index++)
            {
                var parameter = methodData.Parameters[index];
                var variable = parameter as LocalVariable;
                if (variable == null)
                    continue;
                if (escapeFullData[index] == EscapingMode.Smart)
                    candidateVariables.Remove(variable);
            }
        }

        private static void HandleSetArrayItem(ICollection<LocalVariable> candidateVariables, LocalOperation op)
        {
            var assignSetArray = (SetArrayElement)op;
            var right = assignSetArray.Right as LocalVariable;
            if (right != null)
            {
                candidateVariables.Remove(right);
            }
        }

        private static void HandleSetField(HashSet<LocalVariable> candidateVariables, LocalOperation op)
        {
            var assignSetArray = (SetField)op;
            var right = assignSetArray.Right as LocalVariable;
            if (right != null)
            {
                candidateVariables.Remove(right);
            }
        }
    }
}