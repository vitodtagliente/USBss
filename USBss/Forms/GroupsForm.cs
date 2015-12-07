using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace USBss.Forms
{
    public partial class GroupsForm : Form
    {
        /// <summary>
        /// Id del dispositivo
        /// </summary>
        string DeviceId;
        
        /// <summary>
        /// Riferimento alla tabella delle chiavi
        /// </summary>
        Database.Tables.Keys Keys;

        /// <summary>
        /// I gruppi sono stati modificati? 
        /// In caso affermativo chiedi conferma prima di permettere all'utente di chiudere il form 
        /// </summary>
        bool edited = false;

        public GroupsForm(string name, string deviceId)
        {
            InitializeComponent();

            Text = name + " - ACL Groups";
            DeviceId = deviceId;

            Init();
        }

        /// <summary>
        /// Popola la tabella utilizzando le informazioni memorizzate nel database
        /// </summary>
        void Init()
        {
            Keys = Database.LiteDatabase.singleton.GetTable("keys") as Database.Tables.Keys;
            var fetch = Keys.Get(DeviceId);
            foreach(var group in fetch.Keys)
            {
                dataGrid.Rows.Add(group, fetch[group]);
            }
        }

        /// <summary>
        /// Converte il contenuto della tabella in un dizionario
        /// sicuramente più facile da manipolare
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> ParseGrid()
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                var row = dataGrid.Rows[i];
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    result.Add(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
            }
            return result;
        }

        /// <summary>
        /// Aggiorna il database, aggiungi/rimuovi elementi in base alle modifiche fatte
        /// </summary>
        /// <returns></returns>
        bool UpdateDatabase()
        {
            var fetch = Keys.Get(DeviceId);
            var grid = ParseGrid();

            foreach(var key in grid.Keys)
            {
                if(fetch.Keys.ToList().Contains(key) == false)
                {
                    var result = Keys.Insert("insert into keys (id, deviceId, group, key) VALUES ( NULL, '" + 
                        DeviceId + "', '" + key + "', '" + grid[key] + "')");
                    if (result == false)
                        return false;
                }
            }
            foreach (var key in fetch.Keys)
            {
                if (grid.Keys.ToList().Contains(key) == false)
                {
                    var result = Keys.Insert("delete from keys where deviceId = '" + DeviceId + "' AND group = '" + fetch[key] + "'");
                    if (!result)
                        return false;
                }
            }

            return true;
        }

        void EndHandler()
        {
            edited = false;
            if (!UpdateDatabase())
            {
                MessageBox.Show(
                    "Something went wrong!",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                Close();
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            EndHandler();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGrid_SelectionChanged(object sender, EventArgs e)
        {
            edited = true;
        }

        private void GroupsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(edited)
            {
                if (MessageBox.Show(
                    "Would you like to save changes?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    ) == DialogResult.Yes)
                    EndHandler();
                else
                {
                    edited = false;
                    Close();
                }
            }
        }
    }
}
