#region Usings

using System.Reflection;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Runtime;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Linker
{
    public static class LinkerInterpretersTableUtils
    {
        public static bool ReadPurity(MethodBase methodBase, CrRuntimeLibrary crRuntime)
        {
            var method = methodBase.GetInterpreter(crRuntime);
            return AnalyzeFunctionPurity.ReadPurity(method);
        }

        public static bool ReadNoStaticSideEffects(MethodBase methodBase, CrRuntimeLibrary crRuntime)
        {
            var method = methodBase.GetInterpreter(crRuntime).MidRepresentation;
            if (method != null)
            {
                return method.GetProperties().IsReadOnly;
            }
            return false;
        }
    }
}