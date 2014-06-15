#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;
using System;

#endregion

namespace CodeRefractor.RuntimeBase.Optimizations
{
	public class OptimizationAttribute : Attribute 
	{
		public string Category { get; set; }
		public string Name {get;set; }
	}
    
}