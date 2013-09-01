#region Usings

using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;
using Mono.Reflection;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public class MetaLinker
    {
        public readonly Dictionary<string, HashSet<int>> Labels = new Dictionary<string, HashSet<int>>();
        public MethodInfo MethodInfo;

        public void SetEntryPoint(MethodInfo entryMethod)
        {
            MethodInfo = entryMethod;
        }

        public void ComputeDependencies(MethodBase definition)
        {
            ComputeLabels(definition);
            AddClassIfNecessary(definition);
            var methodDefinitionKey = definition.ToString();
            GlobalMethodPool.Instance.MethodInfos[methodDefinitionKey] = definition;

            var body = definition.GetMethodBody();
            if (body == null)
                return;
            var instructions = MethodBodyReader.GetInstructions(definition);

            foreach (var instruction in instructions)
            {
                var opcodeStr = instruction.OpCode.Value;
                switch (opcodeStr)
                {
                    case ObcodeIntValues.CallVirt:
                    case ObcodeIntValues.Call:
                    case ObcodeIntValues.CallInterface:
                        {
                            var operand = (MethodBase) instruction.Operand;
                            if (operand == null)
                                break;
                            AddMethodIfNecessary(operand);
                            break;
                        }
                    case ObcodeIntValues.NewObj:
                        {
                            var operand = (ConstructorInfo) instruction.Operand;
                            if (operand == null)
                                break;
                            AddMethodIfNecessary(operand);
                            break;
                        }
                }
            }
        }

        public void ComputeLabels(MethodBase definition)
        {
            var methodDefinitionKey = definition.ToString();
            var body = definition.GetMethodBody();
            if (body == null)
                return;
            var instructions = MethodBodyReader.GetInstructions(definition);

            foreach (var instruction in instructions)
            {
                var opcodeStr = instruction.OpCode.Value;
                switch (opcodeStr)
                {
                    case ObcodeIntValues.Beq:
                    case ObcodeIntValues.BeqS:
                    case ObcodeIntValues.Bge:
                    case ObcodeIntValues.BgeS:
                    case ObcodeIntValues.Bgt:
                    case ObcodeIntValues.BgtS:
                    case ObcodeIntValues.BrTrueS:
                    case ObcodeIntValues.BrTrue:
                    case ObcodeIntValues.BrZero:
                    case ObcodeIntValues.BrZeroS:
                    case ObcodeIntValues.Blt:
                    case ObcodeIntValues.BltS:
                    case ObcodeIntValues.BrS:
                    case ObcodeIntValues.Br:
                        {
                            var offset = ((Instruction) instruction.Operand).Offset;
                            AddLabelIfDoesntExist(methodDefinitionKey, offset);
                        }
                        break;
                    case ObcodeIntValues.Switch:
                        {
                            var offsets = (Instruction[]) instruction.Operand;
                            foreach (var offset in offsets)
                            {
                                AddLabelIfDoesntExist(methodDefinitionKey, offset.Offset);
                            }
                        }
                        break;
                }
            }
        }

        private void AddLabelIfDoesntExist(string methodDefinitionKey, int offset)
        {
            HashSet<int> labelList;
            if (!Labels.TryGetValue(methodDefinitionKey, out labelList))
            {
                labelList = new HashSet<int>();
                Labels[methodDefinitionKey] = labelList;
            }
            labelList.Add(offset);
        }


        public void EvaluateMethods()
        {
            foreach (var methodBase in GlobalMethodPool.Instance.MethodInfos)
            {
                Interpret(methodBase);
            }
        }

        private void Interpret(KeyValuePair<string, MethodBase> method)
        {
            var interpreter = new MethodInterpreter(method.Value);
            var methodDefinitionKey = method.Value.ToString();

            var labelList = new HashSet<int>();
            Labels.TryGetValue(methodDefinitionKey, out labelList);
            interpreter.SetLabels(labelList);
            interpreter.Process();
            AddClassIfNecessary(interpreter.Method).Add(interpreter);
            GlobalMethodPool.Instance.Interpreters[method.Key] = interpreter;

            var typeData = (ClassTypeData) ProgramData.UpdateType(method.Value.DeclaringType);
            typeData.AddMethodInterpreter(interpreter);
        }

        private static ClassInterpreter AddClassIfNecessary(MethodBase operand)
        {
            var name = ClassInterpreter.GetClassName(operand.DeclaringType);
            ClassInterpreter result;
            if (GlobalMethodPool.Instance.Classes.TryGetValue(name, out result))
            {
                return result;
            }
            var declaringType = TypeData.GetTypeData(operand.DeclaringType);
            var classInfo = new ClassInterpreter {DeclaringType = declaringType};
            GlobalMethodPool.Instance.Classes[name] = classInfo;
            return classInfo;
        }

        private void AddMethodIfNecessary(MethodBase methodBase)
        {
            if (MetaMidRepresentationOperationFactory.HandleRuntimeHelpersMethod(methodBase))
                return;
            var methodDesc = methodBase.ToString();

            var methodRuntimeInfo = methodBase.GetMethodDescriptor();
            if (ProgramData.CrCrRuntimeLibrary.UseMethod(methodRuntimeInfo))
                return;
            if (GlobalMethodPool.Instance.MethodInfos.ContainsKey(methodDesc))
                return;
            var declaringType = methodBase.DeclaringType;
            var isInGlobalAssemblyCache = declaringType.Assembly.GlobalAssemblyCache;
            if (isInGlobalAssemblyCache)
                return; //doesn't run on global assembly cache methods
            GlobalMethodPool.Instance.MethodInfos[methodDesc] = methodBase;
            ComputeDependencies(methodBase);
        }
    }
}