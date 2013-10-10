using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    class AnalyzeParametersAreEscaping : ResultingGlobalOptimizationPass
    {
        private const string EscapeName = "NonEscapingArgs";
        HashSet<LocalVariable> argumentList = new HashSet<LocalVariable>();
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (intermediateCode.GetInterpreter().Kind != MethodKind.Default)
                return;

            var operations = intermediateCode.LocalOperations;

            var argEscaping = ComputeEscapingArgList(intermediateCode, operations);
            var escaping = ComputeArgsEscaping(operations, argEscaping);
            if (argEscaping.Count==0) return;
            intermediateCode.SetAdditionalValue(EscapeName, escaping);
        }

        private HashSet<LocalVariable> ComputeEscapingArgList(MetaMidRepresentation intermediateCode, List<LocalOperation> operations)
        {
            argumentList.Clear();
            argumentList.AddRange(
                intermediateCode.Vars.Arguments
                    .Where(varId => !varId.ComputedType().IsPrimitive)
                );
            if (argumentList.Count == 0)
                return argumentList;
            foreach (var op in operations)
            {
                var usages = op.GetUsages();
                foreach (var localVariable in usages.Where(argumentList.Contains))
                {
                    InFunctionLoweringVars.RemoveCandidatesIfEscapes(localVariable, argumentList, op);
                }
            }
            return argumentList;
        }

        public static Dictionary<int, bool> ComputeArgsEscaping(List<LocalOperation> operations, HashSet<LocalVariable> argEscaping)
        {
            var escaping = new Dictionary<int, bool>();
            
            foreach (var op in operations)
            {
                switch (op.Kind)
                {
                    case OperationKind.Call:
                        var methodData = (MethodData) op.Value;
                        var otherMethodData = GetEscapingParameterData(methodData);
                        if (otherMethodData == null)
                            break;
                        foreach (var parameter in methodData.Parameters)
                        {
                            var argCall = parameter as ArgumentVariable;
                            if (argCall == null)
                                continue;
                            if (!argCall.ComputedType().IsClass)
                                continue;

                            if (otherMethodData.ContainsKey(argCall.Id)
                                ||!argEscaping.Contains(argCall))
                            {
                                escaping[argCall.Id] = true;
                            }
                        }
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
