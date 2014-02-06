using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UseDefDescription
    {
        private volatile LocalVariable[][] _usages = { };
        private volatile LocalVariable[] _definitions = { };

        private volatile Dictionary<int, int> _labelTable;
        private volatile Dictionary<OperationKind, int[]> _instructionMix = new Dictionary<OperationKind, int[]>();

        public void Update(LocalOperation[] operations)
        {
            _usages = new LocalVariable[operations.Length][];
            _definitions = new LocalVariable[operations.Length];


            var instructionMix = BuildInstructionMix(operations);
            SetMigracionMixToField(instructionMix);

            UpdateLabelsTable(operations);

            if (GetOperations(OperationKind.Label).Length != 0
                && _labelTable.Count == 0
                )
            {
                int x = 0;
            }
            
        }

        private void UpdateLabelsTable(LocalOperation[] operations)
        {
            var labelOperations = GetOperations(OperationKind.Label);

            _labelTable = InstructionsUtils.BuildLabelTable(operations, labelOperations);
        }

        private void SetMigracionMixToField(Dictionary<OperationKind, List<int>> instructionMix)
        {
            _instructionMix.Clear();
            foreach (var instruction in instructionMix)
            {
                _instructionMix.Add(instruction.Key, instruction.Value.ToArray());
            }
        }

        private Dictionary<OperationKind, List<int>> BuildInstructionMix(LocalOperation[] operations)
        {
            var instructionMix = new Dictionary<OperationKind, List<int>>();
            for (int index = 0; index < operations.Length; index++)
            {
                var operation = operations[index];
                var operationUsages = operation.GetUsages();

                _usages[index] = operationUsages.ToArray();
                _definitions[index] = operation.GetDefinition();
                List<int> list;
                if (!instructionMix.TryGetValue(operation.Kind, out list))
                {
                    list = new List<int>();
                    instructionMix[operation.Kind] = list;
                }
                list.Add(index);
            }
            return instructionMix;
        }

        public LocalVariable[] GetUsages(int i)
        {
            var usages = _usages[i];
            if (usages == null)
            {

            }
            return usages;
        }

        public LocalVariable GetDefinition(int index)
        {
            return _definitions[index];
        }

        public Dictionary<int, int> GetLabelTable()
        {
            return _labelTable;
        }

        public int[] GetOperations(OperationKind binaryOperator)
        {
            int[] list;
            return _instructionMix.TryGetValue(binaryOperator, out list) ? list : new int[0];
        }
    }
}