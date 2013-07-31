#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;
using Mono.Reflection;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MetaMidRepresentation
    {
        public readonly MidRepresentationVariables Vars = new MidRepresentationVariables();

        public List<LocalOperation> LocalOperations = new List<LocalOperation>();
        private MethodBase _method;

        public MethodBase Method
        {
            get { return _method; }
            set
            {
                _method = value;
                Vars.Variables.Clear();
                Vars.Variables.AddRange(GetMethodBody.LocalVariables);
                var pos = 0;
                var isConstructor = _method is ConstructorInfo;
                if (isConstructor || !Method.IsStatic)
                {
                    Vars.Arguments.Add(new ArgumentVariable("_this")
                    {
                        FixedType = Method.DeclaringType
                    });
                }
                Vars.Arguments.AddRange(_method.GetParameters().Select(param => new ArgumentVariable(param.Name)
                {
                    FixedType = param.ParameterType,
                    Id = pos++
                }));

                pos = 0;
                Vars.LocalVars.AddRange(Vars.Variables.Select(v => new LocalVariable()
                {
                    FixedType = v.LocalType,
                    Id = pos++,
                    Kind = VariableKind.Argument
                }));
            }
        }

        public MethodBody GetMethodBody
        {
            get { return _method.GetMethodBody(); }
        }

        public void PushInt4(int value, EvaluatorStack evaluator)
        {
            AssignValueToStack(value, evaluator);
        }

        private void AssignValueToStack(object value, EvaluatorStack evaluator)
        {
            var result = SetNewVReg(evaluator);
            var assign = new Assignment()
            {
                Left = result,
                Right = new ConstValue(value)
            };
            result.FixedType = value.GetType();
            AddOperation(LocalOperation.Kinds.Assignment, assign);
        }

        public LocalVariable SetNewVReg(EvaluatorStack evaluator)
        {
            var newLocal = evaluator.SetNewVReg();
            Vars.VirtRegs.Add(newLocal);
            return newLocal;
        }

        public void CopyStackIntoLocalVariable(int value, EvaluatorStack evaluator)
        {
            var topVariable = evaluator.Stack.Peek();
            var newLocal = new LocalVariable
            {
                Kind = VariableKind.Local,
                Id = value
            };

            evaluator.Stack.Pop();
            Vars.LocalVariables[value] = newLocal;
            var assingment = new Assignment
            {
                Left = newLocal,
                Right = topVariable
            };

            newLocal.FixedType = topVariable.ComputedType();
            AddOperation(LocalOperation.Kinds.Assignment, assingment);
        }

        public void CopyLocalVariableIntoStack(int value, EvaluatorStack evaluator)
        {
            var locVar = Vars.LocalVariables[value];

            var vreg = SetNewVReg(evaluator);

            var assingment = new Assignment
            {
                Left = vreg,
                Right = locVar
            };

            vreg.FixedType = locVar.ComputedType();
            AddOperation(LocalOperation.Kinds.Assignment, assingment);
        }

        private void AddOperation(LocalOperation.Kinds kind, object value = null)
        {
            var result = new LocalOperation
            {
                Kind = kind,
                Value = value
            };

            LocalOperations.Add(result);
            var assignment = result.Value as Assignment;
            if (assignment != null)
            {
                if (assignment.Left.FixedType == null)
                    throw new InvalidOperationException(
                        String.Format("The data introduced in the IR should be well typed. " +
                                      Environment.NewLine + "Operation: {0}", result));
            }
        }


        public static bool HandleRuntimeHelpersMethod(MethodBase method)
        {
            var declType = method.DeclaringType;
            return declType == typeof (RuntimeHelpers);
        }

        public void Call(object operand, EvaluatorStack evaluator)
        {
            var methodInfo = (MethodBase) operand;
            var methodData = new MethodData(methodInfo);


            if (HandleRuntimeHelpersMethod(Method))
            {
                methodData.ExtractNeededValuesFromStack(evaluator);
                AddOperation(LocalOperation.Kinds.CallRuntime, methodData);
                return;
            }
            methodData.ExtractNeededValuesFromStack(evaluator);
            if (!methodData.IsVoid)
            {
                var vreg = SetNewVReg(evaluator);
                vreg.FixedType = methodInfo.GetReturnType();
                methodData.Result = vreg;
            }
            methodData.FixedType = methodInfo.GetReturnType();
            AddOperation(LocalOperation.Kinds.Call, methodData);
        }

        public void Return(bool isVoid, EvaluatorStack evaluator)
        {
            var returnValue = isVoid ? null : evaluator.Stack.Pop();
            AddOperation(LocalOperation.Kinds.Return, returnValue);
        }

        #region Operators

        public void Add(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Add, evaluator);
        }

        public void Sub(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Sub, evaluator);
        }

        public void Div(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Div, evaluator);
        }

        public void Rem(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Rem, evaluator);
        }

        public void Mul(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Mul, evaluator);
        }

        public void And(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.And, evaluator);
        }

        public void Or(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Or, evaluator);
        }

        public void Xor(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Xor, evaluator);
        }

        private void SetBinaryOperator(string operatorName, EvaluatorStack evaluator)
        {
            var secondVar = evaluator.Stack.Pop();
            var firstVar = evaluator.Stack.Pop();
            var addValue = new BinaryOperator(operatorName)
            {
                Left = firstVar,
                Right = secondVar
            };
            var result = SetNewVReg(evaluator);
            result.FixedType = addValue.ComputedType();
            var assign = new Assignment()
            {
                Left = result,
                Right = addValue
            };
            AddOperation(LocalOperation.Kinds.Operator, assign);
        }

        private void SetUnaryOperator(string operatorName, EvaluatorStack evaluator)
        {
            var firstVar = evaluator.Stack.Pop();
            var addValue = new UnaryOperator(operatorName)
            {
                Left = firstVar,
                FixedType = firstVar.ComputedType()
            };
            var result = SetNewVReg(evaluator);
            var assign = new Assignment()
            {
                Left = result,
                Right = addValue
            };
            assign.Left.FixedType = addValue.ComputedType();
            AddOperation(LocalOperation.Kinds.Operator, assign);
        }

        public void Not(EvaluatorStack evaluator)
        {
            SetUnaryOperator(OpcodeOperatorNames.Not, evaluator);
        }


        public void Neg(EvaluatorStack evaluator)
        {
            SetUnaryOperator(OpcodeOperatorNames.Neg, evaluator);
        }

        #endregion

        #region Compare Operators

        public void Cgt(EvaluatorStack evaluator)
        {
            SetBinaryOperator("cgt", evaluator);
        }

        public void Ceq(EvaluatorStack evaluator)
        {
            SetBinaryOperator("ceq", evaluator);
        }

        public void Clt(EvaluatorStack evaluator)
        {
            SetBinaryOperator("clt", evaluator);
        }

        #endregion

        public void SetLabel(int offset)
        {
            AddOperation(LocalOperation.Kinds.Label, offset);
        }

        public void PushString(string value, EvaluatorStack evaluator)
        {
            AssignValueToStack(value, evaluator);
        }

        public void PushDouble(double value, EvaluatorStack evaluator)
        {
            AssignValueToStack(value, evaluator);
        }

        public void StoreField(FieldInfo fieldInfo, EvaluatorStack evaluator)
        {
            var secondVar = evaluator.Stack.Pop();
            var firstVar = evaluator.Stack.Pop();
            var fieldName = fieldInfo.Name;
            var assignment = new Assignment()
            {
                Left = new FieldSetter()
                {
                    Instance = firstVar,
                    FieldName = fieldName
                },
                Right = secondVar
            };
            assignment.Left.FixedType = secondVar.ComputedType();
            AddOperation(LocalOperation.Kinds.SetField, assignment);
        }


        public void StoreStaticField(EvaluatorStack evaluator, FieldInfo fieldInfo)
        {
            var firstVar = evaluator.Stack.Pop();
            var fieldName = fieldInfo.Name;
            var assignment = new Assignment()
            {
                Left = new StaticFieldSetter()
                {
                    DeclaringType = fieldInfo.DeclaringType,
                    FieldName = fieldName
                },
                Right = firstVar
            };
            assignment.Left.FixedType = firstVar.ComputedType();
            AddOperation(LocalOperation.Kinds.SetStaticField, assignment);
        }


        public void StoresValueFromAddress(EvaluatorStack evaluator, LocalVariableInfo index)
        {
            var varAddress = evaluator.Stack.Pop();
            var varValue = evaluator.Stack.Pop();
            
            var assignment = new DerefAssignment
            {
                Left = (LocalVariable) varAddress,
                Right = (LocalVariable)varValue
            };
            
            AddOperation(LocalOperation.Kinds.DerefAssignment, assignment);
        }
        #region Branching operators

        public void BranchIfTrue(int pushedIntValue, EvaluatorStack evaluator)
        {
            var firstVar = evaluator.Stack.Pop();
            AddOperation(LocalOperation.Kinds.BranchOperator,
                new BranchOperator(OpcodeBranchNames.BrTrue)
                {
                    JumpTo = pushedIntValue,
                    CompareValue = firstVar
                });
        }

        public void BranchIfFalse(int pushedIntValue, EvaluatorStack evaluator)
        {
            var firstVar = evaluator.Stack.Pop();
            AddOperation(LocalOperation.Kinds.BranchOperator,
                new BranchOperator(OpcodeBranchNames.BrFalse)
                {
                    JumpTo = pushedIntValue,
                    CompareValue = firstVar
                });
        }


        public void AlwaysBranch(int offset)
        {
            AddOperation(LocalOperation.Kinds.AlwaysBranch, offset);
        }

        public void BranchIfEqual(int jumpTo, EvaluatorStack evaluator)
        {
            BranchTwoOperators(jumpTo, evaluator, OpcodeBranchNames.Beq);
        }

        private void BranchTwoOperators(int jumpTo, EvaluatorStack evaluator, string opcode)
        {
            var firstVar = evaluator.Stack.Pop();
            var secondVar = evaluator.Stack.Pop();

            AddOperation(LocalOperation.Kinds.BranchOperator,
                new BranchOperator(opcode)
                {
                    JumpTo = jumpTo,
                    CompareValue = firstVar,
                    SecondValue = secondVar
                });
        }

        public void BranchIfGreaterOrEqual(int jumpTo, EvaluatorStack evaluator)
        {
            BranchTwoOperators(jumpTo, evaluator, OpcodeBranchNames.Bge);
        }

        public void BranchIfGreater(int jumpTo, EvaluatorStack evaluator)
        {
            BranchTwoOperators(jumpTo, evaluator, OpcodeBranchNames.Bgt);
        }

        public void BranchIfLessOrEqual(int jumpTo, EvaluatorStack evaluator)
        {
            BranchTwoOperators(jumpTo, evaluator, OpcodeBranchNames.Ble);
        }

        public void BranchIfLess(int jumpTo, EvaluatorStack evaluator)
        {
            BranchTwoOperators(jumpTo, evaluator, OpcodeBranchNames.Blt);
        }

        public void BranchIfNotEqual(int jumpTo, EvaluatorStack evaluator)
        {
            BranchTwoOperators(jumpTo, evaluator, OpcodeBranchNames.Bne);
        }

        #endregion

        public void LoadArgument(int pushedIntValue, EvaluatorStack evaluator)
        {
            var argument = Vars.Arguments[pushedIntValue];
            var vreg = SetNewVReg(evaluator);
            vreg.FixedType = argument.ComputedType();
            var assignment = new Assignment()
            {
                Left = vreg,
                Right = argument
            };
            AddOperation(LocalOperation.Kinds.Assignment, assignment);
        }



        public void LoadAddressIntoEvaluationStack(EvaluatorStack evaluator, LocalVariableInfo index)
        {
            var vreg = SetNewVReg(evaluator);
            vreg.FixedType = index.LocalType.MakeByRefType();

            var argument = Vars.LocalVariables[index.LocalIndex];
            var assignment = new RefAssignment()
            {
                Left = vreg,
                Right = argument
            };
            AddOperation(LocalOperation.Kinds.RefAssignment, assignment);
        }

        public void LoadField(string fieldName, EvaluatorStack evaluator)
        {
            var firstVar = evaluator.Stack.Pop();

            var vreg = SetNewVReg(evaluator);
            vreg.FixedType = firstVar.ComputedType().LocateField(fieldName).FieldType;
            ProgramData.UpdateType(vreg.FixedType);
            var assignment = new Assignment
            {
                Left = vreg,
                Right = new FieldGetter()
                {
                    FieldName = fieldName,
                    Instance = firstVar
                }
            };
            AddOperation(LocalOperation.Kinds.GetField, assignment);
        }

        public void LoadStaticField(EvaluatorStack evaluator, FieldInfo operand)
        {
            var vreg = SetNewVReg(evaluator);
            var fieldName = operand.Name;
            var declaringType = operand.DeclaringType;
            vreg.FixedType = declaringType.LocateField(fieldName).FieldType;
            var typeData = ProgramData.UpdateType(declaringType);
            var assignment = new Assignment
            {
                Left = vreg,
                Right = new StaticFieldGetter()
                {
                    FieldName = fieldName,
                    DeclaringType = typeData
                }
            };
            AddOperation(LocalOperation.Kinds.GetStaticField, assignment);
        }

        public void LoadLength(EvaluatorStack evaluator)
        {
            SetUnaryOperator(OpcodeOperatorNames.LoadLen, evaluator);

            evaluator.Top.FixedType = typeof (int);
        }

        public void ConvI4(EvaluatorStack evaluator)
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvI4, evaluator);
            evaluator.Top.FixedType = typeof (int);
        }

        public void ConvR8(EvaluatorStack evaluator)
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvR8, evaluator);
            evaluator.Top.FixedType = typeof (double);
        }

        public void LoadReferenceInArray(EvaluatorStack evaluator)
        {
            var secondVar = evaluator.Stack.Pop();
            var firstVar = evaluator.Stack.Pop();

            var result = SetNewVReg(evaluator);
            result.FixedType = firstVar.FixedType.GetElementType();
            var arrayVariable = new ArrayVariable(firstVar, secondVar);
            var assignment = new Assignment()
            {
                Left = result,
                Right = arrayVariable
            };
            result.FixedType = arrayVariable.GetElementType();
            AddOperation(LocalOperation.Kinds.GetArrayItem, assignment);
        }

        public void Dup(EvaluatorStack evaluator)
        {
            var variable = evaluator.Stack.Peek();
            var vreg = SetNewVReg(evaluator);
            var assign = new Assignment()
            {
                Left = vreg,
                Right = variable
            };
            vreg.FixedType = variable.ComputedType();
            AddOperation(LocalOperation.Kinds.Assignment, assign);
        }


        public void NewObject(ConstructorInfo constructorInfo, EvaluatorStack evaluator)
        {
            var result = SetNewVReg(evaluator);

            result.FixedType = constructorInfo.DeclaringType;
            var assignment = new Assignment()
            {
                Left = result,
                Right = new NewConstructedObject(constructorInfo)
            };
            ProgramData.UpdateType(constructorInfo.DeclaringType);
            AddOperation(LocalOperation.Kinds.NewObject, assignment);
        }

        public void NewArray(EvaluatorStack evaluator, Type typeArray)
        {
            var arrayLength = evaluator.Stack.Pop();
            var result = SetNewVReg(evaluator);
            var assignment = new Assignment
            {
                Left = result,
                Right = new NewArrayObject()
                {
                    TypeArray = typeArray,
                    ArrayLength = arrayLength
                }
            };
            result.FixedType = typeArray.MakeArrayType();
            AddOperation(LocalOperation.Kinds.NewArray, assignment);
        }

        public LocalOperation LastOperation
        {
            get
            {
                var count = LocalOperations.Count;
                if (count == 0)
                    return null;
                return LocalOperations[count - 1];
            }
        }

        public void SetArrayElementValue(EvaluatorStack evaluator)
        {
            var value = evaluator.Pop();
            var index = evaluator.Pop();
            var array = evaluator.Pop();
            var arrayVariable = new ArrayVariable(array, index);
            var assignment = new Assignment
            {
                Left = arrayVariable,
                Right = value
            };
            arrayVariable.FixedType = value.ComputedType();
            AddOperation(LocalOperation.Kinds.SetArrayItem, assignment);
        }

        public void SetToken(EvaluatorStack evaluator, FieldInfo operand)
        {
            var fieldOperand = operand;
            var fieldType = fieldOperand.FieldType;
            var nesterType = fieldType.DeclaringType;
            var fields = nesterType.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            var value = fields[0].GetValue(null);
            var srcBytes = value.ToByteArray();
            var vreg = SetNewVReg(evaluator);
            var rightConstant = ConstByteArrayList.RegisterConstant(srcBytes);
            var assign = new Assignment()
            {
                Left = vreg,
                Right = new ConstByteArrayValue(rightConstant)
            };
            vreg.FixedType = typeof (byte[]);
            AddOperation(LocalOperation.Kinds.CopyArrayInitializer, assign);
        }

        public void LoadNull(EvaluatorStack evaluator)
        {
            AssignNullToStack(evaluator);
        }

        private void AssignNullToStack(EvaluatorStack evaluator)
        {
            var result = SetNewVReg(evaluator);
            var assign = new Assignment()
            {
                Left = result,
                Right = new ConstValue(null)
            };
            result.FixedType = typeof (object);
            AddOperation(LocalOperation.Kinds.Assignment, assign);
        }

        public void LoadFunction(EvaluatorStack evaluator, MethodBase operand)
        {
            throw new NotImplementedException();
        }

        public void Switch(EvaluatorStack evaluator, Instruction[] instructions)
        {
            var firstVar = (LocalVariable) evaluator.Stack.Pop();
            var assign = new Assignment()
            {
                Left = firstVar,
                Right = new ConstValue(instructions)
            };
            AddOperation(LocalOperation.Kinds.Switch, assign);
        }
    }
}