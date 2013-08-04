using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.Compiler.Config;
using CodeRefractor.Compiler.Optimizations;
using CodeRefractor.Compiler.Optimizations.ConstantDfa;
using CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.Compiler.Optimizations.Jumps;
using CodeRefractor.Compiler.Optimizations.ReachabilityDfa;
using CodeRefractor.Compiler.Optimizations.SimpleDce;
using CodeRefractor.RuntimeBase.Optimizations;

namespace CodeRefractor.CompilerBackend.Optimizations.Util
{
    public class OptimizationLevels : OptimizationLevelBase
    {
        public override List<OptimizationPass> BuildOptimizationPasses2()
        {
            return new OptimizationPass[]
                       {
                           new DeleteGappingVregAssignment(),
                
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
                           new DoubleAssignPropagation(), 
                           new AssignToReturnPropagation(), 
                           new DeadStoreLastSequenceRemover(), 

                           new DceLocalAssigned(), 
                                             
                           new OperatorPartialConstantFolding(), 
                       }.ToList();

        }


        public override List<OptimizationPass> BuildOptimizationPasses0()
        {
            return new OptimizationPass[]
                       {
                           new DeleteVregAssignedAndUsedNextLine(), 
                           new DeleteVregAssignedVariableAndUsedNextLine(), 
                           new DeleteVregAsLocalAssignedAndUsedPreviousLine(), 
                           new ConstantVariablePropagation(), 
                           new ConstantVariableOperatorPropagation(), 
                           new ConstantVariablePropagationInCall(), 
                                             
                           new DeleteJumpNextLine(), 
                           new RemoveUnreferencedLabels(), 
                           new MergeConsecutiveLabels(), 
                                             
                           new ConstantVariableBranchOperatorPropagation(), 
                           new OperatorConstantFolding(), 
                           new DceVRegAssigned(),
                       }.ToList();

        }
    }

}
