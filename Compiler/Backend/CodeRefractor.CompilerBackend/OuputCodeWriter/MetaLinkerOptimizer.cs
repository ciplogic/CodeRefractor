#region Usings

using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.FrontEnd;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MetaLinkerOptimizer
    {
        public static void OptimizeMethods(bool doInline = false)
        {
            var optimizationPasses = CommandLineParse.OptimizationPasses;
            foreach (var methodBase in GlobalMethodPool.Instance.MethodInfos)
            {
                var typeData =
                    (ClassTypeData)ProgramData.UpdateType(
                        methodBase.Value.DeclaringType);
                var interpreter = typeData.GetInterpreter(methodBase.Key);
                if (optimizationPasses == null) return;
                LinkerInterpretersTable.Register(interpreter.MidRepresentation);
            }
            foreach (var methodBase in GlobalMethodPool.Instance.MethodInfos)
            {
                var typeData =
                    (ClassTypeData)ProgramData.UpdateType(
                        methodBase.Value.DeclaringType);
                var interpreter = typeData.GetInterpreter(methodBase.Key);
                var codeWriter = new MethodInterpreterCodeWriter
                                     {
                                         Interpreter = interpreter
                                     };
                codeWriter.ApplyLocalOptimizations(
                    optimizationPasses);
            }


            if (doInline)
                InlineMethods();
        }


        private static void InlineMethods()
        {
            var inliner = new SmallFunctionsInliner();
            foreach (var methodBase in GlobalMethodPool.Instance.MethodInfos)
            {
                var typeData = (ClassTypeData) ProgramData.UpdateType(methodBase.Value.DeclaringType);
                var interpreter = typeData.GetInterpreter(methodBase.Key);

                inliner.OptimizeOperations(interpreter.MidRepresentation);
            }
        }
    }
}