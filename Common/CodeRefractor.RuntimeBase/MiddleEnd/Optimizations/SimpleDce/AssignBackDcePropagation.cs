#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.SimpleDce
{
    /// <summary>
    ///     This optimization in case of two assignments of the form:
    ///     > var1 = identifier
    ///     > var2 = var1
    ///     will transform the code to be > var2 = identifier
    /// </summary>
	[Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    internal class DceNewObjectOrArray : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var dictionary = new Dictionary<LocalVariable, int>();
            var useDef = interpreter.MidRepresentation.UseDef;
            var localOperations = useDef.GetLocalOperations();
            for (var i = 0; i < localOperations.Length; i++)
            {
                var op = localOperations[i];

                var usages = useDef.GetUsages(i);
                foreach (var usage in usages)
                {
                    if (dictionary.ContainsKey(usage))
                        dictionary[usage] = -1;
                }

                var definition = op.GetDefinition();
                if (definition == null)
                    continue;
                if (dictionary.ContainsKey(definition))
                    dictionary[definition] = -1;
                if (op.Kind != OperationKind.NewObject)
                    continue;
                dictionary[definition] = i;
            }
            var toDelete = dictionary.Values.Where(
                val => val != -1
                ).ToArray();
            if (toDelete.Length == 0)
                return;
            interpreter.MidRepresentation.DeleteInstructions(toDelete);
            Result = true;
        }
    }
}