#region Uses

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.UseDefs;

#endregion

namespace CodeRefractor.ClosureCompute.Steps
{
    public class AddNotYetInterpretedMethods : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var methods = closureEntities.MethodImplementations.Values;
            var methdosToAdd = new HashSet<MethodBase>();
            foreach (var method in methods.Where(m => m.Kind == MethodKind.Default))
            {
                var useDef = method.MidRepresentation.UseDef;
                var callOperations = useDef.GetOperationsOfKind(OperationKind.Call);
                var ops = useDef.GetLocalOperations();
                foreach (var callOperation in callOperations)
                {
                    var methodDataInfo = ops[callOperation].Get<MethodData>();
                    if (closureEntities.GetMethodImplementation(methodDataInfo.Info) == null)
                    {
                        methdosToAdd.Add(methodDataInfo.Info);
                    }
                }
            }
            if (methdosToAdd.Count != 0)
            {
                foreach (var methodBase in methdosToAdd)
                {
                    AddMethodToClosure(closureEntities, methodBase);
                }
                return true;
            }
            return false;
        }
    }
}