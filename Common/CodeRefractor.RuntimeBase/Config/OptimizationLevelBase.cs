#region Uses

using System;
using System.Collections.Generic;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.Config
{
    public abstract class OptimizationLevelBase
    {
        public static OptimizationLevelBase Instance;

        public static Dictionary<OptimizationKind, List<OptimizationPassBase>> SortedOptimizations
            = new Dictionary<OptimizationKind, List<OptimizationPassBase>>();

        public static List<OptimizationPassBase> OptimizationPasses = new List<OptimizationPassBase>();
        private static int _optimizerLevel;

        public OptimizationLevelBase()
        {
            EnabledCategories = new HashSet<string>();
        }

        public HashSet<string> EnabledCategories { get; }

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

        public virtual List<OptimizationPassBase> BuildOptimizationPasses0()
        {
            throw new NotImplementedException();
        }

        public virtual List<OptimizationPassBase> BuildOptimizationPasses3()
        {
            throw new NotImplementedException();
        }

        public virtual List<OptimizationPassBase> BuildOptimizationPasses2()
        {
            throw new NotImplementedException();
        }

        public virtual List<OptimizationPassBase> BuildOptimizationPasses1()
        {
            throw new NotImplementedException();
        }

        public static void UpdateOptimizationsFromCategories(List<OptimizationPassBase> optimizationPasses)
        {
            var categories = OptimizationCategories.Instance;
            optimizationPasses.Clear();
            categories.BuildRelationsByReflection();
            var closure = categories.Closure(Instance.EnabledCategories);
            optimizationPasses.AddRange(closure);
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
                List<OptimizationPassBase> list;
                if (!SortedOptimizations.TryGetValue(optimizationPass.Kind, out list))
                {
                    list = new List<OptimizationPassBase>();
                    SortedOptimizations[optimizationPass.Kind] = list;
                }
                list.Add(optimizationPass);
            }
        }
    }
}