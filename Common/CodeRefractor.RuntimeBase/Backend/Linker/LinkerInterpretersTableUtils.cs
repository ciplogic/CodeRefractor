#region Usings

using System.Reflection;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.Linker
{
    public static class LinkerInterpretersTableUtils
    {
        public static bool ReadPurity(MethodBase methodBase)
        {
            var method = methodBase.GetInterpreter();
            return AnalyzeFunctionPurity.ReadPurity(method);
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