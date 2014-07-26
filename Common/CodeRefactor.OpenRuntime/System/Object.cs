using System;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime.System
{
    
    [ExtensionsImplementation(typeof(global::System.Object))]
    public abstract class Object
    {

        [MapMethod(IsStatic = false)]
        public static string ToString(object value)
        {
            return "";
        }
      
     
     
    }
}