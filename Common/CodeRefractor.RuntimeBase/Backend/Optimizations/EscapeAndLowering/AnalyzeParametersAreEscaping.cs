#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Runtime;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering
{
    internal class AnalyzeParametersAreEscaping : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            if (interpreter.Kind != MethodKind.Default)
                return;

            var originalSnapshot = interpreter.Method.BuildEscapingBools(Runtime);

            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            if (ComputeEscapeTable(interpreter, localOperations, interpreter)) return;

            var finalSnapshot = interpreter.Method.BuildEscapingBools(Runtime);
            CheckForChanges(finalSnapshot, originalSnapshot);
        }

        private static bool ComputeEscapeTable(MethodInterpreter intermediateCode, LocalOperation[] operations,
            MethodInterpreter interpreter)
        {
            var argEscaping = ComputeEscapingArgList(intermediateCode.MidRepresentation, operations, Runtime);
            var escaping = ComputeArgsEscaping(operations, argEscaping);
            if (argEscaping.Count == 0) return true;
            intermediateCode.MidRepresentation.SetAdditionalValue(LinkerUtils.EscapeName, escaping);
            var escapingBools = intermediateCode.Method.BuildEscapingBools(Runtime);
            var variables = intermediateCode.MidRepresentation.Vars;
            foreach (var variable in variables.Arguments)
            {
                if (!escapingBools[variable.Id])
                {
                    var oldVariableData = interpreter.AnalyzeProperties.GetVariableData(variable);
                    if (oldVariableData == EscapingMode.Unused)
                        continue;
                    interpreter.AnalyzeProperties.SetVariableData(variable, EscapingMode.Pointer);
                }
            }
            return false;
        }

        private void CheckForChanges(bool[] finalSnapshot, bool[] originalSnapshot)
        {
            Result = false;
            for (var index = 0; index < finalSnapshot.Length; index++)
            {
                var orig = originalSnapshot[index];
                var final = finalSnapshot[index];
                if (orig == final) continue;
                Result = true;
                return;
            }
        }

        public static HashSet<LocalVariable> ComputeEscapingArgList(MetaMidRepresentation intermediateCode,
            LocalOperation[] operations, CrRuntimeLibrary crRuntime)
        {
            var argumentList = new HashSet<LocalVariable>();
            argumentList.Clear();
            argumentList.AddRange(
                intermediateCode.Vars.Arguments
                    .Where(varId => !varId.ComputedType().ClrType.IsPrimitive)
                );
            if (argumentList.Count == 0)
                return argumentList;
            var useDef = intermediateCode.UseDef;
            for (var index = 0; index < operations.Length; index++)
            {
                var op = operations[index];
                var usages = useDef.GetUsages(index);
                foreach (var localVariable in usages.Where(argumentList.Contains))
                {
                    InFunctionLoweringVars.RemoveCandidatesIfEscapes(localVariable, argumentList, op, crRuntime);
                }
            }
            return argumentList;
        }

        public static Dictionary<int, bool> ComputeArgsEscaping(LocalOperation[] operations,
            HashSet<LocalVariable> argEscaping)
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
                            if (!argCall.ComputedType().ClrType.IsClass)
                                continue;

                            if (otherMethodData.ContainsKey(argCall.Id)
                                || !argEscaping.Contains(argCall))
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
            return info.EscapingParameterData(Runtime);
        }
    }
}