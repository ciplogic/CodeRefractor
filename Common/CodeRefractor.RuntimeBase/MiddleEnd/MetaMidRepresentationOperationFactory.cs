#region Usings

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.Shared;
using MsilReader;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MetaMidRepresentationOperationFactory
    {
        private readonly MetaMidRepresentation _representation;
        private readonly EvaluatorStack _evaluator;

        private int _leaveOffset = -1;

        public void LeaveTo(int offsetToLeave)
        {
            _leaveOffset = offsetToLeave;
        }

        public override string ToString()
        {
            var method = _representation.ToString();
            var stackText = _evaluator.ToString();
            return string.Format("Stack: {2} items: ({0}) Method: {1}",
                stackText,
                method,
                _evaluator.Count);
        }

        public bool SkipInstruction(int offset)
        {
            if (_leaveOffset == -1)
                return false;
            if (offset < _leaveOffset)
                return true;
            _leaveOffset = -1;
            return false;
        }

        public MetaMidRepresentationOperationFactory(MetaMidRepresentation representation, EvaluatorStack evaluator)
        {
            _representation = representation;
            _evaluator = evaluator;
        }


        private void AddOperation(OperationKind operationKind, object value = null)
        {
            var result = new LocalOperation
            {
                Kind = operationKind,
                Value = value
            };

            _representation.LocalOperations.Add(result);
            var assignment = result.Value as Assignment;
            if (assignment != null)
            {
                if (assignment.AssignedTo.FixedType == null)
                    throw new InvalidOperationException(
                        String.Format("The data introduced in the IR should be well typed. " +
                                      Environment.NewLine + "Operation: {0}", result));
            }
        }

        private LocalVariable SetNewVReg()
        {
            var newLocal = _evaluator.SetNewVReg();
            _representation.Vars.VirtRegs.Add(newLocal);
            return newLocal;
        }

        private void PushStack(IdentifierValue identifier)
        {
            _evaluator.Push(identifier);
        }

        private void AssignValueToStack(object value)
        {
            PushStack(new ConstValue(value));
        }

        public void CopyLocalVariableIntoStack(int value)
        {
            var locVar = _representation.Vars.LocalVars.First(v => v.Id == value);

            PushStack(locVar);
        }

        public LocalVariable GetVirtReg(int value)
        {
            return  _representation.Vars.VirtRegs.First(v => v.Id == value);
        }


        public void PushInt4(int value)
        {
            AssignValueToStack(value);
        }

        public void PushInt8(long value)
        {
            AssignValueToStack(value);
        }

        public void PushString(string value)
        {
            AssignValueToStack(value);
        }

        public void PushDouble(double value)
        {
            AssignValueToStack(value);
        }

        public void LoadArgument(int pushedIntValue)
        {
            var argument = _representation.Vars.Arguments[pushedIntValue];
            PushStack(argument);
        }

        public void CopyStackIntoArgument(int value)
        {
            var topVariable = _evaluator.Top;

            _evaluator.Pop();
            var newLocal = _representation.Vars.Arguments[value];
            var assingment = new Assignment
            {
                AssignedTo = newLocal,
                Right = topVariable
            };

            AddOperation(OperationKind.Assignment, assingment);
        }

        public void CopyStackIntoLocalVariable(int value)
        {
            var topVariable = _evaluator.Top;
            _evaluator.Pop();
            var newLocal = _representation.Vars.LocalVars[value];
            var assingment = new Assignment
            {
                AssignedTo = newLocal,
                Right = topVariable
            };
            AddOperation(OperationKind.Assignment, assingment);
        }

        private static bool ShowComments = false;

        public void AddCommentInstruction(string comment)
        {
            if (ShowComments)
            {
                AddOperation(OperationKind.Comment, comment);
            }
        }

        public void StoreField(FieldInfo fieldInfo)
        {
            var secondVar = _evaluator.Pop();
            var firstVar = _evaluator.Pop();
            var fieldName = fieldInfo.Name;
            var assignment = new Assignment
            {
                AssignedTo = new FieldSetter
                {
                    Instance = firstVar,
                    FieldName = fieldName
                },
                Right = secondVar
            };
            assignment.AssignedTo.FixedType = secondVar.ComputedType();
            AddOperation(OperationKind.SetField, assignment);
        }

        public void LoadReferenceInArray()
        {
            var secondVar = _evaluator.Pop();
            var firstVar = (LocalVariable) _evaluator.Pop();

            var result = SetNewVReg();
            var arrayVariable = new ArrayVariable(firstVar, secondVar);
            var assignment = new Assignment
            {
                AssignedTo = result,
                Right = arrayVariable
            };
            result.FixedType = new TypeDescription(arrayVariable.GetElementType());
            AddOperation(OperationKind.GetArrayItem, assignment);
        }

        public void LoadReferenceInArrayTyped(Type elementType)
        {
            var secondVar = _evaluator.Pop();
            var firstVar = (LocalVariable) _evaluator.Pop();

            var result = SetNewVReg();
            result.FixedType = new TypeDescription(firstVar.FixedType.ClrType.GetElementType());
            var arrayVariable = new ArrayVariable(firstVar, secondVar);
            var assignment = new Assignment
            {
                AssignedTo = result,
                Right = arrayVariable
            };
            result.FixedType = new TypeDescription(elementType);
            AddOperation(OperationKind.GetArrayItem, assignment);
        }

        public void Return(bool isVoid)
        {
            var returnValue = isVoid ? null : _evaluator.Pop();
            AddOperation(OperationKind.Return, returnValue);
        }


        private void SetUnaryOperator(string operatorName)
        {
            var firstVar = _evaluator.Pop();
            var result = SetNewVReg();


            var assign = new UnaryOperator(operatorName)
            {
                AssignedTo = result,
                Left = firstVar,
            };
            assign.AssignedTo.FixedType = firstVar.ComputedType();
            AddOperation(OperationKind.UnaryOperator, assign);
        }

        public void ConvI()
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvI);
            _evaluator.Top.FixedType = new TypeDescription(typeof (IntPtr));
        }

        public void ConvI4()
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvI4);
            _evaluator.Top.FixedType = new TypeDescription(typeof (int));
        }

        public void ConvU1()
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvU1);
            _evaluator.Top.FixedType = new TypeDescription(typeof (byte));
        }

        public void Not()
        {
            SetUnaryOperator(OpcodeOperatorNames.Not);
        }

        public void Neg()
        {
            SetUnaryOperator(OpcodeOperatorNames.Neg);
        }


        public void LoadLength()
        {
            SetUnaryOperator(OpcodeOperatorNames.LoadLen);

            _evaluator.Top.FixedType = new TypeDescription(typeof (int));
        }

        public void ConvI8()
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvI8);
            _evaluator.Top.FixedType = new TypeDescription(typeof (Int64));
        }

        public void ConvR4()
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvR4);
            _evaluator.Top.FixedType = new TypeDescription(typeof (float));
        }

        public void ConvR8()
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvR8);
            _evaluator.Top.FixedType = new TypeDescription(typeof (double));
        }


        public void Dup()
        {
            var variable = _evaluator.Top;
            var vreg = SetNewVReg();
            var assign = new Assignment
            {
                AssignedTo = vreg,
                Right = variable
            };
            vreg.FixedType = variable.ComputedType();
            AddOperation(OperationKind.Assignment, assign);
        }

        public void Pop()
        {
            _evaluator.Pop();
        }


        private void SetBinaryOperator(string operatorName)
        {
            var secondVar = _evaluator.Pop();
            var firstVar = _evaluator.Pop();

            var result = SetNewVReg();
            var assign = new BinaryOperator(operatorName)
            {
                AssignedTo = result,
                Left = firstVar,
                Right = secondVar
            };
            result.FixedType = assign.ComputedType();
            AddOperation(OperationKind.BinaryOperator, assign);
        }

        public void SetLabel(int offset)
        {
            AddOperation(OperationKind.Label, offset);
        }

        public void Cgt()
        {
            SetBinaryOperator("cgt");
        }

        public void Ceq()
        {
            SetBinaryOperator("ceq");
        }

        public void Clt()
        {
            SetBinaryOperator("clt");
        }

        #region Operators

        public void Add()
        {
            SetBinaryOperator(OpcodeOperatorNames.Add);
        }

        public void Sub()
        {
            SetBinaryOperator(OpcodeOperatorNames.Sub);
        }

        public void Div()
        {
            SetBinaryOperator(OpcodeOperatorNames.Div);
        }

        public void Rem()
        {
            SetBinaryOperator(OpcodeOperatorNames.Rem);
        }

        public void Mul()
        {
            SetBinaryOperator(OpcodeOperatorNames.Mul);
        }

        public void And()
        {
            SetBinaryOperator(OpcodeOperatorNames.And);
        }

        public void Or()
        {
            SetBinaryOperator(OpcodeOperatorNames.Or);
        }

        public void Xor()
        {
            SetBinaryOperator(OpcodeOperatorNames.Xor);
        }

        #endregion

        public static bool HandleRuntimeHelpersMethod(MethodBase method)
        {
            var declType = method.DeclaringType;
            return declType == typeof (RuntimeHelpers);
        }


        public void Call(object operand, CrRuntimeLibrary crRuntime)
        {
            var methodInfo = (MethodBase) operand;
            var interpreter = methodInfo.Register(crRuntime);
            var methodData = new MethodData(interpreter);


            CallMethodData(methodInfo, methodData, OperationKind.Call, crRuntime);
        }

        public void CallVirtual(object operand, CrRuntimeLibrary crRuntime)
        {
            var methodInfo = (MethodBase) operand;
            var interpreter = methodInfo.Register(crRuntime);
            var methodData = new MethodData(interpreter);


            CallMethodData(methodInfo, methodData, OperationKind.CallVirtual, crRuntime);
        }

        public void CallInterface(object operand, CrRuntimeLibrary crRuntime)
        {
            var methodInfo = (MethodBase) operand;
            var interpreter = methodInfo.Register(crRuntime);
            var methodData = new MethodData(interpreter);


            CallMethodData(methodInfo, methodData, OperationKind.CallVirtual, crRuntime);
        }

        private void CallMethodData(MethodBase methodInfo, MethodData methodData, OperationKind operationKind,
            CrRuntimeLibrary crRuntime)
        {
            if (HandleRuntimeHelpersMethod(methodInfo))
            {
                methodData.ExtractNeededValuesFromStack(_evaluator);
                AddOperation(OperationKind.CallRuntime, methodData);
                return;
            }
            if (methodInfo.IsConstructor && methodInfo.DeclaringType == typeof (object))
                return;
            methodData.ExtractNeededValuesFromStack(_evaluator);

            
            if (!methodData.Info.IsStatic && methodData.Parameters.Count > 0 )
            {
                //GetBestVirtualMatch not required as the appropriate method is called in CppHandleCalls and VirtualMethod
                //TODO: GetBestVirtualMatch does not work  with base class constructors
               // if(!methodInfo.IsConstructor && methodData.Parameters[0].ComputedType().ClrType.DeclaringType!=methodInfo.DeclaringType)
              //  methodData.Info = ClassHierarchyAnalysis.GetBestVirtualMatch(methodData.Info,
              //      methodData.Parameters[0].ComputedType().ClrType);
            }
            var declaringType = methodData.Info.DeclaringType;
            if (declaringType.IsSubclassOf(typeof (Delegate)))
            {
                var signature = declaringType.GetMethod("Invoke");
                DelegateManager.RegisterType(declaringType, signature);
            }

            if (!methodData.IsVoid)
            {
                var vreg = SetNewVReg();
                vreg.FixedType = new TypeDescription(methodInfo.GetReturnType());
                methodData.Result = vreg;
            }
            methodData.FixedType = new TypeDescription(methodInfo.GetReturnType());
            AddOperation(operationKind, methodData);
        }


        public void StoreStaticField(FieldInfo fieldInfo)
        {
            var firstVar = _evaluator.Pop();
            var fieldName = fieldInfo.Name;
            var assignment = new Assignment
            {
                AssignedTo = new StaticFieldSetter
                {
                    DeclaringType = fieldInfo.DeclaringType,
                    FieldName = fieldName
                },
                Right = firstVar
            };
            assignment.AssignedTo.FixedType = firstVar.ComputedType();
            AddOperation(OperationKind.SetStaticField, assignment);
        }


        public void StoresValueFromAddress()
        {
            var varAddress = _evaluator.Pop();
            var varValue = _evaluator.Pop();

            var assignment = new DerefAssignment
            {
                Left = (LocalVariable) varAddress,
                Right = (LocalVariable) varValue
            };

            AddOperation(OperationKind.DerefAssignment, assignment);
        }

        #region Branching operators

        public void BranchIfTrue(int pushedIntValue)
        {
            var firstVar = _evaluator.Pop();
            AddOperation(OperationKind.BranchOperator,
                new BranchOperator(OpcodeBranchNames.BrTrue)
                {
                    JumpTo = pushedIntValue,
                    CompareValue = firstVar
                });
        }

        public void BranchIfFalse(int pushedIntValue)
        {
            var firstVar = _evaluator.Pop();
            AddOperation(OperationKind.BranchOperator,
                new BranchOperator(OpcodeBranchNames.BrFalse)
                {
                    JumpTo = pushedIntValue,
                    CompareValue = firstVar
                });
        }


        public void AlwaysBranch(int offset)
        {
            AddOperation(OperationKind.AlwaysBranch, offset);
        }

        public void BranchIfEqual(int jumpTo)
        {
            BranchTwoOperators(jumpTo, OpcodeBranchNames.Beq);
        }

        private void BranchTwoOperators(int jumpTo, string opcode)
        {
            var secondVar = _evaluator.Pop(); // Seems the order here was in reverse
            var firstVar = _evaluator.Pop();

            AddOperation(OperationKind.BranchOperator,
                new BranchOperator(opcode)
                {
                    JumpTo = jumpTo,
                    CompareValue = firstVar,
                    SecondValue = secondVar
                });
        }

        public void BranchIfGreaterOrEqual(int jumpTo)
        {
            BranchTwoOperators(jumpTo, OpcodeBranchNames.Bge);
        }

        public void BranchIfGreater(int jumpTo)
        {
            BranchTwoOperators(jumpTo, OpcodeBranchNames.Bgt);
        }

        public void BranchIfLessOrEqual(int jumpTo)
        {
            BranchTwoOperators(jumpTo, OpcodeBranchNames.Ble);
        }

        public void BranchIfLess(int jumpTo)
        {
            BranchTwoOperators(jumpTo, OpcodeBranchNames.Blt);
        }

        public void BranchIfNotEqual(int jumpTo)
        {
            BranchTwoOperators(jumpTo, OpcodeBranchNames.Bne);
        }

        #endregion

        public void LoadAddressIntoEvaluationStack(LocalVariableInfo index)
        {
            var vreg = SetNewVReg();
            vreg.FixedType = new TypeDescription(
                index.LocalType.MakeByRefType());

            var argument = _representation.Vars.LocalVars.First(v => v.Id == index.LocalIndex);
            var assignment = new RefAssignment
            {
                Left = vreg,
                Right = argument
            };
            AddOperation(OperationKind.RefAssignment, assignment);
        }

        public void LoadFieldAddressIntoEvaluationStack(FieldInfo fieldInfo)
        {
            var firstVar = (LocalVariable) _evaluator.Pop();
            var vreg = SetNewVReg();
            vreg.FixedType = new TypeDescription(fieldInfo.FieldType.MakeByRefType());

            var assignment = new FieldRefAssignment
            {
                Left = vreg,
                Right = firstVar,
                Field = fieldInfo
            };
            AddOperation(OperationKind.FieldRefAssignment, assignment);
        }

        public void LoadField(string fieldName)
        {
            var firstVar = _evaluator.Pop();

            var vreg = SetNewVReg();
            var computedType = firstVar.ComputedType();
            if (computedType.ClrType.IsByRef)
            {
                computedType = new TypeDescription(computedType.ClrType.GetElementType());
            }
            vreg.FixedType =
                new TypeDescription(
                    computedType.ClrType.LocateField(fieldName).FieldType);
            var assignment = new FieldGetter
            {
                AssignedTo = vreg,
                FieldName = fieldName,
                Instance = (LocalVariable) firstVar
            };
            AddOperation(OperationKind.GetField, assignment);
        }

        public void LoadStaticField(FieldInfo operand)
        {
            var vreg = SetNewVReg();
            var fieldName = operand.Name;
            var declaringType = operand.DeclaringType;
            if (declaringType == typeof (IntPtr) && fieldName == "Zero")
            {
                var voidPtr = new TypeDescription(typeof (IntPtr));
                vreg.FixedType = voidPtr;
                var nullPtrAssign = new Assignment
                {
                    AssignedTo = vreg,
                    Right = new ConstValue(0)
                    {
                        FixedType = voidPtr
                    }
                };
                AddOperation(OperationKind.Assignment, nullPtrAssign);
                return;
            }
            vreg.FixedType =
                new TypeDescription(declaringType.LocateField(fieldName, true).FieldType);
            var typeData = new TypeDescription(declaringType);
            var assignment = new Assignment
            {
                AssignedTo = vreg,
                Right = new StaticFieldGetter
                {
                    FieldName = fieldName,
                    DeclaringType = typeData
                }
            };
            AddOperation(OperationKind.GetStaticField, assignment);
        }

        public void NewObject(ConstructorInfo constructorInfo, CrRuntimeLibrary crRuntime)
        {
            if (constructorInfo.DeclaringType == typeof (object))
                return;
            var mappedType = crRuntime.GetMappedType(constructorInfo.DeclaringType);
            if (mappedType != null && mappedType != constructorInfo.DeclaringType)
            {
                constructorInfo = crRuntime.GetMappedConstructor(constructorInfo);
            }
            constructorInfo.Register();
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(constructorInfo.DeclaringType);
            var constructedObject = new NewConstructedObject(constructorInfo);
            var assignment = new Assignment
            {
                AssignedTo = result,
                Right = constructedObject
            };
            AddOperation(OperationKind.NewObject, assignment);

            var interpreter = constructedObject.Info.Register(crRuntime);
            var methodData = new MethodData(interpreter);
            CallMethodData(constructedObject.Info, methodData, OperationKind.Call, crRuntime);
            var vreg = SetNewVReg();
            vreg.FixedType = new TypeDescription(methodData.Info.DeclaringType);
            var assign = new Assignment
            {
                AssignedTo = vreg,
                Right = methodData.Parameters.First()
            };

            AddOperation(OperationKind.Assignment, assign);
        }

        public void NewArray(Type typeArray)
        {
            var arrayLength = _evaluator.Pop();
            var result = SetNewVReg();
            var assignment = new Assignment
            {
                AssignedTo = result,
                Right = new NewArrayObject
                {
                    TypeArray = typeArray,
                    ArrayLength = arrayLength
                }
            };
            result.FixedType = new TypeDescription(typeArray.MakeArrayType());
            AddOperation(OperationKind.NewArray, assignment);
        }

        public void SetArrayElementValue()
        {
            var value = _evaluator.Pop();
            var index = _evaluator.Pop();
            var array = (LocalVariable) _evaluator.Pop();
            var arrayVariable = new ArrayVariable(array, index);
            var assignment = new Assignment
            {
                AssignedTo = arrayVariable,
                Right = value
            };
            arrayVariable.FixedType = value.ComputedType();
            AddOperation(OperationKind.SetArrayItem, assignment);
        }

        public void SetToken(FieldInfo operand)
        {
            var fieldOperand = operand;
            var fieldType = fieldOperand.FieldType;
            var nesterType = fieldType.DeclaringType;
            var fields = nesterType.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            var value = fields[0].GetValue(null);
            var srcBytes = value.ToByteArray();
            var vreg = SetNewVReg();
            var rightConstant = ConstByteArrayList.RegisterConstant(srcBytes);
            var assign = new Assignment
            {
                AssignedTo = vreg,
                Right = new ConstByteArrayValue(rightConstant)
            };
            vreg.FixedType = new TypeDescription(typeof (byte[]));
            AddOperation(OperationKind.CopyArrayInitializer, assign);
        }

        public void LoadNull()
        {
            AssignNullToStack();
        }

        private void AssignNullToStack()
        {
            var nullConst = new ConstValue(null)
            {
                FixedType = new TypeDescription(typeof (object))
            };
            PushStack(nullConst);
        }

        public void LoadFunction(MethodInfo operand)
        {
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(operand.GetType());
            result.CustomData = operand;
            var ptr = operand.MethodHandle.GetFunctionPointer();
            var store = new FunctionPointerStore()
            {
                AssignedTo = result,
                FunctionPointer = operand
            };

            AddOperation(OperationKind.LoadFunction, store);
        }

        public void Switch(Instruction[] instructions)
        {
            var firstVar = (LocalVariable) _evaluator.Pop();
            var offsets = instructions.Select(inst => inst.Offset).ToArray();

            var pos = 0;
            foreach (var offset in offsets)
            {
                AddOperation(OperationKind.BranchOperator,
                    new BranchOperator(OpcodeBranchNames.Beq)
                    {
                        JumpTo = offset,
                        CompareValue = firstVar,
                        SecondValue = new ConstValue(pos)
                    });
                pos++;
            }
        }

        public void SizeOf(Type operand)
        {
            var result = SetNewVReg();
            var assign = new SizeOfAssignment
            {
                AssignedTo = result,
                Right = operand
            };
            result.FixedType = new TypeDescription(typeof (int));
            AddOperation(OperationKind.SizeOf, assign);
        }

        public void LoadValueFromAddress()
        {
            var firstVar = (LocalVariable) _evaluator.Pop();


            var result = SetNewVReg();
            var assignment = new DerefAssignment
            {
                Left = result,
                Right = firstVar
            };
            var ptrType = firstVar.ComputedType();

            if (ptrType.ClrType.GetElementType()!=null)
            result.FixedType = new TypeDescription(ptrType.ClrType.GetElementType());

            else
            {

                result.FixedType = new TypeDescription(ptrType.ClrType);
            }
            AddOperation(OperationKind.DerefAssignment, assignment);
        }

        public void InitObject()
        {
            //TODO: make a mem-clear of the structure that is referenced
        }

        public void LoadAddressOfArrayItemIntoStack(Type operand)
        {
            var indexVar = _evaluator.Pop();
            var arrayVar = (LocalVariable) _evaluator.Pop();

            var result = SetNewVReg();
            var assignment = new RefArrayItemAssignment
            {
                Left = result,
                Index = indexVar,
                ArrayVar = arrayVar
            };
            assignment.Left.FixedType = new TypeDescription(operand.MakeByRefType());
            AddOperation(OperationKind.AddressOfArrayItem, assignment);
        }

        public void StoreElement(Type elemInfo)
        {
            var value = _evaluator.Pop();
            var index = _evaluator.Pop();
            var array = (LocalVariable) _evaluator.Pop();
            var arrayVariable = new ArrayVariable(array, index);
            var assignment = new Assignment
            {
                AssignedTo = arrayVariable,
                Right = value
            };
            arrayVariable.FixedType = new TypeDescription(elemInfo);
            AddOperation(OperationKind.SetArrayItem, assignment);
        }

        public void ClearStack()
        {
            _evaluator.Clear();
        }

        public void Throw()
        {

            var valueToCast = _evaluator.Pop();
          
        //    var result = SetNewVReg();
//            var fieldName = fieldInfo.Name;
//            var assignment = new Assignment
//            {
//                AssignedTo = new StaticFieldSetter
//                {
//                    DeclaringType = fieldInfo.DeclaringType,
//                    FieldName = fieldName
//                },
//                Right = firstVar
//            };
//            assignment.AssignedTo.FixedType = firstVar.ComputedType();
//            AddOperation(OperationKind.SetStaticField, assignment);
        }
        
        public void CastClass(Type operand)
        {
             

            var valueToCast = _evaluator.Pop(); 
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(operand);
            var casting = new ClassCasting()
            {
                AssignedTo = result,
                Value = valueToCast
            };
            AddOperation(OperationKind.CastClass, casting);
        }

        public void Box()
        {
            var valueToBox = _evaluator.Pop(); 
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(typeof(object));
            var boxing = new Boxing()
            {
                AssignedTo = result,
                Value = valueToBox
            };
            AddOperation(OperationKind.Box, boxing);
        }

        public void Unbox(Type operand)
        {
            var valueToBox = _evaluator.Pop();
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(operand);
            var boxing = new Unboxing()
            {
                AssignedTo = result,
                Value = valueToBox
            };
            AddOperation(OperationKind.Unbox, boxing);
        }

        public void LoadObject(Type operand) // TODO: Fix this
        {
            var valueAddress = (LocalVariable)_evaluator.Pop();

            //Look up instance
           var localvar=  GetVirtReg(valueAddress.Id);
           AssignValueToStack(localvar);
        }

        public void StoreObject(Type operand)// TODO: Fix this
        {  
            var value = _evaluator.Pop();
            var valueAddress = (LocalVariable)_evaluator.Pop();
          
            //Look up instance
            var assign = new Assignment
            {
                AssignedTo = valueAddress,
                Right = value
            };
          
            AddOperation(OperationKind.Assignment, assign);
            
        }
    }
}