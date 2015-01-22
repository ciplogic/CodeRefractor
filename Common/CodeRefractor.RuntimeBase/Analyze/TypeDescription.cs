#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class TypeDescription
    {
        public Type ClrType
        {
            set { _clrType = value; }
        }

        public Type GetClrType(ClosureEntities closureEntities)
        {
            return _clrType;
        }

        public TypeCode ClrTypeCode
        {
            get { return Type.GetTypeCode(_clrType); }
            
        }
        public TypeDescription BaseType { get; private set; }

        public bool ContainsGenericParameters { get; set; }


        public string Name { get; private set; }
        public string Namespace { get; set; }
        public bool IsPointer { get; private set; }

        private static readonly HashSet<Type> IgnoredSet = new HashSet<Type>(
            new[]
            {
                typeof (object),
                typeof (IntPtr)
            }
            );

        private Type _clrType;


        public List<FieldDescription> Layout { get; set; }

        public TypeDescription(Type clrType)
        {
           
            try
            {
                if (clrType == null)
                    clrType = typeof(object);
                if (clrType.IsInterface)
                    clrType = typeof(object);

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

        public void ExtractInformation(ClosureEntities closureEntities)
        {
            if (IgnoredSet.Contains(_clrType))
                return;
            if (_clrType.IsPointer || _clrType.IsByRef)
            {
                IsPointer = _clrType.IsPointer;
                return;
            }

            if (_clrType.BaseType != typeof(object))
            {
                BaseType = new TypeDescription(_clrType.BaseType);
            }
            if (_clrType.IsPrimitive)
                return;

            if (ClrTypeCode == TypeCode.Object)
            {
                ExtractFieldsTypes(closureEntities);
            }
        }

        private void ExtractFieldsTypes(ClosureEntities crRuntime)
        {
            var clrType = _clrType.GetReversedType(crRuntime);
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
            if (BaseType != null)
                BaseType.WriteLayout(codeOutput);

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

        private void BuildUnionLayouts(List<FieldDescription> noOffsetFields,
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

        private static void WriteFieldListToLayout(CodeOutput codeOutput, List<FieldDescription> fields)
        {
            foreach (var fieldData in fields)
            {
                if (fieldData.TypeDescription.ContainsGenericParameters)
                {
                }
                var staticString = fieldData.IsStatic ? "static " : "";
                codeOutput.AppendFormat("{2}{0} {1};\n",
                    fieldData.TypeDescription._clrType.ToCppName(),
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
            return _clrType.ToString();
        }

        public void WriteStaticFieldInitialization(CodeOutput codeOutput)
        {
            if (BaseType != null)
                BaseType.WriteLayout(codeOutput);

            var mappedType = _clrType;

            var fieldInfos = mappedType.GetFields().ToList();
            fieldInfos.AddRange(mappedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var fieldData in Layout)
            {
                if (!fieldData.IsStatic)
                    continue;

                codeOutput.AppendFormat("/* static*/ {0} {3}::{1} = {2};\n",
                    fieldData.TypeDescription._clrType.ToCppName(),
                    fieldData.Name.ValidName(),
                    GetDefault(fieldData.TypeDescription._clrType),
                    _clrType.ToCppMangling());
            }
        }

        public Type GetElementType()
        {
            return _clrType.GetElementType();
        }
    }
}