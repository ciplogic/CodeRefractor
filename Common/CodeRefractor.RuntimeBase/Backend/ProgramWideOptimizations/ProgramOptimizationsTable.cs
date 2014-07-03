#region Usings

using System.Collections;
using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.ProgramWideOptimizations
{
    public class ProgramOptimizationsTable : IEnumerable<ProgramOptimizationBase>
    {
        public readonly List<ProgramOptimizationBase> Optimizations = new List<ProgramOptimizationBase>();

        public void Add(ProgramOptimizationBase optimization)
        {
            Optimizations.Add(optimization);
        }

        public bool Optimize(ClosureEntities program)
        {
            var result = false;
            foreach (var optimization in Optimizations)
            {
                result |= optimization.Optimize(program);
            }
            return result;
        }

        public IEnumerator<ProgramOptimizationBase> GetEnumerator()
        {
            return Optimizations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Optimizations).GetEnumerator();
        }
    }
}