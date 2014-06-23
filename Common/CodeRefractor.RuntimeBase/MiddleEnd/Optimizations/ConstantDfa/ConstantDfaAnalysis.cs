#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantDfa
{
    internal class ConstantDfaAnalysis : ResultingInFunctionOptimizationPass
    {
        private Dictionary<int, int> _labelTable = new Dictionary<int, int>();
        private LocalOperation[] _operations;
        private DfaPointOfAnalysis[] _pointsOfAnalysis;

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            _operations = interpreter.MidRepresentation.LocalOperations.ToArray();
            _labelTable = interpreter.MidRepresentation.UseDef.GetLabelTable();
            _pointsOfAnalysis = new DfaPointOfAnalysis[_operations.Length + 1];

            var startingConclusions = new DfaPointOfAnalysis();
            Interpret(0, startingConclusions);

            ApplyResult();
        }

        private void ApplyResult()
        {
            Assignment assignment;
            for (var i = 0; i < _operations.Length; i++)
            {
                var operation = _operations[i];
                switch (operation.Kind)
                {
                    case OperationKind.Assignment:
                        assignment = operation.GetAssignment();
                        HandleAssignment(i, assignment);
                        break;
                    case OperationKind.BinaryOperator:
                        HandleOperator(i, (OperatorBase) operation);
                        break;
                    case OperationKind.BranchOperator:
                        var branchOperator = (BranchOperator) operation;
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

        private void HandleOperator(int i, OperatorBase assignment)
        {
            var binary = assignment as BinaryOperator;
            var unary = assignment as UnaryOperator;
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
            if (startingConclusions.Equals(_pointsOfAnalysis[cursor]))
                return;
            while (canUpdate)
            {
                _pointsOfAnalysis[cursor] = startingConclusions.Merge(_pointsOfAnalysis[cursor]);

                var operation = GetOperation(cursor);
                var analysis = _pointsOfAnalysis[cursor];
                Assignment assignment;
                switch (operation.Kind)
                {
                    case OperationKind.Assignment:
                        assignment = operation.GetAssignment();
                        var constant = assignment.Right as ConstValue;
                        if (constant != null)
                        {
                            analysis.States[assignment.AssignedTo] = new VariableState
                            {
                                Constant = constant,
                                State = VariableState.ConstantState.Constant
                            };
                        }
                        else
                        {
                            analysis.States[assignment.AssignedTo] = new VariableState
                            {
                                State = VariableState.ConstantState.NotConstant
                            };
                        }
                        startingConclusions = analysis;
                        break;

                    case OperationKind.BinaryOperator:
                        assignment = (Assignment) operation;
                        analysis.States[assignment.AssignedTo] = new VariableState
                        {
                            State = VariableState.ConstantState.NotConstant
                        };
                        break;
                    case OperationKind.BranchOperator:
                        var branchOperator = (BranchOperator) operation;
                        Interpret(JumpTo(branchOperator.JumpTo), analysis);
                        break;
                    case OperationKind.Label:
                        break;
                    case OperationKind.Call:
                        break;
                    case OperationKind.Return:
                        return;
                    case OperationKind.AlwaysBranch:
                        var jumpTo = ((AlwaysBranch)operation).JumpTo;
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