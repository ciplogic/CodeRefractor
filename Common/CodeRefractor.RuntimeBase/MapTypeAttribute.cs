#region Usings

using System;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public class MapTypeAttribute : Attribute
    {
        private readonly Type _type;

        public MapTypeAttribute(Type type)
        {
            _type = type;
        }

        public Type MappedType
        {
            get { return _type; }
        }

        public string Code { get; set; }
    }
}