using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Util;
using CodeRefractor.Util;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using MahApps.Metro.Controls;
using VisualCompiler.Views.Dialogs;
using Color = System.Drawing.Color;
using Path = System.IO.Path;
using Timer = System.Timers.Timer;

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
            CompilerUtils.DeleteFilesByWildcards("Test*.exe");

            ViewModel.Window = this;


            TextEditor.Text = ViewModel.SourceCode;
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

            // Dynamic syntax highlighting for Intermediate Representation
            var rules = IL.SyntaxHighlighting.MainRuleSet.Rules;

            var highlightingRule = new HighlightingRule();
            highlightingRule.Color = new HighlightingColor()
            {

                Foreground = new CustomizedBrush(Color.Green),
                FontWeight = FontWeight.FromOpenTypeWeight(130)
            };

            var wordList = ILOperatorKeywords;
            var regex = String.Format(@"\b({0})\b", String.Join("|", wordList));
            highlightingRule.Regex = new Regex(regex);

            rules.Add(highlightingRule);

            highlightingRule = new HighlightingRule();
            highlightingRule.Color = new HighlightingColor()
            {
                Foreground = new CustomizedBrush(Color.Red)
            };

            wordList = ILKeywords;
            regex = String.Format(@"\b({0})\b", String.Join("|", wordList));
            highlightingRule.Regex = new Regex(regex);

            rules.Add(highlightingRule);


            TextEditor.Text = VisualCompilerConstants.InitialCode;

        }



        public void Update()
        {
            ViewModel.RecompileSource();
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if (DataContext != null)
            {

                ViewModel.SourceCode = TextEditor.Text;

            }
        }


        private void Output_OnTextChanged(object sender, EventArgs e)
        {
           
            if (DataContext != null)
            {
               
                          ViewModel.OutputCode = Output.Text;
             
            }
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


        private readonly string[] ILKeywords =
        {
            "LoadFunction","Call","Return","Assignment","BranchOperator","NewObject",
            "SetField","AlwaysBranch", "Label", "BinaryOperator","GetField"

        };

        private readonly string[] ILOperatorKeywords =
        {
            "add", "mul", "div", "sub", "branch"
        };

        #region Folding
        FoldingManager foldingManager;
        AbstractFoldingStrategy foldingStrategy;



        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
            }
        }
        #endregion

        private void RunCSharpButton_Click(object sender, RoutedEventArgs e)
        {
           
                this.ViewModel.CompilerErrors = String.Empty;
            ThreadPool.QueueUserWorkItem((h) =>
            {
                CompileCSharp(true);
            });
        }

        private void CompileCSharp(bool clearoutput)
        {
            try
            {
                CSharpOutput = "";
             
                TextWriter originalConsoleOutput = Console.Out;
                StringWriter writer = new StringWriter();
                Console.SetOut(writer);

                AppDomain appDomain = AppDomain.CreateDomain("Loading Domain");



                var start = Environment.TickCount;

                this.Execute(LastCompiledExecutable);
                var end = Environment.TickCount - start;

                 this.ViewModel.CompilerErrors +=  String.Format("CS time: {0} ms\n", end);
                AppDomain.Unload(appDomain);

                Console.SetOut(originalConsoleOutput);
                string result = writer.ToString();
                CSharpOutput = result;
                this.ViewModel.CompilerErrors += (CSharpOutput);
            }
            catch (Exception ex)
            {

                this.ViewModel.CompilerErrors += ex.Message + "\n" + ex.StackTrace;
            }
        }

        public string CSharpOutput;
        public string CppOutput;
        private IList _selectedCapabilities;

        /// <summary>
        /// This gets executed in the temporary appdomain.
        /// No error handling to simplify demo.
        /// </summary>
        public void Execute(string exeName)
        {
            // load the bytes and run Main() using reflection
            // working with bytes is useful if the assembly doesn't come from disk
            byte[] bytes = File.ReadAllBytes(exeName);
            Assembly assembly = Assembly.Load(bytes);
            MethodInfo main = assembly.EntryPoint;
            main.Invoke(null, null);
        }



        public MainWindowViewModel ViewModel
        {
            get
            {
                MainWindowViewModel f = null;

                Dispatcher.Invoke(() =>
                {
                    f= (MainWindowViewModel) DataContext;
                });
                return f;
            }
        }

        private void RunCPPButton_Click(object sender, RoutedEventArgs e)
        {

           

                ViewModel.CompilerErrors = String.Empty;
            ThreadPool.QueueUserWorkItem((h) =>
            {
                CompileCpp();
            });



        }

        public string LastCompiledExecutable;
        private void CompileCpp()
        {
           
                CppOutput = String.Empty;
                var outputcpp = "OpenRuntime/" + LastCompiledExecutable.Replace(".exe", ".cpp");

                var fileInfo = new FileInfo(outputcpp);
                outputcpp = fileInfo.FullName;

                var outputexe = outputcpp.Replace(".cpp", "_CPP.exe");

                try
                {
                    var sb = new StringBuilder(ViewModel.OutputCode);


                    sb.ToFile(outputcpp);

                    NativeCompilationUtils.SetCompilerOptions("gcc");
                    NativeCompilationUtils.CompileAppToNativeExe(outputcpp, outputexe);


                    // Start the child process.
                    Process p = new Process();
                    // Redirect the output stream of the child process.
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    p.StartInfo.FileName = outputexe;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(outputexe);
                    p.Start();
                    var start = Environment.TickCount;

                    // Do not wait for the child process to exit before
                    // reading to the end of its redirected stream.
                    // p.WaitForExit();
                    // Read the output stream first and then wait.
                    string output = outputexe.ExecuteCommand("");
                    p.WaitForExit();
                    var end = Environment.TickCount - start;

                    CppOutput = output;
                    Dispatcher.Invoke(() =>
                    {
                        ViewModel.CompilerErrors += String.Format("CPP time: {0} ms\n", end) + output +
                                                    p.StandardError.ReadToEnd();
                    });
                }
                catch (Exception ex)
                {

                    Dispatcher.Invoke(() =>
                    {
                        ViewModel.CompilerErrors += ex.Message + "\nStackTrace: \n" +
                                                    ex.StackTrace;
                    });

                }
                finally
                {
                    File.Delete(outputcpp);
                    File.Delete(outputexe);
                }
           
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {

            if (ResetStatus != null)
                ResetStatus.Stop();
          ThreadPool.QueueUserWorkItem((h) =>
          {
              CompileCSharp(false);
              CompileCpp();
         
                if (CSharpOutput == CppOutput)
                {
                    Dispatcher.Invoke(() =>
                    {
                        TestStatus.Content = "PASSED";
                        TestStatus.Background =
                            new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.GreenYellow.R,
                                Color.GreenYellow.G, Color.GreenYellow.B));

                        ViewModel.CompilerErrors = String.Format("Test Passed:\n\nCSharpOutput:\n{0}CPPOutPut:\n{1}",
                            CSharpOutput, CppOutput);
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        TestStatus.Content = "FAILED";
                        TestStatus.Background =
                            new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.Red.R, Color.Red.G, Color.Red.B));
                        ViewModel.CompilerErrors = "Test Failed\n" + ViewModel.CompilerErrors;
                    });
                }
                if (ResetStatus == null)
                {
                    ResetStatus = new Timer(2000);
                    ResetStatus.AutoReset = false;
                    ResetStatus.Elapsed += (o, args) => Dispatcher.Invoke(() =>
                    {
                        TestStatus.Content = "TEST STATUS";
                        TestStatus.Background =
                            new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.White.R, Color.White.G,
                                Color.White.B));
                    });
                }
                ResetStatus.Start();
          });
        }

        private Timer ResetStatus;




        private void OnShowCompilerOptions(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new CompilerOptionsWindow();
            optionsWindow.Owner = this;
            optionsWindow.ShowDialog();
            if (!optionsWindow.ViewModel.Accepted)
                return;
            ViewModel.OptimizationList.Clear();
            ViewModel.OptimizationList.AddRange(optionsWindow.ViewModel.Capabilities);

        }

        private void OnFileOpen(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

