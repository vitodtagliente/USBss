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
        /// <param name="deviceId">Id associato al dispositivo dal protocollo di sicurezza</param>
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
        
        public bool OwnerMode { get; private set; }

        string UserPassword;

        Database.Tables.Keys Keys;
        Database.Tables.Devices Devices;
        
        int clStartCount;

        /// <summary>
        /// Serve a gestire la chiusura o meno del form in maniera controllata
        /// </summary>
        bool CanCloseForm = false;

        /// <summary>
        /// Serve a capire se ci troviamo nella fase di inizializzazione del form o in fasi succesive di utilizzo dell'app
        /// </summary>
        bool InitPhase = true;

        public UsbWatcherForm(string name, string id)
        {
            InitializeComponent();

            // Inizializza le informazioni base del dispositivo in esame
            DeviceName = name;
            DevicePath = name + "/";
            DeviceId = id;
            Text = " (" + DeviceName + ") " + Text;
                        
            clStartCount = dataGrid.ColumnCount;

            InitPhase = true;
            
        }

        private void UsbWatcherForm_Load(object sender, EventArgs e)
        {
            InitDatabase();
                        
            // Non sono il proprietario del dispositivo
            // è necessario che io inserisca una password
            if (OwnerMode == false)
            {
                var keyDialog = new KeyForm("Insert a password...");
                if (keyDialog.ShowDialog() == DialogResult.OK)
                {
                    UserPassword = keyDialog.Password;

                }
            }

            // Avvia il processo di osservazione del dispositivo
            fileSystemWatcher.Path = DevicePath;
            // Se sono il proprietario occorre modificare la tabella in modo tale da mostrare i vari gruppi 
            // che possono r/w il file
            UpdateGroupColumns();
            // che sia prorpietario o meno, questa operazione deve essere obbligatoriamente eseguita
            // altrimenti si avrebbe una tabella vuota
            InitFiles();            
            // Inizializza l'AGL dei file al primo accesso al dispositivo
            InitAGL();
            // Decripta tutti i file
            DecryptAllFiles();
            // Non siamo più nella fase di inizializzazione, da adesso ci troviamo nel vero utilizzo dell'applicazione
            InitPhase = false;
        }

        /// <summary>
        /// Inizializza i riferimenti alle tabelle del database
        /// </summary>
        void InitDatabase()
        {
            // Per il l'identificazione dei dispositivi
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

            // Sono il proprietario di questo dispositivo USB?
            // A seconda della risposta, questo form cambia leggermente comportamento
            OwnerMode = Devices.ImOwner(DeviceId);

            if (OwnerMode == false) return;
            // Per la gestione dei gruppi associati ai dispositivi, utile solo in OwnerMode
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
        }

        /// <summary>
        /// Riempie la griglia
        /// Rileva i file presenti nel dispositivo e li riporta nella tabella
        /// </summary>
        void InitFiles()
        {
            foreach (var file in Directory.GetFiles(DevicePath))
            {
                var attr = File.GetAttributes(file);
                if (attr != ( FileAttributes.Hidden | FileAttributes.Archive ) && attr != FileAttributes.Hidden)
                    dataGrid.Rows.Add(Path.GetFileName(file));
            }
            // Per scielta, decido di non gestire cartelle e sottocartelle del dispositivo,
            // in modo tale da semplificare un pò il progetto
            /*
            foreach (var folder in Directory.GetDirectories(DevicePath))
            {
                var attr = File.GetAttributes(folder);
                if (attr != (FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory))
                    dataGrid.Rows.Add(Path.GetFileName(folder), true);
            }
            */
        }

        /// <summary>
        /// Dopo l'avvio del Form per la gestione dei gruppi associati al dispositivo
        /// occorre aggiornare le colonne della griglia
        /// se sono il proprietario ci inserisco una colonna per ogni gruppo definito
        /// se non lo sono, ci inserisco solo una colonna per scegliere se proteggere o meno il file
        /// </summary>
        void UpdateGroupColumns()
        {
            if (OwnerMode)
            {
                var groups = Keys.GetGroups(DeviceId);
                foreach (var group in groups)
                {
                    if (ColumnExists(group) == false)
                    {
                        var column = new DataGridViewCheckBoxColumn();
                        column.Name = "cl" + group;
                        column.HeaderText = group;
                        column.Width = 50;
                        dataGrid.Columns.Add(column);
                    }
                }
                for (int i = clStartCount; i < dataGrid.Columns.Count; i++)
                {
                    var column = dataGrid.Columns[i];
                    if (groups.Contains(column.HeaderText) == false)
                    {
                        dataGrid.Columns.RemoveAt(i);
                    }
                }
            }
            else
            {
                if(InitPhase)
                {
                    var column = new DataGridViewCheckBoxColumn();
                    column.Name = "clEncrypt";
                    column.HeaderText = "Encrypt";
                    column.Width = 50;
                    dataGrid.Columns.Add(column);
                }
            }
        }

        #region OperazioniSuColonne

        /// <summary>
        /// Controlla se una determinata colonna esiste
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ColumnExists(string name)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].HeaderText == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Ottiene il nome della colonna dato l'indece
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetColumnName(int index)
        {
            if (index >= dataGrid.Columns.Count)
                return string.Empty;
            else
            {
                var column = dataGrid.Columns[index];
                return column.HeaderText;
            }
        }

        /// <summary>
        /// Ottiene l'indice della colonna dato il nome
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetColumnIndex(string name)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var column = dataGrid.Columns[i];
                if (column.HeaderText == name)
                    return i;
            }
            return -1;
        }

        #endregion

        // Inizializza l'AGL dei file presenti sul dispositivo
        void InitAGL()
        {
            var groups = new Dictionary<string, string>();
            if (OwnerMode)
            {
                groups = Keys.Get(DeviceId);
            }

            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];

                if (row.Cells[0].Value == null)
                    continue;
                string filename = DevicePath + row.Cells[0].Value.ToString();
                var agl = new Services.FileAGLService(filename);
                
                if(OwnerMode)
                {
                    if(agl.Exists())
                    {
                        foreach (var name in groups.Keys)
                        {
                            string temp = string.Empty;
                            int clIndex = GetColumnIndex(name);
                            if (clIndex != -1)
                            {
                                if (agl.Access(groups[name], out temp))
                                    row.Cells[clIndex].Value = true;
                            }
                        }
                    }
                }
                else
                {
                    // no wner mode
                }

            }
        }
        
        public void Exit()
        {
            CanCloseForm = true;
            Close();
        }

        private void UsbWatcherForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show(
                "",
                "",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // operazioni di salvataggio
                CanCloseForm = true;
            }
            if (CanCloseForm == false)
                e.Cancel = true;
        }

        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var process = Process.Start(DevicePath);
        }

        /// <summary>
        /// Il dialogo di gestione dei gruppi può essere aperto solo in ownermode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OwnerMode == false) return;

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

        private void cryptFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CryptAllFiles();
        }

        private void decryptFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DecryptAllFiles();
        }

        void CryptAllFiles()
        {
            for (var i = 0; i < dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];
                if (row.Cells[0].Value == null)
                    continue;

                var filename = DevicePath + row.Cells[0].Value.ToString();
                var crypto = new Services.FileEncryptService(filename);

                // se sono il prorpietario devo tener presente che posso criptare il file utilizzando più password
                // a seconda di quanti gruppi possono accedere al file
                if (OwnerMode)
                {
                    crypto.Encrypt(GetKeysByRowIndex(i));
                }
                else
                {
                    crypto.Encrypt(UserPassword);
                }
            }
        }

        /// <summary>
        /// Decripta tutti i file tenendo traccia sulla griglia del vecchio AGL per ognuno
        /// Tale operazione può essere effettuata solo al collegamento del dispositivo
        /// </summary>
        void DecryptAllFiles()
        {
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];
                if (row.Cells[0].Value == null)
                    continue;

                var filename = DevicePath + row.Cells[0].Value.ToString();

                var aglService = new Services.FileAGLService(filename);
                if (aglService.Exists() == false)
                    continue;
                var decryptoService = new Services.FileDecryptService(filename);

                string currentPassword = string.Empty;

                if(OwnerMode)
                {
                    // in tal caso mi basa una sola delle chiavi associate per accedere al file
                    var keys = GetKeysByRowIndex(i);
                    if (keys == null || keys.Count == 0)
                    {
                        dataGrid.Rows.RemoveAt(i);
                        continue;
                    }

                    currentPassword = keys[0];                 
                }
                else
                {
                    currentPassword = UserPassword;
                }
                
                var result = decryptoService.Decrypt(currentPassword, OwnerMode);
            }
        }

        /// <summary>
        /// PAssato l'indice di riga, mi ritorna la lista di tutte le chiavi associare al file
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public List<string> GetKeysByRowIndex(int rowIndex)
        {
            List<string> keys = new List<string>();
            var groups = Keys.Get(DeviceId);
            if (groups.Count == 0)
                return null;
            var row = dataGrid.Rows[rowIndex];
            if (row.Cells[0].Value == null)
                return null;
            for (int y = clStartCount; y < dataGrid.Columns.Count; y++)
            {
                var name = GetColumnName(y);
                var currentCell = row.Cells[y];
                var value = currentCell.Value;
                if (value != null && value.ToString() == true.ToString() && groups.ContainsKey(name))
                {
                    keys.Add(groups[name]); // tengo conto di tutte i gruppi che devono poter accedere al file
                }
            }
            return keys;
        }

    }
}
