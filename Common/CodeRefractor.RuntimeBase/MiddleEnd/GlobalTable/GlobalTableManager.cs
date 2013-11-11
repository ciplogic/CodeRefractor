using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeRefractor.RuntimeBase.MiddleEnd.GlobalTable
{
    public enum GlobalFieldKind{
        None,
        Const,
        Static,
    }
    public class GlobalFieldDefinition
    {
        public string Name{get;set;}
        public Type ParentType {get;set;}
        public object ConstValue{get;set;} 
        public bool Kind{get;set;}
    }

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
