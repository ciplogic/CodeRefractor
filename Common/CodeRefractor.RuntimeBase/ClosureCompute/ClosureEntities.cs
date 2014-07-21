#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeRefractor.Backend;
using CodeRefractor.Backend.ComputeClosure;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.Optimizations.Util;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public class ClosureEntities
    {
        public MethodInfo EntryPoint { get; set; }

        //Left type in should be the type from CLR world
        //Right type should be the type implementing the same layout
        public Dictionary<Type, Type> MappedTypes { get; set; }

        public Dictionary<MethodBaseKey, MethodInterpreter> MethodImplementations { get; set; }
        public Dictionary<MethodInterpreterKey, MethodInterpreter> LookupMethods { get; set; }
        public HashSet<MethodInfo> AbstractMethods { get; set; }

        internal readonly ClosureEntitiesBuilder EntitiesBuilder = new ClosureEntitiesBuilder();

        public ClosureEntities()
        {
            MappedTypes = new Dictionary<Type, Type>();
            MethodImplementations = new Dictionary<MethodBaseKey, MethodInterpreter>(new MethodBaseKeyComparer());
            AbstractMethods = new HashSet<MethodInfo>();
            LookupMethods = new Dictionary<MethodInterpreterKey, MethodInterpreter>();

            EntitiesBuilder.SetupSteps();
        }

        public MethodInterpreter GetMethodImplementation(MethodBase method)
        {
            var key = new MethodBaseKey(method);
            MethodInterpreter result;
            return MethodImplementations.TryGetValue(key, out result) ? result : null;
        }

        public MethodInterpreter ResolveMethod(MethodBase method)
        {
            var result = GetMethodImplementation(method);
            if (result != null)
                return result;
            foreach (var resolverBase in EntitiesBuilder.MethodResolverList)
            {
                result = resolverBase.Resolve(method);
                if (result != null)
                    return result;
            }
            return null;
        }

        public void ComputeFullClosure()
        {
            var updateClosure = true;
            while (updateClosure)
            {
                updateClosure = false;
                foreach (var closureStep in EntitiesBuilder.ClosureSteps)
                {
                    updateClosure |= closureStep.UpdateClosure(this);
                }
            }
        }

        public void UseMethod(MethodBase method, MethodInterpreter interpreter)
        {
            MethodImplementations[method.ToKey()] = interpreter;
            LookupMethods[interpreter.ToKey()] = interpreter;
        }

        public void AddMethodResolver(MethodResolverBase resolveRuntimeMethod)
        {
            EntitiesBuilder.MethodResolverList.Add(resolveRuntimeMethod);
        }

        public StringBuilder BuildFullSourceCode()
        {
            var entryInterpreter = ResolveMethod(EntryPoint);
            List<Type> usedTypes = MappedTypes.Values.ToList();

            var typeTable = new TypeDescriptionTable(usedTypes);
            return CppCodeGenerator.GenerateSourceStringBuilder(entryInterpreter, usedTypes,typeTable,
                MethodImplementations.Values.ToList(), this);
        }


       

        public Type ResolveType(Type type)
        {
            type = type.ReduceType();
            Type result;
            if (MappedTypes.TryGetValue(type, out result))
                return result;
            foreach (var resolverBase in EntitiesBuilder.TypeResolverList)
            {
                var resolved = resolverBase.Resolve(type);
                if (resolved != null)
                {
                    MappedTypes[type] = resolved;
                    return resolved;
                }
                MappedTypes[type] = type;
            }
            return null;
        }

        public bool AddType(Type type)
        {
            if (type == null)
                return false;
            type = type.ReduceType();
            if (MappedTypes.ContainsKey(type))
                return false;
            MappedTypes[type] = ResolveType(type) ?? type;
            return true;
        }

        public void OptimizeClosure()
        {
            var level = new OptimizationLevels();

            var optimizations = level.BuildOptimizationPasses1();
            level.BuildOptimizationPasses2();
            OptimizationLevelBase.UpdateOptimizationsFromCategories(optimizations); 
            var cilMethods = MethodImplementations.Values
                .Where(m => m.Kind == MethodKind.CilInstructions)
                .Cast<CilMethodInterpreter>()
                .ToArray();
            ResultingOptimizationPass.Closure = this;
            var doOptimize = true;
            while (doOptimize)
            {
                doOptimize = false;
                foreach (var cilMethod in cilMethods)
                {
                    doOptimize |= MethodInterpreterCodeWriter.ApplyLocalOptimizations(optimizations, cilMethod);
                }
            }
        }
    }

}