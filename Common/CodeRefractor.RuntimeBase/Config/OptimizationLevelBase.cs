#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Config
{
    public abstract class OptimizationLevelBase
    {
		public HashSet<string> EnabledCategories {get; private set;}

		public OptimizationLevelBase()
		{
			EnabledCategories = new HashSet<string>();
		}

        public virtual List<ResultingOptimizationPass> BuildOptimizationPasses0()
        {
            throw new NotImplementedException();
        }

        public virtual List<ResultingOptimizationPass> BuildOptimizationPasses3()
        {
            throw new NotImplementedException();
        }

        public virtual List<ResultingOptimizationPass> BuildOptimizationPasses2()
        {
            throw new NotImplementedException();
        }

        public virtual List<ResultingOptimizationPass> BuildOptimizationPasses1()
        {
            throw new NotImplementedException();
        }

        public static void UpdateOptimizationsFromCategories(List<ResultingOptimizationPass> optimizationPasses)
        {
            var categories = OptimizationCategories.Instance;
            categories.BuildRelationsByReflection();
            IEnumerable<ResultingOptimizationPass> closure = categories.Closure(Instance.EnabledCategories);
            optimizationPasses.AddRange(closure);
        }

        public static OptimizationLevelBase Instance;
    }
}