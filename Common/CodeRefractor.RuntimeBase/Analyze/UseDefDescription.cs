using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UseDefDescription
    {
        private LocalVariable[][] _usages = {};
        private LocalVariable[] _definitions = { };

        private Dictionary<int,int> _labelTable = new Dictionary<int, int>();

        public void Update(LocalOperation[] operations)
        {
            _usages= new LocalVariable[operations.Length][];
            _definitions=new LocalVariable[operations.Length];
            for (int index = 0; index < operations.Length; index++)
            {
                var operation = operations[index];
                var operationUsages = operation.GetUsages();

                _usages[index] = operationUsages.ToArray();
                _definitions[index] = operation.GetDefinition();
            }
            _labelTable = InstructionsUtils.BuildLabelTable(operations);
        }

        public LocalVariable[] GetUsages(int i)
        {
            return _usages[i];
        }

        public LocalVariable GetDefinition(int index)
        {
            return _definitions[index];
        }

        public Dictionary<int, int> GetLabelTable()
        {
            return _labelTable;
        }
    }
}