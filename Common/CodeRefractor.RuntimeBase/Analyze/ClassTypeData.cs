using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class ClassTypeData:TypeData
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

        public MethodInterpreter GetInterpreter(string methodName)
        {
            int index;
            return !InterpreterMapping.TryGetValue(methodName, out index)
                ? null
                : Interpreters[index];
        }
    }
}