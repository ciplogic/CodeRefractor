using System.IO;
using System.Linq;
using System.Reflection;
using CodeRefractor.FrontEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;

namespace CodeRefractor.MiddleEnd
{
    public class CilMethodInterpreter : MethodInterpreter
    {
        public CilMethodInterpreter(MethodBase method)
            : base(method)
        {
            Kind = MethodKind.CilInstructions;
        }

        public readonly MetaMidRepresentation MidRepresentation = new MetaMidRepresentation();

        public bool Interpreted { get; set; }

        public void Process()
        {
            if (Kind != MethodKind.CilInstructions)
                return;
            if (Interpreted)
                return;
            if (PlatformInvokeMethod.IsPlatformInvoke(Method))
                throw new InvalidDataException(string.Format(
                    "Should not run it on current method: {0}", Method)
                    );
            if (Method.GetMethodBody() == null)
                return;

            MidRepresentation.Vars.SetupLocalVariables(Method);
            var midRepresentationBuilder = new MethodMidRepresentationBuilder(this, Method);
            midRepresentationBuilder.ProcessInstructions();
            Interpreted = true;
        }

        public override string ToString()
        {
            var method = Method;
            var declaringName = method.DeclaringType.Name;
            if (method.IsConstructor)
            {
                return string.Format("{0}.ctor", declaringName);
            }

            var startName = string.Format("{0}.{1}", declaringName, method.Name);
            var declaringParams = string.Join(", ",
                method.GetParameters().Select(p => p.ParameterType.Name)
                ).ToArray();

            return string.Format("{0}({1})",startName, declaringParams);
        }
    }
}