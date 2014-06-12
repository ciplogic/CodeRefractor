#region Usings

using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering
{
    internal class ClearInFunctionUnusedArguments : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var arguments = midRepresentation.Vars.Arguments;
            if (arguments.Count == 0)
                return;
            var properties = interpreter.AnalyzeProperties;
            var argList = arguments
                .Select(a => (LocalVariable) a)
                .Where(argVar => properties.GetVariableData(argVar) != EscapingMode.Unused)
                .ToList();
            argList = UseDefDescription.ComputeUnusedArguments(argList, useDef);
            foreach (var variable in argList)
            {
                properties.SetVariableData(variable, EscapingMode.Unused);
                Result = true;
            }
        }
    }
}