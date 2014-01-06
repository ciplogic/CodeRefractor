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

        public static List<LocalVariable> GetUsagesAndDefinitions(this LocalOperation operation)
        {
            var usages = GetUsages(operation);
            var def = GetDefinition(operation);
            if (def != null)
            {
                usages.Add(def);
            }
            return usages;
        }

        public static List<int> GetUsagesAndDefinitions(this MetaMidRepresentation intermediateCode,
            LocalVariable localVariable)
        {
            var result = new List<int>();
            var instructions = intermediateCode.LocalOperations;
            for(var index = 0; index<instructions.Count;index++)
            {
                var instruction = instructions[index];
                var usages = GetUsagesAndDefinitions(instruction);
                foreach (var variable in usages)
                {
                    if (variable.Equals(localVariable))
                    {
                        result.Add(index);
                        break;
                    }
                }
            }
            return result;
        }

        public static List<LocalVariable> GetUsages(this LocalOperation operation)
        {
            var result = new List<LocalVariable>(2);
            switch (operation.Kind)
            {
                case OperationKind.Assignment:
                    result.AddUsage(((Assignment)operation.Value).Right);
                    break;
                case OperationKind.UnaryOperator:
                    var unaryOperator = ((UnaryOperator)operation.Value);
                    result.AddUsage(unaryOperator.Left);
                    break;
                case OperationKind.BranchOperator:
                    var branchOperator = ((BranchOperator)operation.Value);
                    result.AddUsage(branchOperator.CompareValue); ;
                    result.AddUsage(branchOperator.SecondValue);
                    break;
                case OperationKind.BinaryOperator:
                    var binaryOperator = ((BinaryOperator)operation.Value);
                    result.AddUsage(binaryOperator.Left);
                    result.AddUsage(binaryOperator.Right);
                    break;
                case OperationKind.NewArray:
                    AddUsagesOfNewArray(operation, result);
                    break;
                case OperationKind.GetArrayItem:
                    AddUsagesOfGetArrayItem(operation, result);
                    break;
                case OperationKind.SetArrayItem:
                    AddUsagesOfSetArrayItem(operation, result);
                    break;

                case OperationKind.GetField:
                    AddUsagesOfGetField(operation, result);
                    break;
                case OperationKind.SetField:
                    AddUsagesOfSetField(operation, result);
                    break;

                case OperationKind.Call:
                case OperationKind.CallRuntime:
                    AddUsagesOfCall(operation, result);
                    break;
                case OperationKind.Return:
                    AddUsagesOfReturn(operation, result);
                    break;

                case OperationKind.Label:
                case OperationKind.NewObject:
                case OperationKind.AlwaysBranch:
                    break;

                case OperationKind.FieldRefAssignment:
                case OperationKind.CopyArrayInitializer:
                case OperationKind.GetStaticField:
                case OperationKind.LoadFunction:
                    break;

                case OperationKind.Comment:
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
            return result.Count != 0 &&
                result.Any(localVariable => localVariable.Equals(variable));
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
                var definition = localOperation.GetDefinition();
                if (variable.Equals(definition))
                    result.Add(pos);
            }
            return result;
        }

        public static Assignment GetAssignment(this LocalOperation operation)
        {
            return operation.Value as Assignment;
        }

        public static LocalVariable GetDefinition(this LocalOperation operation)
        {
            var kind = operation.Kind;
            switch (kind)
            {
                case OperationKind.Assignment:
                case OperationKind.NewObject:
                case OperationKind.NewArray:
                case OperationKind.CopyArrayInitializer:
                case OperationKind.GetStaticField:
                case OperationKind.GetArrayItem:
                    var assign = operation.GetAssignment();
                    return assign.AssignedTo;
                case OperationKind.GetField:
                    var fieldGetter = (FieldGetter)operation.Value;
                    return fieldGetter.AssignedTo;
                case OperationKind.BinaryOperator:
                case OperationKind.UnaryOperator:
                    var binOp = (OperatorBase)operation.Value;
                    return binOp.AssignedTo;
                case OperationKind.Call:
                    var value = (MethodData)operation.Value;
                    return value.Result;
                default:
                    return null;
            }
        }

        public static List<int> GetVariableUsages(this MetaMidRepresentation representation, LocalVariable variable)
        {
            return representation.LocalOperations.GetVariableUsages(variable);
        }

        public static List<int> GetVariableUsages(this List<LocalOperation> localOperations, LocalVariable variable)
        {
            var startIndex = 0;
            var endIndex = localOperations.Count - 1;

            var result = GetVariableUsages(localOperations, variable, startIndex, endIndex);
            return result;
        }

        public static List<int> GetVariableUsages(List<LocalOperation> localOperations, LocalVariable variable, int startIndex, int endIndex)
        {
            var result = new List<int>();
            for (var index = startIndex; index <= endIndex; index++)
            {
                var op = localOperations[index];

                if (op.OperationUses(variable))
                    result.Add(index);
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
                case OperationKind.Assignment:
                    SwitchUsageInAssignment(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.BinaryOperator:
                    SwitchUsageInBinaryOperator(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.UnaryOperator:
                    SwitchUsageInUnaryOperator(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.BranchOperator:
                    SwitchUsageInBranchOperator(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.GetField:
                    SwichUsageInGetField(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.SetField:
                    SwitchUsageInSetField(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.SetArrayItem:
                    SwitchUsageInSetArrayItem(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.GetArrayItem:
                    SwitchUsageInGetArrayItem(op, usageVariable, definitionIdentifier);
                    break;

                case OperationKind.Call:
                case OperationKind.CallRuntime:
                    SwitchUsageInCall(op, usageVariable, definitionIdentifier);
                    break;

                case OperationKind.NewArray:
                    SwitchUsageInNewArray(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.Return:
                    SwitchUsageInReturn(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.AlwaysBranch:
                case OperationKind.Label:
                case OperationKind.NewObject:
                    break;
                default:
                    throw new NotImplementedException("Switch usage is not implemented for this operation OperationKind");
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
            var assign = op.GetAssignment();
            var arrayVar = (NewArrayObject)assign.Right;
            if (usageVariable.Equals(assign.AssignedTo))
                assign.AssignedTo = (LocalVariable)definitionIdentifier;
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
            var opGetArrayItem = (Assignment)op.Value;
            var getArrayData = (ArrayVariable)opGetArrayItem.Right;
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
                opGetArrayItem.AssignedTo = (LocalVariable)definitionIdentifier;
            }
        }

        private static void SwitchUsageInSetArrayItem(LocalOperation op, LocalVariable usageVariable,
                                                      IdentifierValue definitionIdentifier)
        {
            var opSetArrayItem = (Assignment)op.Value;
            var setArrayData = (ArrayVariable)opSetArrayItem.AssignedTo;
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
            var opSetField = (Assignment)op.Value;
            var setFieldData = (FieldSetter)opSetField.AssignedTo;
            if (usageVariable.Equals(setFieldData.Instance))
            {
                setFieldData.Instance = definitionIdentifier;
            }
            if (usageVariable.Equals(opSetField.Right))
            {
                opSetField.Right = definitionIdentifier;
            }
            if (usageVariable.Equals(opSetField.AssignedTo))
            {
                opSetField.AssignedTo = (LocalVariable)definitionIdentifier;
            }
        }

        private static void SwichUsageInGetField(LocalOperation op, LocalVariable usageVariable,
                                                 IdentifierValue definitionIdentifier)
        {
            var getFieldData = (FieldGetter)op.Value;
            if (!(definitionIdentifier is LocalVariable))
                return;
            if (usageVariable.Equals(getFieldData.Instance))
            {
                getFieldData.Instance = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(getFieldData.AssignedTo))
            {
                getFieldData.AssignedTo = (LocalVariable)definitionIdentifier;
            }
        }

        private static void SwitchUsageInBranchOperator(LocalOperation op, LocalVariable usageVariable,
                                                        IdentifierValue definitionIdentifier)
        {
            var opBranchOperator = (BranchOperator)op.Value;
            if (usageVariable.Equals(opBranchOperator.CompareValue))
            {
                opBranchOperator.CompareValue = definitionIdentifier;
            }
            if (usageVariable.Equals(opBranchOperator.SecondValue))
            {
                opBranchOperator.SecondValue = definitionIdentifier;
            }
        }

        private static void SwitchUsageInUnaryOperator(LocalOperation op, LocalVariable usageVariable,
                                                       IdentifierValue definitionIdentifier)
        {
            var opUnaryOperator = (UnaryOperator)op.Value;
            if (usageVariable.Equals(opUnaryOperator.Left))
            {
                opUnaryOperator.Left = definitionIdentifier;
            }
            if (usageVariable.Equals(opUnaryOperator.AssignedTo))
            {
                opUnaryOperator.AssignedTo = (LocalVariable)definitionIdentifier;
            }
        }

        private static void SwitchUsageInBinaryOperator(LocalOperation op, LocalVariable usageVariable,
                                                        IdentifierValue definitionIdentifier)
        {
            var opBinaryOperator = (BinaryOperator)op.Value;
            if (usageVariable.Equals(opBinaryOperator.AssignedTo))
            {
                opBinaryOperator.AssignedTo = (LocalVariable)definitionIdentifier;
            }
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
            var opAssignment = (Assignment)op.Value;
            if (usageVariable.Equals(opAssignment.AssignedTo))
            {
                opAssignment.AssignedTo = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(opAssignment.Right))
            {
                opAssignment.Right = definitionIdentifier;
            }
        }
    }
}