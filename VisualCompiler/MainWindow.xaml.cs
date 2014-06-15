using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Util;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using MahApps.Metro.Controls;
using Path = System.IO.Path;

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

               
                TextEditor.Text = InitialCode;
      
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
            CompileCSharp(true);
        }

        private void CompileCSharp(bool clearoutput)
        {
            CSharpOutput = "";
            if(clearoutput)
            this.ViewModel.CompilerErrors = String.Empty;
            TextWriter originalConsoleOutput = Console.Out;
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);

            AppDomain appDomain = AppDomain.CreateDomain("Loading Domain");


            this.Execute(ViewModel.LastCompiledExecutable);
            AppDomain.Unload(appDomain);

            Console.SetOut(originalConsoleOutput);
            string result = writer.ToString();
            CSharpOutput = result;
            this.ViewModel.CompilerErrors += String.Format("Running {0}:\n\n{1}", ViewModel.LastCompiledExecutable,
                (CSharpOutput));
        }

        public string CSharpOutput;
        public string CppOutput;
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
            main.Invoke(null,null);
        }



        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel) DataContext; }
        }

        private void RunCPPButton_Click(object sender, RoutedEventArgs e)
        {
            CompileCpp(true);
        }

        private void CompileCpp(bool clearoutput)
        {
            if (clearoutput)
                ViewModel.CompilerErrors = String.Empty;
            CppOutput = String.Empty;
            var outputcpp = "OpenRuntime/" + ViewModel.LastCompiledExecutable.Replace(".exe", ".cpp");

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
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = outputexe.ExecuteCommand("");
                p.WaitForExit();
                CppOutput = output;
                ViewModel.CompilerErrors += output + p.StandardError.ReadToEnd();
            }
            catch (Exception ex)
            {
                ViewModel.CompilerErrors += ex.Message + "\nStackTrace: \n" + ex.StackTrace;
            }
            finally
            {
                File.Delete(outputcpp);
                File.Delete(outputexe);
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            CompileCSharp(false);
            CompileCpp(false);
            if (CSharpOutput == CppOutput)
            {
                ViewModel.CompilerErrors = String.Format("Test Passed:\n\nCSharpOutput:\n{0}CPPOutPut:\n{1}",CSharpOutput,CppOutput) ;
            }
            else
            {
                ViewModel.CompilerErrors = "Test Failed\n" +ViewModel.CompilerErrors;
            }
        }




        public static string InitialCode = @"using System;

class NBody
{
    public static void Main()
    {
        Console.WriteLine(""Prime numbers: "");
        var len = 1000000;

        var pr = new Action(() =>
        {
            var primes = AddPrimes(len);
            Console.Write(primes);
            Console.WriteLine(""Simpler Example: "");
        });

        pr();

    }

    private static int AddPrimes(int len)
    {
        var primes = 0;
        for (var i = 2; i < len; i++)
        {
            if (i%2 == 0)
                continue;
            var isPrime = true;
            for (var j = 2; j*j <= i; j++)
            {
                if (i%j == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            if (isPrime)
                primes++;
        }
        return primes;
    }
}

class Body { public double x, y, z, vx, vy, vz, mass; }
class Pair { public Body bi, bj; }

class NBodySystem
{
    private Body[] bodies;
    private Pair[] pairs;

    const double Pi = 3.141592653589793;
    const double Solarmass = 4 * Pi * Pi;
    const double DaysPeryear = 365.24;

    public NBodySystem()
    {
        bodies = new Body[] {
            new Body() { // Sun
                mass = Solarmass,
            },
            new Body() { // Jupiter
                x = 4.84143144246472090e+00,
                y = -1.16032004402742839e+00,
                z = -1.03622044471123109e-01,
                vx = 1.66007664274403694e-03 * DaysPeryear,
                vy = 7.69901118419740425e-03 * DaysPeryear,
                vz = -6.90460016972063023e-05 * DaysPeryear,
                mass = 9.54791938424326609e-04 * Solarmass,
            },
            new Body() { // Saturn
                x = 8.34336671824457987e+00,
                y = 4.12479856412430479e+00,
                z = -4.03523417114321381e-01,
                vx = -2.76742510726862411e-03 * DaysPeryear,
                vy = 4.99852801234917238e-03 * DaysPeryear,
                vz = 2.30417297573763929e-05 * DaysPeryear,
                mass = 2.85885980666130812e-04 * Solarmass,
            },
            new Body() { // Uranus
                x = 1.28943695621391310e+01,
                y = -1.51111514016986312e+01,
                z = -2.23307578892655734e-01,
                vx = 2.96460137564761618e-03 * DaysPeryear,
                vy = 2.37847173959480950e-03 * DaysPeryear,
                vz = -2.96589568540237556e-05 * DaysPeryear,
                mass = 4.36624404335156298e-05 * Solarmass,
            },
            new Body() { // Neptune
                x = 1.53796971148509165e+01,
                y = -2.59193146099879641e+01,
                z = 1.79258772950371181e-01,
                vx = 2.68067772490389322e-03 * DaysPeryear,
                vy = 1.62824170038242295e-03 * DaysPeryear,
                vz = -9.51592254519715870e-05 * DaysPeryear,
                mass = 5.15138902046611451e-05 * Solarmass,
            },
        };

        CalculatePairs();
    }

    private void CalculatePairs()
    {
        SetupDefaultParis();

        double px = 0.0, py = 0.0, pz = 0.0;
        foreach (var b in bodies)
        {
            px += b.vx*b.mass;
            py += b.vy*b.mass;
            pz += b.vz*b.mass;
        }
        var sol = bodies[0];
        sol.vx = -px/Solarmass;
        sol.vy = -py/Solarmass;
        sol.vz = -pz/Solarmass;
    }

    private void SetupDefaultParis()
    {
        pairs = new Pair[bodies.Length*(bodies.Length - 1)/2];
        int pi = 0;
        for (int i = 0; i < bodies.Length - 1; i++)
            for (int j = i + 1; j < bodies.Length; j++)
            {
                pairs[pi] = new Pair();
                pairs[pi].bi = bodies[i];
                pairs[pi].bj = bodies[j];
                pi++;
            }
    }

    public void Advance(double dt)
    {
        foreach (var p in pairs)
        {
            Body bi = p.bi, bj = p.bj;
            double dx = bi.x - bj.x, dy = bi.y - bj.y, dz = bi.z - bj.z;
            double d2 = dx * dx + dy * dy + dz * dz;
            double mag = dt / (d2 * Math.Sqrt(d2));
            bi.vx -= dx * bj.mass * mag; bj.vx += dx * bi.mass * mag;
            bi.vy -= dy * bj.mass * mag; bj.vy += dy * bi.mass * mag;
            bi.vz -= dz * bj.mass * mag; bj.vz += dz * bi.mass * mag;
        }
        foreach (var b in bodies)
        {
            b.x += dt * b.vx; b.y += dt * b.vy; b.z += dt * b.vz;
        }
    }

    public double Energy()
    {
        double e = 0.0;
        for (int i = 0; i < bodies.Length; i++)
        {
            var bi = bodies[i];
            e += 0.5 * bi.mass * (bi.vx * bi.vx + bi.vy * bi.vy + bi.vz * bi.vz);
            for (int j = i + 1; j < bodies.Length; j++)
            {
                var bj = bodies[j];
                double dx = bi.x - bj.x, dy = bi.y - bj.y, dz = bi.z - bj.z;
                e -= (bi.mass * bj.mass) / Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }
        return e;
    }
}";
    
    }
}
