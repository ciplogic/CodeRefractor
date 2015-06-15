#region Uses

using System.Linq;
using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.UseDefs;

#endregion

namespace CodeRefractor.ClosureCompute.Steps
{
    internal class AddVirtualMethods : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var result = false;
            var methods = closureEntities.MethodImplementations.Values;
            var cilMethods = methods.Where(m => m.Kind == MethodKind.CilInstructions)
                .Select(m => (CilMethodInterpreter) m)
                .ToArray();
            foreach (var methodBase in cilMethods)
            {
                var method = methodBase;
                var useDef = method.MidRepresentation.UseDef;
                var callOperations = useDef.GetOperationsOfKind(OperationKind.CallVirtual);
                if (callOperations.Length == 0)
                    continue;
                var ops = useDef.GetLocalOperations();
                foreach (var callOperation in callOperations)
                {
                    var methodDataInfo = ops[callOperation].Get<CallMethodVirtual>();
                    var methodToBeAdded = (MethodInfo) methodDataInfo.Info;
                    if (methodToBeAdded == null)
                        continue;
                    result |= closureEntities.AbstractMethods.Add(methodToBeAdded);
                }
            }
            return result;
        }
    }
}