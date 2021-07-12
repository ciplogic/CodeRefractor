﻿#region Uses

using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.UseDefs;

#endregion

namespace CodeRefractor.Backend.ProgramWideOptimizations.ConstParameters
{
    public class CallToFunctionsWithSameConstant : ProgramOptimizationBase
    {
        public override bool Optimize(ClosureEntities closure)
        {
            var methodInterpreters = closure.MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.CilInstructions)
                .Cast<CilMethodInterpreter>()
                .ToList();
            var updateHappen = false;
            foreach (var interpreter in methodInterpreters)
            {
                updateHappen |= HandleInterpreterInstructions(interpreter);
            }
            if (!updateHappen)
                return false;
            var parametersDatas = methodInterpreters
                .Select(ConstantParametersData.GetInterpreterData)
                .ToList();
            var result = false;
            for (var index = 0; index < methodInterpreters.Count; index++)
            {
                var interpreter = methodInterpreters[index];
                var parametersData = parametersDatas[index];
                if (!parametersData.ConstKinds.ContainsValue(ConstantParametersData.ConstValueKind.AssignedConstant))
                    continue;
                result |= ApplyChangesOnMethod(parametersData, interpreter);
            }
            return result;
        }

        bool ApplyChangesOnMethod(ConstantParametersData parametersData, CilMethodInterpreter interpreter)
        {
            var result = false;
            foreach (var constKind in parametersData.ConstKinds)
            {
                if (constKind.Value != ConstantParametersData.ConstValueKind.AssignedConstant)
                    continue;
                var assignedConstant = parametersData.ConstValues[constKind.Key];
                var cilInterpreter = interpreter;
                cilInterpreter.SwitchAllUsagesWithDefinition(constKind.Key, assignedConstant);
                interpreter.AnalyzeProperties.SetVariableData(constKind.Key, EscapingMode.Unused);
                result = true;
            }
            return result;
        }

        static bool HandleInterpreterInstructions(CilMethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.Call).ToList();
            var allOps = useDef.GetLocalOperations();
            var updatedHappen = false;
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (CallMethodStatic)op;
                if (methodData.Interpreter.Kind != MethodKind.CilInstructions)
                    continue;
                var callingInterpreter = (CilMethodInterpreter)methodData.Interpreter;
                var interpreterData = ConstantParametersData.GetInterpreterData(callingInterpreter);
                updatedHappen |= interpreterData.UpdateTable(methodData);
            }
            return updatedHappen;
        }
    }
}