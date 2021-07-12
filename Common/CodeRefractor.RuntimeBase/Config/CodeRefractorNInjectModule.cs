#region Uses

using Ninject.Modules;

#endregion

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