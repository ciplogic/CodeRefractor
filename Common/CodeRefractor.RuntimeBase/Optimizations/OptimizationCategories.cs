#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.RuntimeBase.Optimizations
{
    public class OptimizationCategories
    {
        public const string Level1 = "Level1";
        public const string Level2 = "Level2";
        public const string Level3 = "Level3";
        public const string BlockBased = "BlockBased";
        public const string Analysis = "Analysis";
        public const string Purity = "Purity";
        public const string UseDef = "UseDef";
        public const string Constants = "Constants";
        public const string CommonSubexpressionsElimination = "CommonSubexpressionsElimination";
        public const string DeadCodeElimination = "DeadCodeElimination";
        public const string Global = "Global";
        public const string Inliner = "Inliner";
        public const string Propagation = "Propagation";
        public const string All = "All";

        public OptimizationCategories()
        {
            Relations = new Dictionary<string, string>();
            OptimizationTypes = new Dictionary<string, Type>();
        }

        public Dictionary<string, string> Relations { get; set; }
        public Dictionary<string, Type> OptimizationTypes { get; set; }
        public static OptimizationCategories Instance { get; } = new OptimizationCategories();

        public void AddChildToParentOptimizationRelation(string parent, string child)
        {
            Relations[child] = parent;
        }

        public void BuildRelationsByReflection()
        {
            var assembly = GetType().Assembly;
            OptimizationTypes.Clear();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof (OptimizationPassBase)))
                    continue;
                var optimizationAttribute = type.GetCustomAttribute<OptimizationAttribute>();
                if (optimizationAttribute == null)
                    continue;
                AddChildToParentOptimizationRelation(optimizationAttribute.Category, type.Name);
                OptimizationTypes[type.Name] = type;
            }
        }

        public List<string> ChildrenRelations(string parent)
        {
            return Relations
                .Where(relation => relation.Value == parent)
                .Select(relation => relation.Key)
                .ToList();
        }

        public IEnumerable<OptimizationPassBase> Closure(IEnumerable<string> capabilies)
        {
            var result = new HashSet<string>();
            var resultItems = new List<OptimizationPassBase>();


            var fullCapabilities = BuildFullCapabilities(capabilies);

            foreach (var capability in fullCapabilities)
            {
                Type optimizationType;
                if (!OptimizationTypes.TryGetValue(capability, out optimizationType))
                    continue;
                if (!result.Add(capability))
                    continue;
                var activated = (OptimizationPassBase) Activator.CreateInstance(optimizationType);
                resultItems.Add(activated);
            }

            return resultItems;
        }

        HashSet<string> BuildFullCapabilities(IEnumerable<string> capabilies)
        {
            var fullCapabilities = new HashSet<string>(capabilies);
            var added = true;

            while (added)
            {
                added = false;
                var foundCapabilities = new HashSet<string>();
                foreach (var capability in fullCapabilities)
                {
                    var childRelations = ChildrenRelations(capability);
                    foundCapabilities.AddRange(childRelations);
                }
                foreach (var foundCapability in foundCapabilities)
                {
                    added |= fullCapabilities.Add(foundCapability);
                }
            }
            return fullCapabilities;
        }
    }
}