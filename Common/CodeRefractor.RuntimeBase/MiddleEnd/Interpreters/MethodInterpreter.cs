#region Usings

using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public class MethodInterpreter
    {
        public MethodBase Method { get; set; }
        public MethodKind Kind { get; set; }

        public readonly AnalyzeProperties AnalyzeProperties = new AnalyzeProperties();


        public MethodInterpreter(MethodBase method)
        {
            Method = method;
            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                AnalyzeProperties.IsPure = true;

            AnalyzeProperties.SetupArguments(Method);
        }

        public override string ToString()
        {
            return Method.ToString();
        }


    }
}