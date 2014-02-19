using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Runtime;

namespace CodeRefractor.CompilerBackend.OuputCodeWriter.ComputeClosure
{
    public static class TypesClosureLinker
    {
        public static ClosureResult BuildClosureForEntry(MethodInterpreter entryInterpreter)
        {
            var result = new ClosureResult();
            var methodInterpreters = new Dictionary<MethodInterpreterKey, MethodInterpreter>();
            methodInterpreters.Clear();
            methodInterpreters[entryInterpreter.ToKey()] = entryInterpreter;
            MetaLinker.Interpret(entryInterpreter);

            var foundMethodCount = 1;

            var canContinue = true;
            var dependencies = entryInterpreter.GetMethodClosure();
            while (canContinue)
            {
                foreach (var interpreter in dependencies)
                {
                    MetaLinker.Interpret(interpreter);
                }
                foreach (var dependency in dependencies)
                {
                    methodInterpreters[dependency.ToKey()] = dependency;
                }
                result.MethodInterpreters = methodInterpreters;
                foundMethodCount = methodInterpreters.Count;
                bool foundNewMethods;
                do
                {
                    dependencies = methodInterpreters.Values.ToList();
                    result.UsedTypes = new HashSet<Type>(GetTypesClosure(dependencies, out foundNewMethods));
                    if(!foundNewMethods) break;
                    foreach (var dependency in dependencies)
                    {
                        methodInterpreters[dependency.ToKey()] = dependency;
                    }
                } while (foundNewMethods);
                dependencies = methodInterpreters.Values.ToList().GetMultiMethodsClosure();
                canContinue = foundMethodCount != dependencies.Count;
            }
            return result;
        }
        public static HashSet<Type> GetTypesClosure(List<MethodInterpreter> methodList, out bool foundNewMethods)
        {
            var typesSet = ScanMethodParameters(methodList);

            foundNewMethods = false;

            var resultTypes = BuildScannedDictionaryFromTypesAndInstructions(typesSet);
            var methodDict = methodList.ToDictionary(method => method.Method.ClangMethodSignature());
            var virtMethods = methodList.Where(m => m.Method.IsVirtual).ToArray();
            foreach (var virt in virtMethods)
            {
                var baseClass = virt.Method.DeclaringType;
                var methodName = virt.Method.Name;
                var methodArgs = virt.Method.GetParameters().Select(par => par.ParameterType).ToArray();
                foreach (var type in resultTypes)
                {
                    if (!type.IsSubclassOf(baseClass))
                        continue;
                    var implMethod = type.GetMethod(methodName, methodArgs);
                    if (methodDict.ContainsKey(implMethod.ClangMethodSignature()))
                        continue;
                    var implInterpreter = implMethod.Register();
                    MetaLinker.Interpret(implInterpreter);
                    methodList.Add(implInterpreter);
                    foundNewMethods = true;
                }

            }
            return resultTypes;
        }

        private static HashSet<Type> BuildScannedDictionaryFromTypesAndInstructions(HashSet<Type> typesSet)
        {
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
                    if (fields.Count == 0)
                        continue;
                    fields.AddRange(mappedType.GetFields(
                        BindingFlags.NonPublic
                        | BindingFlags.Instance
                        | BindingFlags.Static));
                    foreach (var fieldInfo in fields)
                    {
                        var fieldType = fieldInfo.FieldType;
                        if (fieldType.IsInterface)
                            continue;
                        if (fieldType.IsSubclassOf(typeof(Array)))
                            fieldType = fieldType.GetElementType();
                        if (fieldType.IsPointer || fieldType.IsByRef)
                            fieldType = fieldType.GetElementType();

                        var typeDesc = UsedTypeList.Set(type);
                        if (typeDesc == null)
                            continue;
                        toAdd.Add(fieldType);
                    }
                }
                isAdded = (toAdd.Count != typesSet.Count);
                typesSet = toAdd;
            } while (isAdded);
            var typesClosure = typesSet.Where(t =>
                IsRefClassType(t) && !t.IsInterface).ToList();
            foreach (var type in typesClosure)
            {
                UsedTypeList.Set(type);
            }

            typesSet.Remove(typeof (void));
            typesSet.Remove(typeof (IntPtr));
            typesSet.Remove(typeof(Array));
            typesSet.RemoveWhere(t => t.IsPrimitive);
            typesSet.RemoveWhere(t => t.IsSubclassOf(typeof (Array)));
            typesSet.RemoveWhere(t => t.GetMappedType() == t && string.IsNullOrEmpty(t.FullName));
            return typesSet;
        }

        public static void SortTypeClosure(List<Type> types)
        {
            var typeComparer = new ClosureTypeComparer(types);
            typeComparer.Sort();
        }

        private static bool IsRefClassType(Type t)
        {
            var typeCode = Type.GetTypeCode(t);
            if (t.IsPrimitive)
                return false;
            if (t.HasElementType)
                return IsRefClassType(t.GetElementType());
            return true;
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
                    typesSet.Add(parameterType.GetReversedType());
                }
                typesSet.Add(method.DeclaringType.GetReversedType());
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
                    if (searchType.Namespace == depType.Namespace)
                        continue;
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