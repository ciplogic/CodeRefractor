using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.Compiler.Shared;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Shared;
using Mono.Reflection;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MethodInterpreter
    {
        public readonly MethodBase Method;

        public MethodInterpreter(MethodBase method)
        {
            Method = method;

            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                PureMethodTable.AddPureFunction(method);
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}(...);", Method.DeclaringType.ToCppMangling(), Method.Name);
        }

        public MethodKind Kind { get; set; }

        public readonly MetaMidRepresentation MidRepresentation = new MetaMidRepresentation();
        public readonly PlatformInvokeRepresentation PlatformInvoke = new PlatformInvokeRepresentation();
        private HashSet<int> _hashedLabels;

        public void Process()
        {
            if(HandlePlatformInvokeMethod(Method))
                return;

            var instructions = MethodBodyReader.GetInstructions(Method);

            MidRepresentation.Method = Method;
            var evaluator = new EvaluatorStack();
            foreach (var instruction in instructions)
            {
                EvaluateInstuction(instruction, evaluator);
            }

        }

        private void EvaluateInstuction(Instruction instruction, EvaluatorStack evaluator)
        {
            var opcodeStr = instruction.OpCode.ToString();
            var offset = 0;
            if (instruction.Operand is Instruction)
            {
                offset = ((Instruction) (instruction.Operand)).Offset;
            }
            if (_hashedLabels.Contains(instruction.Offset))
            {
                MidRepresentation.SetLabel(instruction.Offset);
            }
            var opcodeValue = instruction.OpCode.Value;
            switch (opcodeValue)
            {
                case ObcodeIntValues.Nop:
                    return;
                case ObcodeIntValues.Call:
                case ObcodeIntValues.CallVirt:
                case ObcodeIntValues.CallInterface:
                    MidRepresentation.Call(instruction.Operand, evaluator);
                    return;
                case ObcodeIntValues.NewObj:
                    {
                        var consInfo = (ConstructorInfo) instruction.Operand;
                        MidRepresentation.NewObject(consInfo, evaluator);
                    }
                    return;
            }
            
            if (HandleStores(opcodeStr, instruction, evaluator))
                return;
            if (HandleLoads(opcodeStr, instruction, evaluator))
                return;
            if (HandleOperators(opcodeStr, evaluator))
                return;

            if (HandleBranching(opcodeStr, offset, evaluator))
                return;

            if (opcodeStr == "ret")
            {
                var isVoid = MidRepresentation.Method.GetReturnType().IsVoid();

                MidRepresentation.Return(isVoid, evaluator);
                return;
            }

            if (opcodeStr == "conv.i4")
            {
                MidRepresentation.ConvI4(evaluator);
                return;
            }
            if (opcodeStr == "conv.r8")
            {
                MidRepresentation.ConvR8(evaluator);
                return;
            }
            if (opcodeStr == "dup")
            {
                MidRepresentation.Dup(evaluator);
                return;
            }


            if (opcodeStr == "newarr")
            {
                MidRepresentation.NewArray(evaluator, (Type) instruction.Operand);
                return;
            }
            if (opcodeStr == "stelem.ref")
            {
                MidRepresentation.SetArrayElementValue(evaluator);
                return;
            }
            if (opcodeStr == "ldtoken")
            {
                MidRepresentation.SetToken(evaluator, (FieldInfo) instruction.Operand);
                return;
            }
            if (opcodeStr == "ldftn")
            {
                MidRepresentation.LoadFunction(evaluator, (MethodBase)instruction.Operand);
                return;
            }
            if (opcodeStr == "switch")
            {
                MidRepresentation.Switch(evaluator, (Instruction[])instruction.Operand);
                return;
            }
            if (opcodeStr == "ldsfld")
            {
                MidRepresentation.LoadStaticField(evaluator, (FieldInfo)instruction.Operand);
                return;
            }
            if (opcodeStr == "stsfld")
            {
                MidRepresentation.StoreStaticField(evaluator, (FieldInfo)instruction.Operand);
                return;
            }


            throw new InvalidOperationException(string.Format("Unknown instruction: {0}", instruction));
        }

        private bool HandlePlatformInvokeMethod(MethodBase method)
        {
            var pinvokeAttribute = method.GetCustomAttribute<DllImportAttribute>();

            if (pinvokeAttribute == null)
                return false;
            PlatformInvoke.LibraryName = pinvokeAttribute.Value;
            PlatformInvoke.MethodName = method.Name;
            Kind = MethodKind.PlatformInvoke;
            return true;
        }

        private bool HandleStores(string opcodeStr, Instruction instruction, EvaluatorStack evaluator)
        {
            if (opcodeStr == "stloc.s" || opcodeStr == "stloc")
            {
                MidRepresentation.CopyStackIntoLocalVariable(instruction.GetIntOperand(), evaluator);
                return true;
            }
            if (opcodeStr.StartsWith("stloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "stloc.".Length).ToInt();
                MidRepresentation.CopyStackIntoLocalVariable(pushedIntValue, evaluator);
                return true;
            }

            if (opcodeStr == "stfld")
            {
                var fieldInfo = (FieldInfo) instruction.Operand;
                MidRepresentation.StoreField(fieldInfo, evaluator);
                return true;
            }
            return false;
        }

        private bool HandleLoads(string opcodeStr, Instruction instruction, EvaluatorStack evaluator)
        {
            if (opcodeStr == "ldelem.ref")
            {
                MidRepresentation.LoadReferenceInArray(evaluator);
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
                MidRepresentation.LoadReferenceInArray(evaluator);
                return true;
            }
                

            if (opcodeStr == "ldc.i4.s" || opcodeStr == "ldc.i4")
            {
                MidRepresentation.PushInt4(instruction.GetIntOperand(), evaluator);
                return true;
            }
            if (opcodeStr.StartsWith("ldc.i4."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldc.i4.".Length).ToInt();
                MidRepresentation.PushInt4(pushedIntValue, evaluator);
                return true;
            }
            if (opcodeStr == "ldloc" || opcodeStr == "ldloc.s")
            {
                MidRepresentation.CopyLocalVariableIntoStack(instruction.GetIntOperand(), evaluator);
                return true;
            }

            if (opcodeStr.StartsWith("ldloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldloc.".Length).ToInt();
                MidRepresentation.CopyLocalVariableIntoStack(pushedIntValue, evaluator);
                return true;
            }

            if (opcodeStr == "ldstr")
            {
                MidRepresentation.PushString((string) instruction.Operand, evaluator);
                return true;
            }
            if (opcodeStr == "ldc.r8")
            {
                MidRepresentation.PushDouble((double) instruction.Operand, evaluator);
                return true;
            }
            if (opcodeStr == "ldc.r4")
            {
                MidRepresentation.PushDouble((float) instruction.Operand, evaluator);
                return true;
            }
            if (opcodeStr.StartsWith("ldarg."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldarg.".Length).ToInt();
                MidRepresentation.LoadArgument(pushedIntValue, evaluator);
                return true;
            }
            if (opcodeStr == "ldfld")
            {
                var operand = (FieldInfo)instruction.Operand;

                MidRepresentation.LoadField(operand.Name, evaluator);
                return true;
            }
            if (opcodeStr == "ldlen")
            {
                MidRepresentation.LoadLength(evaluator);
                return true;
            }

            if (opcodeStr == "ldnull")
            {
                MidRepresentation.LoadNull(evaluator);
                return true;
            }
            return false;
        }

        private bool HandleBranching(string opcodeStr, int offset, EvaluatorStack evaluator)
        {
            #region Branching

            if (opcodeStr == OpcodeBranchNames.BrTrueS
                || opcodeStr == OpcodeBranchNames.BrTrue
                || opcodeStr == OpcodeBranchNames.BrInstS
                || opcodeStr == OpcodeBranchNames.BrInst)
            {
                MidRepresentation.BranchIfTrue(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.BrFalseS
                || opcodeStr == OpcodeBranchNames.BrFalse
                || opcodeStr == OpcodeBranchNames.BrNullS
                || opcodeStr == OpcodeBranchNames.BrNull
                || opcodeStr == OpcodeBranchNames.BrZeroS
                || opcodeStr == OpcodeBranchNames.BrZero)
            {
                MidRepresentation.BranchIfFalse(offset, evaluator);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Beq || opcodeStr == OpcodeBranchNames.BeqS)
            {
                MidRepresentation.BranchIfEqual(offset, evaluator);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Bge || opcodeStr == OpcodeBranchNames.BgeS)
            {
                MidRepresentation.BranchIfGreaterOrEqual(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bgt || opcodeStr == OpcodeBranchNames.BgtS)
            {
                MidRepresentation.BranchIfGreater(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Ble || opcodeStr == OpcodeBranchNames.BleS)
            {
                MidRepresentation.BranchIfLessOrEqual(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Blt || opcodeStr == OpcodeBranchNames.BltS)
            {
                MidRepresentation.BranchIfLess(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bne || opcodeStr == OpcodeBranchNames.BneS)
            {
                MidRepresentation.BranchIfNotEqual(offset, evaluator);
                return true;
            }

            if (opcodeStr == "br.s" || opcodeStr == "br")
            {
                MidRepresentation.AlwaysBranch(offset);
                return true;
            }

            #endregion

            return false;
        }

        private bool HandleOperators(string opcodeStr, EvaluatorStack evaluator)
        {
            #region Operators

            if (opcodeStr == OpcodeOperatorNames.Add)
            {
                MidRepresentation.Add(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Sub)
            {
                MidRepresentation.Sub(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Div)
            {
                MidRepresentation.Div(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Mul)
            {
                MidRepresentation.Mul(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Rem)
            {
                MidRepresentation.Rem(evaluator);
                return true;
            }


            if (opcodeStr == OpcodeOperatorNames.And)
            {
                MidRepresentation.And(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Or)
            {
                MidRepresentation.Or(evaluator);
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Xor)
            {
                MidRepresentation.Xor(evaluator);
                return true;
            }

            #region Unary operators

            if (opcodeStr == OpcodeOperatorNames.Not)
            {
                MidRepresentation.Not(evaluator);
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Neg)
            {
                MidRepresentation.Neg(evaluator);
                return true;
            }
            #endregion

            #region Compare operators

            if (opcodeStr == "cgt")
            {
                MidRepresentation.Cgt(evaluator);
                return true;
            }

            if (opcodeStr == "ceq")
            {
                MidRepresentation.Ceq(evaluator);
                return true;
            }
            if (opcodeStr == "clt")
            {
                MidRepresentation.Clt(evaluator);
                return true;
            }

            #endregion

            #endregion

            return false;
        }

        public void SetLabels(IEnumerable<int> labelList)
        {
            _hashedLabels = labelList == null
                ? new HashSet<int>()
                : new HashSet<int>(labelList);
        }
    }

}