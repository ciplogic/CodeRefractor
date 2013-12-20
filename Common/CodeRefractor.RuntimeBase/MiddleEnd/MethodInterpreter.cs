#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Shared;
using MsilReader;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MethodInterpreter
    {
        public readonly MethodBase Method;

        public MetaMidRepresentationOperationFactory OperationFactory;
        public List<Type> ClassSpecializationType = new List<Type>();
        public List<Type> MethodSpecializationType = new List<Type>();
        public MethodKind Kind { get; set; }

        public HashSet<int> LabelList { get; set; }

        public MetaMidRepresentation MidRepresentation = new MetaMidRepresentation();
        public PlatformInvokeRepresentation PlatformInvoke = new PlatformInvokeRepresentation();
        //private HashSet<int> _hashedLabels;


        public MethodInterpreter(MethodBase method)
        {
            Method = method;

            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                PureMethodTable.AddPureFunction(method);
        }

        public void Specialize()
        {
            if (!IsGenericDeclaringType()) return;
            var genTypeArguments = Method.DeclaringType.GetGenericArguments();
            foreach (var genTypeArgument in genTypeArguments)
            {
                ClassSpecializationType.Add(genTypeArgument);
            }
        }

        public MethodInterpreter Clone()
        {
            var result = new MethodInterpreter(Method)
            {
                MidRepresentation = MidRepresentation,
                ClassSpecializationType = ClassSpecializationType,
                Kind = Kind,
                LabelList = LabelList,
                MethodSpecializationType = MethodSpecializationType,
                PlatformInvoke = PlatformInvoke
            };
            return result;
        }

        public bool IsGenericDeclaringType()
        {
            return Method.DeclaringType.IsGenericType;
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}(...);", Method.DeclaringType.ToCppMangling(), Method.Name);
        }

        public void Process()
        {
            if (HandlePlatformInvokeMethod(Method))
                return;
            if(Method.GetMethodBody()==null)
                return;
            var instructions = MethodBodyReader.GetInstructions(Method);
            LabelList = MetaLinker.ComputeLabels(Method);
            MidRepresentation.Method = Method;
            var evaluator = new EvaluatorStack();
            OperationFactory = new MetaMidRepresentationOperationFactory(MidRepresentation, evaluator);

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
            if (LabelList.Contains(instruction.Offset))
            {
                OperationFactory.SetLabel(instruction.Offset);
            }
            var opcodeValue = instruction.OpCode.Value;
            OperationFactory.AddCommentInstruction(instruction.ToString());
            switch (opcodeValue)
            {
                case ObcodeIntValues.Nop:
                    return;
                case ObcodeIntValues.Call:
                case ObcodeIntValues.CallVirt:
                case ObcodeIntValues.CallInterface:
                    OperationFactory.Call(instruction.Operand);
                    return;
                case ObcodeIntValues.NewObj:
                    {
                        var consInfo = (ConstructorInfo) instruction.Operand;
                        OperationFactory.NewObject(consInfo);
                    }
                    return;
            }

            if (HandleStores(opcodeStr, instruction, evaluator))
                return;
            if (HandleLoads(opcodeStr, instruction, evaluator))
                return;
            if (HandleOperators(opcodeStr, evaluator))
                return;

            if (HandleBranching(opcodeStr, offset))
                return;

            if (opcodeStr == "ret")
            {
                var isVoid = MidRepresentation.Method.GetReturnType().IsVoid();

                OperationFactory.Return(isVoid);
                return;
            }
            if (opcodeStr.StartsWith("conv."))
            {
                if (ConversionOperations(opcodeStr)) return;
                return;
            }
            if (opcodeStr == "dup")
            {
                OperationFactory.Dup();
                return;
            }
            if (opcodeStr == "pop")
            {
                OperationFactory.Pop();
                return;
            }


            if (opcodeStr == "newarr")
            {
                OperationFactory.NewArray((Type)instruction.Operand);
                return;
            }
            if (opcodeStr == "stelem.ref"
                 || opcodeStr == "stelem.i1"
                 || opcodeStr == "stelem.i2"
                  || opcodeStr == "stelem.i4"
                   || opcodeStr == "stelem.i8"
                  || opcodeStr == "stelem.r4"
                   || opcodeStr == "stelem.r8"
                || opcodeStr=="stelem.i2")
            {
                OperationFactory.SetArrayElementValue();
                return;
            }
            if (opcodeStr == "ldtoken")
            {
                OperationFactory.SetToken((FieldInfo)instruction.Operand);
                return;
            }
            if (opcodeStr == "ldftn")
            {
                OperationFactory.LoadFunction((MethodInfo)instruction.Operand);
                return;
            }
            if (opcodeStr == "switch")
            {
                OperationFactory.Switch((Instruction[])instruction.Operand);
                return;
            }
            if (opcodeStr == "sizeof")
            {
                OperationFactory.SizeOf((Type)instruction.Operand);
                return;
            }
            if (opcodeStr == "ldsfld")
            {
                OperationFactory.LoadStaticField((FieldInfo)instruction.Operand);
                return;
            }
            if (opcodeStr == "stsfld")
            {
                OperationFactory.StoreStaticField((FieldInfo)instruction.Operand);
                return;
            }
            if (opcodeStr == "ldloca.s" || opcodeStr == "ldloca")
            {
                //TODO: load the address into evaluation stack
                var index = (LocalVariableInfo) instruction.Operand;

                OperationFactory.LoadAddressIntoEvaluationStack(index);
                return;
            }
            if (opcodeStr == "ldflda.s" || opcodeStr == "ldflda")
            {
                var fieldInfo = (FieldInfo)instruction.Operand;

                OperationFactory.LoadFieldAddressIntoEvaluationStack(fieldInfo);
                return;
            }

            if (opcodeStr == "ldelema")
            {
                OperationFactory.LoadAddressOfArrayItemIntoStack((Type) instruction.Operand);
                return;
            }

            if (opcodeStr.StartsWith("stind."))
            {
                //TODO: load the address into evaluation stack
                OperationFactory.StoresValueFromAddress();
                return;
            }

            if (opcodeStr.StartsWith("ldind."))
            {
                //TODO: load the address into evaluation stack
                OperationFactory.LoadValueFromAddress();
                return;
            }

            if (opcodeStr.StartsWith("initobj"))
            {
                //TODO: load the address into evaluation stack
                OperationFactory.InitObject();
                return;
            }
            throw new InvalidOperationException(string.Format("Unknown instruction: {0}", instruction));
        }

        private bool ConversionOperations(string opcodeStr)
        {
            if (opcodeStr == "conv.u1")
            {
                OperationFactory.ConvU1();
                return true;
            }
            if (opcodeStr == "conv.i")
            {
                OperationFactory.ConvI();
                return true;
            }
            if (opcodeStr == "conv.i4")
            {
                OperationFactory.ConvI4();
                return true;
            }
            if (opcodeStr == "conv.i8")
            {
                OperationFactory.ConvI8();
                return true;
            }
            if (opcodeStr == "conv.r4")
            {
                OperationFactory.ConvR4();
                return true;
            }
            if (opcodeStr == "conv.r8")
            {
                OperationFactory.ConvR8();
                return true;
            }
            return false;
        }

        private bool HandlePlatformInvokeMethod(MethodBase method)
        {
            var pinvokeAttribute = method.GetCustomAttribute<DllImportAttribute>();

            if (pinvokeAttribute == null)
                return false;
            PlatformInvoke.LibraryName = pinvokeAttribute.Value;
            PlatformInvoke.MethodName = method.Name;
            PlatformInvoke.CallingConvention = pinvokeAttribute.CallingConvention;
            PlatformInvoke.EntryPoint = pinvokeAttribute.EntryPoint;
            Kind = MethodKind.PlatformInvoke;
            return true;
        }

        private bool HandleStores(string opcodeStr, Instruction instruction, EvaluatorStack evaluator)
        {
            if (opcodeStr == "stloc.s" || opcodeStr == "stloc")
            {
                OperationFactory.CopyStackIntoLocalVariable(instruction.GetIntOperand());
                return true;
            }
            if (opcodeStr.StartsWith("stloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "stloc.".Length).ToInt();
                OperationFactory.CopyStackIntoLocalVariable(pushedIntValue);
                return true;
            } 
            
            if (opcodeStr.StartsWith("starg."))
            {
                var parameter = (ParameterInfo) instruction.Operand;
                var pushedIntValue = parameter.Position;
                OperationFactory.CopyStackIntoArgument(pushedIntValue);
                return true;
            }

            if (opcodeStr == "stfld")
            {
                var fieldInfo = (FieldInfo) instruction.Operand;
                OperationFactory.StoreField(fieldInfo);
                return true;
            }
            return false;
        }

        private bool HandleLoads(string opcodeStr, Instruction instruction, EvaluatorStack evaluator)
        {
            if (opcodeStr == "ldelem.ref")
            {
                OperationFactory.LoadReferenceInArray();
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
                OperationFactory.LoadReferenceInArray();
                
                return true;
            }

            if (opcodeStr == "ldc.i4.s" || opcodeStr == "ldc.i4")
            {
                OperationFactory.PushInt4(instruction.GetIntOperand());
                return true;
            }
            if (opcodeStr.StartsWith("ldc.i4."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldc.i4.".Length).ToInt();
                OperationFactory.PushInt4(pushedIntValue);
                return true;
            }
            if (opcodeStr == "ldloc" || opcodeStr == "ldloc.s")
            {
                OperationFactory.CopyLocalVariableIntoStack(instruction.GetIntOperand());
                return true;
            }

            if (opcodeStr.StartsWith("ldloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldloc.".Length).ToInt();

                OperationFactory.CopyLocalVariableIntoStack(pushedIntValue);
                return true;
            }

            if (opcodeStr == "ldstr")
            {
                OperationFactory.PushString((string) instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldc.r8")
            {
                OperationFactory.PushDouble((double)instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldc.r4")
            {
                OperationFactory.PushDouble((float)instruction.Operand);
                return true;
            }
            if (opcodeStr.StartsWith("ldarg."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldarg.".Length).ToInt();
                OperationFactory.LoadArgument(pushedIntValue);
                return true;
            }
            if (opcodeStr == "ldfld")
            {
                var operand = (FieldInfo) instruction.Operand;

                OperationFactory.LoadField(operand.Name);
                return true;
            }
            if (opcodeStr == "ldlen")
            {
                OperationFactory.LoadLength();
                return true;
            }

            if (opcodeStr == "ldnull")
            {
                OperationFactory.LoadNull();
                return true;
            }
            return false;
        }

        private bool HandleBranching(string opcodeStr, int offset)
        {
            #region Branching

            if (opcodeStr == OpcodeBranchNames.BrTrueS
                || opcodeStr == OpcodeBranchNames.BrTrue
                || opcodeStr == OpcodeBranchNames.BrInstS
                || opcodeStr == OpcodeBranchNames.BrInst)
            {
                OperationFactory.BranchIfTrue(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.BrFalseS
                || opcodeStr == OpcodeBranchNames.BrFalse
                || opcodeStr == OpcodeBranchNames.BrNullS
                || opcodeStr == OpcodeBranchNames.BrNull
                || opcodeStr == OpcodeBranchNames.BrZeroS
                || opcodeStr == OpcodeBranchNames.BrZero)
            {
                OperationFactory.BranchIfFalse(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Beq || opcodeStr == OpcodeBranchNames.BeqS)
            {
                OperationFactory.BranchIfEqual(offset);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Bge || opcodeStr == OpcodeBranchNames.BgeS)
            {
                OperationFactory.BranchIfGreaterOrEqual(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bgt || opcodeStr == OpcodeBranchNames.BgtS)
            {
                OperationFactory.BranchIfGreater(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Ble || opcodeStr == OpcodeBranchNames.BleS)
            {
                OperationFactory.BranchIfLessOrEqual(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Blt || opcodeStr == OpcodeBranchNames.BltS)
            {
                OperationFactory.BranchIfLess(offset);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bne || opcodeStr == OpcodeBranchNames.BneS)
            {
                OperationFactory.BranchIfNotEqual(offset);
                return true;
            }

            if (opcodeStr == "br.s" || opcodeStr == "br")
            {
                OperationFactory.AlwaysBranch(offset);
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
                OperationFactory.Add();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Sub)
            {
                OperationFactory.Sub();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Div)
            {
                OperationFactory.Div();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Mul)
            {
                OperationFactory.Mul();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Rem)
            {
                OperationFactory.Rem();
                return true;
            }


            if (opcodeStr == OpcodeOperatorNames.And)
            {
                OperationFactory.And();
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Or)
            {
                OperationFactory.Or();
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Xor)
            {
                OperationFactory.Xor();
                return true;
            }

            #region Unary operators

            if (opcodeStr == OpcodeOperatorNames.Not)
            {
                OperationFactory.Not();
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Neg)
            {
                OperationFactory.Neg();
                return true;
            }

            #endregion

            #region Compare operators

            if (opcodeStr == "cgt")
            {
                OperationFactory.Cgt();
                return true;
            }

            if (opcodeStr == "ceq")
            {
                OperationFactory.Ceq();
                return true;
            }
            if (opcodeStr == "clt")
            {
                OperationFactory.Clt();
                return true;
            }

            #endregion

            #endregion

            return false;
        }
    }
}