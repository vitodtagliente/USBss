using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using WinUSB;
using USBss.Database;

namespace USBss
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Questo file deve essere presente sul dispositivo USB per notificare 
        /// l'avvenuta installazione del sistema di sicurezza.
        /// </summary>
        public static readonly string UsbConfigurationFile = "ssconfig.ss";

        /// <summary>
        /// Locazione in cui viene creato il database dell'applicazione
        /// </summary>
        public static readonly string AppDatabaseFilename = "devices.db";
        
        /// <summary>
        /// Pattern per l'accesso alla sola istanza della classe
        /// </summary>
        public static MainForm singleton { get; private set; }

        /// <summary>
        /// Gestione Win32 Devices ed eventi
        /// </summary>
        public UsbManager UsbManager { get; private set; }

        public List<Forms.UsbWatcherForm> Watchers { get; private set; }

        /// <summary>
        /// Forza la chiusura dell'applicazione, 
        /// evita che venga ridotta a icona nell'area notifiche di Windows
        /// </summary>
        bool bruteClose = false;

        /// <summary>
        /// Accesso al database locale dell'applicazione
        /// </summary>
        public LiteDatabase Database { get; private set; }

        public static readonly string IV = "usbsecuritysys0A";

        public MainForm()
        {
            InitializeComponent();
            
            singleton = this;

            Database = new LiteDatabase(AppDatabaseFilename);
            Database.Tables.Add(new Database.Tables.Devices());
            Database.Tables.Add(new Database.Tables.Keys());
            Database.Open();

            Watchers = new List<Forms.UsbWatcherForm>();
        }
        
        /// <summary>
        /// All'avvio controlla se ci sono dispositivi connessi
        /// e inizializza il il gestore di eventi
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            UsbManager = new UsbManager();
            UsbManager.StateChanged += new UsbStateChangedEventHandler(DeviceStateChanged);

            // Directory di lavoro per la gestione dei file in user mode
            if (Directory.Exists(Services.FileSSRestoreService.DataDirectory) == false)
                Directory.CreateDirectory(Services.FileSSRestoreService.DataDirectory);

            // display all devices 
            UsbDiskCollection disks = UsbManager.GetAvailableDisks();

            WriteLine("Available USB disks:", Color.DarkCyan);
            foreach (UsbDisk disk in disks)
            {
                WriteLine(disk.ToString());
                OnDeviceAdded(disk.Name);
            }
            WriteLine(string.Empty);

            WriteLine("Waiting for events...", Color.DarkCyan);

            Tests();           
        }

        void Tests()
        {

        }

        /// <summary>
        /// Gestione degli eventi di collegamento/scollegamento dispositivo USB
        /// </summary>
        void DeviceStateChanged(UsbStateChangedEventArgs e)
        {
            string text = string.Empty;
            Color color = Color.Black;
            if(e.State == UsbStateChange.Removed)
            {
                color = Color.DarkRed;
                text = e.Disk.Name + " Removed.";

                OnDeviceRemoved(e.Disk.Name);
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

                OnDeviceRemoving(e.Disk.Name);
            }

            WriteLine(text, color);
        }

        /// <summary>
        /// Evento richiamato al collegamento di un nuovo dispositivo USB
        /// 
        /// Controlla se il dispositivo presenta l'installazione del sistema di sicurezza
        /// 1 - nel caso negativo richiede la conferma per procedere con l'installazione
        /// 2 - nel caso affermativo avvia l'operazione di decrittazione
        /// </summary>
        /// <param name="name">Nome della periferica. Esempio C:, D:, E:, F:, etc...</param>
        void OnDeviceAdded(string name)
        {
            var idService = new Services.DeviceIdentificationService(name);
            var result = idService.Identify();
            if(result.Ok)
            {
                // Avvia il watcher
                DeviceWatcherHandler(name, result.Id);
                return;
            }
            else
            {
                if (result.FileExists && string.IsNullOrEmpty(result.Id))
                {
                    // Il protocollo ss è stato installato sul dispositivo
                    // ma risulta corroto, non possiamo dedurre niente dallo stato corrente
                    // Errore fatale, Il dispositivo è inutilizzabile
                    MessageBox.Show(
                        "The Device's ID is invalid! Cannot access Device.",
                        "USB Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                        );
                    return;
                }                
                else if (result.FileExists == false)
                {
                    // Il dispositivo non presenta alcun modulo ss installato
                    if (MessageBox.Show(
                       "Would you like to setup Security System on this USB Device?",
                       "Configure USB Device?",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question
                       ) == DialogResult.Yes)
                    {
                        var setupService = new Services.DeviceSetupService(idService.Filename);
                        var setupResult = setupService.Setup();
                        if (setupResult)
                        {
                            DeviceWatcherHandler(name, setupService.Id);
                            return;
                        }
                        else
                            MessageBox.Show(
                           "Fatal error during device setup!",
                           "Setup Error",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                    }
                }
                else if (result.FileExists && result.EntryExists == false)
                {
                    // Non sei il propretario del dispositivo
                    // Inseriscilo nel database e in seguito chiedi la chiave di accesso
                    var setupService = new Services.DeviceSetupService(idService.Filename);
                    var setupResult = setupService.Setup(result.Id);
                    if (setupResult)
                    {
                        DeviceWatcherHandler(name, result.Id);
                        return;
                    }
                    else
                        MessageBox.Show(
                       "Fatal error during device setup!",
                       "Setup Error",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                }
            }
        }

        void DeviceWatcherHandler(string deviceName, string deviceId)
        {
            var watcher = Forms.UsbWatcherForm.Start(deviceName, deviceId);
        }

        void OnDeviceRemoving(string name)
        {

        }

        void OnDeviceRemoved(string name)
        {
            Forms.UsbWatcherForm.Close(name);
        }
                
        #region Logging

        public void Write(string text)
        {
            Write(text, Color.Black);
        }

        public void Write(string text, Color color)
        {
            try
            {
                logText.SelectionStart = logText.TextLength;
                logText.SelectionLength = 0;

                logText.SelectionColor = color;

                logText.AppendText(text);
                logText.SelectionColor = logText.ForeColor;
            }
            catch (Exception) { }
        }

        public void WriteLine(string text)
        {
            WriteLine(text, Color.Black);
        }

        public void WriteLine(string text, Color color)
        {
            Write(text + Environment.NewLine, color);
        }

        #endregion

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
            Database.Close();
            Close();
        }

        #endregion
    }
}
