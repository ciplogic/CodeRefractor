#region Uses

using System.Collections.Generic;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;

#endregion

namespace CodeRefractor.Backend.ProgramWideOptimizations.ConstParameters
{
    class ConstantParametersData
    {
        public enum ConstValueKind
        {
            Unused,
            AssignedConstant,
            NonConstant
        }

        public const string Name = "ConstantParametersData";
        public Dictionary<LocalVariable, ConstValueKind> ConstKinds = new Dictionary<LocalVariable, ConstValueKind>();
        public Dictionary<LocalVariable, ConstValue> ConstValues = new Dictionary<LocalVariable, ConstValue>();

        public static ConstantParametersData GetInterpreterData(CilMethodInterpreter interpreter)
        {
            object resultObj;
            var auxiliaryObjects = interpreter.MidRepresentation.AuxiliaryObjects;
            if (auxiliaryObjects.TryGetValue(Name, out resultObj))
                return (ConstantParametersData) resultObj;
            var result = new ConstantParametersData();
            auxiliaryObjects[Name] = result;
            return result;
        }

        public void Reset()
        {
            ConstKinds.Clear();
            ConstValues.Clear();
        }

        public bool UpdateTable(CallMethodStatic callMethodStatic)
        {
            var interpreter = callMethodStatic.Interpreter;
            var arguments = interpreter.AnalyzeProperties.Arguments.ToArray();
            var result = false;
            var parameters = callMethodStatic.Parameters;
            for (var index = 0; index < parameters.Count; index++)
            {
                var argumentVar = arguments[index];
                var param = parameters[index];
                var constValue = param as ConstValue;
                if (constValue == null)
                {
                    ConstValueKind constKind;
                    result = !ConstKinds.TryGetValue(argumentVar, out constKind)
                             || constKind == ConstValueKind.NonConstant;
                    if (!result)
                        continue;
                    ConstKinds[argumentVar] = ConstValueKind.NonConstant;
                    continue;
                }
                ConstValue storedConst;
                if (!ConstValues.TryGetValue(argumentVar, out storedConst))
                {
                    ConstValues[argumentVar] = constValue;
                    ConstValueKind constKind;
                    result = !ConstKinds.TryGetValue(argumentVar, out constKind)
                             || constKind == ConstValueKind.AssignedConstant;
                    if (!result)
                        continue;
                    ConstKinds[argumentVar] = ConstValueKind.AssignedConstant;
                    continue;
                }
                if (!storedConst.ValueEquals(constValue))
                {
                    ConstValueKind constKind;
                    result = !ConstKinds.TryGetValue(argumentVar, out constKind)
                             || constKind == ConstValueKind.NonConstant;
                    if (!result)
                        continue;
                    ConstKinds[argumentVar] = ConstValueKind.NonConstant;
                }
            }
            return result;
        }
    }
}