using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public class ProgramOptimizationsTable
    {
        public List<ProgramOptimizationBase> Optimizations = new List<ProgramOptimizationBase>();

        public void Add(ProgramOptimizationBase optimization)
        {
            Optimizations.Add(optimization);
        }

        public bool Optimize(ProgramClosure program)
        {
            var result = false;
            foreach (var optimization in Optimizations)
            {
                result|=optimization.Optimize(program);
            }
            return result;
        }
    }
}
