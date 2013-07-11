#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.Compiler.Optimizations.SimpleDce
{
    public class VRegReindexAssigned : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var virtRegs = intermediateCode.Vars.VirtRegs;
            var vregConstants = new SortedSet<int>(virtRegs.Select(localVar => localVar.Id));
            var last = vregConstants.LastOrDefault();
            if (last == 0 || last == vregConstants.Count)
                return;
            for (var i = 0; i < virtRegs.Count; i++)
                virtRegs[i].Id = i + 1;
            Result = true;
        }
    }
}