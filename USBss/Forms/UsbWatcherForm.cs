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

        /// <summary>
        /// Devo gestire n instance di questo form per gli n dispositivi connessi al PC
        /// </summary>
        public static Dictionary<string, UsbWatcherForm> Instances { get; private set; }

        /// <summary>
        /// Avvia la routine di gestione del dispositivo USB
        /// </summary>
        /// <param name="deviceName">Nome logico del dispositivo, es. C:, E:, F:, ...</param>
        /// <param name="deviceId">Id associato al dispositvo dal protocollo di sicurezza</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gestione della chiusura di una delle istanze
        /// Questo metodo è chiamato, in generale, quando un dispositvo viene scollegato dal PC
        /// </summary>
        /// <param name="deviceName"></param>
        public static void Close(string deviceName)
        {
            if (Instances == null)
                Instances = new Dictionary<string, UsbWatcherForm>();
            else
            {
                foreach (var key in Instances.Keys)
                    if(Instances[key].DeviceName == deviceName)
                    {
                        Instances[key].Exit();
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
        
        int clStartCount;

        bool end = false;

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
                var keyDialog = new KeyForm("Define a key");
                if (keyDialog.ShowDialog() == DialogResult.OK)
                {
                    var password = keyDialog.Password;

                }
            }

            fileSystemWatcher.Path = DevicePath;
            
            UpdateGroupColumns();
            InitGrid();
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

        void InitGrid()
        {
            foreach (var file in Directory.GetFiles(DevicePath))
            {
                if (File.GetAttributes(file) != FileAttributes.Hidden)
                    dataGrid.Rows.Add(Path.GetFileName(file), false);
            }
            foreach (var folder in Directory.GetDirectories(DevicePath))
            {
                var attr = File.GetAttributes(folder);
                if (attr != (FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory))
                    dataGrid.Rows.Add(Path.GetFileName(folder), true);
            }
        }

        /// <summary>
        /// Dopo l'avvio del Form per la gestione dei gruppi associati al dispositivo
        /// occorre aggiornare le colonne della griglia
        /// </summary>
        void UpdateGroupColumns()
        {
            var groups = Keys.GetGroups(DeviceId);
            foreach(var group in groups)
            {
                if(ColumnExists(group) == false)
                {
                    var column = new DataGridViewCheckBoxColumn();
                    column.Name = "cl" + group;
                    column.HeaderText = group;
                    column.Width = 50;
                    dataGrid.Columns. Add(column);
                }
            }
            for(int i=clStartCount; i<dataGrid.Columns.Count;i++)
            {
                var column = dataGrid.Columns[i];
                if(groups.Contains(column.HeaderText) == false)
                {
                    dataGrid.Columns.RemoveAt(i);
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
        
        public void Exit()
        {
            end = true;
            Close();
        }

        private void UsbWatcherForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (end == false)
                e.Cancel = true;
        }

        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var process = Process.Start(DevicePath);
        }

        private void setGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new GroupsForm(DeviceName, DeviceId);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                UpdateGroupColumns();
            }
        }

        #region FileWatcher_events

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            
        }

        private void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            bool isFolder = false;
            if (File.GetAttributes(e.FullPath) == FileAttributes.Directory)
                isFolder = true;
            dataGrid.Rows.Add(e.Name, isFolder);
        }

        private void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == e.Name)
                {
                    dataGrid.Rows.RemoveAt(i);
                    return;
                }
            }
        }

        private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            for(int i=0; i<dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];
                if(row.Cells[0].Value != null && row.Cells[0].Value.ToString() == e.OldName)
                {
                    row.Cells[0].Value = e.Name;
                    return;
                }
            }
        }

        #endregion
    }
}
