#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;
using System;

#endregion

namespace CodeRefractor.RuntimeBase.Optimizations
{

	public class OptimizationCategories
	{
		public const string BlockBased = "BlockBased";
		public const string Analysis = "Analysis";
		public const string Purity = "Purity";
		public const string UseDef = "UseDef";
		public const string CommonSubexpressions = "CommonSubexpressions";
		public const string DeadCodeElimination = "DeadCodeElimination";
	}
    
}