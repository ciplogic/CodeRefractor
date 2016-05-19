#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.Util;
using static System.String;

#endregion

namespace CodeRefractor.Analyze
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
                    clrType = typeof(object);
                if (clrType.IsInterface)
                    clrType = typeof(object);

                Layout = new List<FieldDescription>();

                ClrTypeCode = Type.GetTypeCode(clrType);

                Name = clrType.Name;
                if (clrType.IsArray)
                {
                    var elementType = clrType.GetElementType();
                    BaseType = new TypeDescription(elementType);
                    IsArray = true;
                }
                Namespace = clrType.Namespace;
                ContainsGenericParameters = clrType.ContainsGenericParameters;
            }
            catch (Exception)
            {
            }
        }

        public bool IsArray { get; private set; }

        public Type ClrType
            => ComputeType();

        Type ComputeType()
        {
            if (IsArray)
            {
                return BaseType.ClrType.MakeArrayType();
            }
            if (ClrTypeCode == TypeCode.Object)
            {

                if (Namespace == "System")
                {
                    switch (Name)
                    {
                        case "Object":
                            return typeof(object);
                    }
                }
            }
            switch (ClrTypeCode)
            {
                case TypeCode.Boolean:
                    return typeof(bool);
                case TypeCode.Int32:
                    return typeof(Int32);
                case TypeCode.Int64:
                    return typeof(long);
                case TypeCode.Char:
                    return typeof(char);
                case TypeCode.Double:
                    return typeof(double);
                case TypeCode.Single:
                    return typeof(float);
                case TypeCode.String:
                    return typeof(string);
                default:
                    {
                        var message = $"Type not known: {Namespace ?? Empty}.{Name}";
                        throw new InvalidOperationException(message);
                    }
            }
        }

        public TypeCode ClrTypeCode { get; set; }
        public TypeDescription BaseType { get; private set; }
        public bool ContainsGenericParameters { get; set; }
        public string Name { get; private set; }
        public string Namespace { get; set; }
        public bool IsPointer { get; private set; }
        public List<FieldDescription> Layout { get; }

        public Type GetClrType(ClosureEntities closureEntities)
        {
            return ClrTypeCode == TypeCode.Object
                ? LocateObjectType(closureEntities, Namespace, Name) 
                : ClrType;
        }

        Type LocateObjectType(ClosureEntities closureEntities, string ns, string name)
        {
            var assemblies = closureEntities.EntryPoint.DeclaringType.Assembly;
            var types = assemblies.GetTypes().Where(type => type.Name == name).ToArray();
            if (types.Length == 1)
                return types.First();
            return types.FirstOrDefault(type => type.Namespace == ns);
        }

        public void ExtractInformation(ClosureEntities closureEntities, Type type)
        {
            if (IgnoredSet.Contains(type))
                return;
            if (type.IsPointer || type.IsByRef)
            {
                IsPointer = type.IsPointer;
                return;
            }

            if (type.BaseType != typeof(object))
            {
                BaseType = new TypeDescription(type.BaseType);
            }
            if (type.IsPrimitive)
                return;

            if (ClrTypeCode == TypeCode.Object)
            {
                ExtractFieldsTypes(closureEntities, type);
            }
        }

        void ExtractFieldsTypes(ClosureEntities crRuntime, Type type)
        {
            var clrType = type.GetReversedType(crRuntime);
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
            var result = $"{type.ToCppName()}(0)";
            return result;
        }

        public override string ToString()
        {
            return ClrType.ToString();
        }

        public void WriteStaticFieldInitialization(CodeOutput codeOutput)
        {
            BaseType?.WriteLayout(codeOutput);

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