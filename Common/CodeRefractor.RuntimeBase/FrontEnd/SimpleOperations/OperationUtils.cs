#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    static class OperationUtils
    {
        internal static void PopulateInstance(object result, object localOperation, Type type)
        {
            var properties = type.GetProperties()
                .Where(prop => prop.CanRead && prop.CanWrite).ToArray();
            var clonablePrimitiveProperties = properties
                .Where(prop => IsNotObjectType(prop.PropertyType))
                .ToArray();

            foreach (var primitiveProperty in clonablePrimitiveProperties)
            {
                var sourceValue = primitiveProperty.GetValue(localOperation, null);
                primitiveProperty.SetValue(result, sourceValue, null);
            }
            var objectBasedProperties = properties
                .Where(prop => !IsNotObjectType(prop.PropertyType)
                               && HasDefaultConstructor(prop.PropertyType))
                .ToArray();
            foreach (var property in objectBasedProperties)
            {
                var newInstance = Activator.CreateInstance(property.PropertyType);
                property.SetValue(result, newInstance, null);

                var sourceObject = property.GetValue(localOperation, null);
                PopulateInstance(newInstance, sourceObject, property.PropertyType);
            }
        }

        static bool IsNotObjectType(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode != TypeCode.Object;
        }

        static bool HasDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static string BuildString(object localOperation)
        {
            if (localOperation == null)
                return "null";
            var sb = new StringBuilder();
            var type = localOperation.GetType();
            var properties = type.GetProperties()
                .Where(prop => prop.CanRead && prop.CanWrite).ToArray();
            var clonablePrimitiveProperties = properties
                .Where(prop => IsNotObjectType(prop.PropertyType))
                .ToArray();

            var primitives = new List<string>();
            foreach (var primitiveProperty in clonablePrimitiveProperties)
            {
                var sourceValue = primitiveProperty.GetValue(localOperation, null);
                primitives.Add($"{primitiveProperty.Name} = {sourceValue}");
            }
            var objectBasedProperties = properties
                .Where(prop => !IsNotObjectType(prop.PropertyType)
                               && HasDefaultConstructor(prop.PropertyType))
                .ToArray();

            foreach (var property in objectBasedProperties)
            {
                var sourceObject = property.GetValue(localOperation, null);
                primitives.Add($"{property.Name} = {BuildString(sourceObject)}");
            }
            var joinedPrimitives = string.Join(", ", primitives);
            sb.AppendFormat("{0} [{1}]", type.Name, joinedPrimitives);
            return sb.ToString();
        }
    }
}