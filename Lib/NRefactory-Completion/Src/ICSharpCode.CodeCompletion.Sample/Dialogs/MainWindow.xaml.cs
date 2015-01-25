using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
using System;
using System.Windows;

namespace CompletionSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string AppTitle = "NRefactory Code Completion";
        private CSharpCompletion completion;

        public MainWindow()
        {
            InitializeComponent();
            Title = AppTitle;
        }

        private readonly string[] ILKeywords =
        {
            "LoadFunction","Call","Return","Assignment","BranchOperator","NewObject",
            "SetField","AlwaysBranch", "Label", "BinaryOperator","GetField", "CallVirtual"

        };

        private readonly string[] ILOperatorKeywords =
        {
            "add", "mul", "div", "sub", "branch"
        };

        public string LastCompiledExecutable;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            completion = new CSharpCompletion();
            editor.Completion = completion;
            editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            
            OpenFile(@"..\SampleFiles\Sample1.cs");
        }

        private void OnFileOpenClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".cs"; // Default file extension 
            dlg.Filter = "C# Files|*.cs?|All Files|*.*"; // Filter files by extension 

            if(dlg.ShowDialog() == true)
            {
                OpenFile(dlg.FileName);
            }
        }

        private void OnSaveFileClick(object sender, RoutedEventArgs e)
        {
            if (editor != null && editor.SaveFile())
            {
                MessageBox.Show("File Saved" + Environment.NewLine + editor.FileName);
            }
        }

        private void OpenFile(string fileName)
        {
            editor.OpenFile(fileName);
            tabItem.Header = System.IO.Path.GetFileName(fileName);
         
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Output_OnTextChanged(object sender, EventArgs e)
        {
        }
    }
}
