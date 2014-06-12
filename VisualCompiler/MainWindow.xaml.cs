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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using MahApps.Metro.Controls;
using VisualCompiler.AvalonEdit.Sample;

namespace VisualCompiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
           

            InitializeComponent();

           
            DataContext = new MainWindowViewModel(this);
          

            TextEditor.Text = ((MainWindowViewModel)DataContext).SourceCode;
            TextEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            TextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(TextEditor.Options);
            foldingStrategy = new BraceFoldingStrategy();

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();


            if (foldingManager == null)
                    foldingManager = FoldingManager.Install(TextEditor.TextArea);
                foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);

           
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if(DataContext!=null)
                ((MainWindowViewModel)DataContext).SourceCode = TextEditor.Text;
        }


        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
//            if (e.Text == ".")
//            {
//                // open code completion after the user has pressed dot:
//                completionWindow = new CompletionWindow(TextEditor.TextArea);
//                // provide AvalonEdit with the data:
//                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
//                data.Add(new MyCompletionData("Item1"));
//                data.Add(new MyCompletionData("Item2"));
//                data.Add(new MyCompletionData("Item3"));
//                data.Add(new MyCompletionData("Another item"));
//                completionWindow.Show();
//                completionWindow.Closed += delegate
//                {
//                    completionWindow = null;
//                };
//            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #region Folding
        FoldingManager foldingManager;
        AbstractFoldingStrategy foldingStrategy;

//        void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (TextEditor.SyntaxHighlighting == null)
//            {
//                foldingStrategy = null;
//            }
//            else
//            {
//                switch (TextEditor.SyntaxHighlighting.Name)
//                {
//                    case "XML":
//                        foldingStrategy = new XmlFoldingStrategy();
//                        TextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
//                        break;
//                    case "C#":
//                    case "C++":
//                    case "PHP":
//                    case "Java":
//                        TextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(TextEditor.Options);
//                        foldingStrategy = new BraceFoldingStrategy();
//                        break;
//                    default:
//                        TextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
//                        foldingStrategy = null;
//                        break;
//                }
//            }
//            if (foldingStrategy != null)
//            {
//                if (foldingManager == null)
//                    foldingManager = FoldingManager.Install(TextEditor.TextArea);
//                foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
//            }
//            else
//            {
//                if (foldingManager != null)
//                {
//                    FoldingManager.Uninstall(foldingManager);
//                    foldingManager = null;
//                }
//            }
//        }

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
            }
        }
        #endregion
    }
}
