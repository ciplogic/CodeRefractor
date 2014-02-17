using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.CompilerBackend.OuputCodeWriter;

namespace CodeRefractor.CompilerBackend.ProgramWideOptimizations
{
    public abstract class ProgramOptimizationBase
    {
        public abstract bool Optimize(ProgramClosure closure);
    }
}
