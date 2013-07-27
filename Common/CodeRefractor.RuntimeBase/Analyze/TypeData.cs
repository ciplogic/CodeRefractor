#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class TypeData
    {
        public string Namespace;
        public string Name;
        public bool IsClass;
        public bool IsArray;
        public List<FieldDataRef> Fields;
        public Type Info;
        public List<MethodInterpreter> Interpreters;
        public Dictionary<String, int> InterpreterMapping = new Dictionary<string, int>();

        public TypeData()
        {
            Fields = new List<FieldDataRef>();
            Interpreters = new List<MethodInterpreter>();
        }

        public override string ToString()
        {
            return string.Format("MethodName='{0}' Namespace='{1}'", Name, Namespace);
        }

        public static TypeData GetTypeData(Type type)
        {
            var assembly = AssemblyData.GetAssemblyData(type.Assembly);
            return ExtractData(type, assembly);
        }

        public static TypeData ExtractData(Type type, AssemblyData assemblyData)
        {
            var programData = ProgramData.Instance;
            var fullName = ComputeFullName(type.Namespace, type.Name);
            var result = programData.LocateType(fullName);
            if (result != null)
                return result;

            result = new TypeData
            {
                Namespace = type.Namespace,
                Name = type.Name,
                IsClass = type.IsClass,
                IsArray = type.IsArray,
                Info = type
            };

            assemblyData.Types[fullName] = result;
            var allFields = type.GetAllFields();
            foreach (var fieldInfo in allFields)
            {
                var fieldData = new FieldDataRef
                {
                    Name = fieldInfo.Name,
                    TypeData = GetTypeData(fieldInfo.FieldType),
                    IsStatic = fieldInfo.IsStatic
                };
                result.Fields.Add(fieldData);
            }
            return result;
        }

        public string FullName
        {
            get
            {
                var typeNs = Namespace;
                var name = Name;
                return ComputeFullName(typeNs, name);
            }
        }

        public static string ComputeFullName(string typeNs, string name)
        {
            return string.IsNullOrEmpty(typeNs)
                ? name
                : string.Format("{0}.{1}", typeNs, name);
        }

        public void AddMethodInterpreter(MethodInterpreter metaLinker)
        {
            InterpreterMapping[metaLinker.Method.ToString()] = Interpreters.Count;
            Interpreters.Add(metaLinker);
        }

        public MethodInterpreter GetInterpreter(string methodName)
        {
            int index;
            return !InterpreterMapping.TryGetValue(methodName, out index)
                ? null
                : Interpreters[index];
        }
    }
}