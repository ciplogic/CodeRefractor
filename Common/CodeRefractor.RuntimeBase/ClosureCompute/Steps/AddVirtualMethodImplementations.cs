#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.ClosureCompute.Steps
{
    public class AddVirtualMethodImplementations : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var abstractMethods = closureEntities.AbstractMethods;
            if (abstractMethods.Count == 0)
                return false;
            var methdosToAdd = new HashSet<MethodBaseKey>();
            foreach (var methodBase in abstractMethods)
            {
                var declaringType = methodBase.DeclaringType;
                if (declaringType == null)
                    continue;

                var implementingTypes = declaringType.ImplementorsOfT(closureEntities);
                foreach (var implementingType in implementingTypes)
                {
                    var implementingMethod = GetImplementingMethod(implementingType, methodBase);
                    if (implementingMethod == null)
                        continue;
                    if (implementingMethod.GetMethodBody() == null)
                        continue;
                    if (closureEntities.GetMethodImplementation(implementingMethod) == null)
                    {
                        methdosToAdd.Add(implementingMethod.ToMethodBaseKey());
                    }
                }
            }
            if (methdosToAdd.Count == 0) return false;
            foreach (var methodBase in methdosToAdd)
            {
                AddMethodToClosure(closureEntities, methodBase.Method);
            }
            return true;
        }

        public static MethodInfo GetImplementingMethod(Type implementingType, MethodInfo info)
        {
//            var matchingMethod = implementingType.GetMethods(ClosureEntitiesBuilder.AllFlags) //Doesnt handle all cases
//                .Where(m => m.Name == info.Name)
//                .FirstOrDefault(met => MethodMatchesParam(met, info));

            var matchingMethod = implementingType.GetMethod(info.Name, ClosureEntitiesBuilder.AllFlags, null,
                info.GetParameters().Select(j => j.ParameterType).ToArray(), null);
            //try again with explicit name
            if (matchingMethod == null)
                matchingMethod = implementingType.GetMethod(info.GetMethodName(), ClosureEntitiesBuilder.AllFlags, null,
                    info.GetParameters().Select(j => j.ParameterType).ToArray(), null);


            return matchingMethod;
        }

        static bool MethodMatchesParam(MethodInfo met, MethodInfo info)
        {
            var srcParameters = met.GetParameters();
            var destParameters = info.GetParameters();
            return MethodBaseKey.ParameterListIsMatching(srcParameters, destParameters);
        }
    }
}