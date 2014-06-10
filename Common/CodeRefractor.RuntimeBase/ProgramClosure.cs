#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations.ConstParameters;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations.Virtual;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase.Backend.ProgramWideOptimizations;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.TypeInfoWriter;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public class ProgramClosure
    {
        public List<Type> UsedTypes;
        public MethodInterpreter EntryInterpreter { get; set; }
        public CrRuntimeLibrary Runtime { get; set; }

        public Dictionary<MethodInterpreterKey, MethodInterpreter> MethodClosure =
            new Dictionary<MethodInterpreterKey, MethodInterpreter>();

        private readonly Dictionary<Type, int> _typeDictionary;

        private VirtualMethodTable _virtualMethodTable;

        public ProgramClosure(MethodInfo entryMethod, CrRuntimeLibrary crRuntime)
        {
            var optimizationsTable = BuildProgramWideOptimizationsTable();
            Runtime = crRuntime;
            Runtime.Closure = this;
            EntryInterpreter = entryMethod.Register();

            ResultingInFunctionOptimizationPass.Runtime = Runtime;
            ResultingGlobalOptimizationPass.Runtime = Runtime;
            BuildMethodClosure();
            MetaLinkerOptimizer.ApplyOptimizations(MethodClosure.Values.ToList());
            if (optimizationsTable.Optimize(this))
                MetaLinkerOptimizer.ApplyOptimizations(MethodClosure.Values.ToList());

            //BuildMethodClosure();
            if (!UsedTypes.Contains(typeof (object)))
            {
                UsedTypes.Add(typeof (object));
            }
            TypesClosureLinker.SortTypeClosure(UsedTypes, crRuntime);

            var typeTable = new TypeDescriptionTable(UsedTypes);
            _typeDictionary = typeTable.ExtractInformation();

            BuildVirtualMethodTable(typeTable, MethodClosure);
        }

        private static ProgramOptimizationsTable BuildProgramWideOptimizationsTable()
        {
            var optimizationsTable = new ProgramOptimizationsTable();
            optimizationsTable.Add(new DevirtualizerIfOneImplemetor());
            optimizationsTable.Add(new CallToFunctionsWithSameConstant());
            return optimizationsTable;
        }

        private void BuildVirtualMethodTable(TypeDescriptionTable typeTable,
            Dictionary<MethodInterpreterKey, MethodInterpreter> methodClosure)
        {
            _virtualMethodTable = new VirtualMethodTable(typeTable);
            foreach (var type in _typeDictionary.Keys)
            {
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    _virtualMethodTable.RegisterMethod(method, methodClosure);
                }
            }
        }

        private void BuildMethodClosure()
        {
            var entryInterpreter = EntryInterpreter;

            var result = TypesClosureLinker.BuildClosureForEntry(entryInterpreter, this);

            MethodClosure = result.MethodInterpreters;

            UsedTypes = result.UsedTypes.ToList();
            foreach (var methodInterpreter in MethodClosure)
            {
                methodInterpreter.Value.Process(Runtime);

                methodInterpreter.Value.MidRepresentation.UpdateUseDef();
            }
        }

        public static void ComputeEscapeAnalysis(List<MethodInterpreter> methodClosures)
        {
            var escapeParameters = new AnalyzeParametersAreEscaping();
            var loweringVars = new InFunctionLoweringVars();
            for (var i = 0; i < 5; i++)
            {
                foreach (var methodInterpreter in methodClosures)
                {
                    escapeParameters.Optimize(methodInterpreter);
                }
                //Parallel.ForEach(methodClosures, methodInterpreter =>
                foreach (var methodInterpreter in methodClosures)
                    loweringVars.Optimize(methodInterpreter);
                //);
            }
        }

        public StringBuilder BuildFullSourceCode(CrRuntimeLibrary crRuntime)
        {
            ComputeEscapeAnalysis(MethodClosure.Values.ToList());

            return CppCodeGenerator.GenerateSourceStringBuilder(EntryInterpreter, UsedTypes,
                MethodClosure.Values.ToList(), _virtualMethodTable, crRuntime);
        }
    }
}