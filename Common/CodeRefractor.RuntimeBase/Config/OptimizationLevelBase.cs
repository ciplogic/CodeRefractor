#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.MiddleEnd.Optimizations.Common;
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
            optimizationPasses.Clear();
            categories.BuildRelationsByReflection();
            IEnumerable<ResultingOptimizationPass> closure = categories.Closure(Instance.EnabledCategories);
            optimizationPasses.AddRange(closure);
        }

        public static OptimizationLevelBase Instance;


        public static Dictionary<OptimizationKind, List<ResultingOptimizationPass>> SortedOptimizations
            = new Dictionary<OptimizationKind, List<ResultingOptimizationPass>>();

        public static List<ResultingOptimizationPass> OptimizationPasses = new List<ResultingOptimizationPass>();
        private static int _optimizerLevel;

        public static int OptimizerLevel
        {
            get { return _optimizerLevel; }
            set
            {
                _optimizerLevel = value;
                var optimizationList = Instance.BuildOptimizationPasses0();
                if (_optimizerLevel >= 1)
                {
                    optimizationList.AddRange(Instance.BuildOptimizationPasses1());
                }
                if (_optimizerLevel >= 2)
                {
                    optimizationList.AddRange(Instance.BuildOptimizationPasses2());
                }
                OptimizationPasses = optimizationList;
                UpdateOptimizationsFromCategories(OptimizationPasses);

                SortOptimizations();
            }
        }

        public static void ClearOptimizations()
        {
            OptimizationPasses.Clear();
        }

        public static void SortOptimizations()
        {
            SortedOptimizations.Clear();
            foreach (var optimizationPass in OptimizationPasses)
            {
                List<ResultingOptimizationPass> list;
                if (!SortedOptimizations.TryGetValue(optimizationPass.Kind, out list))
                {
                    list = new List<ResultingOptimizationPass>();
                    SortedOptimizations[optimizationPass.Kind] = list;
                }
                list.Add(optimizationPass);
            }
        }
    }
}