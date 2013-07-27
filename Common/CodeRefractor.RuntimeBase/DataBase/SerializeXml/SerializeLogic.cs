#region Usings

using System;
using System.Globalization;
using System.IO;

#endregion

namespace CodeRefractor.RuntimeBase.DataBase.SerializeXml
{
    public static class SerializeLogic
    {
        public static void SerializeToFile(this object instance, string fileName)
        {
            var result = instance.Serialize();
            result.ToFile(fileName);
        }

        public static DynNode Serialize(this object instance)
        {
            var type = instance.GetType();
            var result = new DynNode(type.Name);
            SerializeObjectContent(instance, result);

            return result;
        }

        public static void SerializeObjectContent(this object instance, DynNode result)
        {
            var type = instance.GetType();
            foreach (var field in type.GetFields())
            {
                var typeCode = field.FieldType.ExtractTypeCode();
                var name = field.Name;
                var fieldValue = field.GetValue(instance);
                var resultText = GetTextOfObjectByTypeCode(typeCode, fieldValue);
                result[name] = resultText;
            }
            foreach (var property in type.GetProperties())
            {
                var typeCode = property.PropertyType.ExtractTypeCode();
                var name = property.Name;
                var fieldValue = property.GetValue(instance, null);
                var resultText = GetTextOfObjectByTypeCode(typeCode, fieldValue);
                result[name] = resultText;
            }
        }

        public static bool DeserializeFromFile(this object instance, string fileName)
        {
            if (!File.Exists(fileName))
                return false;
            var dynNode = new DynNode("node");
            if (!dynNode.FromFile(fileName))
                return false;
            instance.Deserialize(dynNode);
            return true;
        }

        public static void Deserialize(this object instance, DynNode result)
        {
            var type = instance.GetType();
            foreach (var field in type.GetFields())
            {
                var typeCode = field.FieldType.ExtractTypeCode();
                var name = field.Name;
                var itemValue = result[name];
                var value = GetValueByTypeCode(typeCode, itemValue);
                field.SetValue(instance, value);
            }
            foreach (var property in type.GetProperties())
            {
                var typeCode = property.PropertyType.ExtractTypeCode();
                var name = property.Name;
                var itemValue = result[name];
                var value = GetValueByTypeCode(typeCode, itemValue);
                property.SetValue(instance, value, null);
            }
        }

        private static string GetTextOfObjectByTypeCode(TypeCode typeCode, object fieldValue)
        {
            string resultText;
            switch (typeCode)
            {
                case TypeCode.Int32:
                    resultText = ((int) fieldValue).ToString(CultureInfo.InvariantCulture);
                    break;
                case TypeCode.Boolean:
                    resultText = ((bool)fieldValue).ToString(CultureInfo.InvariantCulture);
                    break;
                case TypeCode.Double:
                    resultText = ((double) fieldValue).ToString(CultureInfo.InvariantCulture);
                    break;
                case TypeCode.String:
                    resultText = fieldValue.ToString();
                    break;
                default:
                    throw new InvalidOperationException("You should not serialize this type code. Is unknown");
            }
            return resultText;
        }

        private static object GetValueByTypeCode(TypeCode typeCode, string itemValue)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                {
                    bool boolResult;
                    var canParseBool = bool.TryParse(itemValue, out boolResult);
                    return canParseBool && boolResult;
                }
                case TypeCode.Int32:
                    return int.Parse(itemValue);
                case TypeCode.Double:
                    return Double.Parse(itemValue);
                case TypeCode.String:
                    return itemValue;
                default:
                    throw new InvalidOperationException("You should not serialize this type code. Is unknown");
            }
        }
    }
}