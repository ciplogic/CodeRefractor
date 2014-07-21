using System.Collections.Generic;
using System.Reflection;

namespace CodeRefractor.ClosureCompute.Steps
{
    public class AddVirtualMethodImplementations : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var abstractMethods = closureEntities.AbstractMethods;
            if (abstractMethods.Count == 0)
                return false;
            var methdosToAdd = new HashSet<MethodBase>();
            foreach (var methodBase in abstractMethods)
            {
                if (closureEntities.GetMethodImplementation(methodBase) == null)
                {
                    methdosToAdd.Add(methodBase);
                }
               
            }
            if (methdosToAdd.Count == 0) return false;
            foreach (var methodBase in methdosToAdd)
            {
                AddMethodToClosure(closureEntities, methodBase);
            }
            return true;
        }
    }
}