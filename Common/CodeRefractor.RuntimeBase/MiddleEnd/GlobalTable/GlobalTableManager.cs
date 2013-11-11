using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeRefractor.RuntimeBase.MiddleEnd.GlobalTable
{
    public class GlobalTableManager
    {
        public Dictionary<Type, GlobalFieldDefinition> GlobalConstants { get; set; }
        public void DefineConstant(Type parentType, string fieldName, object value)
        {
            var constDefinition = new GlobalFieldDefinition
            {
                ParentType = parentType,
                Name = fieldName,
                ConstValue = value,
            };
        }
    }
}
