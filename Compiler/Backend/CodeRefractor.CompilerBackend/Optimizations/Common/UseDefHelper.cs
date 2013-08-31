#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    public static class UseDefHelper
    {
        public static LocalOperation GetNextUsage(this List<LocalOperation> operations, LocalVariable variable,
                                                  int startPos, out int pos)
        {
            pos = startPos;
            for (var i = startPos; i < operations.Count; i++)
            {
                var operation = operations[i];
                pos = i;
                if (operation.IsBranchOperation())
                    return null;
                if (OperationUses(operation, variable))
                    return operation;
            }
            return null;
        }

        public static List<LocalVariable> GetUsages(this LocalOperation operation)
        {
            var result = new List<LocalVariable>();
            switch (operation.Kind)
            {
                case LocalOperation.Kinds.Assignment:
                    result.AddUsage(((Assignment)operation.Value).Right);
                    break;
                case LocalOperation.Kinds.UnaryOperator:
                    var unaryOperator = ((UnaryOperator)operation.Value);
                    result.AddUsage(unaryOperator.Left);
                    break;
                case LocalOperation.Kinds.BranchOperator:
                    var branchOperator = ((BranchOperator)operation.Value);
                    result.AddUsage(branchOperator.CompareValue);
                    break;
                case LocalOperation.Kinds.BinaryOperator:
                    var binaryOperator = ((BinaryOperator)operation.Value);
                    result.AddUsage(binaryOperator.Left);
                    result.AddUsage(binaryOperator.Right);
                    break;
                case LocalOperation.Kinds.NewArray:
                    AddUsagesOfNewArray(operation, result);
                    break;
                case LocalOperation.Kinds.GetArrayItem:
                    AddUsagesOfGetArrayItem(operation, result);
                    break;
                case LocalOperation.Kinds.SetArrayItem:
                    AddUsagesOfSetArrayItem(operation, result);
                    break;

                case LocalOperation.Kinds.GetField:
                    AddUsagesOfGetField(operation, result);
                    break;
                case LocalOperation.Kinds.SetField:
                    AddUsagesOfSetField(operation, result);
                    break;

                case LocalOperation.Kinds.Call:
                    AddUsagesOfCall(operation, result);
                    break;
                case LocalOperation.Kinds.Return:    
                    AddUsagesOfReturn(operation, result);
                    break;

                case LocalOperation.Kinds.Label:
                case LocalOperation.Kinds.NewObject:
                case LocalOperation.Kinds.AlwaysBranch:
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }

        private static void AddUsagesOfCall(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (MethodData)operation.Value;
            var parameters = assignment.Parameters;
            foreach (var parameter in parameters)
            {
                result.AddUsage(parameter);
            }
        }

        private static void AddUsagesOfReturn(LocalOperation operation, List<LocalVariable> result)
        {
            var returnedValue = operation.Get<IdentifierValue>();

            result.AddUsage(returnedValue);
            
        }
        private static void AddUsagesOfGetField(LocalOperation operation, List<LocalVariable> result)
        {
            var arrayVar = (FieldGetter)operation.Value;
            result.AddUsage(arrayVar.Instance);
        }
        private static void AddUsagesOfSetField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (FieldSetter)assignment.AssignedTo;
            result.AddUsage(arrayVar.Instance);
            result.AddUsage(assignment.Right);
        }



        private static void AddUsagesOfNewArray(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (NewArrayObject)assignment.Right;
            result.AddUsage(arrayVar.ArrayLength);
        }

        private static void AddUsagesOfGetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (ArrayVariable)assignment.Right;
            result.AddUsage(arrayVar.Parent);
            result.AddUsage(arrayVar.Index);
        }


        private static void AddUsagesOfSetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (ArrayVariable)assignment.AssignedTo;
            result.AddUsage(arrayVar.Parent);
            result.AddUsage(arrayVar.Index);
            result.AddUsage(assignment.Right);
        }

        public static bool OperationUses(this LocalOperation operation, LocalVariable variable)
        {
            var result = GetUsages(operation);
            if (result.Count == 0)
                return false;

            return result.Any(localVariable => localVariable.Equals(variable));
        }

        private static void AddUsage(this List<LocalVariable> usages, IdentifierValue usage)
        {
            var localVar = usage as LocalVariable;
            if (localVar == null)
                return;
            usages.Add(localVar);
        }

        public static HashSet<int> GetVariableDefinitions(this MetaMidRepresentation midRepresentation, LocalVariable variable)
        {
            var result = new HashSet<int>();
            var pos = -1;
            foreach (var localOperation in midRepresentation.LocalOperations)
            {
                pos++;
                var definition = localOperation.GetUseDefinition();
                if (variable.Equals(definition))
                    result.Add(pos);
            }
            return result;
        } 
        public static LocalVariable GetUseDefinition(this LocalOperation operation)
        {
            var kind = operation.Kind;
            switch (kind)
            {
                case LocalOperation.Kinds.Assignment:
                case LocalOperation.Kinds.NewObject:
                case LocalOperation.Kinds.NewArray:
                    var assign = operation.GetAssignment();
                    return assign.AssignedTo;
                case LocalOperation.Kinds.BinaryOperator:
                    var binOp = (BinaryOperator)operation.Value;
                    return binOp.AssignedTo;
                case LocalOperation.Kinds.UnaryOperator:
                    var unOp = (UnaryOperator)operation.Value;
                    return unOp.AssignedTo;

                default:
                    return null;
            }
        }

        public static HashSet<int> GetVariableUsages(this MetaMidRepresentation representation, LocalVariable variable)
        {
            return representation.LocalOperations.GetVariableUsages(variable);
        }

        public static HashSet<int> GetVariableUsages(this List<LocalOperation> localOperations, LocalVariable variable)
        {
            var result = new HashSet<int>();
            var pos = -1;
            foreach (var op in localOperations)
            {
                pos++;

                if (op.OperationUses(variable))
                    result.Add(pos);

            }
            return result;
        }

        public static void SwitchUsageWithDefinition(this LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            if (!(definitionIdentifier is LocalVariable) && !(definitionIdentifier is ConstValue))
                return;
            switch (op.Kind)
            {
                case LocalOperation.Kinds.Assignment:
                    SwitchUsageInAssignment(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.BinaryOperator:
                    SwitchUsageInBinaryOperator(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.UnaryOperator:
                    SwitchUsageInUnaryOperator(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.BranchOperator:
                    SwitchUsageInBranchOperator(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.GetField:
                    SwichUsageInGetField(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.SetField:
                    SwitchUsageInSetField(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.SetArrayItem:
                    SwitchUsageInSetArrayItem(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.GetArrayItem:
                    SwitchUsageInGetArrayItem(op, usageVariable, definitionIdentifier);
                    break;

                case LocalOperation.Kinds.Call:
                    SwitchUsageInCall(op, usageVariable, definitionIdentifier);
                    break;

                case LocalOperation.Kinds.NewArray:
                    SwitchUsageInNewArray(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.Return:
                    SwitchUsageInReturn(op, usageVariable, definitionIdentifier);
                    break;
                case LocalOperation.Kinds.AlwaysBranch:
                case LocalOperation.Kinds.Label:
                case LocalOperation.Kinds.NewObject:
                    break;
                default:
                    throw new NotImplementedException("Switch usage is not implemented for this operation kind");
            }
        }

        private static void SwitchUsageInReturn(LocalOperation op, LocalVariable usageVariable,
                                                IdentifierValue definitionIdentifier)
        {
            var returnValue = op.Get<IdentifierValue>();
            if (usageVariable.Equals(returnValue))
            {
                op.Value = definitionIdentifier;
            }
        }

        private static void SwitchUsageInNewArray(LocalOperation op, LocalVariable usageVariable,
                                                  IdentifierValue definitionIdentifier)
        {
            var arrayVar = (NewArrayObject) op.GetAssignment().Right;
            if (usageVariable.Equals(arrayVar.ArrayLength))
                arrayVar.ArrayLength = definitionIdentifier;
        }

        private static void SwitchUsageInCall(LocalOperation op, LocalVariable usageVariable,
                                              IdentifierValue definitionIdentifier)
        {
            var methodData = op.Get<MethodData>();
            for (var index = 0; index < methodData.Parameters.Count; index++)
            {
                var identifierValue = methodData.Parameters[index];
                if (usageVariable.Equals(identifierValue))
                    methodData.Parameters[index] = definitionIdentifier;
            }
        }

        private static void SwitchUsageInGetArrayItem(LocalOperation op, LocalVariable usageVariable,
                                                      IdentifierValue definitionIdentifier)
        {
            var opGetArrayItem = (Assignment) op.Value;
            var getArrayData = (ArrayVariable) opGetArrayItem.Right;
            if (usageVariable.Equals(getArrayData.Parent))
            {
                getArrayData.Parent = definitionIdentifier;
            }
            if (usageVariable.Equals(getArrayData.Index))
            {
                getArrayData.Index = definitionIdentifier;
            }
            if (usageVariable.Equals(opGetArrayItem.AssignedTo))
            {
                opGetArrayItem.AssignedTo = (LocalVariable) definitionIdentifier;
            }
        }

        private static void SwitchUsageInSetArrayItem(LocalOperation op, LocalVariable usageVariable,
                                                      IdentifierValue definitionIdentifier)
        {
            var opSetArrayItem = (Assignment) op.Value;
            var setArrayData = (ArrayVariable) opSetArrayItem.AssignedTo;
            if (usageVariable.Equals(setArrayData.Parent))
            {
                setArrayData.Parent = definitionIdentifier;
            }
            if (usageVariable.Equals(setArrayData.Index))
            {
                setArrayData.Index = definitionIdentifier;
            }
            if (usageVariable.Equals(opSetArrayItem.Right))
            {
                opSetArrayItem.Right = definitionIdentifier;
            }
        }

        private static void SwitchUsageInSetField(LocalOperation op, LocalVariable usageVariable,
                                                  IdentifierValue definitionIdentifier)
        {
            var opSetField = (Assignment) op.Value;
            var setFieldData = (FieldSetter) opSetField.AssignedTo;
            if (usageVariable.Equals(setFieldData.Instance))
            {
                setFieldData.Instance = definitionIdentifier;
            }
            if (usageVariable.Equals(opSetField.Right))
            {
                opSetField.Right = definitionIdentifier;
            }
        }

        private static void SwichUsageInGetField(LocalOperation op, LocalVariable usageVariable,
                                                 IdentifierValue definitionIdentifier)
        {
            var getFieldData = (FieldGetter)op.Value;
            if (usageVariable.Equals(getFieldData.Instance))
            {
                getFieldData.Instance = definitionIdentifier;
            }
        }

        private static void SwitchUsageInBranchOperator(LocalOperation op, LocalVariable usageVariable,
                                                        IdentifierValue definitionIdentifier)
        {
            var opBranchOperator = (BranchOperator) op.Value;
            if (usageVariable.Equals(opBranchOperator.CompareValue))
            {
                opBranchOperator.CompareValue = definitionIdentifier;
            }
        }

        private static void SwitchUsageInUnaryOperator(LocalOperation op, LocalVariable usageVariable,
                                                       IdentifierValue definitionIdentifier)
        {
            var opUnaryOperator = (UnaryOperator) op.Value;
            if (usageVariable.Equals(opUnaryOperator.Left))
            {
                opUnaryOperator.Left = definitionIdentifier;
            }
        }

        private static void SwitchUsageInBinaryOperator(LocalOperation op, LocalVariable usageVariable,
                                                        IdentifierValue definitionIdentifier)
        {
            var opBinaryOperator = (BinaryOperator) op.Value;
            if (usageVariable.Equals(opBinaryOperator.Right))
            {
                opBinaryOperator.Right = definitionIdentifier;
            }
            if (usageVariable.Equals(opBinaryOperator.Left))
            {
                opBinaryOperator.Left = definitionIdentifier;
            }
        }

        private static void SwitchUsageInAssignment(LocalOperation op, LocalVariable usageVariable,
                                                    IdentifierValue definitionIdentifier)
        {
            var opAssignment = (Assignment) op.Value;
            if (usageVariable.Equals(opAssignment.Right))
            {
                opAssignment.Right = definitionIdentifier;
            }
        }
    }
}