#region Usings

using System.Collections.Generic;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation
{
    public abstract class ConstantVariablePropagationBase : ResultingOptimizationPass
    {
        protected static ConstValue GetConstantFromOperation(List<LocalOperation> operations, int i,
            out Assignment srcVariableDefinition)
        {
            var srcOperation = operations[i];
            srcVariableDefinition = null;
            return srcOperation.Kind != LocalOperation.Kinds.Assignment
                ? null
                : ConstantFromOperation(out srcVariableDefinition, srcOperation);
        }

        public static ConstValue ConstantFromOperation(out Assignment srcVariableDefinition, LocalOperation srcOperation)
        {
            srcVariableDefinition = srcOperation.Value as Assignment;
            if (srcVariableDefinition == null)
                return null;
            var constValue = srcVariableDefinition.Right as ConstValue;
            return constValue;
        }

        public static bool SameVariable(LocalVariable src, LocalVariable dest)
        {
            return
                src != null
                && dest != null
                && (src.Kind == dest.Kind
                    && src.Id == dest.Id);
        }
    }
}