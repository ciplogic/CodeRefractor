using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime.System.Reflection
{
     [ExtensionsImplementation(typeof(MemberInfo))]
    public abstract class MemberInfo
    {
         [MapMethod(IsStatic = false)]
        protected MemberInfo()
        {
        }

         [MapMethod(IsStatic = false)]
        public abstract string Name { get; }

    }
}
