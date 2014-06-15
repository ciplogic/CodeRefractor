#region Usings

using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace CodeRefractor.RuntimeBase.Optimizations
{

	public class OptimizationCategories
	{
	    public const string BlockBased = "BlockBased";
		public const string Analysis = "Analysis";
		public const string Purity = "Purity";
		public const string UseDef = "UseDef";

        public const string Propagation = "Propagation";
        public const string CommonSubexpressionsElimination = "CommonSubexpressionsElimination";
		public const string DeadCodeElimination = "DeadCodeElimination";
	    public const string All = "All";

        public Dictionary<string, string> Relations { get; set; }
        public Dictionary<string, Type> OptimizationTypes { get; set; }

	    public OptimizationCategories()
	    {
	        Relations = new Dictionary<string, string>();
            OptimizationTypes = new Dictionary<string, Type>();
	    }
	    public void AddChildToParentOptimizationRelation(string parent, string child)
	    {
	        Relations[child] = parent;
	    }


	    public void BuildRelationsByReflection()
	    {
	        var assembly = GetType().Assembly;
	        foreach (var type in assembly.GetTypes())
	        {
	            if(!type.IsSubclassOf(typeof(ResultingOptimizationPass)))
                    continue;
	            var optimizationAttribute = type.GetCustomAttribute<OptimizationAttribute>();
                if(optimizationAttribute==null)
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

	    public IEnumerable<ResultingOptimizationPass> Closure(IEnumerable<string> capabilies)
	    {
	        var result = new HashSet<string>();
            var resultItems = new List<ResultingOptimizationPass>();


            var fullCapabilities = BuildFullCapabilities(capabilies);

	        foreach (var capability in fullCapabilities)
	        {
	            Type optimizationType;
	            if(!OptimizationTypes.TryGetValue(capability, out optimizationType))
                    continue;
                if (!result.Add(capability))
                    continue;
	            var activated = (ResultingOptimizationPass) Activator.CreateInstance(optimizationType);
                resultItems.Add(activated);
	        }

            return resultItems;
	    }

	    private HashSet<string> BuildFullCapabilities(IEnumerable<string> capabilies)
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