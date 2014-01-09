using System.Reflection;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.CompilerBackend.Linker
{
    public static class LinkerInterpretersTableUtils
    {
        public static bool ReadPurity(MethodBase methodBase)
        {
            var method = methodBase.GetInterpreter();
            if (method != null)
            {
                return AnalyzeFunctionPurity.ReadPurity(method);
            }

            var methodRuntimeInfo = methodBase.GetMethodDescriptor();
            if (!LinkerInterpretersTable.RuntimeMethods.ContainsKey(methodRuntimeInfo))
                return false;
            var runtimeMethod = LinkerInterpretersTable.RuntimeMethods[methodRuntimeInfo];
            return runtimeMethod.GetCustomAttribute<PureMethodAttribute>() != null;
        }

        public static bool ReadNoStaticSideEffects(MethodBase methodBase)
        {
            var method = methodBase.GetInterpreter().MidRepresentation;
            if (method != null)
            {
                return method.GetProperties().IsReadOnly;
            }
            return false;
        }
    }
}