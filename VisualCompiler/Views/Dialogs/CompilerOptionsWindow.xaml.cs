using System.Linq;
using System.Windows;

namespace VisualCompiler.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CompilerOptionsWindow.xaml
    /// </summary>
    public partial class CompilerOptionsWindow
    {
        public CompilerOptionsViewModel ViewModel
        {
            get { return (CompilerOptionsViewModel) DataContext; }
        }

        public MainWindow MainWindow
        {
            get { return (MainWindow)Application.Current.MainWindow; }
        }
        public CompilerOptionsWindow()
        {
            InitializeComponent();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            ViewModel.Accepted = true;
            var result = (ListBox.SelectedItems.Cast<object>()
                .Select(selectedItem => selectedItem.ToString()))
                .ToList();
            ViewModel.Capabilities = result;
            MainWindow.Update();
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            ViewModel.Accepted = false;
            Close();
        }

        private void OnSelectAll(object sender, RoutedEventArgs e)
        {
            foreach (var optimization in ViewModel.OptimizationList)
            {
                ListBox.SelectedItems.Add(optimization);
            }
        }

        private void OnUnselectAll(object sender, RoutedEventArgs e)
        {
            foreach (var optimization in ViewModel.OptimizationList)
            {
                ListBox.SelectedItems.Remove(optimization);
            }
        }
    }
}
