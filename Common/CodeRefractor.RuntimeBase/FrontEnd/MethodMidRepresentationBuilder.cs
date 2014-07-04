using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.Util;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeRefractor.FrontEnd
{
    class MethodMidRepresentationBuilder
    {
        private readonly MethodInterpreter _methodInterpreter;
        private readonly MethodBase _method;

        public MethodMidRepresentationBuilder(MethodInterpreter methodInterpreter, MethodBase method)
        {
            _methodInterpreter = methodInterpreter;
            _method = method;
        }

        public void ProcessInstructions()
        {
            var instructions = _method.GetInstructions().ToArray();
            var labelList = ComputeLabels(_method);
            var evaluator = new EvaluatorStack();
            var operationFactory = new MetaMidRepresentationOperationFactory(_methodInterpreter.MidRepresentation, evaluator);

            for (var index = 0; index < instructions.Length; index++)
            {
                var instruction = instructions[index];
                EvaluateInstruction(instruction, operationFactory, labelList);
            }
            //   Ensure.IsTrue(evaluator.Count == 0, "Stack not empty!");
            var analyzeProperties = _methodInterpreter.AnalyzeProperties;
            analyzeProperties.Setup(_methodInterpreter.MidRepresentation.Vars.Arguments, _methodInterpreter.MidRepresentation.Vars.VirtRegs,
                _methodInterpreter.MidRepresentation.Vars.LocalVars);
        }


        public static HashSet<int> ComputeLabels(MethodBase definition)
        {
            var labels = new HashSet<int>();
            var body = definition.GetMethodBody();
            if (body == null)
                return labels;
            var instructions = definition.GetInstructions();

            foreach (var instruction in instructions)
            {
                var opcodeStr = instruction.OpCode.Value;
                switch (opcodeStr)
                {
                    case OpcodeIntValues.Beq:
                    case OpcodeIntValues.BeqS:
                    case OpcodeIntValues.BneUn:
                    case OpcodeIntValues.BneUnS:
                    case OpcodeIntValues.Bge:
                    case OpcodeIntValues.BgeS:
                    case OpcodeIntValues.Bgt:
                    case OpcodeIntValues.BgtS:
                    case OpcodeIntValues.BrTrueS:
                    case OpcodeIntValues.BrTrue:
                    case OpcodeIntValues.BrZero:
                    case OpcodeIntValues.BrZeroS:
                    case OpcodeIntValues.Ble: // Were missing leading to no labels being generated
                    case OpcodeIntValues.BleS:
                    case OpcodeIntValues.Blt:
                    case OpcodeIntValues.BltS:
                    case OpcodeIntValues.BrS:
                    case OpcodeIntValues.Br:
                    case OpcodeIntValues.Leave:
                    case OpcodeIntValues.LeaveS:
                    {
                        var offset = ((Instruction)instruction.Operand).Offset;
                        AddLabelIfDoesntExist(offset, labels);
                    }
                        break;
                    case OpcodeIntValues.Switch:
                    {
                        var offsets = (Instruction[])instruction.Operand;
                        foreach (var offset in offsets)
                        {
                            AddLabelIfDoesntExist(offset.Offset, labels);
                        }
                    }
                        break;
                }
            }
            return labels;
        }

        public static void AddLabelIfDoesntExist(int offset, HashSet<int> labels)
        {
            labels.Add(offset);
        }

        private void EvaluateInstruction(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory,
            HashSet<int> labelList)
        {
            var opcodeStr = instruction.OpCode.ToString();
            var offset = 0;
            if (instruction.Operand is Instruction)
            {
                offset = ((Instruction)(instruction.Operand)).Offset;
            }
            if (labelList.Contains(instruction.Offset))
            {
                operationFactory.SetLabel(instruction.Offset);
            }
            if (operationFactory.SkipInstruction(instruction.Offset))
                return;
            var opcodeValue = instruction.OpCode.Value;
            operationFactory.AddCommentInstruction(instruction.ToString());
            if (HandleCalls(instruction, operationFactory, opcodeValue))
                return;

            if (HandleStores(opcodeStr, instruction, operationFactory))
                return;
            if (HandleLoads(opcodeStr, instruction, operationFactory))
                return;
            if (HandleOperators(opcodeStr, operationFactory))
                return;

            if (HandleBranching(opcodeStr, offset, operationFactory))
                return;
            if (HandleBoxing(opcodeStr, offset, operationFactory, instruction))
                return;

            if (HandleClassCast(opcodeStr, offset, operationFactory, instruction))
                return;
            if (HandleIsInst(opcodeStr, offset, operationFactory, instruction))
                return;


            if (HandleException(opcodeStr, offset, operationFactory, instruction))
                return;


            if (HandleExtraInstructions(instruction, operationFactory, opcodeStr)) return;
            throw new InvalidOperationException(String.Format("Unknown instruction: {0}", instruction));
        }
        private bool HandleExtraInstructions(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, string opcodeStr)
        {
            if (opcodeStr == "ret")
            {
                var isVoid = _method.GetReturnType().IsVoid();

                operationFactory.Return(isVoid);
                return true;
            }
            if (opcodeStr.StartsWith("conv."))
            {
                if (ConversionOperations(opcodeStr, operationFactory)) return true;
                return true;
            }
            if (opcodeStr == "dup")
            {
                operationFactory.Dup();
                return true;
            }
            if (opcodeStr == "pop")
            {
                operationFactory.Pop();
                return true;
            }
            if (HandleArrayOperations(instruction, operationFactory, opcodeStr)) return true;
            if (opcodeStr == "ldtoken")
            {
                operationFactory.SetToken((FieldReference)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldftn")
            {
                operationFactory.LoadFunction((MethodDefinition)instruction.Operand);
                return true;
            }
            if (opcodeStr == "switch")
            {
                operationFactory.Switch((Instruction[])instruction.Operand);
                return true;
            }
            if (opcodeStr == "sizeof")
            {
                operationFactory.SizeOf((Type)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldsfld")
            {
                operationFactory.LoadStaticField((FieldReference)instruction.Operand);
                return true;
            }

            if (opcodeStr == "stsfld")
            {
                operationFactory.StoreStaticField((FieldReference)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldloca.s" || opcodeStr == "ldloca")
            {
                //TODO: load the address into evaluation stack
                var index = (VariableDefinition)instruction.Operand;

                operationFactory.LoadAddressIntoEvaluationStack(index);
                return true;
            }
            if (opcodeStr == "ldflda.s" || opcodeStr == "ldflda")
            {
                var fieldInfo = (FieldReference)instruction.Operand;

                operationFactory.LoadFieldAddressIntoEvaluationStack(fieldInfo);
                return true;
            }

            if (opcodeStr == "ldelema")
            {
                operationFactory.LoadAddressOfArrayItemIntoStack((Type)instruction.Operand);
                return true;
            }

            if (opcodeStr.StartsWith("stind."))
            {
                //TODO: load the address into evaluation stack
                operationFactory.StoresValueFromAddress();
                return true;
            }

            if (opcodeStr.StartsWith("ldind."))
            {
                //TODO: load the address into evaluation stack
                operationFactory.LoadValueFromAddress();
                return true;
            }

            if (opcodeStr.StartsWith("initobj"))
            {
                //TODO: load the address into evaluation stack
                operationFactory.InitObject();
                return true;
            }
            if (opcodeStr == "volatile.") //TODO: handle this
            {
                operationFactory.PushDouble(0);
                return true;
            }
            return false;
        }

        private static bool HandleArrayOperations(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, string opcodeStr)
        {
            if (opcodeStr == "newarr")
            {
                operationFactory.NewArray((Type)((TypeReference)instruction.Operand).GetClrType());
                return true;
            }
            if (opcodeStr == "stelem.i1"
                || opcodeStr == "stelem.i2"
                || opcodeStr == "stelem.i4"
                || opcodeStr == "stelem.i8"
                || opcodeStr == "stelem.r4"
                || opcodeStr == "stelem.r8"
                || opcodeStr == "stelem.i2")
            {
                operationFactory.SetArrayElementValue();
                return true;
            }
            if (opcodeStr == "stelem.ref")
            {
                operationFactory.SetArrayElementValue();
                return true;
            }
            if (opcodeStr == "stelem")
            {
                var elemInfo = (Type)instruction.Operand;
                operationFactory.StoreElement();
                return true;
            }
            return false;
        }

        private static bool HandleCalls(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory,
            short opcodeValue)
        {
            switch (opcodeValue)
            {
                case OpcodeIntValues.Nop:
                    return true;
                case OpcodeIntValues.Call:
                    operationFactory.Call(instruction.Operand);
                    return true;
                case OpcodeIntValues.CallVirt:
                    operationFactory.CallVirtual(instruction.Operand);
                    return true;

                case OpcodeIntValues.NewObj:
                    {
                        var consInfo = (MethodBase)((MethodReference)instruction.Operand).GetMethod();
                        operationFactory.NewObject(consInfo);
                    }
                    return true;
            }
            return false;
        }

        private static bool ConversionOperations(string opcodeStr,
            MetaMidRepresentationOperationFactory operationFactory)
        {
            if (opcodeStr == "conv.u1")
            {
                operationFactory.ConvU1();
                return true;
            }
            if (opcodeStr == "conv.i")
            {
                operationFactory.ConvI();
                return true;
            }
            if (opcodeStr == "conv.i4")
            {
                operationFactory.ConvI4();
                return true;
            }
            if (opcodeStr == "conv.i8")
            {
                operationFactory.ConvI8();
                return true;
            }
            if (opcodeStr == "conv.r4")
            {
                operationFactory.ConvR4();
                return true;
            }
            if (opcodeStr == "conv.r8")
            {
                operationFactory.ConvR8();
                return true;
            }
            return false;
        }

        private static bool HandleStores(string opcodeStr, Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory)
        {
            if (opcodeStr == "stloc.s" || opcodeStr == "stloc")
            {
                operationFactory.CopyStackIntoLocalVariable(instruction.GetIntOperand());
                return true;
            }
            if (opcodeStr.StartsWith("stloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "stloc.".Length).ToInt();
                operationFactory.CopyStackIntoLocalVariable(pushedIntValue);
                return true;
            }

            if (opcodeStr.StartsWith("starg."))
            {
                var parameter = (ParameterReference)instruction.Operand;
                var pushedIntValue = parameter.Index;
                operationFactory.CopyStackIntoArgument(pushedIntValue);
                return true;
            }

            if (opcodeStr == "stfld")
            {
                var fieldInfo = (FieldReference)instruction.Operand;
                operationFactory.StoreField(fieldInfo);
                return true;
            }

            if (opcodeStr.StartsWith("stobj"))
            {

                operationFactory.StoreObject((Type)instruction.Operand);
                return true;
            }

            return false;
        }

        private static bool HandleLoads(string opcodeStr, Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory)
        {
            if (opcodeStr == "ldelem.ref")
            {
                operationFactory.LoadReferenceInArray();
                return true;
            }
            if (opcodeStr == "ldelem.i"
                || opcodeStr == "ldelem.i1"
                || opcodeStr == "ldelem.i2"
                || opcodeStr == "ldelem.i4"
                || opcodeStr == "ldelem.i8"
                || opcodeStr == "ldelem.i1"
                || opcodeStr == "ldelem.r4"
                || opcodeStr == "ldelem.r8"
                || opcodeStr == "ldelem.u1"
                || opcodeStr == "ldelem.u2"
                || opcodeStr == "ldelem.u2"
                || opcodeStr == "ldelem.u4"
                || opcodeStr == "ldelem.u8"
                )
            {
                operationFactory.LoadReferenceInArray();

                return true;
            }

            if (opcodeStr == "ldelem")
            {
                operationFactory.LoadReferenceInArrayTyped();
                return true;
            }

            if (opcodeStr == "ldc.i4.s" || opcodeStr == "ldc.i4")
            {
                operationFactory.PushInt4(instruction.GetIntOperand());
                return true;
            }
            if (opcodeStr == "ldc.i8.s" || opcodeStr == "ldc.i8")
            {
                operationFactory.PushInt8(instruction.GetLongOperand());
                return true;
            }
            if (opcodeStr.StartsWith("ldc.i4."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldc.i4.".Length).ToInt();
                operationFactory.PushInt4(pushedIntValue);
                return true;
            }
            if (opcodeStr == "ldloc" || opcodeStr == "ldloc.s")
            {
                operationFactory.CopyLocalVariableIntoStack(instruction.GetIntOperand());
                return true;
            }

            if (opcodeStr.StartsWith("ldloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldloc.".Length).ToInt();

                operationFactory.CopyLocalVariableIntoStack(pushedIntValue);
                return true;
            }

            if (opcodeStr == "ldstr")
            {
                operationFactory.PushString((string)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldc.r8")
            {
                operationFactory.PushDouble((double)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldc.r4")
            {
                operationFactory.PushDouble((float)instruction.Operand);
                return true;
            }
            if (opcodeStr.StartsWith("ldarg."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldarg.".Length).ToInt();
                operationFactory.LoadArgument(pushedIntValue);
                return true;
            }

            if (opcodeStr.StartsWith("ldobj"))
            {


                operationFactory.LoadObject((Type)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldfld")
            {
                var operand = (FieldReference)instruction.Operand;

                operationFactory.LoadField(operand.Name);
                return true;
            }
            if (opcodeStr == "ldlen")
            {
                operationFactory.LoadLength();
                return true;
            }

            if (opcodeStr == "ldnull")
            {
                operationFactory.LoadNull();
                return true;
            }
            return false;
        }

        private static bool HandleClassCast(string opcodeStr, int offset, MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {


            if (opcodeStr == "castclass"
                )
            {
                operationFactory.CastClass(((TypeReference)instruction.Operand).GetClrType());
                return true;
            }

            return false;
        }

        private static bool HandleIsInst(string opcodeStr, int offset, MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {


            if (opcodeStr == "isinst"
                )
            {
                operationFactory.IsInst(((TypeReference)instruction.Operand).GetClrType());
                return true;
            }

            return false;
        }

        private static bool HandleException(string opcodeStr, int offset, MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {


            if (opcodeStr == "throw"
                )
            {
                operationFactory.Throw();
                return true;
            }

            return false;
        }


        private static bool HandleBoxing(string opcodeStr, int offset, MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            #region Branching

            if (opcodeStr == "box"
                )
            {
                operationFactory.Box();
                return true;
            }
            if (opcodeStr == "unbox.any"
              )
            {
                operationFactory.Unbox((Type)instruction.Operand);
                return true;
            }

            #endregion

            return false;
        }

        private static bool HandleBranching(string opcodeStr, int offset,
            MetaMidRepresentationOperationFactory operationFactory)
        {
            #region Branching

            if (opcodeStr == OpcodeBranchNames.Leave
                || opcodeStr == OpcodeBranchNames.LeaveS
                )
            {
                operationFactory.AlwaysBranch(offset);
                operationFactory.LeaveTo(offset);
                operationFactory.ClearStack();
                return true;
            }


            if (opcodeStr == OpcodeBranchNames.BrTrueS
                || opcodeStr == OpcodeBranchNames.BrTrue
                || opcodeStr == OpcodeBranchNames.BrInstS
                || opcodeStr == OpcodeBranchNames.BrInst)
            {
                operationFactory.BranchIfTrue(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.BrFalseS
                || opcodeStr == OpcodeBranchNames.BrFalse
                || opcodeStr == OpcodeBranchNames.BrNullS
                || opcodeStr == OpcodeBranchNames.BrNull
                || opcodeStr == OpcodeBranchNames.BrZeroS
                || opcodeStr == OpcodeBranchNames.BrZero)
            {
                operationFactory.BranchIfFalse(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Beq || opcodeStr == OpcodeBranchNames.BeqS)
            {
                operationFactory.BranchIfEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Bge || opcodeStr == OpcodeBranchNames.BgeS)
            {
                operationFactory.BranchIfGreaterOrEqual(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bgt || opcodeStr == OpcodeBranchNames.BgtS)
            {
                operationFactory.BranchIfGreater(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Ble || opcodeStr == OpcodeBranchNames.BleS)
            {
                operationFactory.BranchIfLessOrEqual(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Blt || opcodeStr == OpcodeBranchNames.BltS)
            {
                operationFactory.BranchIfLess(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bne
                || opcodeStr == OpcodeBranchNames.BneUn
                || opcodeStr == OpcodeBranchNames.BneUnS
                || opcodeStr == OpcodeBranchNames.BneS)
            {
                operationFactory.BranchIfNotEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.BrS || opcodeStr == OpcodeBranchNames.Br)
            {
                operationFactory.AlwaysBranch(offset);
                return true;
            }

            #endregion

            return false;
        }

        private static bool HandleOperators(string opcodeStr, MetaMidRepresentationOperationFactory operationFactory)
        {
            #region Operators

            if (opcodeStr == OpcodeOperatorNames.Add)
            {
                operationFactory.Add();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Sub)
            {
                operationFactory.Sub();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Div)
            {
                operationFactory.Div();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Mul)
            {
                operationFactory.Mul();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Rem)
            {
                operationFactory.Rem();
                return true;
            }


            if (opcodeStr == OpcodeOperatorNames.And)
            {
                operationFactory.And();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Or)
            {
                operationFactory.Or();
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Xor)
            {
                operationFactory.Xor();
                return true;
            }

            #region Unary operators

            if (opcodeStr == OpcodeOperatorNames.Not)
            {
                operationFactory.Not();
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Neg)
            {
                operationFactory.Neg();
                return true;
            }

            #endregion

            #region Compare operators

            if (opcodeStr == "cgt" || opcodeStr == "cgt.un")
            {
                operationFactory.Cgt();
                return true;
            }

            if (opcodeStr == "ceq")
            {
                operationFactory.Ceq();
                return true;
            }
            if (opcodeStr == "clt")
            {
                operationFactory.Clt();
                return true;
            }

            #endregion

            #endregion

            return false;
        }
    }
}
