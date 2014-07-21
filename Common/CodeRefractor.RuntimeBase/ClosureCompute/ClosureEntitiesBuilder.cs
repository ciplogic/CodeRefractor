using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.ClosureCompute.Steps;
using CodeRefractor.ClosureCompute.Steps.AddTypes;

namespace CodeRefractor.ClosureCompute
{
    public class ClosureEntitiesBuilder
    {
        public const BindingFlags AllFlags = (BindingFlags)(-1);
        public List<ClosureComputeBase> ClosureSteps { get; set; }

        public List<MethodResolverBase> MethodResolverList { get; set; }
        public List<TypeResolverBase> TypeResolverList { get; set; }

        internal void SetupSteps()
        {
            MethodResolverList = new List<MethodResolverBase>();

            TypeResolverList = new List<TypeResolverBase>();

            ClosureSteps = new List<ClosureComputeBase>();
            //Method closure steps
            AddClosureStep<AddEntryPointInterpretedMethod>();
            AddClosureStep<AddNotYetInterpretedMethods>();
           

            //Type closure steps
            AddClosureStep<AddStringTypeToClosure>();
            AddClosureStep<AddParameterTypesToClosure>();
            AddClosureStep<AddLocalVariableTypesToClosure>();
            AddClosureStep<AddVirtualMethods>();
        }
        private void AddClosureStep<T>() where T : ClosureComputeBase, new()
        {
            ClosureSteps.Add(new T());
        }

        public void AddTypeResolver(TypeResolverBase typeResolver)
        {
            TypeResolverList.Add(typeResolver);
        }

    }
}