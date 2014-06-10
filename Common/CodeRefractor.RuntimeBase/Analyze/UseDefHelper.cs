#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public static class UseDefHelper
    {
        #region GetUsages

        public static List<LocalVariable> GetUsages(this LocalOperation operation)
        {
            var result = new List<LocalVariable>(2);
            switch (operation.Kind)
            {
                case OperationKind.Assignment:
                    result.AddUsage(((Assignment) operation.Value).Right);
                    break;
                case OperationKind.UnaryOperator:
                    var unaryOperator = ((UnaryOperator) operation.Value);
                    result.AddUsage(unaryOperator.Left);
                    break;
                case OperationKind.BranchOperator:
                    var branchOperator = ((BranchOperator) operation.Value);
                    result.AddUsage(branchOperator.CompareValue);
                    ;
                    result.AddUsage(branchOperator.SecondValue);
                    break;
                case OperationKind.BinaryOperator:
                    var binaryOperator = ((BinaryOperator) operation.Value);
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
                case OperationKind.SetStaticField:
                    AddUsagesOfSetStaticField(operation, result);
                    break;

                case OperationKind.Call:
                case OperationKind.CallVirtual:
                case OperationKind.CallInterface:
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
                    AddUsagesOfFieldRefAssignment(operation, result);
                    break;
                case OperationKind.RefAssignment:
                    AddUsagesOfRefAssignment(operation, result);
                    break;
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

        private static void AddUsagesOfRefAssignment(LocalOperation operation, List<LocalVariable> result)
        {
            var refData = (RefAssignment) operation.Value;
            result.Add(refData.Left);
            result.Add(refData.Right);
        }

        private static void AddUsagesOfFieldRefAssignment(LocalOperation operation, List<LocalVariable> result)
        {
            var refFieldValue = (FieldRefAssignment) operation.Value;
            result.Add(refFieldValue.Right);
        }

        private static void AddUsagesOfSetStaticField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment) operation.Value;
            result.AddUsage(assignment.Right);
        }

        private static void AddUsagesOfCall(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (MethodData) operation.Value;
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
            var arrayVar = (FieldGetter) operation.Value;
            result.AddUsage(arrayVar.Instance);
        }

        private static void AddUsagesOfSetField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment) operation.Value;
            var arrayVar = (FieldSetter) assignment.AssignedTo;
            result.AddUsage(arrayVar.Instance);
            result.AddUsage(assignment.Right);
        }

        private static void AddUsagesOfNewArray(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment) operation.Value;
            var arrayVar = (NewArrayObject) assignment.Right;
            result.AddUsage(arrayVar.ArrayLength);
        }

        private static void AddUsagesOfGetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment) operation.Value;
            var arrayVar = (ArrayVariable) assignment.Right;
            result.AddUsage(arrayVar.Parent);
            result.AddUsage(arrayVar.Index);
        }


        private static void AddUsagesOfSetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment) operation.Value;
            var arrayVar = (ArrayVariable) assignment.AssignedTo;
            result.AddUsage(arrayVar.Parent);
            result.AddUsage(arrayVar.Index);
            result.AddUsage(assignment.Right);
        }

        public static bool OperationUses(this LocalOperation operation, LocalVariable variable)
        {
            var result = GetUsages(operation);
            return result.Contains(variable);
        }

        private static void AddUsage(this List<LocalVariable> usages, IdentifierValue usage)
        {
            var localVar = usage as LocalVariable;
            if (localVar == null)
                return;
            usages.Add(localVar);
        }

        public static Assignment GetAssignment(this LocalOperation operation)
        {
            return operation.Value as Assignment;
        }

        #endregion  //GetUsages

        #region GetDefinition

        public static LocalVariable GetDefinition(this LocalOperation operation)
        {
            var kind = operation.Kind;
            switch (kind)
            {
                case OperationKind.Return:
                case OperationKind.AlwaysBranch:
                case OperationKind.Label:
                case OperationKind.BranchOperator:
                case OperationKind.SetField:
                case OperationKind.SetArrayItem:
                case OperationKind.SetStaticField:
                    return null;
                case OperationKind.Assignment:
                case OperationKind.NewObject:
                case OperationKind.NewArray:
                case OperationKind.CopyArrayInitializer:
                case OperationKind.GetStaticField:
                case OperationKind.GetArrayItem:
                    var assign = operation.GetAssignment();
                    return assign.AssignedTo;
                case OperationKind.GetField:
                    var fieldGetter = (FieldGetter) operation.Value;
                    return fieldGetter.AssignedTo;
                case OperationKind.BinaryOperator:
                case OperationKind.UnaryOperator:
                    var binOp = (OperatorBase) operation.Value;
                    return binOp.AssignedTo;
                case OperationKind.Call:
                case OperationKind.CallVirtual:
                case OperationKind.CallInterface:
                    var value = (MethodData) operation.Value;
                    return value.Result;
                case OperationKind.FieldRefAssignment:
                    var refFieldValue = (FieldRefAssignment) operation.Value;
                    return refFieldValue.Left;

                case OperationKind.RefAssignment:
                    var refValue = (RefAssignment) operation.Value;
                    return refValue.Left;
                default:
                    throw new InvalidDataException("Case not handled");
            }
        }

        #endregion

        public static HashSet<LocalVariable> GetAllUsedVariables(MethodInterpreter interpreter,
            bool includeDefinitions = false)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            return GetAllUsedVariables(useDef, includeDefinitions);
        }

        public static HashSet<LocalVariable> GetAllUsedVariables(this UseDefDescription useDef,
            bool includeDefinitions = false)
        {
            var result = new HashSet<LocalVariable>();
            var operations = useDef.GetLocalOperations();
            for (var index = 0; index < operations.Length; index++)
            {
                var op = operations[index];
                var usages = useDef.GetUsages(index);
                foreach (var usage in usages)
                {
                    result.Add(usage);
                }
                if (includeDefinitions)
                {
                    var definition = useDef.GetDefinition(index);
                    if (definition != null)
                    {
                        result.Add(definition);
                    }
                }
            }
            return result;
        }

        public static List<int> GetVariableUsages(this IList<LocalOperation> localOperations, LocalVariable variable)
        {
            var startIndex = 0;
            var endIndex = localOperations.Count - 1;

            var result = GetVariableUsages(localOperations, variable, startIndex, endIndex);
            return result;
        }

        public static List<int> GetVariableUsages(IList<LocalOperation> localOperations, LocalVariable variable,
            int startIndex, int endIndex)
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

        public static void SwitchAllUsagesWithDefinition(this MethodInterpreter interpreter, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var midRep = interpreter.MidRepresentation;
            var localOperations = midRep.UseDef.GetLocalOperations();
            foreach (var operation in localOperations)
            {
                operation.SwitchUsageWithDefinition(usageVariable, definitionIdentifier);
            }

            midRep.UpdateUseDef();
        }

        #region SwitchUsageWithDefinition

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
                case OperationKind.SetStaticField:
                    SwitchUsageInSetStaticField(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.SetArrayItem:
                    SwitchUsageInSetArrayItem(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.GetArrayItem:
                    SwitchUsageInGetArrayItem(op, usageVariable, definitionIdentifier);
                    break;

                case OperationKind.Call:
                case OperationKind.CallVirtual:
                case OperationKind.CallInterface:
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

                case OperationKind.RefAssignment:
                    SwitchUsageInRefAssignment(op, usageVariable, definitionIdentifier);
                    break;

                case OperationKind.FieldRefAssignment:
                    SwitchUsageInFieldRefAssignment(op, usageVariable, definitionIdentifier);
                    break;
                default:
                    throw new NotImplementedException(
                        string.Format("Switch usage is not implemented for this operation '{0}'", op.Kind));
            }
        }

        private static void SwitchUsageInRefAssignment(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var returnValue = op.Get<RefAssignment>();
            if (usageVariable.Equals(returnValue.Right))
            {
                returnValue.Right = (LocalVariable) definitionIdentifier;
            }
            if (usageVariable.Equals(returnValue.Left))
            {
                returnValue.Left = (LocalVariable) definitionIdentifier;
            }
        }

        private static void SwitchUsageInFieldRefAssignment(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var returnValue = op.Get<FieldRefAssignment>();
            if (usageVariable.Equals(returnValue.Right))
            {
                returnValue.Right = (LocalVariable) definitionIdentifier;
            }
            if (usageVariable.Equals(returnValue.Left))
            {
                returnValue.Left = (LocalVariable) definitionIdentifier;
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
            var arrayVar = (NewArrayObject) assign.Right;
            if (usageVariable.Equals(assign.AssignedTo))
                assign.AssignedTo = (LocalVariable) definitionIdentifier;
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
                getArrayData.Parent = (LocalVariable) definitionIdentifier;
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
                setArrayData.Parent = (LocalVariable) definitionIdentifier;
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
            if (usageVariable.Equals(opSetField.AssignedTo))
            {
                opSetField.AssignedTo = (LocalVariable) definitionIdentifier;
            }
        }

        private static void SwitchUsageInSetStaticField(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opSetField = (Assignment) op.Value;

            if (usageVariable.Equals(opSetField.Right))
            {
                opSetField.Right = definitionIdentifier;
            }
        }


        private static void SwichUsageInGetField(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var getFieldData = (FieldGetter) op.Value;
            if (!(definitionIdentifier is LocalVariable))
                return;
            if (usageVariable.Equals(getFieldData.Instance))
            {
                getFieldData.Instance = (LocalVariable) definitionIdentifier;
            }
            if (usageVariable.Equals(getFieldData.AssignedTo))
            {
                getFieldData.AssignedTo = (LocalVariable) definitionIdentifier;
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
            if (usageVariable.Equals(opBranchOperator.SecondValue))
            {
                opBranchOperator.SecondValue = definitionIdentifier;
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
            if (usageVariable.Equals(opUnaryOperator.AssignedTo))
            {
                opUnaryOperator.AssignedTo = (LocalVariable) definitionIdentifier;
            }
        }

        private static void SwitchUsageInBinaryOperator(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opBinaryOperator = (BinaryOperator) op.Value;
            if (usageVariable.Equals(opBinaryOperator.AssignedTo))
            {
                opBinaryOperator.AssignedTo = (LocalVariable) definitionIdentifier;
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
            var opAssignment = (Assignment) op.Value;
            if (usageVariable.Equals(opAssignment.AssignedTo))
            {
                opAssignment.AssignedTo = (LocalVariable) definitionIdentifier;
            }
            if (usageVariable.Equals(opAssignment.Right))
            {
                opAssignment.Right = definitionIdentifier;
            }
        }

        #endregion // SwitchUsageWithDefinition
    }
}