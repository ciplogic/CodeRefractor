#region Usings

using System.Linq;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    internal class ClearInFunctionUnusedArguments : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
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