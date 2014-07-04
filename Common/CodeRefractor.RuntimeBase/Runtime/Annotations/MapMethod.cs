using System;

namespace CodeRefractor.Runtime.Annotations
{
    public class MapMethod : Attribute
    {
        public MapMethod(){}
        public MapMethod(Type declaringType)
        {
            DeclaringType = declaringType;
            IsStatic = true;
        }

        public bool IsStatic { get; set; }

        public Type DeclaringType { get; private set; }

        public string Name { get; set; }
    }
}