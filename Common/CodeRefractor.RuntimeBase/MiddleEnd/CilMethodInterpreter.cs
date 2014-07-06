using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CodeRefractor.FrontEnd;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;

namespace CodeRefractor.MiddleEnd
{
    public class CilMethodInterpreter : MethodInterpreter
    {
        public CilMethodInterpreter(MethodBase method) : base(method)
        {
            Kind = MethodKind.Default;
        }


        public MetaMidRepresentation MidRepresentation = new MetaMidRepresentation();

        public bool Interpreted { get; set; }

        public void Process()
        {
            if (Kind != MethodKind.Default)
                return;
            if (Interpreted)
                return;
            if (PlatformInvokeMethod.IsPlatformInvoke(Method))
                throw new InvalidDataException(string.Format(
                    "Should not run it on current method: {0}", Method)
                    );
            if (Method.GetMethodBody() == null)
                return;
            var midRepresentationBuilder = new MethodMidRepresentationBuilder(this, Method);
            midRepresentationBuilder.ProcessInstructions();
            Interpreted = true;
        }


        public Dictionary<int, int> GetLabelTable()
        {
            return MidRepresentation.UseDef.GetLabelTable();
        }
    }
}