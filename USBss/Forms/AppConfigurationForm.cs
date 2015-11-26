using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USBss.Forms
{
    public partial class AppConfigurationForm : Form
    {
        public Services.AppConfigurationService service { get; private set; }

        public AppConfigurationForm()
        {
            InitializeComponent();
            service = new Services.AppConfigurationService(MainForm.AppConfigurationFile);
        }

        private void AppConfigurationForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            // apply changes

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExportKey_Click(object sender, EventArgs e)
        {

        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

        }
    }
}
