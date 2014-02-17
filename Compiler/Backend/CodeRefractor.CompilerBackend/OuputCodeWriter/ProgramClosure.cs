#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeRefactor.OpenRuntime;
using CodeRefractor.CodeWriter.TypeInfoWriter;
using CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public class ProgramClosure
    {
        public List<Type> UsedTypes;
        public MethodInterpreter EntryInterpreter { get; set; }
        public List<MethodInterpreter> MethodClosure = new List<MethodInterpreter>();
        private TypeDescriptionTable _typeTable;
        private Dictionary<Type, int> _typeDictionary;

        VirtualMethodTable _virtualMethodTable;

        public ProgramClosure(MethodInfo entryMethod)
        {
            EntryInterpreter = entryMethod.Register();

            BuildMethodClosure();
            MetaLinkerOptimizer.ApplyOptimizations(MethodClosure);
            BuildMethodClosure();
            UsedTypes.Add(typeof (CrString));
            TypesClosureLinker.SortTypeClosure(UsedTypes);

            _typeTable = new TypeDescriptionTable(UsedTypes);
            _typeDictionary = _typeTable.ExtractInformation();

            BuildVirtualMethodTable(_typeTable);
        }

        private void BuildVirtualMethodTable(TypeDescriptionTable typeTable)
        {
            _virtualMethodTable = new VirtualMethodTable(typeTable);
            foreach (var type in _typeDictionary.Keys)
            {
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    _virtualMethodTable.RegisterMethod(method);
                }
            }
        }

        private void BuildMethodClosure()
        {
            var entryInterpreter = EntryInterpreter;

            var result = TypesClosureLinker.BuildClosureForEntry(entryInterpreter);

            MethodClosure = result.MethodInterpreters;
            UsedTypes = result.UsedTypes;
        }

        public static void ComputeEscapeAnalysis(List<MethodInterpreter> methodClosures)
        {
            var escapeParameters = new AnalyzeParametersAreEscaping();
            var loweringVars = new InFunctionLoweringVars();
            for (var i = 0; i < 2; i++)
            {
                foreach (var methodInterpreter in methodClosures)
                {
                    escapeParameters.Optimize(methodInterpreter);
                }
                Parallel.ForEach(methodClosures, methodInterpreter =>
                    //foreach (var methodInterpreter in methodClosures)
                { loweringVars.Optimize(methodInterpreter); }
                    );
            }
        }

        public StringBuilder BuildFullSourceCode()
        {
            ComputeEscapeAnalysis(MethodClosure);

            return CppCodeGenerator.GenerateSourceStringBuilder(EntryInterpreter, UsedTypes, MethodClosure, _virtualMethodTable);
        }
    }
}