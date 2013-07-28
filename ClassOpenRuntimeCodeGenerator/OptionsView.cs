using System.Windows.Forms;

namespace ClassOpenRuntimeCodeGenerator
{
    public partial class OptionsView : Form
    {
        private Form1ViewModelData _viewModel;

        public OptionsView()
        {
            InitializeComponent();
        }

        public Form1ViewModelData ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                UpdateView();

            }
        }

        private void UpdateView()
        {
            cbxCppMethod.Checked = _viewModel.IsCppMethod;
            cbxPure.Checked = _viewModel.IsPure;
            tbxHeader.Text = _viewModel.Header;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void OptionsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateModel();
        }

        private void UpdateModel()
        {
            _viewModel.IsCppMethod = cbxCppMethod.Checked;
            _viewModel.IsPure = cbxPure.Checked;
            _viewModel.Header = tbxHeader.Text;
        }
    }
}
