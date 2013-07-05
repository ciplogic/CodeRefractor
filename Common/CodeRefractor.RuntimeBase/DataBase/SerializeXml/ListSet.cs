using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeRefractor.RuntimeBase.DataBase
{
    class ListSet <T>: List<T>
    {
        public virtual void Serialize(DynNode dataNode)
        {
            dataNode["FieldCount"] = Count.Str();
            foreach (var child in this)
            {
                var childType = child.GetType();
                var typeFields = childType.GetFields();

                var childNode = dataNode.Add(childType.Name);
                foreach (var fieldInfo in typeFields)
                {
                    SerializeItem(dataNode, child, fieldInfo);
                }
                
            }
        }

        private void SerializeItem(DynNode dataNode, T child, FieldInfo fieldInfo)
        {
            var fieldValue = fieldInfo.GetValue(child);
            var fieldName = fieldInfo.Name;
            var fieldTypeCode = fieldInfo.FieldType.ExtractTypeCode();
            switch (fieldTypeCode)
            {
                case TypeCode.String:
                    dataNode[fieldName] = fieldValue.ToString();
                    break;
                case TypeCode.Int32:
                    dataNode[fieldName] = fieldValue.ToString();
                    break;

            }
        }

        
        public virtual void Deserialize(DynNode dataNode)
        {
            
        }
    }
}
