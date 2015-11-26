using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using WinUSB;

namespace USBss
{
    public partial class MainForm : Form
    {
        private static readonly string CR = Environment.NewLine;
        public static readonly string UsbConfigurationFile = "ssconfig.ss";
        public static readonly string AppConfigurationFile = "config.ss";

        public static MainForm singleton { get; private set; }

        public UsbManager UsbManager { get; private set; }

        bool bruteClose = false;

        public MainForm()
        {
            InitializeComponent();
            
            singleton = this;

            if (!File.Exists(AppConfigurationFile))
                FirstTime();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UsbManager = new UsbManager();
            UsbManager.StateChanged += new UsbStateChangedEventHandler(DeviceStateChanged);

            // display all devices 
            UsbDiskCollection disks = UsbManager.GetAvailableDisks();

            WriteLine("Available USB disks:", Color.DarkCyan);
            foreach (UsbDisk disk in disks)
            {
                WriteLine(disk.ToString());
            }
            WriteLine(string.Empty);

            WriteLine("Waiting for events...", Color.DarkCyan);
        }

        void DeviceStateChanged(UsbStateChangedEventArgs e)
        {
            string text = string.Empty;
            Color color = Color.Black;
            if(e.State == UsbStateChange.Removed)
            {
                color = Color.DarkRed;
                text = e.Disk.Name + " Removed.";
            }
            else if(e.State == UsbStateChange.Added)
            {
                color = Color.DarkGreen;
                text = e.Disk.ToString() + " Added.";

                OnDeviceAdded(e.Disk.Name);
            }
            else
            {
                text = e.State + " " + e.Disk.ToString();
            }

            WriteLine(text, color);
        }

        void OnDeviceAdded(string name)
        {
            if(!IsSecureDevice(name))
            {
                if(MessageBox.Show(
                    "Would you like to setup Security System on this USB Device?",
                    "Configure USB Device?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    ) == DialogResult.Yes)
                {

                }
            }
        }

        public bool IsSecureDevice(string name)
        {
            if(File.Exists(name))
            {
                if (File.Exists(name + "/" + UsbConfigurationFile))
                    return true;
            }
            return false;
        }

        void FirstTime()
        {
            if (MessageBox.Show(
                        "This is the first time this application runs.\n" +
                        "Would you like to configure it?\n" +
                        "Say YES to configure now, NO to use default settings\n" +
                        "(You can even configure application later)",
                        "Configure Application?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                        ) == DialogResult.Yes)
            {
                var form = new Forms.AppConfigurationForm();
                if(form.ShowDialog() == DialogResult.OK)
                {
                    form.service.Configure();
                }
            }
        }

        public void Write(string text)
        {
            Write(text, Color.Black);
        }

        public void Write(string text, Color color)
        {
            logText.SelectionStart = logText.TextLength;
            logText.SelectionLength = 0;

            logText.SelectionColor = color;
            logText.AppendText(text);
            logText.SelectionColor = logText.ForeColor;
        }

        public void WriteLine(string text)
        {
            WriteLine(text, Color.Black);
        }

        public void WriteLine(string text, Color color)
        {
            Write(text + CR, color);
        }

        #region NotificationEvents

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(WindowState != FormWindowState.Minimized && !bruteClose)
            {
                Hide();
                notifyIcon1.Visible = true;
                WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;
            }
        }

        #endregion

        #region Menu

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bruteClose = true;
            Close();
        }

        #endregion
    }
}
