#region Usings

using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Runtime;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MetaLinkerOptimizer
    {
        public static void OptimizeMethods(bool doInline = false)
        {
            LinkerInterpretersTable.Instance.Clear();
            foreach (var methodBase in GlobalMethodPool.Instance.MethodInfos)
            {
                var typeData =
                    (ClassTypeData)ProgramData.UpdateType(
                        methodBase.Value.DeclaringType);
                var interpreter = typeData.GetInterpreter(methodBase.Value);
                if(interpreter.Kind==MethodKind.PlatformInvoke)
                    continue;
                LinkerInterpretersTable.Register(interpreter.MidRepresentation);
            }

            foreach (var usedMethod in CrRuntimeLibrary.Instance.UsedCppMethods)
            {
                LinkerInterpretersTable.Instance.RegisterRuntimeMethod(usedMethod);
            }
            bool doOptimize;
            do
            {
                doOptimize = false;
                foreach (var methodBase in LinkerInterpretersTable.Instance.Methods.Values)
                {
                    var interpreter = methodBase.GetInterpreter();
                    var codeWriter = new MethodInterpreterCodeWriter
                                         {
                                             Interpreter = interpreter
                                         };
                    doOptimize = codeWriter.ApplyLocalOptimizations(
                        CommandLineParse.SortedOptimizations[OptimizationKind.InFunction]);
                }
                foreach (var methodBase in LinkerInterpretersTable.Instance.Methods.Values)
                {
                    var interpreter = methodBase.GetInterpreter();
                    var codeWriter = new MethodInterpreterCodeWriter
                                         {
                                             Interpreter = interpreter
                                         };

                    doOptimize = codeWriter.ApplyLocalOptimizations(
                        CommandLineParse.SortedOptimizations[OptimizationKind.Global]);
                }
            } while (doOptimize); 

            if (doInline)
                InlineMethods();
        }


        private static void InlineMethods()
        {
            var inliner = new SmallFunctionsInliner();
            foreach (var methodBase in GlobalMethodPool.Instance.MethodInfos)
            {
                var typeData = (ClassTypeData) ProgramData.UpdateType(methodBase.Value.DeclaringType);
                var interpreter = typeData.GetInterpreter(methodBase.Value);

                inliner.OptimizeOperations(interpreter.MidRepresentation);
            }
        }
    }
}