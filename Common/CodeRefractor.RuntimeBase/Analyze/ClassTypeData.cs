#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class ClassTypeData : TypeData
    {
        public List<FieldDataRef> Fields;
        public List<MethodInterpreter> Interpreters;
        public Dictionary<string, int> InterpreterMapping = new Dictionary<string, int>();

        public ClassTypeData()
        {
            Fields = new List<FieldDataRef>();
            Interpreters = new List<MethodInterpreter>();
        }

        public static void PopulateFields(Type type, ClassTypeData result)
        {
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
        }

        public void AddMethodInterpreter(MethodInterpreter metaLinker)
        {
            InterpreterMapping[metaLinker.Method.ToString()] = Interpreters.Count;
            Interpreters.Add(metaLinker);
        }

        public MethodInterpreter GetInterpreter(MethodBase methodBase)
        {
            var methodName = methodBase.ToString();
            int index;
            if (InterpreterMapping.TryGetValue(methodName, out index))
                return Interpreters[index];
            var linker = new MetaLinker();
            linker.SetEntryPoint(methodBase);
            linker.Interpret();
            return GetInterpreter(methodBase);
        }
    }
}