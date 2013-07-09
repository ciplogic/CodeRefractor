using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeRefractor.Compiler.Backend;
using CodeRefractor.Compiler.MiddleEnd;
using CodeRefractor.Compiler.Optimizations;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.Compiler.Shared;
using CodeRefractor.Compiler.Util;
using Mono.Reflection;

namespace CodeRefractor.Compiler.FrontEnd
{
    public class MethodInterpreter
    {
        internal readonly MethodBase Method;

        public MethodInterpreter(MethodBase method)
        {
            Method = method;
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}(...);", Method.DeclaringType.ToCppMangling(), Method.Name);
        }

        private readonly MetaMidRepresentation _midRepresentation = new MetaMidRepresentation();
        readonly CppMethodCodeWriter _methodCodeWriter = new CppMethodCodeWriter();
        private HashSet<int> _hashedLabels;

        public void Process()
        {
            var instructions = MethodBodyReader.GetInstructions(Method);
            _midRepresentation.Method = Method;
            var evaluator = new EvaluatorStack();
            foreach (var instruction in instructions)
            {

                var opcodeStr = instruction.OpCode.ToString();
                int offset = 0;
                if (instruction.Operand is Instruction)
                {
                    offset = ((Instruction)(instruction.Operand)).Offset;
                }
                if (_hashedLabels.Contains(instruction.Offset))
                {
                    _midRepresentation.SetLabel(instruction.Offset);
                }
                if (opcodeStr == "nop")
                    continue;
                
                if (HandleStores(opcodeStr, instruction, evaluator)) 
                    continue;
                if (HandleLoads(opcodeStr, instruction, evaluator)) 
                    continue;
                if (HandleOperators(opcodeStr, evaluator)) 
                    continue;
                if (opcodeStr.StartsWith("call"))
                {
                    _midRepresentation.Call(instruction.Operand, evaluator);
                    continue;
                }

                if (HandleBranching(opcodeStr, offset, evaluator)) 
                    continue;

                if (opcodeStr == "ret")
                {
                    var isVoid = _midRepresentation.Method.GetReturnType().IsVoid();

                    _midRepresentation.Return(isVoid,evaluator);
                    continue;
                }
                
                if(opcodeStr=="conv.i4")
                {
                    _midRepresentation.ConvI4(evaluator);
                    continue;
                }
                if (opcodeStr == "conv.r8")
                {
                    _midRepresentation.ConvR8(evaluator);
                    continue;
                }
                if(opcodeStr=="dup")
                {
                    _midRepresentation.Dup(evaluator);
                    continue;
                }

                if (opcodeStr == "newobj")
                {
                    var consInfo = (ConstructorInfo)instruction.Operand;
                    _midRepresentation.NewObject(consInfo, evaluator);
                    continue;
                }


                if(opcodeStr=="newarr")
                {
                    _midRepresentation.NewArray(evaluator, (Type)instruction.Operand);
                    continue;
                }
                if (opcodeStr == "stelem.ref")
                {
                    _midRepresentation.SetArrayElementValue(evaluator);
                    continue;
                }
                if (opcodeStr == "stelem")
                {
                    _midRepresentation.SetElement(evaluator, (Type) instruction.Operand);
                    continue;
                }
                throw new InvalidOperationException(string.Format("Unknown instruction: {0}", instruction));
            }

        }

        private bool HandleStores(string opcodeStr, Instruction instruction, EvaluatorStack evaluator)
        {
            if (opcodeStr == "stloc.s" || opcodeStr == "stloc")
            {
                _midRepresentation.CopyStackIntoLocalVariable(instruction.GetIntOperand(), evaluator);
                return true;
            }
            if (opcodeStr.StartsWith("stloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "stloc.".Length).ToInt();
                _midRepresentation.CopyStackIntoLocalVariable(pushedIntValue, evaluator);
                return true;
            }

            if (opcodeStr == "stfld")
            {
                var fieldInfo = (FieldInfo) instruction.Operand;
                _midRepresentation.StoreField(fieldInfo, evaluator);
                return true;
            }
            return false;
        }

        private bool HandleLoads(string opcodeStr, Instruction instruction, EvaluatorStack evaluator)
        {
            if (opcodeStr == "ldelem.ref")
            {
                _midRepresentation.LoadReferenceInArray(evaluator);
                return true;
            }
                
            if (opcodeStr == "ldc.i4.s" || opcodeStr == "ldc.i4")
            {
                _midRepresentation.PushInt4(instruction.GetIntOperand(), evaluator);
                return true;
            }
            if (opcodeStr.StartsWith("ldc.i4."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldc.i4.".Length).ToInt();
                _midRepresentation.PushInt4(pushedIntValue, evaluator);
                return true;
            }
            if (opcodeStr == "ldloc" || opcodeStr == "ldloc.s")
            {
                _midRepresentation.CopyLocalVariableIntoStack(instruction.GetIntOperand(), evaluator);
                return true;
            }

            if (opcodeStr.StartsWith("ldloc."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldloc.".Length).ToInt();
                _midRepresentation.CopyLocalVariableIntoStack(pushedIntValue, evaluator);
                return true;
            }

            if (opcodeStr == "ldstr")
            {
                _midRepresentation.PushString((string) instruction.Operand, evaluator);
                return true;
            }
            if (opcodeStr == "ldc.r8")
            {
                _midRepresentation.PushDouble((double) instruction.Operand, evaluator);
                return true;
            }
            if (opcodeStr == "ldc.r4")
            {
                _midRepresentation.PushDouble((float) instruction.Operand, evaluator);
                return true;
            }
            if (opcodeStr.StartsWith("ldarg."))
            {
                var pushedIntValue = opcodeStr.Remove(0, "ldarg.".Length).ToInt();
                _midRepresentation.LoadArgument(pushedIntValue, evaluator);
                return true;
            }
            if (opcodeStr == "ldfld")
            {
                var operand = (FieldInfo)instruction.Operand;

                _midRepresentation.LoadField(operand.Name, evaluator);
                return true;
            }
            if (opcodeStr == "ldlen")
            {
                _midRepresentation.LoadLength(evaluator);
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
                _midRepresentation.BranchIfTrue(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.BrFalseS
                || opcodeStr == OpcodeBranchNames.BrFalse
                || opcodeStr == OpcodeBranchNames.BrNullS
                || opcodeStr == OpcodeBranchNames.BrNull
                || opcodeStr == OpcodeBranchNames.BrZeroS
                || opcodeStr == OpcodeBranchNames.BrZero)
            {
                _midRepresentation.BranchIfFalse(offset, evaluator);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Beq || opcodeStr == OpcodeBranchNames.BeqS)
            {
                _midRepresentation.BranchIfEqual(offset, evaluator);
                return true;
            }

            if (opcodeStr == OpcodeBranchNames.Bge || opcodeStr == OpcodeBranchNames.BgeS)
            {
                _midRepresentation.BranchIfGreaterOrEqual(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bgt || opcodeStr == OpcodeBranchNames.BgtS)
            {
                _midRepresentation.BranchIfGreater(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Ble || opcodeStr == OpcodeBranchNames.BleS)
            {
                _midRepresentation.BranchIfLessOrEqual(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Blt || opcodeStr == OpcodeBranchNames.BltS)
            {
                _midRepresentation.BranchIfLess(offset, evaluator);
                return true;
            }
            if (opcodeStr == OpcodeBranchNames.Bne || opcodeStr == OpcodeBranchNames.BneS)
            {
                _midRepresentation.BranchIfNotEqual(offset, evaluator);
                return true;
            }

            if (opcodeStr == "br.s" || opcodeStr == "br")
            {
                _midRepresentation.AlwaysBranch(offset);
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
                _midRepresentation.Add(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Sub)
            {
                _midRepresentation.Sub(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Div)
            {
                _midRepresentation.Div(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Mul)
            {
                _midRepresentation.Mul(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Rem)
            {
                _midRepresentation.Rem(evaluator);
                return true;
            }


            if (opcodeStr == OpcodeOperatorNames.And)
            {
                _midRepresentation.And(evaluator);
                return true;
            }
            if (opcodeStr == OpcodeOperatorNames.Or)
            {
                _midRepresentation.Or(evaluator);
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Xor)
            {
                _midRepresentation.Xor(evaluator);
                return true;
            }

            #region Unary operators

            if (opcodeStr == OpcodeOperatorNames.Not)
            {
                _midRepresentation.Not(evaluator);
                return true;
            }

            if (opcodeStr == OpcodeOperatorNames.Neg)
            {
                _midRepresentation.Neg(evaluator);
                return true;
            }
            #endregion

            #region Compare operators

            if (opcodeStr == "cgt")
            {
                _midRepresentation.Cgt(evaluator);
                return true;
            }

            if (opcodeStr == "ceq")
            {
                _midRepresentation.Ceq(evaluator);
                return true;
            }
            if (opcodeStr == "clt")
            {
                _midRepresentation.Clt(evaluator);
                return true;
            }

            #endregion

            #endregion

            return false;
        }

        public string WriteMethodCode()
        {
            return _methodCodeWriter.WriteCode(_midRepresentation);
        }

        public string WriteHeaderMethod()
        {
            var sb= new StringBuilder();
            var methodName = Method.Name;
            var isStatic = Method.GetIsStatic();
            var arguments = CppMethodCodeWriter.GetArgumentsAsText(Method);
            var retType = Method.GetReturnType().ToCppMangling();
            if(Method is ConstructorInfo)
            {
                methodName = Method.DeclaringType.Name+"_ctor";
            }
            sb.AppendFormat(Method.GetIsStatic()
                ? "static {0} {1}({2}) ;"
                : "static {0} {1}({2}) ;",
                retType, methodName, arguments);
            sb.AppendLine();
            return sb.ToString();
        }

        public void SetLabels(IEnumerable<int> labelList)
        {
            _hashedLabels = labelList == null
                ? new HashSet<int>()
                : new HashSet<int>(labelList);
        }

        public void ApplyOptimizations(IEnumerable<OptimizationPass> optimizationPasses)
        {
            if (optimizationPasses == null)
                return;
            var optimizationsList = new List<OptimizationPass>(optimizationPasses);
            var didOptimize = true;
            while (didOptimize)
            {
                didOptimize = false;
                foreach (var optimizationPass in optimizationsList)
                {
                    didOptimize = optimizationPass.Optimize(_midRepresentation);
                    if(didOptimize)
                        break;
                }
            }
        }
    }
}