using System.Collections.ObjectModel;

namespace ICSharpCode.CodeCompletion.Sample.Dialogs
{
    public class CompilerOptionsViewModel : NotificationViewModel
    {
        public CompilerOptionsViewModel()
        {
            OptimizationList = new ObservableCollection<string>();
        }
        public ObservableCollection<string> OptimizationList { get; set; }
    }
}