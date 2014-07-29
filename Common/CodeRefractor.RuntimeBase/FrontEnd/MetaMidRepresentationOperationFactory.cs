#region Uses

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Casts;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;
using Mono.Reflection;

#endregion

namespace CodeRefractor.FrontEnd
{
    public class MetaMidRepresentationOperationFactory
    {
        private readonly MetaMidRepresentation _representation;
        private readonly EvaluatorStack _evaluator;

        private int _leaveOffset = -1;
        public Type ConstrainedClass { get; set; }

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


        private void AddOperation(LocalOperation value = null)
        {
            _representation.LocalOperations.Add(value);
            var assignment = value as Assignment;
            if (assignment != null)
            {
                if (assignment.AssignedTo.FixedType == null)
                    throw new InvalidOperationException(
                        String.Format("The data introduced in the IR should be well typed. " +
                                      Environment.NewLine + "Operation: {0}", value));
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
            var local = value as LocalVariable;
            if (local != null)
                _evaluator.Push(local);
            else
                PushStack(new ConstValue(value));
        }

        public void CopyLocalVariableIntoStack(int value)
        {
            var locVar = _representation.Vars.LocalVars.First(v => v.Id == value);

            PushStack(locVar);
        }

        public LocalVariable GetVirtReg(int value)
        {
            return _representation.Vars.VirtRegs.First(v => v.Id == value);
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

        public void LoadArgument(int pushedIntValue, AnalyzeProperties methodProperties)
        {
            var argument = methodProperties.Arguments[pushedIntValue];
            PushStack(argument);
        }

        public void CopyStackIntoArgument(int value, AnalyzeProperties methodProperties)
        {
            var topVariable = _evaluator.Top;

            _evaluator.Pop();
            var newLocal = methodProperties.Arguments[value];
            var assingment = new Assignment
            {
                AssignedTo = newLocal,
                Right = topVariable
            };

            AddOperation(assingment);
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
            AddOperation(assingment);
        }

        private static bool ShowComments = false;

        public void AddCommentInstruction(string comment)
        {
            if (ShowComments)
            {
                AddOperation(new Comment {Message = comment});
            }
        }

        public void StoreField(FieldInfo fieldInfo)
        {
            var secondVar = _evaluator.Pop();
            var firstVar = _evaluator.Pop();
            var fieldName = fieldInfo.Name;
            var assignment = new SetField
            {
                Instance = firstVar,
                FieldName = fieldName,
                Right = secondVar
            };
            assignment.FixedType = secondVar.ComputedType();
            AddOperation(assignment);
        }

        public void LoadReferenceInArray()
        {
            var secondVar = _evaluator.Pop();
            var firstVar = (LocalVariable) _evaluator.Pop();

            var result = SetNewVReg();
            var arrayVariable = new GetArrayElement
            {
                AssignedTo = result,
                Instance = firstVar,
                Index = secondVar,
            };
            result.FixedType = new TypeDescription(arrayVariable.GetElementType());
            AddOperation(arrayVariable);
        }

        public void LoadReferenceInArrayTyped()
        {
            var index = _evaluator.Pop();
            var instance = (LocalVariable) _evaluator.Pop();

            var result = SetNewVReg();
            var arrayVariable = GetArrayElement.Create(result, instance, index);

            AddOperation(arrayVariable);
        }

        public void Return(bool isVoid)
        {
            var returnValue = isVoid ? null : _evaluator.Pop();
            AddOperation(new Return
            {
                Returning = returnValue
            });
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
            AddOperation(assign);
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
            AddOperation(assign);
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
            AddOperation(assign);
        }

        public void SetLabel(int offset)
        {
            AddOperation(new Label {JumpTo = offset});
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


        public void Call(MethodBase operand)
        {
            var methodInfo = operand;
            //TODO System.Void System.Array::Resize<System.Char>(!!0[]&,System.Int32) Weird Signature

            if (methodInfo == null) return;
            MethodInterpreter interpreter;
            if (PlatformInvokeMethod.IsPlatformInvoke(methodInfo))
            {
                interpreter = new PlatformInvokeMethod(methodInfo);
                var pMethodData = new CallMethodStatic(interpreter);
                CallMethodData(methodInfo, pMethodData);
                return;

            }
            interpreter = new CilMethodInterpreter(methodInfo);
            var methodData = new CallMethodStatic(interpreter);
            CallMethodData(methodInfo, methodData);
        }

        public void CallVirtual(MethodBase operand)
        {
            var methodInfo = operand;

            var interpreter = new CilMethodInterpreter(methodInfo);

            var methodData = new CallMethodVirtual(interpreter);

            CallMethodDataVirtual(methodInfo, methodData);
        }

        private void CallMethodDataVirtual(MethodBase methodInfo, CallMethodVirtual callMethodStatic)
        {
            if (HandleRuntimeHelpersMethod(methodInfo))
            {
                callMethodStatic.ExtractNeededValuesFromStack(_evaluator);
                AddOperation(callMethodStatic);
                return;
            }
            if (methodInfo.IsConstructor && methodInfo.DeclaringType == typeof (object))
                return;
            callMethodStatic.ExtractNeededValuesFromStack(_evaluator);


            if (!callMethodStatic.Info.IsStatic && callMethodStatic.Parameters.Count > 0)
            {
                //GetBestVirtualMatch not required as the appropriate method is called in CppHandleCalls and VirtualMethod
                //TODO: GetBestVirtualMatch does not work  with base class constructors
                // if(!methodInfo.IsConstructor && CallMethodStatic.Parameters[0].ComputedType().ClrType.DeclaringType!=methodInfo.DeclaringType)
                //  CallMethodStatic.Info = ClassHierarchyAnalysis.GetBestVirtualMatch(CallMethodStatic.Info,
                //      CallMethodStatic.Parameters[0].ComputedType().ClrType);
            }
            var declaringType = callMethodStatic.Info.DeclaringType;
            if (declaringType.IsSubclassOf(typeof (Delegate)))
            {
                var signature = declaringType.GetMethod("Invoke");
                DelegateManager.RegisterType(declaringType, signature);
            }

            if (!callMethodStatic.IsVoid)
            {
                var vreg = SetNewVReg();
                vreg.FixedType = new TypeDescription(methodInfo.GetReturnType());
                callMethodStatic.Result = vreg;
            }
            if (callMethodStatic.Result != null)
                callMethodStatic.Result.FixedType = new TypeDescription(methodInfo.GetReturnType());
            AddOperation(callMethodStatic);
        }

        private void CallMethodData(MethodBase methodInfo, CallMethodStatic callMethodStatic)
        {
            if (HandleRuntimeHelpersMethod(methodInfo))
            {
                callMethodStatic.ExtractNeededValuesFromStack(_evaluator);
                AddOperation(callMethodStatic);
                return;
            }
            if (methodInfo.IsConstructor && methodInfo.DeclaringType == typeof (object))
                return;
            callMethodStatic.ExtractNeededValuesFromStack(_evaluator);


            if (!callMethodStatic.Info.IsStatic && callMethodStatic.Parameters.Count > 0)
            {
                //GetBestVirtualMatch not required as the appropriate method is called in CppHandleCalls and VirtualMethod
                //TODO: GetBestVirtualMatch does not work  with base class constructors
                // if(!methodInfo.IsConstructor && CallMethodStatic.Parameters[0].ComputedType().ClrType.DeclaringType!=methodInfo.DeclaringType)
                //  CallMethodStatic.Info = ClassHierarchyAnalysis.GetBestVirtualMatch(CallMethodStatic.Info,
                //      CallMethodStatic.Parameters[0].ComputedType().ClrType);
            }
            var declaringType = callMethodStatic.Info.DeclaringType;
            if (declaringType.IsSubclassOf(typeof (Delegate)))
            {
                var signature = declaringType.GetMethod("Invoke");
                DelegateManager.RegisterType(declaringType, signature);
            }

            if (!callMethodStatic.IsVoid)
            {
                var vreg = SetNewVReg();
                vreg.FixedType = new TypeDescription(methodInfo.GetReturnType());
                callMethodStatic.Result = vreg;
            }
            if (callMethodStatic.Result != null)
                callMethodStatic.Result.FixedType = new TypeDescription(methodInfo.GetReturnType());
            AddOperation(callMethodStatic);
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
            AddOperation(assignment);
        }


        public void StoresValueFromAddress()
        {
            var varValue = _evaluator.Pop();
            var varAddress = _evaluator.Pop();

            //Deal with constants
            if (varValue is ConstValue)
            {
                var assignment = new Assignment
                {
                    AssignedTo = (LocalVariable) varAddress,
                    Right = varValue
                };
                AddOperation(assignment);
            }
            else
            {
//                var assignment = new DerefAssignment()
//                {
//                    Left = (LocalVariable) varAddress,
//                    Right = (LocalVariable) varValue
//                };
                var assignment = new Assignment
                {
                    AssignedTo = (LocalVariable) varAddress,
                    Right = varValue
                };
                AddOperation(assignment);
            }
        }

        #region Branching operators

        public void BranchIfTrue(int pushedIntValue)
        {
            var firstVar = _evaluator.Pop();
            AddOperation(new BranchOperator(OpcodeBranchNames.BrTrue)
            {
                JumpTo = pushedIntValue,
                CompareValue = firstVar
            });
        }

        public void BranchIfFalse(int pushedIntValue)
        {
            var firstVar = _evaluator.Pop();
            AddOperation(new BranchOperator(OpcodeBranchNames.BrFalse)
            {
                JumpTo = pushedIntValue,
                CompareValue = firstVar
            });
        }


        public void AlwaysBranch(int offset)
        {
            AddOperation(new AlwaysBranch {JumpTo = offset});
        }

        public void BranchIfEqual(int jumpTo)
        {
            BranchTwoOperators(jumpTo, OpcodeBranchNames.Beq);
        }

        private void BranchTwoOperators(int jumpTo, string opcode)
        {
            var secondVar = _evaluator.Pop(); // Seems the order here was in reverse
            var firstVar = _evaluator.Pop();

            AddOperation(new BranchOperator(opcode)
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
            AddOperation(assignment);
        }

        public void LoadFieldAddressIntoEvaluationStack(FieldInfo fieldInfo)
        {
            var firstVar = (LocalVariable) _evaluator.Pop();
            var vreg = SetNewVReg();
            var clrType = fieldInfo.FieldType;
            if (clrType == null)
                clrType = _evaluator.GenericArguments.First();
            else
            {
                clrType = clrType.MakeByRefType();
            }
            vreg.FixedType = new TypeDescription(clrType);

            var assignment = new FieldRefAssignment
            {
                Left = vreg,
                Right = firstVar,
                Field = fieldInfo
            };
            AddOperation(assignment);
        }

        public void LoadField(string fieldName)
        {
            var firstVar = _evaluator.Pop();

            var vreg = SetNewVReg();
            var computedType = firstVar.ComputedType();
            if (computedType.GetClrType().IsByRef)
            {
                computedType = new TypeDescription(computedType.GetClrType().GetElementType());
            }
            vreg.FixedType =
                new TypeDescription(
                    computedType.GetClrType().LocateField(fieldName).FieldType);
            var assignment = new GetField
            {
                AssignedTo = vreg,
                FieldName = fieldName,
                Instance = (LocalVariable) firstVar
            };
            AddOperation(assignment);
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
                AddOperation(nullPtrAssign);
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
            AddOperation(assignment);
        }

        public void NewObject(MethodBase constructorInfo)
        {
            if (constructorInfo.DeclaringType == typeof (object))
                return;
            
            constructorInfo.Register();
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(constructorInfo.DeclaringType);
            var constructedObject = new NewConstructedObject
            {
                AssignedTo = result,
                Info = constructorInfo
            };
            AddOperation(constructedObject);

            var interpreter = new CilMethodInterpreter(constructedObject.Info);
            var methodData = new CallMethodStatic(interpreter);
            CallMethodData(constructedObject.Info, methodData);
            var vreg = SetNewVReg();
            vreg.FixedType = new TypeDescription(methodData.Info.DeclaringType);
            var assign = new Assignment
            {
                AssignedTo = vreg,
                Right = methodData.Parameters.First()
            };

            AddOperation(assign);
        }

        public void NewArray(Type typeArray)
        {
            if (typeArray == null)
            {
                //hacky way to get first parameter
                typeArray = _evaluator.GenericArguments.First();
            }
            var arrayLength = _evaluator.Pop();
            var result = SetNewVReg();
            var assignment = new NewArrayObject
            {
                AssignedTo = result,
                TypeArray = typeArray,
                ArrayLength = arrayLength
            };
            result.FixedType = new TypeDescription(typeArray.MakeArrayType());
            AddOperation(assignment);
        }

        public void SetArrayElementValue()
        {
            var right = _evaluator.Pop();
            var index = _evaluator.Pop();
            var instance = (LocalVariable) _evaluator.Pop();
            var setArrayElement = new SetArrayElement
            {
                Instance = instance,
                Index = index,
                Right = right
            };
            AddOperation(setArrayElement);
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
            AddOperation(assign);
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

        public void LoadFunction(MethodBase operand)
        {
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(operand.GetType());
            var ptr = operand.MethodHandle.GetFunctionPointer();
            var store = new FunctionPointerStore
            {
                AssignedTo = result,
                FunctionPointer = operand,
                CustomData = (MethodInfo) operand
            };

            AddOperation(store);
        }

        public void Switch(Instruction[] instructions)
        {
            var firstVar = (LocalVariable) _evaluator.Pop();
            var offsets = instructions.Select(inst => inst.Offset).ToArray();

            var pos = 0;
            foreach (var offset in offsets)
            {
                AddOperation(new BranchOperator(OpcodeBranchNames.Beq)
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
            AddOperation(assign);
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

            if (ptrType.GetClrType().GetElementType() != null)
                result.FixedType = new TypeDescription(ptrType.GetClrType().GetElementType());

            else
            {
                result.FixedType = new TypeDescription(ptrType.GetClrType());
            }
            AddOperation(assignment);
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
            AddOperation(assignment);
        }

        public void StoreElement()
        {
            var value = _evaluator.Pop();
            var index = _evaluator.Pop();
            var array = (LocalVariable) _evaluator.Pop();
            var setArrayElement = new SetArrayElement
            {
                Instance = array,
                Index = index,
                Right = value
            };
            AddOperation(setArrayElement);
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
            var casting = new ClassCasting
            {
                AssignedTo = result,
                Value = valueToCast
            };
            AddOperation(casting);
        }

        public void IsInst(Type operand)
        {
            //TODO: Fix this, we need to actually compare type ids here ... at runtime we cannot tell what has been passed

            var valueToCast = _evaluator.Pop();
            var assigned = SetNewVReg();
            assigned.FixedType = new TypeDescription(typeof (bool));
            var casting = new IsInstance
            {
                AssignedTo = assigned,
                Right = valueToCast,
                CastTo = operand
            };
            AddOperation(casting);
        }


        public void Box(Type operand)
        {
            var valueToBox = _evaluator.Pop();
            var result = SetNewVReg();
            //The type of the value on the stack is not necessarily the boxing type
            valueToBox.FixedType = new TypeDescription(operand);
            result.FixedType = new TypeDescription(typeof (object));
            var boxing = new Boxing
            {
                AssignedTo = result,
                Right = valueToBox
            };
            AddOperation(boxing);
        }

        public void Unbox(Type operand)
        {
            var valueToBox = _evaluator.Pop();
            var result = SetNewVReg();
            result.FixedType = new TypeDescription(operand);
            var boxing = new Unboxing
            {
                AssignedTo = result,
                Right = valueToBox
            };
            AddOperation(boxing);
        }

        public void LoadObject(Type operand) // TODO: Fix this
        {
            var valueAddress = (LocalVariable) _evaluator.Pop();

            AssignValueToStack(valueAddress);
        }

        public void StoreObject(Type operand) // TODO: Fix this
        {
            var value = _evaluator.Pop();
            var valueAddress = (LocalVariable) _evaluator.Pop();

            //Look up instance
            var assign = new Assignment
            {
                AssignedTo = valueAddress,
                Right = value
            };

            AddOperation(assign);
        }
    }
}