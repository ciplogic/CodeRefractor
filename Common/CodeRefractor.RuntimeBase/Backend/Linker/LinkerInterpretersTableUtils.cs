#region Usings

using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Optimizations.Purity;
using CodeRefractor.Runtime;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Linker
{
    public static class LinkerInterpretersTableUtils
    {
        public static bool ReadPurity(MethodBase methodBase, ClosureEntities crRuntime)
        {
            var method = methodBase.GetInterpreter(crRuntime);
            return AnalyzeFunctionPurity.ReadPurity(method as CilMethodInterpreter);
        }

        public static bool ReadNoStaticSideEffects(MethodBase methodBase, ClosureEntities crRuntime)
        {
            var method = methodBase.GetInterpreter(crRuntime) as CilMethodInterpreter;
            if (method!=null&& method.MidRepresentation != null)
            {
                return method.AnalyzeProperties.IsReadOnly;
            }
            return false;
        }
    }
}