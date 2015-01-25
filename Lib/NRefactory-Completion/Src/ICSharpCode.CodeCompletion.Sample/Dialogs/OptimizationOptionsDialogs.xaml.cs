using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ICSharpCode.CodeCompletion.Sample.Dialogs
{
    /// <summary>
    /// Interaction logic for OptimizationOptionsDialogs.xaml
    /// </summary>
    public partial class OptimizationOptionsDialogs : Window
    {
        public OptimizationOptionsDialogs()
        {
            InitializeComponent();
        }

        private void OnSelectAll(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnUnselectAll(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
