#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.ConstantDfa;
using CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.Operator;
using CodeRefractor.CompilerBackend.Optimizations.Jumps;
using CodeRefractor.CompilerBackend.Optimizations.ReachabilityDfa;
using CodeRefractor.CompilerBackend.Optimizations.SimpleDce;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Util
{
    public class OptimizationLevels : OptimizationLevelBase
    {
        public override List<OptimizationPass> BuildOptimizationPasses2()
        {
            return new OptimizationPass[]
                       {
                           new ReachabilityLines(),
                           new ConstantDfaAnalysis(),
                           new EvaluatePureFunctionWithConstantCall(),
                           new VRegVariablePropagation()
                       }.ToList();
        }


        public override List<OptimizationPass> BuildOptimizationPasses1()
        {
            return new OptimizationPass[]
                       {
                           new ConstantVariablePropagation(),
                           new ConstantVariableOperatorPropagation(),
                           new ConstantVariablePropagationInCall(),
                           new DeleteJumpNextLine(),
                           new RemoveUnreferencedLabels(),
                           new MergeConsecutiveLabels(),
                           new ConstantVariableBranchOperatorPropagation(),
                           new OperatorConstantFolding(),
                           new DceVRegUnused(),

                           new DoubleAssignPropagation(),
                           new AssignToReturnPropagation(),
                           new DeadStoreLastSequenceRemover(),
                           new DeadStoreAssignment(), 
                           new DceLocalAssigned(),
                           new OperatorPartialConstantFolding(),
                           new OneDefUsedNextLinePropagation(), 
                       }.ToList();
        }


        public override List<OptimizationPass> BuildOptimizationPasses0()
        {
            return new OptimizationPass[]
                       {
                       }.ToList();
        }
    }
}