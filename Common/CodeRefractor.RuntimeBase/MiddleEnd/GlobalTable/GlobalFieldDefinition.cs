using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeRefractor.RuntimeBase.MiddleEnd.GlobalTable
{
    public class GlobalFieldDefinition
    {
        public string Name{get;set;}
        public Type ParentType {get;set;}
        public object ConstValue{get;set;} 
        public bool Kind{get;set;}
    }
    
}
