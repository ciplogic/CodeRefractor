#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.Analyze
{
    public class UsedTypeList
    {
        public static readonly UsedTypeList Instance = new UsedTypeList();
        public readonly Dictionary<Type, TypeDescription> UserTypeDesc = new Dictionary<Type, TypeDescription>();

        public static TypeDescription Set(Type type, ClosureEntities crRuntime)
        {
            if (type == null)
                return null;
            type = type.ResolveTypeByResolvers();
            var typeList = Instance.UserTypeDesc;
            TypeDescription typeDesc;
            var indexOf = typeList.TryGetValue(type, out typeDesc);
            if (indexOf)
            {
                return typeDesc;
            }
            var typeDescription = new TypeDescription(type);
            Instance.UserTypeDesc[type] = typeDescription;
            typeDescription.ExtractInformation(crRuntime);
            return typeDescription;
        }

        public static HashSet<Type> GetFieldTypeDependencies(Type type)
        {
            var fields = type.GetFields().ToList();

            fields.AddRange(type.GetFields(
                ClosureEntitiesBuilder.AllFlags));

            var result = new HashSet<Type>();
            result.AddRange(fields
                .Select(field => field.FieldType)
                .Select(fieldType =>
                {
                    if (fieldType.IsSubclassOf(typeof (Array)))
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