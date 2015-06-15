#region Uses

using System;
using System.Linq;
using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public abstract class MethodInterpreter
    {
        public MethodInterpreter(MethodBase method)
        {
            Method = method;
            AnalyzeProperties = new AnalyzeProperties();
            AnalyzeProperties.SetupArguments(Method);

            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                AnalyzeProperties.IsPure = true;
        }

        public MethodBase Method { get; set; }
        public MethodKind Kind { get; set; }
        public AnalyzeProperties AnalyzeProperties { get; }
        public Type OverrideDeclaringType { get; set; }

        public override string ToString()
        {
            var typeName = Method.DeclaringType.Name;
            var name = Method.Name;
            var parameters = Method.GetParameters();
            var argsText = string.Join(", ", parameters.Select(par =>
                string.Format("{0}: {1}", par.Name, par.ParameterType.Name)));
            return string.Format("{0}.{1}({2})", typeName, name, argsText);
        }
    }
}