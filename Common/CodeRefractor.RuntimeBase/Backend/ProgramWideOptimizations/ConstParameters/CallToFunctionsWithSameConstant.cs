#region Usings

using System.Linq;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations.ConstParameters
{
    public class CallToFunctionsWithSameConstant : ResultingProgramOptimizationBase
    {
        protected override void DoOptimize(ProgramClosure closure)
        {
            var methodInterpreters = closure.MethodClosure.Values
                .Where(m => m.Kind == MethodKind.Default)
                .ToList();
            var updateHappen = false;
            foreach (var interpreter in methodInterpreters)
            {
                updateHappen |= HandleInterpreterInstructions(interpreter);
            }
            if (!updateHappen)
                return;
            var parametersDatas = methodInterpreters
                .Select(ConstantParametersData.GetInterpreterData)
                .ToList();
            for (var index = 0; index < methodInterpreters.Count; index++)
            {
                var interpreter = methodInterpreters[index];
                var parametersData = parametersDatas[index];
                if (!parametersData.ConstKinds.ContainsValue(ConstantParametersData.ConstValueKind.AssignedConstant))
                    continue;
                foreach (var constKind in parametersData.ConstKinds)
                {
                    if (constKind.Value != ConstantParametersData.ConstValueKind.AssignedConstant)
                        continue;
                    var assignedConstant = parametersData.ConstValues[constKind.Key];
                    interpreter.SwitchAllUsagesWithDefinition(constKind.Key, assignedConstant);
                    interpreter.AnalyzeProperties.SetVariableData(constKind.Key, EscapingMode.Unused);
                    Result = true;
                }
            }
        }

        private static bool HandleInterpreterInstructions(MethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.Call).ToList();
            var allOps = useDef.GetLocalOperations();
            var updatedHappen = false;
            foreach (var callOp in calls)
            {
                var op = allOps[callOp];
                var methodData = (MethodData) op;
                var callingInterpreter = methodData.Interpreter;
                if (callingInterpreter.Kind != MethodKind.Default)
                    continue;
                var interpreterData = ConstantParametersData.GetInterpreterData(callingInterpreter);
                updatedHappen |= interpreterData.UpdateTable(methodData);
            }
            return updatedHappen;
        }
    }
}