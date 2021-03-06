#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Backend.ComputeClosure;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.ComputeClosure;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend
{
    public static class TypesClosureLinker
    {
        public static ClosureResult BuildClosureForEntry(MethodInterpreter entryInterpreter,
            ProgramClosure programClosure)
        {
            var result = new ClosureResult();
            var methodInterpreters = new Dictionary<MethodInterpreterKey, MethodInterpreter>();
            methodInterpreters.Clear();
            methodInterpreters[entryInterpreter.ToKey()] = entryInterpreter;
            MetaLinker.Interpret(entryInterpreter, programClosure.Runtime);

            var canContinue = true;
            var dependencies = entryInterpreter.GetMethodClosure(programClosure.Runtime);
            while (canContinue)
            {
                
                foreach (var interpreter in dependencies)
                {
                    MetaLinker.Interpret(interpreter, programClosure.Runtime);
                }
                foreach (var dependency in dependencies)
                {
                    methodInterpreters[dependency.ToKey()] = dependency;
                }
                result.MethodInterpreters = methodInterpreters;
                var foundMethodCount = methodInterpreters.Count;
                dependencies = methodInterpreters.Values.ToList();
                bool foundNewMethods;
                 result.UsedTypes =
                    new HashSet<Type>(GetTypesClosure(dependencies, out foundNewMethods, programClosure,
                        programClosure.Runtime));

                
                foreach (var dependency in dependencies)
                {
                    methodInterpreters[dependency.ToKey()] = dependency;
                }
                foreach (var interpreter in methodInterpreters.Values)
                {
                    interpreter.Process(programClosure.Runtime);
                }

                dependencies = methodInterpreters.Values.ToList().GetMultiMethodsClosure(programClosure.Runtime); 
               
               
                canContinue = foundMethodCount != dependencies.Count;
            }
            return result;
        }

        public static HashSet<Type> GetTypesClosure(List<MethodInterpreter> methodList, out bool foundNewMethods,
            ProgramClosure programClosure, CrRuntimeLibrary crRuntime)
        {
            var typesSet = ScanMethodParameters(methodList, crRuntime);

            //For struct types we need to add them separately
            foreach (var methodInterpreter in methodList)
            {
                foreach (var operation in methodInterpreter.MidRepresentation.LocalOperations)
                {
                    if (operation is Assignment)
                    {
                        var type = (operation as Assignment).AssignedTo.FixedType.ClrType;
                        if (!typesSet.Contains(type) && !type.IsByRef)
                            typesSet.Add(type);
                    }
                    if (operation is RefAssignment)
                    {
                        var type = (operation as RefAssignment).Left.FixedType.ClrType;
                        if (!typesSet.Contains(type) && !type.IsByRef)
                            typesSet.Add(type);
                    }
                }
            }

            foundNewMethods = false;

            var resultTypes = BuildScannedDictionaryFromTypesAndInstructions(typesSet, crRuntime);
            var methodDict = methodList.ToDictionary(method => method.Method.Register(crRuntime).ToKey());
            var virtMethods = methodList.Where(m => m.Method.IsVirtual).ToArray();
            foreach (var virt in virtMethods)
            {
                var baseClass = virt.Method.DeclaringType;
                var methodName = virt.Method.Name;
                var methodArgs = virt.Method.GetParameters().Select(par => par.ParameterType).ToArray();
                foreach (var type in resultTypes)
                {
                    var hasinterface = false;//TODO: for some reason I cannot use .Contains / Linq here ?
                    foreach (var @interface in type.GetInterfaces())
                    {
                        if (@interface == baseClass)
                        {
                             hasinterface = true;
                         //   methodName = baseClass.Name +"."+methodName;
                            break;
                        }
                           
                    }
                    if ((!(type.IsSubclassOf(baseClass) || hasinterface)))
                        continue;
                    MethodInfo implMethod;
                    if (!hasinterface)
                    {
                         implMethod = type.GetMethod(methodName, methodArgs);
                    }
                    else
                    {
                        implMethod =
                            type.GetInterfaceMap(baseClass)
                                .InterfaceMethods.Where(m => m.Name == methodName && (!m.GetParameters().Select(j=>j.ParameterType).Except(methodArgs).Any())).FirstOrDefault();
                        

                    }
                    if (methodDict.ContainsKey(implMethod.Register(crRuntime).ToKey()))
                        continue;
                    var implInterpreter = implMethod.Register();
                    MetaLinker.Interpret(implInterpreter, crRuntime);
                    methodList.Add(implInterpreter);
                    foundNewMethods = true;
                }
            }
            return resultTypes;
        }

        private static HashSet<Type> BuildScannedDictionaryFromTypesAndInstructions(HashSet<Type> typesSet,
            CrRuntimeLibrary crRuntime)
        {
            bool isAdded;
            do
            {
                var toAdd = new HashSet<Type>(typesSet);
                foreach (var type in typesSet)
                {
                    AddBaseTypesToHash(type, toAdd);
                    var mappedType = crRuntime.GetMappedType(type);
                    if (type.IsPrimitive)
                        continue;

                    var fieldTypes = UsedTypeList.GetFieldTypeDependencies(type);
                    toAdd.AddRange(fieldTypes);
                    var fields = mappedType.GetFields().ToList();

                    fields.AddRange(mappedType.GetFields(
                        BindingFlags.NonPublic
                        | BindingFlags.Instance
                        | BindingFlags.Static));
                    foreach (var fieldInfo in fields)
                    {
                        var fieldType = fieldInfo.FieldType;
                        if (fieldType.IsInterface)
                            continue;
                        if (fieldType.IsSubclassOf(typeof (Array)))
                            fieldType = fieldType.GetElementType();
                        if (fieldType.IsPointer || fieldType.IsByRef)
                            fieldType = fieldType.GetElementType();

                        var typeDesc = UsedTypeList.Set(type, crRuntime);
                        if (typeDesc == null)
                            continue;
                        toAdd.Add(fieldType);
                        AddBaseTypesToHash(fieldType, toAdd);
                    }
                }
                isAdded = (toAdd.Count != typesSet.Count);
                typesSet = toAdd;
            } while (isAdded);
            var typesClosure = typesSet.Where(t =>
                IsRefClassType(t) && !t.IsInterface).ToList();
            foreach (var type in typesClosure)
            {
                UsedTypeList.Set(type, crRuntime);
            }

            typesSet.Remove(typeof (void));
            typesSet.Remove(typeof (IntPtr));
            typesSet.Remove(typeof (Array));

           // typesSet.RemoveWhere(t => t.IsInterface); Interfaces should be treated as classes
            typesSet.RemoveWhere(t => t.IsPrimitive);
            typesSet.RemoveWhere(t => t.IsSubclassOf(typeof (Array)));
            typesSet.RemoveWhere(t => t.GetMappedType() == t && string.IsNullOrEmpty(t.FullName));
            return typesSet;
        }

        private static void AddBaseTypesToHash(Type fieldType, HashSet<Type> toAdd)
        {
            while (fieldType.BaseType != null && fieldType.BaseType != typeof (object))
            {
                toAdd.Add(fieldType.BaseType);
                fieldType = fieldType.BaseType;
            }
        }

        public static void SortTypeClosure(List<Type> types, CrRuntimeLibrary crRuntime)
        {
            var typeComparer = new ClosureTypeComparer(types, crRuntime);
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

        private static HashSet<Type> ScanMethodParameters(List<MethodInterpreter> closure, CrRuntimeLibrary crRuntime)
        {
            var typesSet = new HashSet<Type>();
            foreach (var interpreter in closure)
            {
                var method = interpreter.Method;
                foreach (var parameter in method.GetParameters())
                {
                    var parameterType = parameter.ParameterType;

                    if (parameterType.IsSubclassOf(typeof (Array)))
                        continue;
                    if (parameterType.IsByRef)
                        parameterType = parameterType.GetElementType();
                    typesSet.Add(parameterType.GetReversedType(crRuntime));
                }
                typesSet.Add(method.DeclaringType.GetReversedType(crRuntime));
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