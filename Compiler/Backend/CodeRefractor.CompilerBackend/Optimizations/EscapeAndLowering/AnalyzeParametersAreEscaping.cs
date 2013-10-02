using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    class AnalyzeParametersAreEscaping : ResultingGlobalOptimizationPass
    {
        private const string EscapeName = "NonEscapingArgs";
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (intermediateCode.GetInterpreter().Kind != MethodKind.Default)
                return;

            var operations = intermediateCode.LocalOperations;
            
            var escaping = ComputeArgsEscaping(operations);

            intermediateCode.SetAdditionalValue(EscapeName, escaping);
        }

        public static Dictionary<int, bool> ComputeArgsEscaping(List<LocalOperation> operations)
        {
            var escaping = new Dictionary<int, bool>();
            foreach (var op in operations)
            {
                switch (op.Kind)
                {
                    case OperationKind.Assignment:
                        var right = op.GetAssignment().Right as LocalVariable;
                        if (right == null || right.Kind != VariableKind.Argument) continue;
                        escaping[right.Id] = true;
                        break;

                    case OperationKind.Call:
                        var methodData = (MethodData) op.Value;
                        var otherMethodData = GetEscapingParameterData(methodData);
                        if (otherMethodData == null)
                            break;
                        for (var i = 0; i < methodData.Parameters.Count; i++)
                        {
                            var parameter = methodData.Parameters[i];
                            var argCall = parameter as ArgumentVariable;
                            if (argCall == null)
                                continue;
                            if (!argCall.ComputedType().IsClass)
                                continue;
                            if (otherMethodData.ContainsKey(argCall.Id))
                            {
                                escaping[argCall.Id] = true;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            return escaping;
        }

        public static Dictionary<int, bool> GetEscapingParameterData(MethodData methodData)
        {
            var info = methodData.Info;
            return EscapingParameterData(info);
        }

        public static Dictionary<int, bool> EscapingParameterData(MethodBase info)
        {
            var calledMethod = info.GetMethod();
            var otherMethodData = (Dictionary<int, bool>) calledMethod.GetAdditionalProperty(EscapeName);
            return otherMethodData;
        }
    }
}
