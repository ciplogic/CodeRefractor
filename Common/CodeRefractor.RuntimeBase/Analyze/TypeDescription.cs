#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.Runtime;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class TypeDescription
    {
        static readonly HashSet<Type> IgnoredSet = new HashSet<Type>(
            new[]
            {
                typeof (object),
                typeof (IntPtr)
            }
            );

        public TypeDescription(Type clrType)
        {
            try
            {
                if (clrType == null)
                    clrType = typeof (object);
                if (clrType.IsInterface)
                    clrType = typeof (object);

                Layout = new List<FieldDescription>();
                ClrType = clrType;

                Name = clrType.Name;
                Namespace = clrType.Namespace;
                ContainsGenericParameters = clrType.ContainsGenericParameters;
            }
            catch (Exception)
            {
            }
        }

        public Type ClrType { get; set; }
        public TypeCode ClrTypeCode => Type.GetTypeCode(ClrType);
        public TypeDescription BaseType { get; private set; }
        public bool ContainsGenericParameters { get; set; }
        public string Name { get; private set; }
        public string Namespace { get; set; }
        public bool IsPointer { get; private set; }
        public List<FieldDescription> Layout { get; }

        public Type GetClrType(ClosureEntities closureEntities)
        {
            return ClrType;
        }

        public void ExtractInformation(ClosureEntities closureEntities)
        {
            if (IgnoredSet.Contains(ClrType))
                return;
            if (ClrType.IsPointer || ClrType.IsByRef)
            {
                IsPointer = ClrType.IsPointer;
                return;
            }

            if (ClrType.BaseType != typeof (object))
            {
                BaseType = new TypeDescription(ClrType.BaseType);
            }
            if (ClrType.IsPrimitive)
                return;

            if (ClrTypeCode == TypeCode.Object)
            {
                ExtractFieldsTypes(closureEntities);
            }
        }

        void ExtractFieldsTypes(ClosureEntities crRuntime)
        {
            var clrType = ClrType.GetReversedType(crRuntime);
            if (clrType.Assembly.GlobalAssemblyCache)
                return;
            if (clrType.IsInterface)
                return;
            var fields = clrType.GetFields(ClosureEntitiesBuilder.AllFlags).ToArray();
            if (fields.Length == 0)
                return;
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.IsLiteral)
                    continue;
                var typeOfField = new TypeDescription(fieldInfo.FieldType);

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

        public void WriteLayout(CodeOutput codeOutput)
        {
            BaseType?.WriteLayout(codeOutput);

            var noOffsetFields = new List<FieldDescription>();
            var dictionary = new SortedDictionary<int, List<FieldDescription>>();
            BuildUnionLayouts(noOffsetFields, dictionary);
            foreach (var fieldList in dictionary.Values)
            {
                codeOutput.Append("union")
                    .BracketOpen();
                WriteFieldListToLayout(codeOutput, fieldList);
                codeOutput.BracketClose();
            }
            WriteFieldListToLayout(codeOutput, noOffsetFields);
        }

        void BuildUnionLayouts(List<FieldDescription> noOffsetFields,
            SortedDictionary<int, List<FieldDescription>> dictionary)
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

        static void WriteFieldListToLayout(CodeOutput codeOutput, List<FieldDescription> fields)
        {
            foreach (var fieldData in fields)
            {
                if (fieldData.TypeDescription.ContainsGenericParameters)
                {
                }
                var staticString = fieldData.IsStatic ? "static " : "";
                codeOutput.AppendFormat("{2}{0} {1};\n",
                    fieldData.TypeDescription.ClrType.ToCppName(),
                    fieldData.Name.ValidName(),
                    staticString
                    );
            }
        }

        public static string GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                var obj = Activator.CreateInstance(type);
                return obj.ToString();
            }
            var result = string.Format("{0}(0)", type.ToCppName());
            return result;
        }

        public override string ToString()
        {
            return ClrType.ToString();
        }

        public void WriteStaticFieldInitialization(CodeOutput codeOutput)
        {
            if (BaseType != null)
                BaseType.WriteLayout(codeOutput);

            var mappedType = ClrType;

            var fieldInfos = mappedType.GetFields().ToList();
            fieldInfos.AddRange(mappedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var fieldData in Layout)
            {
                if (!fieldData.IsStatic)
                    continue;

                codeOutput.AppendFormat("/* static*/ {0} {3}::{1} = {2};\n",
                    fieldData.TypeDescription.ClrType.ToCppName(),
                    fieldData.Name.ValidName(),
                    GetDefault(fieldData.TypeDescription.ClrType),
                    ClrType.ToCppMangling());
            }
        }

        public Type GetElementType()
        {
            return ClrType.GetElementType();
        }
    }
}