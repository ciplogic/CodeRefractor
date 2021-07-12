#region Uses

using System;
using System.Collections.Generic;
using System.IO;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Casts;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.Interpreters.Cil;

#endregion

namespace CodeRefractor.MiddleEnd.UseDefs
{
    public static class UseDefHelper
    {
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
                case OperationKind.SizeOf:
                    return null;
                case OperationKind.StaticFieldRefAssignment:
                    return null;

                case OperationKind.CopyArrayInitializer:
                case OperationKind.GetStaticField:
                    var assign = operation.GetAssignment();
                    return assign.AssignedTo;
                case OperationKind.GetArrayItem:
                    return operation.Get<GetArrayElement>().AssignedTo;
                case OperationKind.Assignment:
                    return operation.Get<Assignment>().AssignedTo;
                case OperationKind.NewObject:
                    return operation.Get<NewConstructedObject>().AssignedTo;
                case OperationKind.NewArray:
                    return operation.Get<NewArrayObject>().AssignedTo;
                case OperationKind.GetField:
                    var fieldGetter = (GetField) operation;
                    return fieldGetter.AssignedTo;
                case OperationKind.Box:
                    var boxing = (Boxing) operation;
                    return boxing.AssignedTo;
                case OperationKind.Unbox:
                    var unboxing = (Unboxing) operation;
                    return unboxing.AssignedTo;

                case OperationKind.CastClass:
                    var cast = (ClassCasting) operation;
                    return cast.AssignedTo;
                case OperationKind.IsInstance:
                    var isInst = (IsInstance) operation;
                    return isInst.AssignedTo;

                case OperationKind.BinaryOperator:
                case OperationKind.UnaryOperator:
                    var binOp = (OperatorBase) operation;
                    return binOp.AssignedTo;
                case OperationKind.Call:
                case OperationKind.CallVirtual:
                case OperationKind.CallInterface:
                    var value = (CallMethodStatic) operation;
                    return value.Result;
                case OperationKind.FieldRefAssignment:
                    var refFieldValue = (FieldRefAssignment) operation;
                    return refFieldValue.Left;

                case OperationKind.RefAssignment:
                    var refValue = (RefAssignment) operation;
                    return refValue.Left;

                case OperationKind.DerefAssignment:
                    var refValue2 = (DerefAssignment) operation;
                    return refValue2.Left;

                case OperationKind.LoadFunction:
                    var val = (FunctionPointerStore) operation;
                    //  return null;
                    return (LocalVariable) val.AssignedTo;

                case OperationKind.AddressOfArrayItem:
                    var refValue3 = (RefArrayItemAssignment) operation;
                    return refValue3.Left;

                default:
                    throw new InvalidDataException("Case not handled");
            }
        }

        #endregion

        public static HashSet<LocalVariable> GetAllUsedVariables(CilMethodInterpreter interpreter,
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

        public static void SwitchAllUsagesWithDefinition(this CilMethodInterpreter interpreter,
            LocalVariable usageVariable,
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

        #region GetUsages

        public static List<LocalVariable> GetUsages(this LocalOperation operation)
        {
            var result = new List<LocalVariable>(2);
            switch (operation.Kind)
            {
                case OperationKind.Assignment:
                    result.AddUsage(((Assignment) operation).Right);
                    break;
                case OperationKind.Box:
                    result.AddUsage(((Boxing) operation).Right);
                    break;
                case OperationKind.Unbox:
                    result.AddUsage(((Unboxing) operation).Right);
                    break;
                case OperationKind.CastClass:
                    result.AddUsage(((ClassCasting) operation).Value);
                    break;
                case OperationKind.IsInstance:
                    result.AddUsage(((IsInstance) operation).Right);
                    break;
                case OperationKind.UnaryOperator:
                    var unaryOperator = ((UnaryOperator) operation);
                    result.AddUsage(unaryOperator.Left);
                    break;
                case OperationKind.BranchOperator:
                    var branchOperator = ((BranchOperator) operation);
                    result.AddUsage(branchOperator.CompareValue);
                    ;
                    result.AddUsage(branchOperator.SecondValue);
                    break;
                case OperationKind.BinaryOperator:
                    var binaryOperator = ((BinaryOperator) operation);
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

                case OperationKind.DerefAssignment:
                    AddUsagesOfDerefAssignment(operation, result);
                    break;

                case OperationKind.CopyArrayInitializer:
                case OperationKind.GetStaticField:
                case OperationKind.LoadFunction:
                    break;

                case OperationKind.Comment:
                    break;

                case OperationKind.SizeOf:
                    AddUsagesOfSizeOf((SizeOfAssignment) operation, result);
                    break;
                case OperationKind.AddressOfArrayItem:
                    AddUsagesOfRefArrayItemAssignment(operation, result);
                    break;
                case OperationKind.StaticFieldRefAssignment:
                    break;

                default:
                    throw new NotImplementedException();
            }
            return result;
        }
        

        static void AddUsagesOfRefAssignment(LocalOperation operation, List<LocalVariable> result)
        {
            var refData = (RefAssignment) operation;
            result.Add(refData.Left);
            result.Add(refData.Right);
        }


        static void AddUsagesOfRefArrayItemAssignment(LocalOperation operation, List<LocalVariable> result)
        {
            var refData = (RefArrayItemAssignment) operation;
            result.Add(refData.Left);
            result.Add(refData.ArrayVar);
        }
        
        static void AddUsagesOfDerefAssignment(LocalOperation operation, List<LocalVariable> result)
        {
            var refData = (DerefAssignment)operation;
            result.Add(refData.Left);
            result.Add(refData.Right);
        }


        static void AddUsagesOfFieldRefAssignment(LocalOperation operation, List<LocalVariable> result)
        {
            var refFieldValue = (FieldRefAssignment)operation;
            result.Add(refFieldValue.Right);
        }

        static void AddUsagesOfSetStaticField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation;
            result.AddUsage(assignment.Right);
        }

        static void AddUsagesOfCall(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (CallMethodStatic)operation;
            var parameters = assignment.Parameters;
            foreach (var parameter in parameters)
            {
                result.AddUsage(parameter);
            }
        }

        public static T Get<T>(this LocalOperation operation) where T : LocalOperation
        {
            return operation as T;
        }

        static void AddUsagesOfSizeOf(SizeOfAssignment sizeOf, List<LocalVariable> result)
        {
            result.AddUsage(sizeOf.AssignedTo);
        }

        static void AddUsagesOfReturn(LocalOperation operation, List<LocalVariable> result)
        {
            var returnedValue = operation.Get<Return>();

            result.AddUsage(returnedValue.Returning);
        }

        static void AddUsagesOfGetField(LocalOperation operation, List<LocalVariable> result)
        {
            var arrayVar = (GetField)operation;
            result.AddUsage(arrayVar.Instance);
        }

        static void AddUsagesOfSetField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (SetField)operation;
            result.AddUsage(assignment.Instance);
            result.AddUsage(assignment.Right);
        }

        static void AddUsagesOfNewArray(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (NewArrayObject)operation;
            result.AddUsage(assignment.ArrayLength);
        }

        static void AddUsagesOfGetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (GetArrayElement)operation;
            result.AddUsage(assignment.Instance);
            result.AddUsage(assignment.Index);
        }


        static void AddUsagesOfSetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (SetArrayElement)operation;
            result.AddUsage(assignment.Instance);
            result.AddUsage(assignment.Index);
            result.AddUsage(assignment.Right);
        }

        public static bool OperationUses(this LocalOperation operation, LocalVariable variable)
        {
            var result = GetUsages(operation);
            return result.Contains(variable);
        }

        static void AddUsage(this List<LocalVariable> usages, IdentifierValue usage)
        {
            var localVar = usage as LocalVariable;
            if (localVar == null)
                return;
            usages.Add(localVar);
        }

        public static Assignment GetAssignment(this LocalOperation operation)
        {
            return operation as Assignment;
        }

        #endregion  //GetUsages

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
                case OperationKind.IsInstance:
                    SwitchUsageInIsInstance(op, usageVariable, definitionIdentifier);
                    break;

                case OperationKind.RefAssignment:
                    SwitchUsageInRefAssignment(op, usageVariable, definitionIdentifier);
                    break;

                case OperationKind.FieldRefAssignment:
                    SwitchUsageInFieldRefAssignment(op, usageVariable, definitionIdentifier);
                    break;
                case OperationKind.Unbox:
                    SwitchUsageUnbox(op, usageVariable, definitionIdentifier);
                    break;
                default:
                    throw new NotImplementedException(
                        $"Switch usage is not implemented for this operation '{op.Kind}'");
            }
        }

        private static void SwitchUsageUnbox(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var unbox = (Unboxing) op;
            if (usageVariable.Equals(unbox.AssignedTo))
                unbox.AssignedTo = (LocalVariable) definitionIdentifier;
            if (usageVariable.Equals(unbox.Right))
            {
                unbox.Right = definitionIdentifier;
            }
        }

        static void SwitchUsageInRefAssignment(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var returnValue = op.Get<RefAssignment>();
            if (usageVariable.Equals(returnValue.Right))
            {
                returnValue.Right = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(returnValue.Left))
            {
                returnValue.Left = (LocalVariable)definitionIdentifier;
            }
        }

        static void SwitchUsageInFieldRefAssignment(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var returnValue = op.Get<FieldRefAssignment>();
            if (usageVariable.Equals(returnValue.Right))
            {
                returnValue.Right = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(returnValue.Left))
            {
                returnValue.Left = (LocalVariable)definitionIdentifier;
            }
        }

        static void SwitchUsageInReturn(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var returnValue = op.Get<Return>();
            if (usageVariable.Equals(returnValue.Returning))
            {
                returnValue.Returning = definitionIdentifier;
            }
        }

        static void SwitchUsageInNewArray(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var assign = (NewArrayObject)op;
            if (usageVariable.Equals(assign.AssignedTo))
                assign.AssignedTo = (LocalVariable)definitionIdentifier;
            if (usageVariable.Equals(assign.ArrayLength))
                assign.ArrayLength = definitionIdentifier;
        }

        static void SwitchUsageInCall(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var methodData = op.Get<CallMethodStatic>();
            for (var index = 0; index < methodData.Parameters.Count; index++)
            {
                var identifierValue = methodData.Parameters[index];
                if (usageVariable.Equals(identifierValue))
                    methodData.Parameters[index] = definitionIdentifier;
            }
            if (methodData.Result != null && definitionIdentifier is LocalVariable)
            {
                if (usageVariable.Equals(methodData.Result))
                    methodData.Result = (LocalVariable)definitionIdentifier;
            }
        }

        static void SwitchUsageInGetArrayItem(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var getArrayData = (GetArrayElement)op;
            if (usageVariable.Equals(getArrayData.Instance))
            {
                getArrayData.Instance = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(getArrayData.Index))
            {
                getArrayData.Index = definitionIdentifier;
            }
            if (usageVariable.Equals(getArrayData.AssignedTo))
            {
                getArrayData.AssignedTo = (LocalVariable)definitionIdentifier;
            }
        }

        static void SwitchUsageInSetArrayItem(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var setArrayData = (SetArrayElement)op;
            if (usageVariable.Equals(setArrayData.Instance))
            {
                setArrayData.Instance = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(setArrayData.Index))
            {
                setArrayData.Index = definitionIdentifier;
            }
            if (usageVariable.Equals(setArrayData.Right))
            {
                setArrayData.Right = definitionIdentifier;
            }
        }

        static void SwitchUsageInSetField(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opSetField = (SetField)op;
            if (usageVariable.Equals(opSetField.Instance))
            {
                opSetField.Instance = definitionIdentifier;
            }
            if (usageVariable.Equals(opSetField.Right))
            {
                opSetField.Right = definitionIdentifier;
            }
        }

        static void SwitchUsageInSetStaticField(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opSetField = (Assignment)op;

            if (usageVariable.Equals(opSetField.Right))
            {
                opSetField.Right = definitionIdentifier;
            }
        }


        static void SwichUsageInGetField(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var getFieldData = (GetField)op;
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

        static void SwitchUsageInBranchOperator(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opBranchOperator = (BranchOperator)op;
            if (usageVariable.Equals(opBranchOperator.CompareValue))
            {
                opBranchOperator.CompareValue = definitionIdentifier;
            }
            if (usageVariable.Equals(opBranchOperator.SecondValue))
            {
                opBranchOperator.SecondValue = definitionIdentifier;
            }
        }

        static void SwitchUsageInUnaryOperator(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opUnaryOperator = (UnaryOperator)op;
            if (usageVariable.Equals(opUnaryOperator.Left))
            {
                opUnaryOperator.Left = definitionIdentifier;
            }
            if (usageVariable.Equals(opUnaryOperator.AssignedTo))
            {
                opUnaryOperator.AssignedTo = (LocalVariable)definitionIdentifier;
            }
        }

        static void SwitchUsageInBinaryOperator(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opBinaryOperator = (BinaryOperator)op;
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


        static void SwitchUsageInIsInstance(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opAssignment = (IsInstance)op;
            if (usageVariable.Equals(opAssignment.AssignedTo))
            {
                opAssignment.AssignedTo = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(opAssignment.Right))
            {
                opAssignment.Right = definitionIdentifier;
            }
        }

        static void SwitchUsageInAssignment(LocalOperation op, LocalVariable usageVariable,
            IdentifierValue definitionIdentifier)
        {
            var opAssignment = (Assignment)op;
            if (usageVariable.Equals(opAssignment.AssignedTo))
            {
                opAssignment.AssignedTo = (LocalVariable)definitionIdentifier;
            }
            if (usageVariable.Equals(opAssignment.Right))
            {
                opAssignment.Right = definitionIdentifier;
            }
        }

        #endregion // SwitchUsageWithDefinition
    }
}