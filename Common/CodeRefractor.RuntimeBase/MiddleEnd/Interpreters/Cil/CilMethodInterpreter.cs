#region Uses

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.NonCil;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters.Cil
{
    public class CilMethodInterpreter : MethodInterpreter, IEnumerable<LocalOperation>
    {
        public readonly MetaMidRepresentation MidRepresentation = new MetaMidRepresentation();

        public CilMethodInterpreter(MethodBase method)
            : base(method)
        {
            Kind = MethodKind.CilInstructions;
        }

        public bool Interpreted { get; set; }

        public IEnumerator<LocalOperation> GetEnumerator()
        {
            return MidRepresentation.LocalOperations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Process(ClosureEntities closureEntities)
        {
            if (Kind != MethodKind.CilInstructions)
                return;
            if (Interpreted)
                return;
            Ensure.AreEqual(false, PlatformInvokeMethod.IsPlatformInvoke(Method),
                $"Should not run it on current method: {Method}"
                );
            if (Method.GetMethodBody() == null)
                return;

            MidRepresentation.Vars.SetupLocalVariables(Method);
            var midRepresentationBuilder = new MethodMidRepresentationBuilder(this, Method);
            midRepresentationBuilder.ProcessInstructions(closureEntities);
            Interpreted = true;
        }

        public override string ToString()
        {
            var method = Method;
            var declaringName = method.DeclaringType.Name;
            if (method.IsConstructor)
            {
                return $"{declaringName}.ctor";
            }

            var startName = $"{declaringName}.{method.Name}";
            var declaringParams = string.Join(", ",
                method.GetParameters().Select(p => p.ParameterType.Name)
                ).ToArray();

            return $"{startName}({declaringParams})";
        }
    }
}