#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment;
using CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Jumps;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Licm;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ReachabilityDfa;
using CodeRefractor.RuntimeBase.Backend.Optimizations.SimpleDce;
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
                           
                        
                //new ConstantDfaAnalysis()
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
            }.ToList();
        }


        public override List<OptimizationPass> BuildOptimizationPasses1()
        {
            return new OptimizationPass[]
            {
                new AssignmentWithVregPrevLineFolding(),
                new DeleteAssignmentWithSelf(),
                new RemoveDeadStoresInBlockOptimizationPass(),
                new OperatorPartialConstantFolding(),
                new OperatorConstantFolding(),
                new FoldVariablesDefinitionsOptimizationPass(),
                new PropagationVariablesOptimizationPass(),
                new DceNewObjectOrArray(),
                new ConstantVariableBranchOperatorPropagation(),
                new ConstantVariableEvaluation(),
                new EvaluatePureFunctionWithConstantCall(),
                new RemoveDeadStoresToFunctionCalls(),
                new RemoveDeadPureFunctionCalls(),
                new AssignmentVregWithConstNextLineFolding(),
                new DoubleAssignPropagation(),
                new AssignToReturnPropagation(),
                new DceLocalAssigned(),
                new DeleteCallToConstructorOfObject(),
                new ConstantVariablePropagation(),
                new ConstantVariableOperatorPropagation(),
                new ConstantVariablePropagationInCall(),
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