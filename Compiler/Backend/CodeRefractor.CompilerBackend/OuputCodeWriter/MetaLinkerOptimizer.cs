#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.FrontEnd;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public class MetaLinkerOptimizer
    {
        private readonly MetaLinker _metaLinker;

        public MetaLinkerOptimizer(MetaLinker metaLinker)
        {
            _metaLinker = metaLinker;
        }


        public static void OptimizeMethods(bool doInline = false)
        {
            var optimizationPasses = CommandLineParse.OptimizationPasses;
            foreach (var methodBase in GlobalMethodPool.Instance.MethodInfos)
            {
                var typeData =
                    (ClassTypeData) ProgramData.UpdateType(
                        methodBase.Value.DeclaringType);
                var interpreter = typeData.GetInterpreter(methodBase.Key);
                if (optimizationPasses == null) return;
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