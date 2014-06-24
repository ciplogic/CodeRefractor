#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Shared;
using MsilReader;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public static class MetaLinker
    {
        public static List<MethodInterpreter> ComputeDependencies(MethodBase definition)
        {
            var resultDict = new Dictionary<string, MethodInterpreter>();
            var body = definition.GetMethodBody();
            if (body == null)
            {
                return new List<MethodInterpreter>();
            }
            var instructions = MethodBodyReader.GetInstructions(definition);

            foreach (var instruction in instructions)
            {
                var opcodeStr = instruction.OpCode.Value;
                switch (opcodeStr)
                {
                    case OpcodeIntValues.CallVirt:
                    case OpcodeIntValues.Call:
                    case OpcodeIntValues.CallInterface:
                    {
                        var operand = (MethodBase) instruction.Operand;
                        if (operand == null)
                            break;
                        AddMethodIfNecessary(operand, resultDict);
                        break;
                    }
                    case OpcodeIntValues.NewObj:
                    {
                        var operand = (ConstructorInfo) instruction.Operand;
                        if (operand == null)
                            break;
                        AddMethodIfNecessary(operand, resultDict);
                        break;
                    }
                }
            }
            return resultDict.Values.ToList();
        }


        private static void AddMethodIfNecessary(MethodBase methodBase, Dictionary<string, MethodInterpreter> resultDict)
        {
            if (MetaMidRepresentationOperationFactory.HandleRuntimeHelpersMethod(methodBase))
                return;
            var interpreter = methodBase.Register();
            resultDict[interpreter.ToString()] = interpreter;

            var declaringType = methodBase.DeclaringType;
            var isInGlobalAssemblyCache = declaringType.Assembly.GlobalAssemblyCache;
            if (isInGlobalAssemblyCache)
                return; //doesn't run on global assembly cache methods
            ComputeDependencies(methodBase);
        }


        public static void Interpret(MethodInterpreter methodInterpreter, CrRuntimeLibrary crRuntime)
        {
            if (methodInterpreter.Kind != MethodKind.Default)
                return;
            if (methodInterpreter.Interpreted)
                return;
            methodInterpreter.Process(crRuntime);
        }
    }
}