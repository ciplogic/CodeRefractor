#region Usings

using System;
using System.Reflection;

#endregion

namespace CodeRefractor.RuntimeBase.Runtime
{
    public class CppMethodDefinition
    {
        public CppMethodBodyAttribute AttributeData { get; set; }
        public Type DeclaringType { get; set; }
        public Type MappedType { get; set; }
        public MethodBase MethodDefinition;

        public override string ToString()
        {
            return String.Format("[{0}]: {1}", MappedType.Name, MethodDefinition);
        }
    }
}