#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.ClosureCompute.Resolvers;
using CodeRefractor.ClosureCompute.Steps;
using CodeRefractor.MiddleEnd;
using CodeRefractor.RuntimeBase.Backend;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.TypeInfoWriter;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public class ClosureEntities
    {
        public MethodInfo EntryPoint { get; set; }

        //Left type in should be the type from CLR world
        //Right type should be the type implementing the same layout
        public Dictionary<Type, Type> MappedTypes { get; set; }

        public Dictionary<MethodBase, MethodInterpreter> MethodImplementations { get; set; }
        public Dictionary<MethodInterpreterKey, MethodInterpreter> LookupMethods { get; set; }
        public HashSet<MethodInfo> AbstractMethods { get; set; }
        public List<ClosureComputeBase> ClosureSteps { get; set; }

        public List<MethodResolverBase> MethodResolverList { get; set; }
        public List<TypeResolverBase> TypeResolverList { get; set; }

        public ClosureEntities()
        {
            MappedTypes = new Dictionary<Type, Type>();
            MethodImplementations = new Dictionary<MethodBase, MethodInterpreter>();
            AbstractMethods = new HashSet<MethodInfo>();
            LookupMethods = new Dictionary<MethodInterpreterKey, MethodInterpreter>();
            MethodResolverList = new List<MethodResolverBase>();

            TypeResolverList = new List<TypeResolverBase>();

            ClosureSteps = new List<ClosureComputeBase>();
            AddClosureStep<AddEntryPointInterpretedMethod>();
            AddClosureStep<AddNotYetInterpretedMethods>();
            AddClosureStep<AddParameterTypesToClosure>();
        }

        public MethodInterpreter GetMethodImplementation(MethodBase method)
        {
            MethodInterpreter result;
            return MethodImplementations.TryGetValue(method, out result) ? result : null;
        }

        public MethodInterpreter ResolveMethod(MethodBase method)
        {
            var result = GetMethodImplementation(method);
            if (result != null)
                return result;
            foreach (var resolverBase in MethodResolverList)
            {
                result = resolverBase.Resolve(method);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void AddClosureStep<T>() where T : ClosureComputeBase, new()
        {
            ClosureSteps.Add(new T());
        }

        public void ComputeFullClosure()
        {
            var updateClosure = true;
            while (updateClosure)
            {
                updateClosure = false;
                foreach (var closureStep in ClosureSteps)
                {
                    updateClosure |= closureStep.UpdateClosure(this);
                }
            }
        }

        public void UseMethod(MethodBase method, MethodInterpreter interpreter)
        {
            MethodImplementations[method] = interpreter;
            LookupMethods[interpreter.ToKey()] = interpreter;
        }

        public void AddMethodResolver(MethodResolverBase resolveRuntimeMethod)
        {
            MethodResolverList.Add(resolveRuntimeMethod);
        }

        public StringBuilder BuildFullSourceCode()
        {
            var entryInterpreter = ResolveMethod(EntryPoint);
            var usedTypes = MappedTypes.Values.ToList();
            var typeTable=new TypeDescriptionTable(usedTypes);
            var virtualMethodTable = new VirtualMethodTable(typeTable);
            return CppCodeGenerator.GenerateSourceStringBuilder(entryInterpreter, usedTypes,
                MethodImplementations.Values.ToList(), virtualMethodTable,  this);
        }

        public Type ResolveType(Type type)
        {
            foreach (var resolverBase in TypeResolverList)
            {
                var resolved = resolverBase.Resolve(type);
                if (resolved != null)
                    return resolved;
            }
            return null;
        }

        public bool AddType(Type type)
        {
            if (type == null)
                return false;
            if (MappedTypes.ContainsKey(type))
                return false;
            MappedTypes[type] = ResolveType(type) ?? type;
            return true;
        }
    }

    public static class ClosureEntitiesUtils
    {


        public static ClosureEntities BuildClosureEntities(MethodInfo definition, Assembly runtimeAssembly)
        {
            var closureEntities = new ClosureEntities { EntryPoint = definition };
            var resolveRuntimeMethod = new ResolveRuntimeMethod(runtimeAssembly);
            closureEntities.AddMethodResolver(resolveRuntimeMethod);

            var extensionsResolverMethod = new ResolveRuntimeMethodUsingExtensions(runtimeAssembly);
            closureEntities.AddMethodResolver(extensionsResolverMethod);

            closureEntities.ComputeFullClosure();
            return closureEntities;
        }

    }
}