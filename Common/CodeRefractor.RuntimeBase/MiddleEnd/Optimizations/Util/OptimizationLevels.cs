#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment;
using CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Jumps;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Licm;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ReachabilityDfa;
using CodeRefractor.RuntimeBase.Backend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.Backend.Optimizations.SimpleDce;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment;
using CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.Purity;
using CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Util
{
    public class OptimizationLevels : OptimizationLevelBase
    {
        static OptimizationLevels()
        {
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level1, OptimizationCategories.BlockBased
                );
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level1, OptimizationCategories.Propagation
                );
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level2, OptimizationCategories.Level1
                );
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level3, OptimizationCategories.Level2
                );
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level1, OptimizationCategories.BlockBased
                );
        }
        public override List<ResultingOptimizationPass> BuildOptimizationPasses0()
        {
            return new List<ResultingOptimizationPass>();
        }

        public override List<ResultingOptimizationPass> BuildOptimizationPasses3()
        {
            return new ResultingOptimizationPass[]
            {
                //new OneDefUsedNextLinePropagation(), //??
                //new OneDefUsedPreviousLinePropagation(), //??
                           
                        
                //new ConstantDfaAnalysis()
            }.ToList();
        }


        public override List<ResultingOptimizationPass> BuildOptimizationPasses2()
        {
            this.EnabledCategories.Add(OptimizationCategories.CommonSubexpressionsElimination);
            return new ResultingOptimizationPass[]
            {
                //new OneAssignmentDeadStoreAssignment(), //??
                //  //?? 
                          
                // CSE
            }.ToList();
        }


        public override List<ResultingOptimizationPass> BuildOptimizationPasses1()
        {
            
            EnabledCategories.Add(OptimizationCategories.Propagation);
            EnabledCategories.Add(OptimizationCategories.DeadCodeElimination);

            return new ResultingOptimizationPass[]
            {
                new AssignmentWithVregPrevLineFolding(),
                new DeleteAssignmentWithSelf(),
                new RemoveDeadStoresInBlockOptimizationPass(),
                new OperatorPartialConstantFolding(),
                new OperatorConstantFolding(),
                //new FoldVariablesDefinitionsOptimizationPass(),
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