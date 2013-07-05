using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
