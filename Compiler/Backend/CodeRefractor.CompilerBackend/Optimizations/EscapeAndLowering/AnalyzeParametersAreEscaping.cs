using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    class AnalyzeParametersAreEscaping : ResultingGlobalOptimizationPass
    {
        public const string EscapeName = "NonEscapingArgs";
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            if (methodInterpreter.Kind != MethodKind.Default)
                return;

            var originalSnapshot = CppFullFileMethodWriter.BuildEscapingBools(methodInterpreter.Method);

            var localOperations = methodInterpreter.MidRepresentation.UseDef.GetLocalOperations();
            if (ComputeEscapeTable(methodInterpreter, localOperations)) return;

            var finalSnapshot= CppFullFileMethodWriter.BuildEscapingBools(methodInterpreter.Method);
            CheckForChanges(finalSnapshot, originalSnapshot);
        }

        private static bool ComputeEscapeTable(MethodInterpreter intermediateCode, LocalOperation[] operations)
        {
            var argEscaping = ComputeEscapingArgList(intermediateCode.MidRepresentation, operations);
            var escaping = ComputeArgsEscaping(operations, argEscaping);
            if (argEscaping.Count == 0) return true;
            intermediateCode.MidRepresentation.SetAdditionalValue(EscapeName, escaping);
            var escapingBools = CppFullFileMethodWriter.BuildEscapingBools(intermediateCode.Method);
            var variables = intermediateCode.MidRepresentation.Vars;
            foreach (var variable in variables.Arguments)
            {
                VariableData variableData = variables.GetVariableData(variable.Name);
                if (!escapingBools[variable.Id])
                    variableData.Escaping = EscapingMode.Pointer;
            }
            return false;
        }

        private void CheckForChanges(bool[] finalSnapshot, bool[] originalSnapshot)
        {
            Result = false;
            for (int index = 0; index < finalSnapshot.Length; index++)
            {
                var orig = originalSnapshot[index];
                var final = finalSnapshot[index];
                if (orig == final) continue;
                Result = true;
                return;
            }
        }

        public static HashSet<LocalVariable> ComputeEscapingArgList(MetaMidRepresentation intermediateCode, LocalOperation[] operations)
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
            for (int index = 0; index < operations.Length; index++)
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

        public static Dictionary<int, bool> ComputeArgsEscaping(LocalOperation[] operations, HashSet<LocalVariable> argEscaping)
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
            var interpreter = info.GetInterpreter();
            if (interpreter == null)
                return null;
            var calledMethod = interpreter.MidRepresentation;
            var otherMethodData = (Dictionary<int, bool>) calledMethod.GetAdditionalProperty(EscapeName);
            if (otherMethodData == null)
            {
                //var operations = interpreter.MidRepresentation.LocalOperations.ToArray();
                //ComputeEscapeTable(interpreter, operations);
                //otherMethodData = (Dictionary<int, bool>)calledMethod.GetAdditionalProperty(EscapeName);
                return null;
            }
            return otherMethodData;
        }
    }
}
