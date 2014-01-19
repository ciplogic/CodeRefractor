using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using CodeRefractor.RuntimeBase.Runtime;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class TypeDescription
    {
        public Type ClrType { get; private set; }
        public TypeCode ClrTypeCode { get; private set; }
        public TypeDescription BaseType { get; private set; }


        public string Name { get; private set; }
        public string Namespace { get; set; }
        public bool IsPointer { get; private set; }
        static HashSet<Type> ignoredSet = new HashSet<Type>(
             new []
             {
                 typeof(object),
                 typeof(IntPtr)
             }
            );
            
            
            List<FieldDescription> Layout { get; set; }

        public TypeDescription(Type clrType)
        {
            ClrType = clrType;
            Name = clrType.Name;
            Namespace = clrType.Namespace;
            Layout = new List<FieldDescription>();
            ExtractInformation();
        }

        private void ExtractInformation()
        {
            ClrTypeCode = Type.GetTypeCode(ClrType);

            if (ignoredSet.Contains(ClrType))
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
                var typeField = UsedTypeList.Set(fieldInfo.FieldType);
                if (typeField == null)
                    continue;
                var fieldDescription = new FieldDescription
                {
                    Name = fieldInfo.Name,
                    TypeDescription = typeField,
                    IsStatic = fieldInfo.IsStatic
                };
                Layout.Add(fieldDescription);
            }
        }

        public void WriteLayout(StringBuilder sb)
        {
            if (BaseType != null)
                BaseType.WriteLayout(sb);

            var mappedType = ClrType;
            
            var fieldInfos = mappedType.GetFields().ToList();
            fieldInfos.AddRange(mappedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var fieldData in Layout)
            {
                string staticString;
                if (fieldData.IsStatic) 
                    staticString = "static";
                else staticString = "";
                sb.AppendFormat("{2} {0} {1};", 
                        fieldData.TypeDescription.ClrType.ToCppName(),
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
            var result = string.Format("std::shared_ptr <{0}>(0)",type.ToCppName());
            return result;
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
                        fieldData.TypeDescription.ClrType.ToCppName(),
                        fieldData.Name.ValidName(),
                        GetDefault(fieldData.TypeDescription.ClrType),
                        ClrType.ToCppMangling())
                        .AppendLine();
            }
        }
        
    }
}