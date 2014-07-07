using System.Reflection;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.MiddleEnd
{
    public class CppMethodInterpreter : MethodInterpreter
    {
        public readonly CppRepresentation CppRepresentation = new CppRepresentation();
        public CppMethodInterpreter(MethodBase method) : base(method)
        {
            Kind=MethodKind.RuntimeCppMethod;
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