using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace USBss.Forms
{
    public partial class UsbWatcherForm : Form
    {
        #region Static

        public static Dictionary<string, UsbWatcherForm> Instances { get; private set; }

        public static UsbWatcherForm Start(string deviceName, string deviceId)
        {
            if (Instances == null)
                Instances = new Dictionary<string, UsbWatcherForm>();
            else
            {
                foreach (var key in Instances.Keys)
                    if (key == deviceId)
                        return Instances[key];
            }

            var watcher = new UsbWatcherForm(deviceName, deviceId);
            Instances.Add(deviceId, watcher);
            watcher.Show();
            return watcher;
        }

        public static void Close(string deviceName)
        {
            if (Instances == null)
                Instances = new Dictionary<string, UsbWatcherForm>();
            else
            {
                foreach (var key in Instances.Keys)
                    if(Instances[key].DeviceName == deviceName)
                    {
                        Instances[key].Close();
                        Instances.Remove(key);
                        return;
                    }
            }
        }

        #endregion

        public string DevicePath { get; private set; }
        public string DeviceId { get; private set; }
        public string DeviceName { get; private set; }

        public Dictionary<string, string> Groups { get; private set; }

        public bool OwnerMode { get; private set; }

        Database.Tables.Keys Keys;
        Database.Tables.Devices Devices;

        FileSystemWatcher watcherService;
        int clStartCount;

        public UsbWatcherForm(string name, string id)
        {
            InitializeComponent();

            DeviceName = name;
            DevicePath = name + "/";
            DeviceId = id;
            Text = Text + " (" + DevicePath + ")";

            clStartCount = dataGrid.ColumnCount;
            
        }

        private void UsbWatcherForm_Load(object sender, EventArgs e)
        {
            InitDatabase();

            OwnerMode = Devices.ImOwner(DeviceId);
            
            // Non sono il proprietario del dispositivo
            // è necessario che io inserisca una password
            if (OwnerMode == false)
            {
                if (MessageBox.Show(
                    "Would you like to define a key to this device?",
                    "No keys found!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    ) == DialogResult.Yes)
                {
                    var keyDialog = new KeyForm("Define a key");
                    if(keyDialog.ShowDialog() == DialogResult.OK)
                    {
                        var password = keyDialog.Password;

                    }
                    
                }
            }
        }

        void InitDatabase()
        {
            Keys = Database.LiteDatabase.singleton.GetTable("keys") as Database.Tables.Keys;

            if (Keys == null)
            {
                MessageBox.Show(
                    "Internal Error! Keys table not found!",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                Close();
            }

            Devices = Database.LiteDatabase.singleton.GetTable("devices") as Database.Tables.Devices;

            if (Devices == null)
            {
                MessageBox.Show(
                    "Internal Error! Devices table not found!",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                Close();
            }
        }

        void UpdateGrid()
        {
            var groups = Keys.GetGroups(DeviceId);
            foreach(var group in groups)
            {
                if(ColumnExists(group) == false)
                {
                    DataGridViewColumn clmn = new DataGridViewColumn();
                    clmn.HeaderText = group;
                    clmn.Name = "cl" + group;
                    dataGrid.Columns.Add(clmn);
                }
            }
        }

        bool ColumnExists(string name)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].HeaderText == name)
                    return true;
            }
            return false;
        }

        bool KeysDialogHandler()
        {
            return false;
        }        

        private void UsbWatcherForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var process = Process.Start(DevicePath);
            //process.EnableRaisingEvents = true;
            //process.Exited += processExited;
        }
        
    }
}
