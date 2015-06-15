#region Uses

using System;

#endregion

namespace CodeRefractor.Runtime.Annotations
{
    public class MapTypeAttribute : Attribute
    {
        public MapTypeAttribute(Type type)
        {
            MappedType = type;
        }

        public Type MappedType { get; }
    }
}