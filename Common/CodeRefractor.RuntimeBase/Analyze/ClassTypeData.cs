#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Runtime;
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
            GetDeclaringTypeAsMapped(ref methodBase);
         
            var methodName = methodBase.ToString();
            int index;
            if (InterpreterMapping.TryGetValue(methodName, out index))
                return Interpreters[index];
            if(DelegateManager.IsTypeDelegate(methodBase.DeclaringType))
            {
                var methodInterpreter = LinkerInterpretersTable.Register(methodBase);
                methodInterpreter.Kind=MethodKind.Delegate;
                
                return methodInterpreter;
            }
            var linker = new MetaLinker();
            linker.SetEntryPoint(methodBase);
            linker.Interpret();
            return GetInterpreter(methodBase);
        }

        public static MethodInterpreter GetInterpreterStatic(MethodBase methodBase)
        {
            var declaringType = GetDeclaringTypeAsMapped(ref methodBase);
            var typeData = (ClassTypeData)ProgramData.UpdateType(declaringType);
            return typeData.GetInterpreter(methodBase);
        }

        private static Type GetDeclaringTypeAsMapped(ref MethodBase methodBase)
        {
            var declaringType = methodBase.DeclaringType;

            var mappedType = CrRuntimeLibrary.Instance.GetMappedType(declaringType);
            if (mappedType != null)
            {
                declaringType = mappedType;
                methodBase = ClassHierarchyAnalysis.GetBestVirtualMatch(methodBase, declaringType);
            }
            return declaringType;
        }
    }
}