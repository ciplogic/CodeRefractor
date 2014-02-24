using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UsedTypeList
    {
        public readonly Dictionary<Type, TypeDescription> UserTypeDesc = new Dictionary<Type, TypeDescription>(); 
        public static TypeDescription Set(Type type)
        {
            if (type == null)
                return null;
            type = type.ResolveTypeByResolvers();
            var typeList = Instance.UserTypeDesc;
            TypeDescription typeDesc;
            var indexOf = typeList.TryGetValue(type, out typeDesc);
            if (indexOf )
            {
                return typeDesc;
            }
            var typeDescription = new TypeDescription(type);
            Instance.UserTypeDesc[type] = typeDescription;
            typeDescription.ExtractInformation();
            return typeDescription;
        }

        public static readonly UsedTypeList Instance  = new UsedTypeList();

        public static HashSet<Type> GetFieldTypeDependencies(Type type)
        {

            var fields = type.GetFields().ToList();

            fields.AddRange(type.GetFields(
                BindingFlags.NonPublic
                | BindingFlags.Instance
                | BindingFlags.Static));

            var result = new HashSet<Type>();
            result.AddRange(fields
                .Select(field=>field.FieldType)
                .Select(fieldType =>
                {
                    if (fieldType.IsSubclassOf(typeof(Array)))
                        return fieldType.GetElementType();

                    if (fieldType.IsPointer || fieldType.IsByRef)
                        fieldType = fieldType.GetElementType();
                    return fieldType;

                })
                );

            return result;
        }
    }
}