using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class FieldDescription
    {
        public string Name { get; set; }
        public TypeDescription TypeDescription { get; set; }
        public bool IsStatic { get; set; }
    }
    public class TypeDescription
    {
        public Type ClrType { get; private set; }

        public TypeCode ClrTypeCode { get; set; }

        public TypeDescription BaseType { get; private set; }



        List<FieldDescription> Layout { get; set; }

        public TypeDescription(Type clrType)
        {
            ClrType = clrType;
            Layout = new List<FieldDescription>();
            ExtractInformation();
        }

        private void ExtractInformation()
        {
            ClrTypeCode = Type.GetTypeCode(ClrType);

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
            if (ClrType.IsSubclassOf(typeof (Array)))
            {
                UsedTypeList.Set(ClrType.GetElementType());
            }
            var fields = ClrType.GetFields(BindingFlags.NonPublic|
                BindingFlags.Public|BindingFlags.Instance
                |BindingFlags.DeclaredOnly|BindingFlags.Static
                );
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.IsLiteral)
                    continue;
                var typeField = UsedTypeList.Set(fieldInfo.FieldType);
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
                var staticString = fieldData.IsStatic ? "static" : "";
                    sb.AppendFormat("{2} {0} {1};", 
                        fieldData.TypeDescription.ClrType.ToCppName(),
                        fieldData.Name.ValidName(),
                        staticString
                        ).AppendLine();
            }
        }
        
    }
}