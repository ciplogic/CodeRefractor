#region Uses

using System;

#endregion

namespace CodeRefractor.Runtime.Annotations
{
    public class MapMethod : Attribute
    {
        public bool IsStatic { get; set; }
        public string Name { get; set; }
    }
}