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
using CodeRefractor.CompilerBackend.Optimizations.Licm;
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
            return new List<OptimizationPass>();
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
                           
                // CSE
                new PrecomputeRepeatedPureFunctionCall(),
                new PrecomputeRepeatedBinaryOperators(),
                new PrecomputeRepeatedUnaryOperators(),
                new PrecomputeRepeatedFieldGets(), 
             
                // new AssignmentWithVregPrevLineFolding(),
            }.ToList();
        }


        public override List<OptimizationPass> BuildOptimizationPasses1()
        {
            return new OptimizationPass[]
            {
                new DeleteAssignmentWithSelf(),
                new RemoveDeadStoresInBlockOptimizationPass(),
                new OperatorPartialConstantFolding(),
                new OperatorConstantFolding(),
                //new FoldVariablesDefinitionsOptimizationPass(),

                new PropagationVariablesOptimizationPass(),
                new DceNewObjectOrArray(),
                new ConstantVariableBranchOperatorPropagation(),
                new ConstantVariableEvaluation(),
                //new EvaluatePureFunctionWithConstantCall(),
                new RemoveDeadStoresToFunctionCalls(),
                new RemoveDeadPureFunctionCalls(), 
                         
                //new AssignmentVregWithConstNextLineFolding(),  
                          
                new DoubleAssignPropagation(),
                new AssignToReturnPropagation(),
                new DceLocalAssigned(),
                new DeleteCallToConstructorOfObject(), 
                //new ConstantVariablePropagation(),

                           
                new ConstantVariableOperatorPropagation(),
                //new ConstantVariablePropagationInCall(),

                new AnalyzeFunctionPurity(),
                new AnalyzeFunctionNoStaticSideEffects(),
                new AnalyzeFunctionIsGetter(),
                new AnalyzeFunctionIsSetter(),
                new AnalyzeFunctionIsEmpty(),
                new DeleteJumpNextLine(),
                new RemoveUnreferencedLabels(),
                new MergeConsecutiveLabels(),
                new DeadStoreAssignment(),
                new OneAssignmentDeadStoreAssignment(),
                new RemoveCallsToEmptyMethods(),
                new InlineGetterAndSetterMethods(),
                new ReachabilityLines(),
                new DceVRegUnused(),
                new LoopInvariantCodeMotion(),
                new ClearInFunctionUnusedArguments(),
                new ReplaceCallsToFunctionsWithUnusedArguments(), 
            }.ToList();
        }
    }
}