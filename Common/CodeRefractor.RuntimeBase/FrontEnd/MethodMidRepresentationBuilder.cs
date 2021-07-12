﻿#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.Shared;
using CodeRefractor.Util;
using Mono.Reflection;

#endregion

namespace CodeRefractor.FrontEnd
{
    class MethodMidRepresentationBuilder
    {
        //without using this code, R# will complain of too high cyclomatic complexity
        static readonly HashSet<int> BranchOpcodes = new HashSet<int>
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
            OpcodeIntValues.LeaveS
        };

        readonly MethodBase _method;
        readonly CilMethodInterpreter _methodInterpreter;

        public MethodMidRepresentationBuilder(CilMethodInterpreter methodInterpreter, MethodBase method)
        {
            _methodInterpreter = methodInterpreter;
            _method = method;
        }

        public void ProcessInstructions(ClosureEntities closureEntities)
        {
            var instructions = _method.GetInstructions().ToArray();
            var exceptionRanges = ExceptionCatchClauseRanges.ComputeExceptionInstructionRanges(_method);
            var genericArguments = _method.DeclaringType.GetGenericArguments();
            var methodGenericArguments = (_method.IsConstructor) ? new Type[0] : _method.GetGenericArguments();
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
                if (ExceptionCatchClauseRanges.IndexInRanges(instruction, exceptionRanges))
                    continue;
                EvaluateInstruction(instruction, operationFactory, labelList, closureEntities);
            }
            //   Ensure.IsTrue(evaluator.Count == 0, "Stack not empty!");
            var analyzeProperties = _methodInterpreter.AnalyzeProperties;
            analyzeProperties.Setup(_methodInterpreter.MidRepresentation.Vars.VirtRegs,
                _methodInterpreter.MidRepresentation.Vars.LocalVars);
        }

        static HashSet<int> ComputeLabels(MethodBase definition)
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

        void EvaluateInstruction(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory,
            HashSet<int> labelList, ClosureEntities closureEntities)
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
            if (HandleLoads(opcodeStr, instruction, operationFactory, closureEntities))
                return;
            if (HandleOperators(opcodeStr, operationFactory, instruction))
                return;

            if (HandleBranching(opcodeStr, offset, operationFactory, instruction))
                return;
            if (HandleBoxing(opcodeStr, offset, operationFactory, instruction))
                return;

            if (HandleClassCast(operationFactory, instruction))
                return;
            if (HandleIsInst(operationFactory, instruction))
                return;


            if (HandleException(operationFactory, instruction))
                return;


            if (HandleExtraInstructions(instruction, operationFactory, opcodeStr, closureEntities)) return;
            throw new InvalidOperationException($"Unknown instruction: {instruction}");
        }

        bool HandleExtraInstructions(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, string opcodeStr, ClosureEntities closureEntities)
        {
            if (instruction.OpCode == OpCodes.Ret)
            {
                var isVoid = _method.GetReturnType().IsVoid();

                operationFactory.Return(isVoid);
                return true;
            }
            if (opcodeStr == "constrained.")
            {
                operationFactory.ConstrainedClass = (Type)instruction.Operand;
                return true;
            }

            if (opcodeStr.StartsWith("conv."))
            {
                if (ConversionOperations(instruction, operationFactory)) return true;
                return true;
            }

            if (instruction.OpCode == OpCodes.Dup)
            {
                operationFactory.Dup();
                return true;
            }
            if (instruction.OpCode == OpCodes.Pop)
            {
                operationFactory.Pop();
                return true;
            }
            if (HandleArrayOperations(instruction, operationFactory, opcodeStr))
            {
                return true;
            }
            if (HandleLoadStoreInstructions(instruction, operationFactory, opcodeStr, closureEntities))
            {
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

        static bool HandleLoadStoreInstructions(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, string opcodeStr, ClosureEntities closureEntities)
        {
            if (instruction.OpCode == OpCodes.Ldtoken)
            {
                operationFactory.SetToken((FieldInfo)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldftn)
            {
                operationFactory.LoadFunction((MethodBase)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Switch)
            {
                operationFactory.Switch((Instruction[])instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Sizeof)
            {
                operationFactory.SizeOf((Type)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldsfld)
            {
                operationFactory.LoadStaticField((FieldInfo)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Stsfld)
            {
                operationFactory.StoreStaticField((FieldInfo)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldloca_S || instruction.OpCode == OpCodes.Ldloca)
            {
                //TODO: load the address into evaluation stack
                var index = instruction.Operand;

                operationFactory.LoadAddressIntoEvaluationStack((LocalVariableInfo)index);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldflda)
            {
                var fieldInfo = (FieldInfo)instruction.Operand;

                operationFactory.LoadFieldAddressIntoEvaluationStack(fieldInfo);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldsflda)
            {
                var fieldInfo = (FieldInfo)instruction.Operand;

                operationFactory.LoadStaticFieldAddressIntoEvaluationStack(fieldInfo);
                return true;
            }

            if (instruction.OpCode == OpCodes.Ldelema)
            {
                var ldElemTypeDefinition = (Type)instruction.Operand;
                operationFactory.LoadAddressOfArrayItemIntoStack(ldElemTypeDefinition);
                return true;
            }

            if (opcodeStr.StartsWith("stind.", StringComparison.Ordinal))
            {
                //TODO: load the address into evaluation stack
                operationFactory.StoresValueFromAddress();
                return true;
            }

            if (opcodeStr.StartsWith("ldind.", StringComparison.Ordinal))
            {
                //TODO: load the address into evaluation stack
                operationFactory.LoadValueFromAddress(closureEntities);
                return true;
            }
            return false;
        }

        static bool HandleArrayOperations(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, string opcodeStr)
        {
            if (instruction.OpCode == OpCodes.Newarr)
            {
                operationFactory.NewArray((Type)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Stelem_I1
                || instruction.OpCode == OpCodes.Stelem_I2
                || instruction.OpCode == OpCodes.Stelem_I4
                || instruction.OpCode == OpCodes.Stelem_I8
                || instruction.OpCode == OpCodes.Stelem_R4
                || instruction.OpCode == OpCodes.Stelem_R8)
            {
                operationFactory.SetArrayElementValue();
                return true;
            }
            if (instruction.OpCode == OpCodes.Stelem_Ref)
            {
                operationFactory.SetArrayElementValue();
                return true;
            }
            if (opcodeStr == "stelem.any")
            {
                operationFactory.SetArrayElementValue();
                return true;
            }
            if (instruction.OpCode == OpCodes.Stelem)
            {
                var elemInfo = (Type)instruction.Operand;
                operationFactory.StoreElement();
                return true;
            }
            return false;
        }

        static bool HandleCalls(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory,
            short opcodeValue)
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

        static bool ConversionOperations(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory)
        {
            if (instruction.OpCode == OpCodes.Conv_U1)
            {
                operationFactory.ConvU1();
                return true;
            }
            if (instruction.OpCode == OpCodes.Conv_I)
            {
                operationFactory.ConvI();
                return true;
            }
            if (instruction.OpCode == OpCodes.Conv_I4)
            {
                operationFactory.ConvI4();
                return true;
            }
            if (instruction.OpCode == OpCodes.Conv_I8)
            {
                operationFactory.ConvI8();
                return true;
            }
            if (instruction.OpCode == OpCodes.Conv_R4)
            {
                operationFactory.ConvR4();
                return true;
            }
            if (instruction.OpCode == OpCodes.Conv_R8)
            {
                operationFactory.ConvR8();
                return true;
            }
            return false;
        }

        bool HandleStores(string opcodeStr, Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory)
        {
            if (instruction.OpCode == OpCodes.Stloc_S || instruction.OpCode == OpCodes.Stloc)
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

            if (instruction.OpCode == OpCodes.Stfld)
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

        bool HandleLoads(string opcodeStr, Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, ClosureEntities closureEntities)
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
                operationFactory.LoadReferenceInArrayTyped(closureEntities);
                return true;
            }
            if (opcodeStr == "ldelem.any")
            {
                operationFactory.LoadReferenceInArrayTyped(closureEntities);
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
            if (instruction.OpCode == OpCodes.Ldloc || instruction.OpCode == OpCodes.Ldloc_S)
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

            if (instruction.OpCode == OpCodes.Ldstr)
            {
                operationFactory.PushString((string)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldc_R8)
            {
                operationFactory.PushDouble((double)instruction.Operand);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldc_R4)
            {
                operationFactory.PushFloat((float)instruction.Operand);
                return true;
            }

            if (instruction.OpCode == OpCodes.Ldarga || instruction.OpCode == OpCodes.Ldarga_S)
            {
                operationFactory.LoadArgument(GetParameterIndex(instruction), _methodInterpreter.AnalyzeProperties);
                return true;
            }

            if (instruction.OpCode == OpCodes.Ldarg_S)
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
            if (instruction.OpCode == OpCodes.Ldfld)
            {
                var operand = (FieldInfo)instruction.Operand;

                operationFactory.LoadField(operand.Name, closureEntities);
                return true;
            }
            if (instruction.OpCode == OpCodes.Ldlen)
            {
                operationFactory.LoadLength();
                return true;
            }

            if (instruction.OpCode == OpCodes.Ldnull)
            {
                operationFactory.LoadNull();
                return true;
            }
            return false;
        }

        static bool HandleClassCast(MetaMidRepresentationOperationFactory operationFactory,
            Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Castclass)
            {
                operationFactory.CastClass((Type)instruction.Operand);
                return true;
            }

            return false;
        }

        static bool HandleIsInst(MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Isinst)
            {
                operationFactory.IsInst((Type)instruction.Operand);
                return true;
            }

            return false;
        }

        static bool HandleException(MetaMidRepresentationOperationFactory operationFactory,
            Instruction instruction)
        {
            if (instruction.OpCode == OpCodes.Throw)
            {
                operationFactory.Throw();
                return true;
            }

            return false;
        }

        static bool HandleBoxing(string opcodeStr, int offset,
            MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            #region Branching

            if (opcodeStr == "box"
                )
            {
                operationFactory.Box((Type)instruction.Operand);
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

        static bool HandleBranching(string opcodeStr, int offset,
            MetaMidRepresentationOperationFactory operationFactory, Instruction instruction)
        {
            #region Branching

            if (instruction.OpCode == OpCodes.Leave
                || instruction.OpCode == OpCodes.Leave_S
                )
            {
                //TODO: ignore flow commands for exceptions
                //operationFactory.LeaveTo(offset);
                return true;
            }

            if (instruction.OpCode == OpCodes.Endfinally
                )
            {
                //TODO: ignore flow commands for exceptions
                return true;
            }
            if (instruction.OpCode == OpCodes.Brtrue_S
                || instruction.OpCode == OpCodes.Brtrue)
            {
                operationFactory.BranchIfTrue(offset);
                return true;
            }

            if (instruction.OpCode == OpCodes.Br_S ||
                instruction.OpCode == OpCodes.Br)
            {
                operationFactory.AlwaysBranch(offset);
                return true;
            }

            if (instruction.OpCode == OpCodes.Brfalse
                || instruction.OpCode == OpCodes.Brfalse_S)
            {
                operationFactory.BranchIfFalse(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Beq || opcodeStr == OpcodeBranchNames.BeqS)
            {
                operationFactory.BranchIfEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.BgeUn || opcodeStr == OpcodeBranchNames.BgeUnS)
            //Todo: Fix this cannot treat unsigned as signed
            {
                operationFactory.BranchIfGreaterOrEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Bge || opcodeStr == OpcodeBranchNames.BgeS)
            {
                operationFactory.BranchIfGreaterOrEqual(offset);
                return true;
            }


            if (opcodeStr == OpcodeBranchNames.BgtUn || opcodeStr == OpcodeBranchNames.BgtUnS)
            //Todo: Fix this cannot treat unsigned as signed
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

            if (opcodeStr == OpcodeBranchNames.BleUn || opcodeStr == OpcodeBranchNames.BleUnS)
            //Todo: Fix this cannot treat unsigned as signed
            {
                operationFactory.BranchIfLessOrEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.BltUn || opcodeStr == OpcodeBranchNames.BltUnS)
            //Todo: Fix this cannot treat unsigned as signed
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

            #endregion

            return false;
        }

        static bool HandleOperators(string opcodeStr, MetaMidRepresentationOperationFactory operationFactory,
            Instruction instruction)
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

        static int GetVariableIndex(Instruction instruction)
        {
            var localVarInfo = (LocalVariableInfo)instruction.Operand;
            return localVarInfo.LocalIndex;
        }

        static int GetParameterIndex(Instruction instruction)
        {
            var localVarInfo = (ParameterInfo)instruction.Operand;
            return localVarInfo.Position;
        }
    }
}