using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public static class MidRepresentationUtils
    {
        public static object GetAdditionalProperty(this MetaMidRepresentation intermediateCode, string itemName)
        {
            if (intermediateCode == null)
                return null;
            var additionalData = intermediateCode.AuxiliaryObjects;

            object itemValue;
            return !additionalData.TryGetValue(itemName, out itemValue) ? null : itemValue;
        }

        public static bool ReadAdditionalBool(this MetaMidRepresentation intermediateCode, string itemName)
        {
            var result = intermediateCode.GetAdditionalProperty(itemName);
            return result != null && (bool) result;
        }

        public static void SetAdditionalValue(this MetaMidRepresentation intermediateCode, string itemName,
            object valueToSet)
        {
            var additionalData = intermediateCode.AuxiliaryObjects;
            additionalData[itemName] = valueToSet;
        }


        public static bool[] GetUsedArguments(MethodInterpreter interpreter)
        {
            var argsCount = interpreter.Method.GetParameters().Length;
            if (interpreter.Method is ConstructorInfo || !interpreter.Method.IsStatic)
                argsCount++;
            var result = new bool[argsCount];
            var analyzeData = interpreter.AnalyzeProperties;
            var arguments = analyzeData.Arguments;
            for (var index = 0; index < arguments.Count; index++)
            {
                var argument = arguments[index];
                result[index] = analyzeData.GetVariableData(argument) != EscapingMode.Unused;
            }
            return result;
        }
    }
}