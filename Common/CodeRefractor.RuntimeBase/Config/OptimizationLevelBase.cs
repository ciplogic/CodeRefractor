#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Config
{
    public class OptimizationLevelBase
    {
        public virtual List<OptimizationPass> BuildOptimizationPasses0()
        {
            throw new NotImplementedException();
        }

        public virtual List<OptimizationPass> BuildOptimizationPasses3()
        {
            throw new NotImplementedException();
        }

        public virtual List<OptimizationPass> BuildOptimizationPasses2()
        {
            throw new NotImplementedException();
        }

        public virtual List<OptimizationPass> BuildOptimizationPasses1()
        {
            throw new NotImplementedException();
        }

        public static OptimizationLevelBase Instance;
    }
}