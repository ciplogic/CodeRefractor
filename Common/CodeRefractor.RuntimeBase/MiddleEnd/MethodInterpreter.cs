#region Usings

using System;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.MiddleEnd
{
    public class MethodInterpreter
    {
        public MethodBase Method { get; set; }
        public MethodKind Kind { get; set; }

        public readonly AnalyzeProperties AnalyzeProperties = new AnalyzeProperties();


        public bool Interpreted { get; set; }

        public MethodInterpreter(MethodBase method)
        {
            Method = method;
            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                AnalyzeProperties.IsPure = true;
            
        }

        public override string ToString()
        {
            return Method.ToString();
        }


    }
}