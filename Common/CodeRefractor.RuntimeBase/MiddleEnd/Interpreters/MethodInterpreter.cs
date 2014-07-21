#region Usings

using System;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public abstract class MethodInterpreter
    {
        public MethodBase Method { get; set; }
        public MethodKind Kind { get; set; }

        public readonly AnalyzeProperties AnalyzeProperties = new AnalyzeProperties();
        public Type OverrideDeclaringType { get; set; }

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
            var typeName = Method.DeclaringType.Name;
            var name = Method.Name;
            var parameters = Method.GetParameters();
            var argsText = String.Join(", ", parameters.Select(par =>
                String.Format("{0}: {1}", par.Name, par.ParameterType.Name)));
            return string.Format("{0}.{1}({2})",typeName, name, argsText);
        }


    }
}