#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment;
using CodeRefractor.MiddleEnd.Optimizations.Dfa.ReachabilityDfa;
using CodeRefractor.MiddleEnd.Optimizations.EscapeAndLowering;
using CodeRefractor.MiddleEnd.Optimizations.Inliner;
using CodeRefractor.MiddleEnd.Optimizations.Jumps;
using CodeRefractor.MiddleEnd.Optimizations.Licm;
using CodeRefractor.MiddleEnd.Optimizations.Purity;
using CodeRefractor.MiddleEnd.Optimizations.SimpleDce;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment;
using CodeRefractor.RuntimeBase.Backend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase.Backend.Optimizations.SimpleDce;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Util
{
    public class OptimizationLevels : OptimizationLevelBase
    {
        static OptimizationLevels()
        {
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
                OptimizationCategories.All, OptimizationCategories.Level3
                );

            //level 1 optimizations
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level1, OptimizationCategories.BlockBased
                );

            //level 2 optimizations
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level1, OptimizationCategories.Analysis
                );
            
            //level 3 optimizations
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level3, OptimizationCategories.Global
                );
            OptimizationCategories.Instance.AddChildToParentOptimizationRelation(
                OptimizationCategories.Level3, OptimizationCategories.Inliner
                );
        }
        public override List<OptimizationPassBase> BuildOptimizationPasses0()
        {
            return new List<OptimizationPassBase>();
        }

        public override List<OptimizationPassBase> BuildOptimizationPasses3()
        {
            return new OptimizationPassBase[]
            {
                //new OneDefUsedNextLinePropagation(), //??
                //new OneDefUsedPreviousLinePropagation(), //??
                           
                        
                //new ConstantDfaAnalysis()
            }.ToList();
        }


        public override List<OptimizationPassBase> BuildOptimizationPasses2()
        {
            this.EnabledCategories.Add(OptimizationCategories.Inliner);
            this.EnabledCategories.Add(OptimizationCategories.Global);
            return new OptimizationPassBase[]
            {
                //new OneAssignmentDeadStoreAssignment(), //??
                //  //?? 
                          
                // CSE
            }.ToList();
        }


        public override List<OptimizationPassBase> BuildOptimizationPasses1()
        {
            
            EnabledCategories.Add(OptimizationCategories.Propagation);
            EnabledCategories.Add(OptimizationCategories.DeadCodeElimination);
            EnabledCategories.Add(OptimizationCategories.Analysis);
            EnabledCategories.Add(OptimizationCategories.CommonSubexpressionsElimination);

            return new OptimizationPassBase[]
            {
                //new FoldVariablesDefinitionsOptimizationPass(),

            }.ToList();
        }
    }
}