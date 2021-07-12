#region Uses

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.UseDefs
{
    public class UseDefDescription
    {
        readonly Dictionary<OperationKind, int[]> _instructionMix = new Dictionary<OperationKind, int[]>();
        volatile LocalVariable[] _definitions = { };
        Dictionary<int, int> _labelTable;
        LocalOperation[] _operations;
        volatile LocalVariable[][] _usages = { };

        public void Update(LocalOperation[] operations)
        {
            _operations = operations;
            _usages = new LocalVariable[operations.Length][];
            _definitions = new LocalVariable[operations.Length];


            var instructionMix = BuildInstructionMix(operations);
            SetInstructionMixToField(instructionMix);

            UpdateLabelsTable(operations);
        }

        void UpdateLabelsTable(LocalOperation[] operations)
        {
            var labelOperations = GetOperationsOfKind(OperationKind.Label);

            _labelTable = InstructionsUtils.BuildLabelTable(operations, labelOperations);
        }

        void SetInstructionMixToField(Dictionary<OperationKind, List<int>> instructionMix)
        {
            _instructionMix.Clear();
            foreach (var instruction in instructionMix)
            {
                _instructionMix.Add(instruction.Key, instruction.Value.ToArray());
            }
        }

        public static List<LocalVariable> ComputeUnusedArguments(
            List<LocalVariable> argList,
            UseDefDescription useDef)
        {
            var allUsages = useDef.GetAllUsedVariables();
            var stillUnused = new List<LocalVariable>();
            var unusedArguments = new HashSet<LocalVariable>(argList);
            foreach (var unusedArgument in unusedArguments)
            {
                if (!allUsages.Contains(unusedArgument))
                    stillUnused.Add(unusedArgument);
            }
            return stillUnused.ToList();
        }

        Dictionary<OperationKind, List<int>> BuildInstructionMix(LocalOperation[] operations)
        {
            var instructionMix = new Dictionary<OperationKind, List<int>>();
            for (var index = 0; index < operations.Length; index++)
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

        public int[] GetOperationsOfKind(OperationKind binaryOperator)
        {
            int[] list;
            return _instructionMix.TryGetValue(binaryOperator, out list) ? list : new int[0];
        }
    }
}