using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UseDefDescription
    {
        private LocalVariable[][] _usages = {};
        private LocalVariable[] _definitions = { };

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
        }

        public LocalVariable[] GetUsages(int i)
        {
            return _usages[i];
        }

        public LocalVariable GetDefinition(int index)
        {
            return _definitions[index];
        }
    }
}