using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class TypeDescription
    {
        public Type ClrType { get; private set; }
        public TypeCode ClrTypeCode { get; private set; }
        public TypeDescription BaseType { get; private set; }

        public bool ContainsGenericParameters { get; set; }


        public string Name { get; private set; }
        public string Namespace { get; set; }
        public bool IsPointer { get; private set; }
        static readonly HashSet<Type> IgnoredSet = new HashSet<Type>(
             new []
             {
                 typeof(object),
                 typeof(IntPtr)
             }
            );
            
            
           public List<FieldDescription> Layout { get; set; }

        public TypeDescription(Type clrType)
        {
            ClrType = clrType;

            Name = clrType.Name;
            Namespace = clrType.Namespace;
            ContainsGenericParameters = clrType.ContainsGenericParameters;
        }

        public void ExtractInformation()
        {
            Layout = new List<FieldDescription>();
            ClrTypeCode = Type.GetTypeCode(ClrType);

            if (IgnoredSet.Contains(ClrType))
                return;
            if (ClrType.IsPointer || ClrType.IsByRef)
            {
                UsedTypeList.Set(ClrType.GetElementType());
                IsPointer = ClrType.IsPointer;
                return;
            }

            if (ClrType.BaseType != typeof (object))
            {
                BaseType = UsedTypeList.Set(ClrType.BaseType);
            }

            if (ClrTypeCode == TypeCode.Object)
            {
                ExtractFieldsTypes();
            }
        }

        private void ExtractFieldsTypes()
        {
            var clrType = ClrType.GetReversedType();
            if (clrType.Assembly.GlobalAssemblyCache)
                return;
            if (clrType.IsInterface)
                return;
            if (clrType.IsSubclassOf(typeof (Array)))
            {
                UsedTypeList.Set(clrType.GetElementType());
            }
            var fields = clrType.GetFields(BindingFlags.NonPublic|
                BindingFlags.Public|BindingFlags.Instance
                |BindingFlags.DeclaredOnly|BindingFlags.Static
                ).ToArray();
            if(fields.Length==0)
                return;
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.IsLiteral)
                    continue;
                var typeOfField = UsedTypeList.Set(fieldInfo.FieldType);
                if (typeOfField== null)
                    continue;
                var fieldDescription = new FieldDescription
                {
                    Name = fieldInfo.Name,
                    TypeDescription = typeOfField,
                    IsStatic = fieldInfo.IsStatic
                };
                var fieldOffsetAttribute = fieldInfo.GetCustomAttribute<FieldOffsetAttribute>();
                if (fieldOffsetAttribute != null)
                {
                    fieldDescription.Offset = fieldOffsetAttribute.Value;
                }
                Layout.Add(fieldDescription);
            }
        }

        public void WriteLayout(StringBuilder sb)
        {
            if (BaseType != null)
                BaseType.WriteLayout(sb);

            var noOffsetFields = new List<FieldDescription>();
            var dictionary = new SortedDictionary<int, List<FieldDescription>>();
            BuildUnionLayouts(noOffsetFields, dictionary);
            foreach (var fieldList in dictionary.Values)
            {
                sb.AppendLine("union {");
                WriteFieldListToLayout(sb, fieldList);
                sb.AppendLine("};");
            } 
            WriteFieldListToLayout(sb, noOffsetFields);
        }

        private void BuildUnionLayouts(List<FieldDescription> noOffsetFields, SortedDictionary<int, List<FieldDescription>> dictionary)
        {
            foreach (var fieldData in Layout)
            {
                var key = fieldData.Offset ?? -1;
                if (key == -1)
                {
                    noOffsetFields.Add(fieldData);
                    continue;
                }
                if (!dictionary.ContainsKey(key))
                {
                    dictionary[key] = new List<FieldDescription>();
                }
                dictionary[key].Add(fieldData);
            }
        }

        private static void WriteFieldListToLayout(StringBuilder sb, List<FieldDescription> fields)
        {
            foreach (var fieldData in fields)
            {
                if (fieldData.TypeDescription.ContainsGenericParameters)
                {
                    
                }
                var staticString = fieldData.IsStatic ? "static" : "";
                sb.AppendFormat("{2} {0} {1};",
                    fieldData.TypeDescription.ClrType.ToCppName(true),
                    fieldData.Name.ValidName(),
                    staticString
                    ).AppendLine();
            }
        }

        public static string GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                var obj = Activator.CreateInstance(type);
                return obj.ToString();
            }
            var result = string.Format("std::shared_ptr <{0}>(0)", type.ToCppName(true));
            return result;
        }

        public override string ToString()
        {
            return ClrType.ToString();
        }

        public void WriteStaticFieldInitialization(StringBuilder sb)
        {
            if (BaseType != null)
                BaseType.WriteLayout(sb);

            var mappedType = ClrType;

            var fieldInfos = mappedType.GetFields().ToList();
            fieldInfos.AddRange(mappedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var fieldData in Layout)
            {
                if(!fieldData.IsStatic)
                    continue;
                sb.AppendFormat(" /* static*/ {0} {3}::{1} = {2};",
                        fieldData.TypeDescription.ClrType.ToCppName(true),
                        fieldData.Name.ValidName(),
                        GetDefault(fieldData.TypeDescription.ClrType),
                        ClrType.ToCppMangling())
                        .AppendLine();
            }
        }
        
    }
}