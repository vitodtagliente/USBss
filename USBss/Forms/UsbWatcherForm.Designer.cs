namespace USBss.Forms
{
    partial class UsbWatcherForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UsbWatcherForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.setGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.securityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cryptFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decryptFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logText = new System.Windows.Forms.RichTextBox();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.clFilename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.securityToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(334, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showInExplorerToolStripMenuItem,
            this.toolStripSeparator1,
            this.setGroupsToolStripMenuItem,
            this.closeDeviceToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // showInExplorerToolStripMenuItem
            // 
            this.showInExplorerToolStripMenuItem.Name = "showInExplorerToolStripMenuItem";
            this.showInExplorerToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.showInExplorerToolStripMenuItem.Text = "&Show in Explorer";
            this.showInExplorerToolStripMenuItem.Click += new System.EventHandler(this.showInExplorerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // setGroupsToolStripMenuItem
            // 
            this.setGroupsToolStripMenuItem.Name = "setGroupsToolStripMenuItem";
            this.setGroupsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setGroupsToolStripMenuItem.Text = "&Set Groups";
            this.setGroupsToolStripMenuItem.Visible = false;
            this.setGroupsToolStripMenuItem.Click += new System.EventHandler(this.setGroupsToolStripMenuItem_Click);
            // 
            // closeDeviceToolStripMenuItem
            // 
            this.closeDeviceToolStripMenuItem.Name = "closeDeviceToolStripMenuItem";
            this.closeDeviceToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.closeDeviceToolStripMenuItem.Text = "&Close Device";
            this.closeDeviceToolStripMenuItem.Click += new System.EventHandler(this.closeDeviceToolStripMenuItem_Click);
            // 
            // securityToolStripMenuItem
            // 
            this.securityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cryptFilesToolStripMenuItem,
            this.decryptFilesToolStripMenuItem});
            this.securityToolStripMenuItem.Name = "securityToolStripMenuItem";
            this.securityToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.securityToolStripMenuItem.Text = "&Security";
            this.securityToolStripMenuItem.Visible = false;
            // 
            // cryptFilesToolStripMenuItem
            // 
            this.cryptFilesToolStripMenuItem.Name = "cryptFilesToolStripMenuItem";
            this.cryptFilesToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.cryptFilesToolStripMenuItem.Text = "&Encrypt Files";
            this.cryptFilesToolStripMenuItem.Click += new System.EventHandler(this.cryptFilesToolStripMenuItem_Click);
            // 
            // decryptFilesToolStripMenuItem
            // 
            this.decryptFilesToolStripMenuItem.Name = "decryptFilesToolStripMenuItem";
            this.decryptFilesToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.decryptFilesToolStripMenuItem.Text = "&Decrypt Files";
            this.decryptFilesToolStripMenuItem.Click += new System.EventHandler(this.decryptFilesToolStripMenuItem_Click);
            // 
            // logText
            // 
            this.logText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logText.Location = new System.Drawing.Point(0, 24);
            this.logText.Name = "logText";
            this.logText.ReadOnly = true;
            this.logText.Size = new System.Drawing.Size(334, 337);
            this.logText.TabIndex = 2;
            this.logText.Text = "";
            // 
            // dataGrid
            // 
            this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clFilename});
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.Location = new System.Drawing.Point(0, 24);
            this.dataGrid.MultiSelect = false;
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.Size = new System.Drawing.Size(334, 337);
            this.dataGrid.TabIndex = 4;
            this.dataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellValueChanged);
            // 
            // clFilename
            // 
            this.clFilename.HeaderText = "Filename";
            this.clFilename.Name = "clFilename";
            this.clFilename.ReadOnly = true;
            this.clFilename.Width = 150;
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Created);
            this.fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Deleted);
            this.fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileSystemWatcher_Renamed);
            // 
            // UsbWatcherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 361);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.logText);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UsbWatcherForm";
            this.Opacity = 0.8D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "USBss - Watcher";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UsbWatcherForm_FormClosing);
            this.Load += new System.EventHandler(this.UsbWatcherForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.RichTextBox logText;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem setGroupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem securityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cryptFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decryptFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeDeviceToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn clFilename;
    }
}