using System.Collections;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.Optimizations;

namespace VisualCompiler.Views.Dialogs
{
    public class CompilerOptionsViewModel : NotificationViewModel
    {
        private List<string> _optimizationList;

        public CompilerOptionsViewModel()
        {
            _optimizationList = new List<string>()
            {
                OptimizationCategories.Analysis,
                OptimizationCategories.BlockBased,
                OptimizationCategories.CommonSubexpressionsElimination,
                OptimizationCategories.DeadCodeElimination,
                OptimizationCategories.Propagation,
                OptimizationCategories.Purity,
                OptimizationCategories.All,
            };
        }

        public List<string> Capabilities { get; set; }
        public bool Accepted { get; set; }

        public List<string> OptimizationList
        {
            get { return  _optimizationList; }
        }
    }
}