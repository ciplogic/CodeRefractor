#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.Shared;
using MsilReader;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MethodInterpreter
    {
        public MethodBase Method { get; set; }
        public TypeDescription DeclaringType { get; set; }

        public List<Type> ClassSpecializationType = new List<Type>();
        public List<Type> MethodSpecializationType = new List<Type>();
        public MethodKind Kind { get; set; }

        public readonly AnalyzeProperties AnalyzeProperties = new AnalyzeProperties();

        public MetaMidRepresentation MidRepresentation = new MetaMidRepresentation();
        public readonly CppRepresentation CppRepresentation = new CppRepresentation();
        public readonly MethodDescription Description = new MethodDescription();

        public bool Interpreted { get; set; }

        public void SetDeclaringType(MethodBase method)
        {
            DeclaringType = new TypeDescription(method.DeclaringType);
            if (!DeclaringType.ClrType.IsGenericType)
            {
                Method = method;
                return;
            }
            Method = MethodInterpreterUtils.GetGenericMethod(method, DeclaringType);
            if (Method == null)
            {
                Method = DeclaringType.ClrType.GetMethod(method.Name);
            }
        }

        public MethodInterpreter(MethodBase method)
        {
            SetDeclaringType(method);
            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                AnalyzeProperties.IsPure = true;
            MidRepresentation.Vars.SetupArguments(Method);
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
                MethodSpecializationType = MethodSpecializationType
            };
            return result;
        }

        public bool IsGenericDeclaringType()
        {
            return Method.DeclaringType.IsGenericType;
        }

        public override string ToString()
        {
            return Method.GenerateKey();
        }


        public static HashSet<int> ComputeLabels(MethodBase definition)
        {
            var labels = new HashSet<int>();
            var body = definition.GetMethodBody();
            if (body == null)
                return labels;
            var instructions = MethodBodyReader.GetInstructions(definition);

            foreach (var instruction in instructions)
            {
                var opcodeStr = instruction.OpCode.Value;
                switch (opcodeStr)
                {
                    case OpcodeIntValues.Beq:
                    case OpcodeIntValues.BeqS:
                    case OpcodeIntValues.Bne:
                    case OpcodeIntValues.BneS:
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
                        var offset = ((Instruction) instruction.Operand).Offset;
                        AddLabelIfDoesntExist(offset, labels);
                    }
                        break;
                    case OpcodeIntValues.Switch:
                    {
                        var offsets = (Instruction[]) instruction.Operand;
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

        public void Process(CrRuntimeLibrary crRuntime)
        {
            if (Kind != MethodKind.Default)
                return;
            if (Interpreted)
                return;
            if (HandlePlatformInvokeMethod(Method))
                return;
            if (Method.GetMethodBody() == null)
                return;
            var instructions = MethodBodyReader.GetInstructions(Method);

            var labelList = ComputeLabels(Method);
            MidRepresentation.Method = Method;


            MidRepresentation.Vars.SetupLocalVariables(Method);
            var evaluator = new EvaluatorStack();
            var operationFactory = new MetaMidRepresentationOperationFactory(MidRepresentation, evaluator);

            for (var index = 0; index < instructions.Length; index++)
            {
                var instruction = instructions[index];
                EvaluateInstuction(instruction, operationFactory, labelList, crRuntime);
            }
            //Ensure.IsTrue(evaluator.Count == 0, "Stack not empty!");
            AnalyzeProperties.Setup(MidRepresentation.Vars.Arguments, MidRepresentation.Vars.VirtRegs,
                MidRepresentation.Vars.LocalVars);
            Interpreted = true;
        }

        private void EvaluateInstuction(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory,
            HashSet<int> labelList, CrRuntimeLibrary crRuntime)
        {
            var opcodeStr = instruction.OpCode.ToString();
            var offset = 0;
            if (instruction.Operand is Instruction)
            {
                offset = ((Instruction) (instruction.Operand)).Offset;
            }
            if (labelList.Contains(instruction.Offset))
            {
                operationFactory.SetLabel(instruction.Offset);
            }
            if (operationFactory.SkipInstruction(instruction.Offset))
                return;
            var opcodeValue = instruction.OpCode.Value;
            operationFactory.AddCommentInstruction(instruction.ToString());
            if (HandleCalls(instruction, operationFactory, opcodeValue, crRuntime))
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
            

            if (HandleExtraInstructions(instruction, operationFactory, opcodeStr)) return;
            throw new InvalidOperationException(String.Format("Unknown instruction: {0}", instruction));
        }

        private bool HandleExtraInstructions(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, string opcodeStr)
        {
            if (opcodeStr == "ret")
            {
                var isVoid = Method.GetReturnType().IsVoid();

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
                operationFactory.SetToken((FieldInfo) instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldftn")
            {
                operationFactory.LoadFunction((MethodInfo) instruction.Operand);
                return true;
            }
            if (opcodeStr == "switch")
            {
                operationFactory.Switch((Instruction[]) instruction.Operand);
                return true;
            }
            if (opcodeStr == "sizeof")
            {
                operationFactory.SizeOf((Type) instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldsfld")
            {
                operationFactory.LoadStaticField((FieldInfo) instruction.Operand);
                return true;
            }
            if (opcodeStr == "stsfld")
            {
                operationFactory.StoreStaticField((FieldInfo) instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldloca.s" || opcodeStr == "ldloca")
            {
                //TODO: load the address into evaluation stack
                var index = (LocalVariableInfo) instruction.Operand;

                operationFactory.LoadAddressIntoEvaluationStack(index);
                return true;
            }
            if (opcodeStr == "ldflda.s" || opcodeStr == "ldflda")
            {
                var fieldInfo = (FieldInfo) instruction.Operand;

                operationFactory.LoadFieldAddressIntoEvaluationStack(fieldInfo);
                return true;
            }

            if (opcodeStr == "ldelema")
            {
                operationFactory.LoadAddressOfArrayItemIntoStack((Type) instruction.Operand);
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
            return false;
        }

        private static bool HandleArrayOperations(Instruction instruction,
            MetaMidRepresentationOperationFactory operationFactory, string opcodeStr)
        {
            if (opcodeStr == "newarr")
            {
                operationFactory.NewArray((Type) instruction.Operand);
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
                var elemInfo = (Type) instruction.Operand;
                operationFactory.StoreElement(elemInfo);
                return true;
            }
            return false;
        }

        private static bool HandleCalls(Instruction instruction, MetaMidRepresentationOperationFactory operationFactory,
            short opcodeValue, CrRuntimeLibrary crRuntime)
        {
            switch (opcodeValue)
            {
                case OpcodeIntValues.Nop:
                    return true;
                case OpcodeIntValues.Call:
                    operationFactory.Call(instruction.Operand, crRuntime);
                    return true;
                case OpcodeIntValues.CallVirt:
                    operationFactory.CallVirtual(instruction.Operand, crRuntime);
                    return true;

                case OpcodeIntValues.CallInterface:
                    operationFactory.CallInterface(instruction.Operand, crRuntime);
                    return true;
                case OpcodeIntValues.NewObj:
                {
                    var consInfo = (ConstructorInfo) instruction.Operand;
                    operationFactory.NewObject(consInfo, crRuntime);
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

        private bool HandlePlatformInvokeMethod(MethodBase method)
        {
            var pinvokeAttribute = method.GetCustomAttribute<DllImportAttribute>();

            if (pinvokeAttribute == null)
                return false;
            Description.LibraryName = pinvokeAttribute.Value;
            Description.MethodName = method.Name;
            Description.CallingConvention = pinvokeAttribute.CallingConvention;
            Description.EntryPoint = pinvokeAttribute.EntryPoint;
            Kind = MethodKind.PlatformInvoke;
            return true;
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
                var parameter = (ParameterInfo) instruction.Operand;
                var pushedIntValue = parameter.Position;
                operationFactory.CopyStackIntoArgument(pushedIntValue);
                return true;
            }

            if (opcodeStr == "stfld")
            {
                var fieldInfo = (FieldInfo) instruction.Operand;
                operationFactory.StoreField(fieldInfo);
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
                operationFactory.LoadReferenceInArrayTyped((Type) instruction.Operand);
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
                operationFactory.PushString((string) instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldc.r8")
            {
                operationFactory.PushDouble((double) instruction.Operand);
                return true;
            }
            if (opcodeStr == "ldc.r4")
            {
                operationFactory.PushDouble((float) instruction.Operand);
                return true;
            }
            if (opcodeStr.StartsWith("ldarg."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldarg.".Length).ToInt();
                operationFactory.LoadArgument(pushedIntValue);
                return true;
            }
            if (opcodeStr == "ldfld")
            {
                var operand = (FieldInfo) instruction.Operand;

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
                operationFactory.CastClass((Type)instruction.Operand);
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

            if (opcodeStr == "cgt")
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

        public Dictionary<int, int> GetLabelTable()
        {
            return MidRepresentation.UseDef.GetLabelTable();
        }
    }
}