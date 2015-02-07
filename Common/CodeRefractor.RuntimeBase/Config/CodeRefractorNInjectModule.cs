using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeRefractor.RuntimeBase.Config;
using Ninject.Modules;

namespace CodeRefractor.Config
{
    public class CodeRefractorNInjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<CommandLineParse>().To<CommandLineParse>().InSingletonScope();
        }
    }
}
