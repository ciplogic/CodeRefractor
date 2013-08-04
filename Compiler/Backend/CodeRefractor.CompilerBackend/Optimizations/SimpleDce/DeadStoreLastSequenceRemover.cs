using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.Compiler.Optimizations.SimpleDce
{
    /// <summary>
    /// This optimization will remove all assignments that are not used subsequently
    /// from the last sequence of instructions (from the last return to the last found label, jump 
    /// or start of the function
    /// </summary>
    internal class DeadStoreLastSequenceRemover : ResultingOptimizationPass
    {
        private int _endSequence;
        private List<LocalVariable> _readVariables;
        private int _startSequence;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOperations = intermediateCode.LocalOperations;
            ComputeSequenceRange(localOperations);

            _readVariables = new List<LocalVariable>();
            for (var index = _endSequence; index >= _startSequence; index--)
            {
                var instruction = localOperations[index];
                ProcessInstructionReads(instruction);
                var isDeadStoreRead = ProcessInstructionWrites(instruction);
                if (!isDeadStoreRead)
                    continue;
                localOperations.RemoveAt(index);
                Result = true;
                return;
            }
        }

        private void ComputeSequenceRange(List<LocalOperation> localOperations)
        {
            _endSequence = localOperations.Count - 1;
            var i = _endSequence;
            var startFound = false;
            while (i >= 0)
            {
                var instruction = localOperations[i];

                switch (instruction.Kind)
                {
                    case LocalOperation.Kinds.Label:
                    case LocalOperation.Kinds.AlwaysBranch:
                    case LocalOperation.Kinds.BranchOperator:
                        startFound = true;
                        break;
                }
                if (startFound)
                    break;
                i--;
            }
            _startSequence = i + 1;
        }

        #region Handle Writes

        private bool ProcessInstructionWrites(LocalOperation instruction)
        {
            switch (instruction.Kind)
            {
                case LocalOperation.Kinds.Assignment:
                    return HandleAssignWrites(instruction);
            }
            return false;
        }

        private bool HandleAssignWrites(LocalOperation instruction)
        {
            var assign = (Assignment) instruction.Value;
            var left = assign.Left;
            var removeRead = RemoveRead(left);
            return !removeRead;
        }

        #endregion

        #region Handle Reads

        private void ProcessInstructionReads(LocalOperation instruction)
        {
            switch (instruction.Kind)
            {
                case LocalOperation.Kinds.Return:
                    HandleReturnReads(instruction);
                    break;
                case LocalOperation.Kinds.Assignment:
                    HandleAssignReads(instruction);
                    break;
                case LocalOperation.Kinds.Call:
                    HandleCallReads(instruction);
                    break;

                default:
                    throw new NotImplementedException("Instruction not supported, skipped the optimization pass");
            }
        }

        private void HandleCallReads(LocalOperation instruction)
        {
            var methodData = (MethodData) instruction.Value;
            foreach (var identifierValue in methodData.Parameters)
            {
                AddRead(identifierValue);
            }
        }

        private void HandleAssignReads(LocalOperation instruction)
        {
            var assign = (Assignment) instruction.Value;
            AddRead(assign.Right);
        }

        private void HandleReturnReads(LocalOperation instruction)
        {
            if (instruction.Value == null) return;
            var returnAssign = (IdentifierValue) instruction.Value;
            AddRead(returnAssign);
        }

        #endregion

        #region Read/Writes helpers

        public void AddRead(IdentifierValue variable)
        {
            var localVariable = variable as LocalVariable;
            if (localVariable == null)
                return;
            if (DoesRead(localVariable))
                return;
            _readVariables.Add(localVariable);
        }

        public bool DoesRead(LocalVariable localVariable)
        {
            return _readVariables.Any(readVariable => readVariable.Equals(localVariable));
        }

        /// <summary>
        /// Returns true if the removal of the read was already in the subsequent reads.
        /// </summary>
        /// <param name="localVariable"></param>
        /// <returns></returns>
        public bool RemoveRead(LocalVariable localVariable)
        {
            if (!DoesRead(localVariable))
                return false;

            _readVariables.RemoveAll(readVariable => readVariable.Equals(localVariable));
            return true;
        }

        #endregion
    }
}