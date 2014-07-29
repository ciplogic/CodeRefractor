using System.Linq;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;

namespace CodeRefractor.ClosureCompute.Steps.AddTypes
{
    public class AddLocalVariableTypesToClosure : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var methods = closureEntities.MethodImplementations.Values
                .Where(method=>method.Kind==MethodKind.CilInstructions)
                .Select(m=>(CilMethodInterpreter)m)
                .ToArray();
            var result = false;
            foreach (var method in methods)
            {
                result |= UpdateClosureForMethod(method, closureEntities);
            }
            return result;
        }

        private static bool UpdateClosureForMethod(CilMethodInterpreter method, ClosureEntities closureEntities)
        {
            var representationVariables = method.MidRepresentation.Vars;
            var localVariables = representationVariables.LocalVars;
            var virtRegs = representationVariables.VirtRegs;
            var result = false;
            foreach (var parameter in localVariables)
            {
                result |= closureEntities.AddType(parameter.FixedType.GetClrType(closureEntities));
            }
            foreach (var parameter in virtRegs)
            {
                result |= closureEntities.AddType(parameter.FixedType.GetClrType(closureEntities));
            }
            return result;
        }
    }
}