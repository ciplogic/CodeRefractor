#region Uses

using System;

#endregion

namespace CodeRefractor.MiddleEnd.GlobalTable
{
    public class GlobalFieldDefinition
    {
        public string Name { get; set; }
        public Type ParentType { get; set; }
        public object ConstValue { get; set; }
        public bool Kind { get; set; }
    }
}