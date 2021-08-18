
namespace EDIC
{
    partial class EDICmainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EDICmainForm));
            this.CMDR_lable = new System.Windows.Forms.Label();
            this.SysName = new System.Windows.Forms.Label();
            this.SysLink = new System.Windows.Forms.LinkLabel();
            this.StarportName = new System.Windows.Forms.Label();
            this.StarportLink = new System.Windows.Forms.LinkLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.LastUpdated = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ShipLabel = new System.Windows.Forms.Label();
            this.ShipLink = new System.Windows.Forms.LinkLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CMDR_lable
            // 
            this.CMDR_lable.AutoSize = true;
            this.CMDR_lable.Location = new System.Drawing.Point(12, 29);
            this.CMDR_lable.MinimumSize = new System.Drawing.Size(42, 13);
            this.CMDR_lable.Name = "CMDR_lable";
            this.CMDR_lable.Size = new System.Drawing.Size(42, 13);
            this.CMDR_lable.TabIndex = 0;
            this.CMDR_lable.Text = "CMDR ";
            // 
            // SysName
            // 
            this.SysName.AutoSize = true;
            this.SysName.Location = new System.Drawing.Point(12, 69);
            this.SysName.Name = "SysName";
            this.SysName.Size = new System.Drawing.Size(44, 13);
            this.SysName.TabIndex = 1;
            this.SysName.Text = "System:";
            // 
            // SysLink
            // 
            this.SysLink.AutoSize = true;
            this.SysLink.Location = new System.Drawing.Point(62, 69);
            this.SysLink.Name = "SysLink";
            this.SysLink.Size = new System.Drawing.Size(44, 13);
            this.SysLink.TabIndex = 2;
            this.SysLink.TabStop = true;
            this.SysLink.Text = "SysLink";
            this.SysLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // StarportName
            // 
            this.StarportName.AutoSize = true;
            this.StarportName.Location = new System.Drawing.Point(12, 91);
            this.StarportName.Name = "StarportName";
            this.StarportName.Size = new System.Drawing.Size(47, 13);
            this.StarportName.TabIndex = 3;
            this.StarportName.Text = "Starport:";
            // 
            // StarportLink
            // 
            this.StarportLink.AutoSize = true;
            this.StarportLink.Location = new System.Drawing.Point(62, 91);
            this.StarportLink.Name = "StarportLink";
            this.StarportLink.Size = new System.Drawing.Size(64, 13);
            this.StarportLink.TabIndex = 4;
            this.StarportLink.TabStop = true;
            this.StarportLink.Text = "StarportLink";
            this.StarportLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StarportLink_LinkClicked);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(15, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Force update";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LastUpdated
            // 
            this.LastUpdated.AutoSize = true;
            this.LastUpdated.Location = new System.Drawing.Point(12, 133);
            this.LastUpdated.Name = "LastUpdated";
            this.LastUpdated.Size = new System.Drawing.Size(75, 13);
            this.LastUpdated.TabIndex = 6;
            this.LastUpdated.Text = "Last updated: ";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ShipLabel
            // 
            this.ShipLabel.AutoSize = true;
            this.ShipLabel.Location = new System.Drawing.Point(12, 47);
            this.ShipLabel.Name = "ShipLabel";
            this.ShipLabel.Size = new System.Drawing.Size(34, 13);
            this.ShipLabel.TabIndex = 7;
            this.ShipLabel.Text = "Ship: ";
            // 
            // ShipLink
            // 
            this.ShipLink.AutoSize = true;
            this.ShipLink.Location = new System.Drawing.Point(62, 47);
            this.ShipLink.Name = "ShipLink";
            this.ShipLink.Size = new System.Drawing.Size(48, 13);
            this.ShipLink.TabIndex = 8;
            this.ShipLink.TabStop = true;
            this.ShipLink.Text = "ShipLink";
            this.ShipLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ShipLink_LinkClicked);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(200, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // EDICmainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 155);
            this.Controls.Add(this.ShipLink);
            this.Controls.Add(this.ShipLabel);
            this.Controls.Add(this.LastUpdated);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.StarportLink);
            this.Controls.Add(this.StarportName);
            this.Controls.Add(this.SysLink);
            this.Controls.Add(this.SysName);
            this.Controls.Add(this.CMDR_lable);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EDICmainForm";
            this.Text = "EDIC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EDICmainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CMDR_lable;
        private System.Windows.Forms.Label SysName;
        private System.Windows.Forms.LinkLabel SysLink;
        private System.Windows.Forms.Label StarportName;
        private System.Windows.Forms.LinkLabel StarportLink;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label LastUpdated;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label ShipLabel;
        private System.Windows.Forms.LinkLabel ShipLink;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
    }
}

