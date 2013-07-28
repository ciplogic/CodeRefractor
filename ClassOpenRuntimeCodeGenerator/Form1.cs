#region Usings

using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CodeRefractor.RuntimeBase.DataBase.SerializeXml;

#endregion

namespace ClassOpenRuntimeCodeGenerator
{
    public partial class Form1 : Form
    {
        private readonly Form1ViewModelData _viewModel = new Form1ViewModelData();
        private const string FormConfigFileName = "form1.config";

        public Form1()
        {
            InitializeComponent();
            
            _viewModel.DeserializeFromFile(FormConfigFileName);
            UpdateView();
        }

        private void UpdateView()
        {
            tbxAssemblyPath.Text = _viewModel.AssemblyPath;
            tbxSourceName.Text = _viewModel.SourceType;
            cbxCoreLibraries.Checked = _viewModel.IsCoreLibraries;
            tbxTargetTypeName.Text = _viewModel.TargetType;
        }

        private void UpdateModel()
        {
            _viewModel.AssemblyPath = tbxAssemblyPath.Text;
            _viewModel.SourceType = tbxSourceName.Text;
            _viewModel.IsCoreLibraries = cbxCoreLibraries.Checked;
            _viewModel.TargetType = tbxTargetTypeName.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Type typeToScan;
            if (!LocateType(out typeToScan))
            {
                MessageBox.Show("Error loading type");
                return;
            }
            var cb = new CodeBuilder(typeToScan, tbxTargetTypeName.Text, _viewModel);
            tbxOutputSource.Text = cb.GenerateSource();
        }

        private bool LocateType(out Type typeToScan)
        {
            Assembly assembly;
            typeToScan = null;
            var sourceTypeName = tbxSourceName.Text;
            if (cbxCoreLibraries.Checked)
            {
                var assemblies = new[]
                {
                    typeof (int).Assembly,
                    typeof (Console).Assembly
                };
                var firstAssembly = assemblies.FirstOrDefault(a => GetTypeFromAssembly(a, sourceTypeName) != null);
                if (firstAssembly != null)
                    typeToScan = GetTypeFromAssembly(firstAssembly, sourceTypeName);
                return typeToScan!=null;
            }
            
            try
            {
                assembly = Assembly.LoadFile(tbxAssemblyPath.Text);
            }
            catch
            {
                MessageBox.Show("Error loading assembly");
                return false;
            }

            if (GetTypeFromAssembly(assembly, sourceTypeName)==null) return false;
            
            return true;
        }

        private static Type GetTypeFromAssembly(Assembly assembly, string sourceTypeName)
        {
            Type typeToScan;
            
            try
            {
                typeToScan = assembly.GetType(sourceTypeName);
            }
            catch
            {
                return null;
            }
            return typeToScan;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog();
            browser.ShowDialog();
            var directory = browser.SelectedPath;
            tbxAssemblyPath.Text = directory;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateModel();
            _viewModel.SerializeToFile(FormConfigFileName);
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            var option = new OptionsView {ViewModel = _viewModel};
            option.ShowDialog();
        }

    }
}