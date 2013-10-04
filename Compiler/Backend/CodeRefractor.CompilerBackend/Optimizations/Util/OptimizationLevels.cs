#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.ConstantDfa;
using CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment;
using CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering;
using CodeRefractor.CompilerBackend.Optimizations.Inliner;
using CodeRefractor.CompilerBackend.Optimizations.Jumps;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.CompilerBackend.Optimizations.ReachabilityDfa;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.CompilerBackend.Optimizations.SimpleDce;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Util
{
    public class OptimizationLevels : OptimizationLevelBase
    {
        public override List<OptimizationPass> BuildOptimizationPasses0()
        {
            return new OptimizationPass[0].ToList();
        }

        public override List<OptimizationPass> BuildOptimizationPasses3()
        {
            return new OptimizationPass[]
                       {
                           
                           //new OneDefUsedNextLinePropagation(), //??
                           //new OneDefUsedPreviousLinePropagation(), //??
                           
                        
                           new ConstantDfaAnalysis()

                       }.ToList();
        }


        public override List<OptimizationPass> BuildOptimizationPasses2()
        {
            return new OptimizationPass[]
            {
                
                new OneAssignmentDeadStoreAssignment(), //??
                           //  //?? 
                
                
            }.ToList();
        }


        public override List<OptimizationPass> BuildOptimizationPasses1()
        {
            return new OptimizationPass[]
                       {
                           new DceVRegUnused(),
                           new DeleteAssignmentWithSelf(), 
                        
                           new AssignmentWithVregPrevLineFolding(), 
                           new PropagationVariablesOptimizationPass(), 
                           new RemoveDeadStoresInBlockOptimizationPass(), 
                           new FoldVariablesDefinitionsOptimizationPass(), 
                           new OperatorPartialConstantFolding(),
                           new OperatorConstantFolding(),
                           new ConstantVariableBranchOperatorPropagation(),
                           new AssignmentVregWithConstNextLineFolding(), 
                           
                           new EvaluatePureFunctionWithConstantCall(),
                           
                           new RemoveDeadStoresToFunctionCalls(), 
                           new RemoveDeadPureFunctionCalls(), 
                           
                           new PrecomputeRepeatedPureFunctionCall(), 
                           new PrecomputeRepeatedBinaryOperators(), 
                           new PrecomputeRepeatedUnaryOperators(), 

                           new AssignToReturnPropagation(),
                           new DeadStoreLastSequenceRemover(),
                           new DceLocalAssigned(),
                           
                           new ConstantVariablePropagation(),
                           new ConstantVariableOperatorPropagation(),
                           new ConstantVariablePropagationInCall(),
                           
                           new VRegVariablePropagation(),
                           
                           new DoubleAssignPropagation(),
                           
      
                           new DeleteCallToConstructorOfObject(), 
                            new AnalyzeFunctionPurity(),
                            new AnalyzeFunctionIsGetter(),
                            new AnalyzeFunctionIsSetter(),
                            new AnalyzeFunctionIsEmpty(),
                            new AnalyzeParametersAreEscaping(), 

                            new InlineGetterAndSetterMethods(), 
                           
                            new DeleteJumpNextLine(),
                            new RemoveUnreferencedLabels(),
                            new MergeConsecutiveLabels(),
                           
                           
                            new DeadStoreAssignment(), 
                             new OneAssignmentDeadStoreAssignment(),
                           new ReachabilityLines(),
                           new PropagationVariablesOptimizationPass(),
                           new InFunctionLoweringVars()
                             
                       }.ToList();
        }
    }
}