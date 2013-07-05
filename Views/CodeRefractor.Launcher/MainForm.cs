using System;
using System.Windows.Forms;
using CodeRefractor.Compiler.Util;
using CodeRefractor.RuntimeBase.DataBase.SerializeXml;

namespace CodeRefractor.Launcher
{
    public partial class MainForm : Form
    {
        NativeCompilationUtils.Options _viewModel = new NativeCompilationUtils.Options();
        private const string CompilerXml = "compilerOptions.xml";

        public MainForm()
        {
            InitializeComponent();
            _viewModel = NativeCompilationUtils.CompilerOptions;
            _viewModel.DeserializeFromFile(CompilerXml);
            UpdateView();
        }

        private void UpdateView()
        {
            _viewModel = NativeCompilationUtils.CompilerOptions;
            cbxCompiler.Text = _viewModel.CompilerKind;

            tbxCompilerExe.Text = _viewModel.CompilerExe;
            tbxPath.Text = _viewModel.PathOfCompilerTools;
            tbxCppFlags.Text = _viewModel.OptimizationFlags;
            tbxLinkFlags.Text = _viewModel.LinkerOptions;
        }

        public void UpdateModel()
        {
            _viewModel.CompilerKind = cbxCompiler.SelectedText;
            _viewModel.CompilerExe = tbxCompilerExe.Text;
            _viewModel.PathOfCompilerTools = tbxPath.Text;
            _viewModel.OptimizationFlags = tbxCppFlags.Text;
            _viewModel.LinkerOptions = tbxLinkFlags.Text;
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void BrowseAssemblyClick(object sender, EventArgs e)
        {
            var fileName = BrowseForExe();

            tbxInputAssembly.Text = fileName;
        }

        private static string BrowseForExe()
        {
            var openDialog = new OpenFileDialog { Filter = "Exe files (*.exe)|*.exe|All files (*.*)|*.*" };
            var result = openDialog.ShowDialog();
            var fileName = string.Empty;
            switch (result)
            {
                case DialogResult.Cancel:
                    return fileName;
                default:
                    fileName = openDialog.FileName;
                    break;
            }
            return fileName;
        }

        private void OnApplyClick(object sender, EventArgs e)
        {
            UpdateModel();
            _viewModel.SerializeToFile(CompilerXml);
        }

        private void cbxCompiler_SelectedIndexChanged(object sender, EventArgs e)
        {
            NativeCompilationUtils.SetCompilerOptions(cbxCompiler.Text);
            UpdateView();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            var sourceAssembly = tbxInputAssembly.Text;
            var destExe = tbxOutputExe.Text;
            try
            {
                NativeCompilationUtils.CallCompiler(sourceAssembly, destExe);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Compilation errors: {0}", ex.Message));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            var fileName = BrowseForExe();

            tbxOutputExe.Text = fileName;
        }
    }
}
