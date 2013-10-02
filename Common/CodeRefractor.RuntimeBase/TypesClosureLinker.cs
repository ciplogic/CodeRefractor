using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.RuntimeBase
{
    public static class TypesClosureLinker
    {
        public static List<Type> GetTypesClosure(List<MethodInterpreter> closure)
        {
            var typesSet = ScanMethodParameters(closure);
            bool isAdded;
            do
            {
                var toAdd = new HashSet<Type>(typesSet);
                foreach (var type in typesSet)
                {
                    var mappedType = type.ReversedType();
                    if (type.IsPrimitive)
                        continue;
                    var fields = mappedType.GetFields().ToList();
                    fields.AddRange(mappedType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                    foreach (var fieldInfo in fields)
                    {
                        var fieldType = fieldInfo.FieldType;
                        if (fieldType.IsSubclassOf(typeof(Array)))
                            fieldType = fieldType.GetElementType();
                        if (fieldType.IsByRef)
                            fieldType = fieldType.GetElementType();
                        toAdd.Add(fieldType);
                    }
                }
                isAdded = (toAdd.Count != typesSet.Count);
                typesSet = toAdd;

            } while (isAdded);
            var typesClosure = typesSet.Where(t => !t.IsPrimitive).ToList();
            SortTypeDependencies(typesClosure);

            return typesClosure;
        }

        private static HashSet<Type> ScanMethodParameters(List<MethodInterpreter> closure)
        {
            var typesSet = new HashSet<Type>();
            foreach (var interpreter in closure)
            {
                var method = interpreter.Method;
                foreach (var parameter in method.GetParameters())
                {
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsSubclassOf(typeof(Array)))
                        continue;
                    if (parameterType.IsByRef)
                        parameterType = parameterType.GetElementType();
                    typesSet.Add(parameterType);
                }
                typesSet.Add(method.DeclaringType);
            }
            return typesSet;
        }

        private static void SortTypeDependencies(List<Type> typesClosure)
        {
            for (var i = 0; i < typesClosure.Count; i++)
            {
                var searchType = typesClosure[i];
                var typeDeps = TypeDependencies(searchType);
                if (typeDeps.Count == 0)
                    continue;
                foreach (var depType in typeDeps)
                {
                    var depSearchId = typesClosure.IndexOf(depType);
                    if (depSearchId <= i) continue;

                    typesClosure[depSearchId] = searchType;
                    typesClosure[i] = depType;
                    i = -1;
                    break;
                }
            }
        }

        private static List<Type> TypeDependencies(Type searchType)
        {
            var toAdd = new HashSet<Type>();

            var fields = searchType.GetFields().ToList();
            fields.AddRange(searchType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.IsPrimitive)
                    continue;
                toAdd.Add(fieldInfo.FieldType);
            }
            return toAdd.ToList();
        }
    }
}