#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Dfa.ConstantDfa
{
    internal class DfaPointOfAnalysis
    {
        public readonly Dictionary<LocalVariable, VariableState> States = new Dictionary<LocalVariable, VariableState>();

        public ConstValue GetConstVariableState(LocalVariable localVariable)
        {
            if (localVariable == null)
                return null;
            VariableState variableState;
            if (States.TryGetValue(localVariable, out variableState) &&
                variableState.State == VariableState.ConstantState.Constant)
            {
                return variableState.Constant;
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("Consts: {0} ({1})", States.Count, string.Join(", ", States.Keys.Select(v => v.Name)));
        }

        public DfaPointOfAnalysis Merge(DfaPointOfAnalysis pointsOfAnalysis)
        {
            var result = new DfaPointOfAnalysis();
            var combinedList = new List<Tuple<LocalVariable, VariableState>>();
            foreach (var variableState in States)
            {
                combinedList.Add(new Tuple<LocalVariable, VariableState>(variableState.Key, variableState.Value));
            }
            if (pointsOfAnalysis != null)
            {
                foreach (var variableState in pointsOfAnalysis.States)
                {
                    combinedList.Add(new Tuple<LocalVariable, VariableState>(variableState.Key, variableState.Value));
                }
            }
            foreach (var tuple in combinedList)
            {
                VariableState variableState;
                if (result.States.TryGetValue(tuple.Item1, out variableState))
                {
                    if (!tuple.Item2.Equals(variableState))
                    {
                        result.States[tuple.Item1] = new VariableState
                        {
                            State = VariableState.ConstantState.NotConstant
                        };
                    }
                }
                else
                {
                    result.States[tuple.Item1] = tuple.Item2;
                }
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as DfaPointOfAnalysis;
            if (other == null)
                return false;
            return Compare(other);
        }

        private bool Compare(DfaPointOfAnalysis other)
        {
            if (States.Count != other.States.Count)
                return false;
            foreach (var variableState in States)
            {
                VariableState result;
                if (!other.States.TryGetValue(variableState.Key, out result))
                    return false;
                if (!result.Compare(variableState.Value))
                    return false;
            }
            return true;
        }

        public bool Equals(DfaPointOfAnalysis other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Compare(other);
        }

        public override int GetHashCode()
        {
            return (States != null ? States.GetHashCode() : 0);
        }
    }
}