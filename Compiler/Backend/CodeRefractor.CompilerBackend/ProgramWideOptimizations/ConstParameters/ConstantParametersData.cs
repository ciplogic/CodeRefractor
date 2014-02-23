using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations.ConstParameters
{
    class ConstantParametersData
    {
        public const string Name = "ConstantParametersData";
        public Dictionary<LocalVariable, ConstValue>ConstValues = new Dictionary<LocalVariable, ConstValue>();
        public Dictionary<LocalVariable, ConstValueKind> ConstKinds = new Dictionary<LocalVariable, ConstValueKind>(); 
        public enum ConstValueKind
        {
            Unused,
            AssignedConstant,
            NonConstant
        }

        public static ConstantParametersData GetInterpreterData(MethodInterpreter interpreter)
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

        public bool UpdateTable(MethodData methodData)
        {
            var interpreter = methodData.Interpreter;
            var arguments = interpreter.MidRepresentation.Vars.Arguments
                .ToArray();
            var result = false;
            var parameters = methodData.Parameters;
            for (int index = 0; index < parameters.Count; index++)
            {
                var argumentVar = arguments[index];
                var param = parameters[index];
                var constValue = param as ConstValue;
                if (constValue == null)
                {
                    ConstValueKind constKind;
                    result = !ConstKinds.TryGetValue(argumentVar, out constKind)
                        || constKind==ConstValueKind.NonConstant;
                    
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
                    ConstKinds[argumentVar] = ConstValueKind.AssignedConstant;
                    continue;
                }
                if (!storedConst.ValueEquals(constValue))
                {

                    ConstValueKind constKind;
                    result = !ConstKinds.TryGetValue(argumentVar, out constKind)
                             || constKind == ConstValueKind.NonConstant;
                    ConstKinds[argumentVar] = ConstValueKind.NonConstant;
                }
            }
            return result;
        }
    }
}