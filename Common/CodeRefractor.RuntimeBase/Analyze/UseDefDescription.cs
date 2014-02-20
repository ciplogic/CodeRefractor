using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UseDefDescription
    {
        private volatile LocalVariable[][] _usages = { };
        private volatile LocalVariable[] _definitions = { };

        private Dictionary<int, int> _labelTable;
        private Dictionary<OperationKind, int[]> _instructionMix = new Dictionary<OperationKind, int[]>();
        private LocalOperation[] _operations;


        private readonly HashSet<string> _unusedArguments = new HashSet<string>();

        public HashSet<string> UnusedArguments
        {
            get { return _unusedArguments; }
        }

        public void Update(LocalOperation[] operations)
        {
            _operations = operations;
            _usages = new LocalVariable[operations.Length][];
            _definitions = new LocalVariable[operations.Length];


            var instructionMix = BuildInstructionMix(operations);
            SetInstructionMixToField(instructionMix);

            UpdateLabelsTable(operations);
        }

        private void UpdateLabelsTable(LocalOperation[] operations)
        {
            var labelOperations = GetOperations(OperationKind.Label);

            _labelTable = InstructionsUtils.BuildLabelTable(operations, labelOperations);
        }

        private void SetInstructionMixToField(Dictionary<OperationKind, List<int>> instructionMix)
        {
            _instructionMix.Clear();
            foreach (var instruction in instructionMix)
            {
                _instructionMix.Add(instruction.Key, instruction.Value.ToArray());
            }
        }

        public void ComputeUnusedArguments(List<LocalVariable> argList)
        {
            var unusedArguments = new HashSet<LocalVariable>(argList);
            for (int index = 0; index < _operations.Length; index++)
            {
                var usages = GetUsages(index);
                var definition = GetDefinition(index);
                if (definition != null)
                    unusedArguments.Remove(definition);
                foreach (var usage in usages)
                {
                    unusedArguments.Remove(usage);
                }
                if(unusedArguments.Count==0)
                    return;
            }
            UnusedArguments.Clear();
            foreach (var argument in unusedArguments)
            {
                UnusedArguments.Add(argument.Name);
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

        public Dictionary<int, int> GetLabelTable(bool doClone = false)
        {
            return !doClone ? _labelTable : new Dictionary<int, int>(_labelTable);
        }

        public LocalOperation[] GetLocalOperations()
        {
            return _operations;
        }

        public int[] GetOperations(OperationKind binaryOperator)
        {
            int[] list;
            return _instructionMix.TryGetValue(binaryOperator, out list) ? list : new int[0];
        }
    }
}