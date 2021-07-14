#region Uses

using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime.Annotations;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters.NonCil
{
    public class CppMethodInterpreter : MethodInterpreter
    {
        public readonly CppRepresentation CppRepresentation = new CppRepresentation();

        public CppMethodInterpreter(MethodBase method) : base(method)
        {
            Kind = MethodKind.RuntimeCppMethod;
        }

        public static bool IsCppMethod(MethodBase method)
        {
            return GetCppAttribute(method) != null;
        }

        public void SetupInternalFields(MethodBase resultMethod)
        {
            var cppAttribute = GetCppAttribute(resultMethod);

            var cppRepresentation = CppRepresentation;
            cppRepresentation.Kind = CppKinds.RuntimeLibrary;
            cppRepresentation.Header = cppAttribute.Header;
            cppRepresentation.Source = cppAttribute.Code;
            var pureAttribute = resultMethod.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                AnalyzeProperties.IsPure = true;
        }

        private static CppMethodBodyAttribute GetCppAttribute(MethodBase resultMethod)
        {
            return resultMethod.GetCustomAttribute<CppMethodBodyAttribute>();
        }
    }
}