#region Uses

using System;

#endregion

namespace CodeRefractor.Runtime.Annotations
{
    public class ExtensionsImplementation : Attribute
    {
        public ExtensionsImplementation(Type declaringType)
        {
            DeclaringType = declaringType;
        }

        public Type DeclaringType { get; private set; }
    }
}