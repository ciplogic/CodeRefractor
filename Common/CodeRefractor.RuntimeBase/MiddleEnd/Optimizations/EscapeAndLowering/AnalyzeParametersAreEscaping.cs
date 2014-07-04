#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.EscapeAndLowering
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    internal class AnalyzeParametersAreEscaping : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            if (interpreter.Kind != MethodKind.Default)
                return;

            var originalSnapshot = interpreter.Method.BuildEscapingBools(Closure);

            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            if (ComputeEscapeTable(interpreter, localOperations, interpreter)) return;

            var finalSnapshot = interpreter.Method.BuildEscapingBools(Closure);
            CheckForChanges(finalSnapshot, originalSnapshot);
        }

        private static bool ComputeEscapeTable(MethodInterpreter intermediateCode, LocalOperation[] operations,
            MethodInterpreter interpreter)
        {
            var argEscaping = ComputeEscapingArgList(intermediateCode.MidRepresentation, operations, Runtime);
            var escaping = ComputeArgsEscaping(operations, argEscaping);
            if (argEscaping.Count == 0) return true;
            intermediateCode.MidRepresentation.SetAdditionalValue(LinkerUtils.EscapeName, escaping);
            var escapingBools = intermediateCode.Method.BuildEscapingBools(Closure);
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
                    InFunctionLoweringVars.RemoveCandidatesIfEscapes(localVariable, argumentList, op);
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
                        var methodData = (CallMethodStatic) op;
                        var otherMethodData = GetEscapingParameterData(methodData);
                        if (otherMethodData == null)
                            break;
                        foreach (var parameter in methodData.Parameters)
                        {
                            var argCall = parameter as LocalVariable;
                            if (argCall == null || argCall.Kind!=VariableKind.Argument)
                                continue;
                            if (!(argCall.ComputedType().ClrType.IsClass||argCall.ComputedType().ClrType.IsInterface))
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

        public static Dictionary<int, bool> GetEscapingParameterData(CallMethodStatic callMethodStatic)
        {
            var info = callMethodStatic.Info;
            return info.EscapingParameterData(Closure);
        }
    }
}