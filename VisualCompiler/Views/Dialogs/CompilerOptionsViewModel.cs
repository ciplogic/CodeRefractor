using System.Collections;
using System.Collections.Generic;

namespace VisualCompiler.Views.Dialogs
{
    public class CompilerOptionsViewModel : NotificationViewModel
    {
        private List<string> _optimizationList;

        public CompilerOptionsViewModel()
        {
            _optimizationList = new List<string>()
            {
                "Level0",
                "Level1",
                "Level2",
                "Level3",
                "DCE",
            };
        }

        public IList Capabilities { get; set; }
        public bool Accepted { get; set; }

        public List<string> OptimizationList
        {
            get { return  _optimizationList; }
        }
    }
}