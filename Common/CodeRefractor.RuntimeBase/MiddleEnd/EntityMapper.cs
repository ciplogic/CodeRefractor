using System.Reflection;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class EntityMapper
    {
        public virtual MethodInterpreter MapMethod(MethodBase method)
        {
            return null;
        }
    }
}