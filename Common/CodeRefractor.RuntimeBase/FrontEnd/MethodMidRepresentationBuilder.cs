#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.Util;
using Mono.Reflection;

#endregion

namespace CodeRefractor.FrontEnd
{
    internal class MethodMidRepresentationBuilder
    {
        private readonly CilMethodInterpreter _methodInterpreter;
        private readonly MethodBase _method;

        public MethodMidRepresentationBuilder(CilMethodInterpreter methodInterpreter, MethodBase method)
        {
            _methodInterpreter = methodInterpreter;
            _method = method;
        }

        public void ProcessInstructions()
        {
            var instructions = _method.GetInstructions().ToArray();
            var genericArguments = _method.DeclaringType.GetGenericArguments();
            Type[] methodGenericArguments = (_method.IsConstructor) ? new Type[0] : _method.GetGenericArguments();
            var finalGeneric = new List<Type>();
            finalGeneric.AddRange(genericArguments);
            finalGeneric.AddRange(methodGenericArguments);


            var labelList = ComputeLabels(_method);
            var evaluator = new EvaluatorStack(finalGeneric.ToArray());
            var operationFactory = new MetaMidRepresentationOperationFactory(_methodInterpreter.MidRepresentation,
                evaluator);

            for (var index = 0; index < instructions.Length; index++)
            {
                var instruction = instructions[index];
                EvaluateInstruction(instruction, operationFactory, labelList);
            }
            //   Ensure.IsTrue(evaluator.Count == 0, "Stack not empty!");
            var analyzeProperties = _methodInterpreter.AnalyzeProperties;
            analyzeProperties.Setup(_methodInterpreter.MidRepresentation.Vars.VirtRegs,
                _methodInterpreter.MidRepresentation.Vars.LocalVars);
        }

        //without using this code, R# will complain of too high cyclomatic complexity
        private static readonly HashSet<int> BranchOpcodes = new HashSet<int>
        {
            OpcodeIntValues.Beq,
            OpcodeIntValues.BeqS,
            OpcodeIntValues.BneUn,
            OpcodeIntValues.BneUnS,
            OpcodeIntValues.Bge,
            OpcodeIntValues.BgeS,
            OpcodeIntValues.BgeUn,
            OpcodeIntValues.BgeUnS,
            OpcodeIntValues.Bgt,
            OpcodeIntValues.BgtS,
            OpcodeIntValues.BgtUn,
            OpcodeIntValues.BgtUnS,
            OpcodeIntValues.BrTrueS,
            OpcodeIntValues.BrTrue,
            OpcodeIntValues.BrZero,
            OpcodeIntValues.BrZeroS,
            OpcodeIntValues.Ble, 
            OpcodeIntValues.BleS,
            OpcodeIntValues.BleUn, // Were missing leading to no labels being generated
            OpcodeIntValues.BleUnS,
            OpcodeIntValues.Blt,
            OpcodeIntValues.BltS,
            OpcodeIntValues.BltUn,
            OpcodeIntValues.BltUnS,
            OpcodeIntValues.BrS,
            OpcodeIntValues.Br,
            OpcodeIntValues.Leave,
            OpcodeIntValues.LeaveS,
        };

        private static HashSet<int> ComputeLabels(MethodBase definition)
        {
            var labels = new HashSet<int>();
            var body = definition.GetMethodBody();
            if (body == null)
                return labels;
            var instructions = definition.GetInstructions();
            foreach (var instruction in instructions)
            {
                var opcodeStr = instruction.OpCode.Value;
                if (opcodeStr == OpcodeIntValues.Switch)
                {
                    var offsets = (Instruction[])instruction.Operand;
                    foreach (var offset in offsets)
                    {
                        AddLabelIfDoesntExist(offset.Offset, labels);
                    }
                }
                if (BranchOpcodes.Contains(opcodeStr))
                {
                    var offset = ((Instruction)instruction.Operand).Offset;
                    AddLabelIfDoesntExist(offset, labels);
                }
            }
            return labels;
        }

        public static void AddLabelIfDoesntExist(int offset, HashSet<int> labels)
        {
            labels.Add(offset);
        }

        private void EvaluateInstruction(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory, HashSet<int> labelList)
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
            if (HandleOperators(opcodeStr, operationFactory, instruction))
                return;

            if (HandleBranching(opcodeStr, offset, operationFactory))
                return;
            if (HandleBoxing(opcodeStr, offset, operationFactory, instruction))
                return;

            if (HandleClassCast(operationFactory, instruction))
                return;
            if (HandleIsInst(operationFactory, instruction))
                return;


            if (HandleException(operationFactory, instruction))
                return;


            if (HandleExtraInstructions(instruction, operationFactory, opcodeStr)) return;
            throw new InvalidOperationException(String.Format("Unknown instruction: {0}", instruction));
        }

        private bool HandleExtraInstructions(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory, string opcodeStr)
        {
            if (opcodeStr == "constrained.")
            {
                operationFactory.ConstrainedClass = (Type) instruction.Operand;
                return true;
            }

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
                operationFactory.SetToken((FieldInfo)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldftn")
            {
                operationFactory.LoadFunction((MethodBase)instruction.Operand);
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
                operationFactory.LoadStaticField((FieldInfo)instruction.Operand);
                return true;
            }
         

            if (opcodeStr == "stsfld")
            {
                operationFactory.StoreStaticField((FieldInfo)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldloca.s" || opcodeStr == "ldloca")
            {
                //TODO: load the address into evaluation stack
                var index = instruction.Operand;

                operationFactory.LoadAddressIntoEvaluationStack((LocalVariableInfo)index);
                return true;
            }
            if (opcodeStr == "ldflda.s" || opcodeStr == "ldflda")
            {
                var fieldInfo = (FieldInfo)instruction.Operand;

                operationFactory.LoadFieldAddressIntoEvaluationStack(fieldInfo);
                return true;
            }

            if (opcodeStr == "ldelema")
            {
                var ldElemTypeDefinition = (Type)instruction.Operand;
                operationFactory.LoadAddressOfArrayItemIntoStack(ldElemTypeDefinition);
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

        private static bool HandleArrayOperations(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory, string opcodeStr)
        {
            if (opcodeStr == "newarr")
            {
                operationFactory.NewArray((Type)instruction.Operand);
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
            if (opcodeStr == "stelem.any")
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

        private static bool HandleCalls(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory, short opcodeValue)
        {
            switch (opcodeValue)
            {
                case OpcodeIntValues.Nop:
                    return true;
                case OpcodeIntValues.Call:
                    operationFactory.Call((MethodBase)instruction.Operand);
                    return true;
                case OpcodeIntValues.CallVirt:
                    operationFactory.CallVirtual((MethodBase)instruction.Operand);
                    operationFactory.ConstrainedClass = null;
                    return true;

                case OpcodeIntValues.NewObj:
                    {
                        var consInfo = ((MethodBase)instruction.Operand);
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

        private bool HandleStores(string opcodeStr, Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory)
        {
            if (opcodeStr == "stloc.s" || opcodeStr == "stloc")
            {
                operationFactory.CopyStackIntoLocalVariable(GetVariableIndex(instruction));
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
                var parameter = (ParameterInfo)instruction.Operand;
                var pushedIntValue = parameter.Position;
                operationFactory.CopyStackIntoArgument(pushedIntValue, _methodInterpreter.AnalyzeProperties);
                return true;
            }

            if (opcodeStr == "stfld")
            {
                var fieldInfo = (FieldInfo)instruction.Operand;
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

        private bool HandleLoads(string opcodeStr, Instruction instruction,
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
            if (opcodeStr == "ldelem.any")
            {
                operationFactory.LoadReferenceInArrayTyped();
                return true;
            }


            if (opcodeStr == "ldc.i4.s" || opcodeStr == "ldc.i4")
            {
                operationFactory.PushInt4(Convert.ToInt32(instruction.Operand));
                return true;
            }
            if (opcodeStr == "ldc.i8.s" || opcodeStr == "ldc.i8")
            {
                operationFactory.PushInt8(Convert.ToInt64(instruction.Operand));
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
                operationFactory.CopyLocalVariableIntoStack(GetVariableIndex(instruction));
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

            if (opcodeStr == "ldarga" || opcodeStr == "ldarga.s")
            {
                operationFactory.LoadArgument(GetParameterIndex(instruction), _methodInterpreter.AnalyzeProperties);
                return true;
            }

            if (opcodeStr == "ldarg.s")
            {
                operationFactory.LoadArgument(GetParameterIndex(instruction), _methodInterpreter.AnalyzeProperties);
                return true;
            }

            if (opcodeStr.StartsWith("ldarg."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldarg.".Length).ToInt();
                operationFactory.LoadArgument(pushedIntValue, _methodInterpreter.AnalyzeProperties);
                return true;
            }

            if (opcodeStr.StartsWith("ldobj"))
            {
                operationFactory.LoadObject((Type)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldfld")
            {
                var operand = (FieldInfo)instruction.Operand;

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

        private static bool HandleClassCast(MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Castclass)
            {
                operationFactory.CastClass((Type)instruction.Operand);
                return true;
            }

            return false;
        }

        private static bool HandleIsInst(MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Isinst)
            {
                operationFactory.IsInst((Type)instruction.Operand);
                return true;
            }

            return false;
        }

        private static bool HandleException(MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Throw)
            {
                operationFactory.Throw();
                return true;
            }

            return false;
        }


        private static bool HandleBoxing(string opcodeStr, int offset,
            MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
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

            if (opcodeStr == OpcodeBranchNames.BgeUn || opcodeStr == OpcodeBranchNames.BgeUnS) //Todo: Fix this cannot treat unsigned as signed
            {
                operationFactory.BranchIfGreaterOrEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Bge || opcodeStr == OpcodeBranchNames.BgeS)
            {
                operationFactory.BranchIfGreaterOrEqual(offset);
                return true;
            }



            if (opcodeStr == OpcodeBranchNames.BgtUn || opcodeStr == OpcodeBranchNames.BgtUnS) //Todo: Fix this cannot treat unsigned as signed
            {
                operationFactory.BranchIfGreater(offset);
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

            if (opcodeStr == OpcodeBranchNames.BleUn || opcodeStr == OpcodeBranchNames.BleUnS)  //Todo: Fix this cannot treat unsigned as signed
            {
                operationFactory.BranchIfLessOrEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.BltUn || opcodeStr == OpcodeBranchNames.BltUnS)  //Todo: Fix this cannot treat unsigned as signed
            {
                operationFactory.BranchIfLess(offset);
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

        private static bool HandleOperators(string opcodeStr, MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            #region Operators

            if (instruction.OpCode == OpCodes.Add)
            {
                operationFactory.Add();
                return true;
            }
            if (instruction.OpCode == OpCodes.Sub)
            {
                operationFactory.Sub();
                return true;
            }
            if (instruction.OpCode == OpCodes.Div)
            {
                operationFactory.Div();
                return true;
            }
            if (instruction.OpCode == OpCodes.Mul)
            {
                operationFactory.Mul();
                return true;
            }
            if (instruction.OpCode == OpCodes.Rem)
            {
                operationFactory.Rem();
                return true;
            }


            if (instruction.OpCode == OpCodes.And)
            {
                operationFactory.And();
                return true;
            }
            if (instruction.OpCode == OpCodes.Or)
            {
                operationFactory.Or();
                return true;
            }

            if (instruction.OpCode == OpCodes.Xor)
            {
                operationFactory.Xor();
                return true;
            }

            #region Unary operators

            if (instruction.OpCode == OpCodes.Not)
            {
                operationFactory.Not();
                return true;
            }

            if (instruction.OpCode == OpCodes.Neg)
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

        private static int GetVariableIndex(Instruction instruction)
        {
            var localVarInfo = (LocalVariableInfo)instruction.Operand;
            return localVarInfo.LocalIndex;
        }

        private static int GetParameterIndex(Instruction instruction)
        {
            var localVarInfo = (ParameterInfo)instruction.Operand;
            return localVarInfo.Position;
        }
    }
}