using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefactor.Analyze;
using CodeRefractor.Compiler.FrontEnd;
using CodeRefractor.Compiler.MiddleEnd.SimpleOperations;
using CodeRefractor.Compiler.Shared;
using CodeRefractor.Compiler.Util;

namespace CodeRefractor.Compiler.MiddleEnd
{
    public class MetaMidRepresentation
    {
        readonly SortedDictionary<int, LocalVariable> _localVariables = new SortedDictionary<int, LocalVariable>();

        public readonly List<LocalOperation> LocalOperations = new List<LocalOperation>();
        public readonly List<LocalVariableInfo> Variables = new List<LocalVariableInfo>();
        public readonly List<ArgumentVariable> Arguments = new List<ArgumentVariable>();
        public readonly List<LocalVariable> VirtRegs = new List<LocalVariable>();
        public readonly List<LocalVariable> LocalVars = new List<LocalVariable>();

        public MethodData MethodData;
        private MethodBase _method;

        public MethodBase Method
        {
            get { return _method; }
            set
            {
                _method = value;
                Variables.Clear();
                Variables.AddRange(_method.GetMethodBody().LocalVariables);
                int pos=0;
                var isConstructor = _method is ConstructorInfo;
                if(isConstructor || !Method.IsStatic)
                {
                    Arguments.Add(new ArgumentVariable("_this")
                                      {
                                          FixedType = Method.DeclaringType
                                      });
                }
                Arguments.AddRange(_method.GetParameters().Select(param => new ArgumentVariable(param.Name)
                                                                             {
                                                                                 FixedType = param.ParameterType,
                                                                                 Id=pos++
                                                                             }));

                pos = 0;
                LocalVars.AddRange(Variables.Select(v=>new LocalVariable()
                {
                    FixedType = v.LocalType,
                    Id = pos++,
                    Kind = VariableKind.Argument
                }));
                MethodData = new MethodData(_method);
            }
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
            AddOperation(SimpleOperations.LocalOperation.Kinds.Assignment, assign);
        }

        public LocalVariable SetNewVReg(EvaluatorStack evaluator)
        {
            var newLocal = evaluator.SetNewVReg();
            VirtRegs.Add(newLocal);
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
            _localVariables[value] = newLocal;
            var assingment = new Assignment
                                 {
                                     Left = newLocal,
                                     Right = topVariable
                                 };

            newLocal.FixedType = topVariable.ComputedType();
            AddOperation(SimpleOperations.LocalOperation.Kinds.Assignment, assingment);
        }

        public void CopyLocalVariableIntoStack(int value, EvaluatorStack evaluator)
        {
            var locVar = _localVariables[value];
            
            var vreg = SetNewVReg(evaluator);

            var assingment = new Assignment
            {
                Left = vreg,
                Right = locVar
            };

            vreg.FixedType = locVar.ComputedType();
            AddOperation(SimpleOperations.LocalOperation.Kinds.Assignment, assingment);
        }

        void AddOperation(LocalOperation.Kinds kind, object value = null)
        {
            var result = new LocalOperation
                             {
                Kind = kind,
                Value = value
            };
            LocalOperations.Add(result);
            var assignment = result.Value as Assignment;
            if(assignment!=null)
            {
                if(assignment.Left.FixedType==null)
                    throw new InvalidOperationException(string.Format("The data introduced in the IR should be well typed. "+
                        Environment.NewLine+"Operation: {0}",result));
            }
        }



        public void Call(object operand, EvaluatorStack evaluator)
        {
            var methodInfo = (MethodBase) operand;
            var methodData = new MethodData(methodInfo);


            methodData.ExtractNeededValuesFromStack(evaluator);
            if (!methodData.IsVoid)
            {
                var vreg = SetNewVReg(evaluator);
                vreg.FixedType = methodInfo.GetReturnType();
                methodData.Result = vreg;
            }
            methodData.FixedType = methodInfo.GetReturnType();
            AddOperation(SimpleOperations.LocalOperation.Kinds.Call, methodData);
        }

        public void Return(bool isVoid, EvaluatorStack evaluator)
        {
            var returnValue = isVoid?null:evaluator.Stack.Pop();
            AddOperation(LocalOperation.Kinds.Return,returnValue);
        }

        #region Operators
        public void Add(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Add,evaluator);
        }

        public void Sub(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Sub,evaluator);
        }

        public void Div(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Div,evaluator);
        }

        public void Rem(EvaluatorStack evaluator)
        {
            SetBinaryOperator(OpcodeOperatorNames.Rem,evaluator);
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
            AddOperation(SimpleOperations.LocalOperation.Kinds.Operator, assign);
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
            AddOperation(SimpleOperations.LocalOperation.Kinds.Operator, assign);
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
            AddOperation(SimpleOperations.LocalOperation.Kinds.Label, offset);
        }

        public void PushString(string value, EvaluatorStack evaluator)
        {
            AssignValueToStack(value,evaluator);
        }

        public void PushDouble(double value, EvaluatorStack evaluator)
        {
            AssignValueToStack(value,evaluator);
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
            AddOperation(SimpleOperations.LocalOperation.Kinds.SetField, assignment);
        }

        #region Branching operators

        public void BranchIfTrue(int pushedIntValue, EvaluatorStack evaluator)
        {
            var firstVar = evaluator.Stack.Pop();
            AddOperation(SimpleOperations.LocalOperation.Kinds.BranchOperator,
                new BranchOperator(OpcodeBranchNames.BrTrue)
                {
                    JumpTo = pushedIntValue,
                    CompareValue = firstVar
                });
        }

        public void BranchIfFalse(int pushedIntValue, EvaluatorStack evaluator)
        {
            var firstVar = evaluator.Stack.Pop();
            AddOperation(SimpleOperations.LocalOperation.Kinds.BranchOperator,
                new BranchOperator(OpcodeBranchNames.BrFalse)
                {
                    JumpTo = pushedIntValue,
                    CompareValue = firstVar
                });
        }



        public void AlwaysBranch(int offset)
        {
            AddOperation(SimpleOperations.LocalOperation.Kinds.AlwaysBranch, offset);
        }

        public void BranchIfEqual(int jumpTo, EvaluatorStack evaluator)
        {
            BranchTwoOperators(jumpTo, evaluator, OpcodeBranchNames.Beq);
        }

        private void BranchTwoOperators(int jumpTo, EvaluatorStack evaluator, string opcode)
        {
            var firstVar = evaluator.Stack.Pop();
            var secondVar = evaluator.Stack.Pop();

            AddOperation(SimpleOperations.LocalOperation.Kinds.BranchOperator,
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
            
            var argument = Arguments[pushedIntValue];
            var vreg = SetNewVReg(evaluator);
            vreg.FixedType = argument.ComputedType();
            var assignment = new Assignment()
                                 {
                                     Left = vreg,
                                     Right = argument
                                 };
            AddOperation(LocalOperation.Kinds.LoadArgument, assignment);
        }

        public void LoadField(string fieldName, EvaluatorStack evaluator)
        {
            var firstVar = evaluator.Stack.Pop();
            
            var vreg = SetNewVReg(evaluator);
            vreg.FixedType = firstVar.ComputedType().LocateField(fieldName).FieldType;
            ProgramData.Instance.UpdateType(vreg.FixedType);
            var assignment = new Assignment
                                 {
                                     Left = vreg,
                                     Right = new FieldGetter()
                                                 {
                                                     FieldName = fieldName,
                                                     Instance = firstVar
                                                 }
                                 };
            AddOperation(SimpleOperations.LocalOperation.Kinds.GetField, assignment);
        }

        public void LoadLength(EvaluatorStack evaluator)
        {
            SetUnaryOperator(OpcodeOperatorNames.LoadLen, evaluator);

            evaluator.Top.FixedType = typeof(int);
        }

        public void ConvI4(EvaluatorStack evaluator)
        {
            SetUnaryOperator(OpcodeOperatorNames.ConvI4, evaluator);
            evaluator.Top.FixedType = typeof(int);
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
            AddOperation(SimpleOperations.LocalOperation.Kinds.GetArrayItem, assignment);
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
            ProgramData.Instance.UpdateType(constructorInfo.DeclaringType);
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

        public void SetElement(EvaluatorStack evaluator, Type operand)
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
    }
}