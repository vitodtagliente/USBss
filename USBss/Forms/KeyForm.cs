using System;
using System.Windows.Forms;

namespace USBss.Forms
{
    public partial class KeyForm : Form
    {
        public string Password { get; private set; }

        public KeyForm(string text = "Key Dialog")
        {
            InitializeComponent();

            Text = text;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Password = txtPassword.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void KeyForm_Load(object sender, EventArgs e)
        {

        }
    }
}
