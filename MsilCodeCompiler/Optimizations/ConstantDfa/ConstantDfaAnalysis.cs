#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.Compiler.Optimizations.ReachabilityDfa;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.Compiler.Optimizations.ConstantDfa
{
    internal class ConstantDfaAnalysis : ResultingOptimizationPass
    {
        private DfaPointOfAnalysis[] _pointsOfAnalysis;
        private List<LocalOperation> _operations;
        private Dictionary<int, int> _labelTable = new Dictionary<int, int>();

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            _operations = intermediateCode.LocalOperations;
            _labelTable = InstructionsUtils.BuildLabelTable(_operations);
            _pointsOfAnalysis = new DfaPointOfAnalysis[_operations.Count + 1];

            var startingConclusions = new DfaPointOfAnalysis();
            Interpret(0, startingConclusions);

            ApplyResult();
        }

        private void ApplyResult()
        {
            Assignment assignment;
            for (var i = 0; i < _operations.Count; i++)
            {
                var operation = _operations[i];
                switch (operation.Kind)
                {
                    case LocalOperation.Kinds.Assignment:
                        assignment = (Assignment) operation.Value;
                        HandleAssignment(i, assignment);
                        break;
                    case LocalOperation.Kinds.Operator:
                        assignment = (Assignment) operation.Value;
                        HandleOperator(i, assignment);
                        break;
                    case LocalOperation.Kinds.BranchOperator:
                        var branchOperator = (BranchOperator) operation.Value;
                        HandleBranchOperator(i, branchOperator);
                        break;
                }
            }
        }

        private void HandleBranchOperator(int i, BranchOperator branchOperator)
        {
            var localVariable = branchOperator.CompareValue as LocalVariable;
            if (localVariable == null)
                return;
            var analysis = _pointsOfAnalysis[i];

            var variableState = analysis.GetConstVariableState(localVariable);
            if (variableState == null)
                return;
            branchOperator.CompareValue = localVariable;
        }

        private void HandleAssignment(int i, Assignment assignment)
        {
            var constant = assignment.Right as ConstValue;
            if (constant == null)
            {
                var analysis = _pointsOfAnalysis[i];
                var variableState = analysis.GetConstVariableState(assignment.Right as LocalVariable);
                if (variableState != null)
                {
                    assignment.Right = variableState;
                    Result = true;
                }
            }
        }

        private void HandleOperator(int i, Assignment assignment)
        {
            var binary = assignment.Right as BinaryOperator;
            var unary = assignment.Right as UnaryOperator;
            if (unary != null)
            {
                var analysis = _pointsOfAnalysis[i];
                var variableState = analysis.GetConstVariableState(unary.Left as LocalVariable);
                if (variableState != null)
                {
                    unary.Left = variableState;
                    Result = true;
                }
            }
            if (binary != null)
            {
                var analysis = _pointsOfAnalysis[i];
                var variableState = analysis.GetConstVariableState(binary.Left as LocalVariable);
                if (variableState != null)
                {
                    binary.Left = variableState;
                    Result = true;
                }

                variableState = analysis.GetConstVariableState(binary.Right as LocalVariable);
                if (variableState != null)
                {
                    binary.Right = variableState;
                    Result = true;
                }
            }
        }

        private int JumpTo(int labelId)
        {
            return _labelTable[labelId];
        }

        private void Interpret(int cursor, DfaPointOfAnalysis startingConclusions)
        {
            var canUpdate = true;
            Assignment assignment;
            BranchOperator branchOperator;
            if (startingConclusions.Equals(_pointsOfAnalysis[cursor]))
                return;
            while (canUpdate)
            {
                _pointsOfAnalysis[cursor] = startingConclusions.Merge(_pointsOfAnalysis[cursor]);

                var operation = GetOperation(cursor);
                var analysis = _pointsOfAnalysis[cursor];
                switch (operation.Kind)
                {
                    case LocalOperation.Kinds.Assignment:
                        assignment = (Assignment) operation.Value;
                        var constant = assignment.Right as ConstValue;
                        if (constant != null)
                        {
                            analysis.States[assignment.Left] = new VariableState
                            {
                                Constant = constant,
                                State = VariableState.ConstantState.Constant
                            };
                        }
                        else
                        {
                            analysis.States[assignment.Left] = new VariableState
                            {
                                State = VariableState.ConstantState.NotConstant
                            };
                        }
                        startingConclusions = analysis;
                        break;

                    case LocalOperation.Kinds.Operator:
                        assignment = (Assignment) operation.Value;
                        analysis.States[assignment.Left] = new VariableState
                        {
                            State = VariableState.ConstantState.NotConstant
                        };
                        break;
                    case LocalOperation.Kinds.BranchOperator:
                        branchOperator = (BranchOperator) operation.Value;
                        Interpret(JumpTo(branchOperator.JumpTo), analysis);
                        break;
                    case LocalOperation.Kinds.Label:
                        break;
                    case LocalOperation.Kinds.Call:
                        break;
                    case LocalOperation.Kinds.Return:
                        return;
                    case LocalOperation.Kinds.AlwaysBranch:
                        var jumpTo = (int) operation.Value;
                        Interpret(JumpTo(jumpTo), analysis);
                        return;
                    default:
                        throw new InvalidOperationException(String.Format("Unhandled: {0}", operation));
                }
                cursor++;
                canUpdate = !(startingConclusions.Equals(_pointsOfAnalysis[cursor]));
            }
        }

        private LocalOperation GetOperation(int i)
        {
            return _operations[i];
        }
    }
}