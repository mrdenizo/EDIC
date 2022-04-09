
namespace EDIC
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Other = new System.Windows.Forms.TabPage();
            this.ExpotType = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.EDDNcheck = new System.Windows.Forms.CheckBox();
            this.UseRichPresence = new System.Windows.Forms.CheckBox();
            this.OpenDialogButton = new System.Windows.Forms.Button();
            this.JournalPath = new System.Windows.Forms.TextBox();
            this.JournalPathLabel = new System.Windows.Forms.Label();
            this.InaraSettings = new System.Windows.Forms.TabPage();
            this.FrontierIDlink = new System.Windows.Forms.LinkLabel();
            this.InaraCheck = new System.Windows.Forms.CheckBox();
            this.FrontierIdLabel = new System.Windows.Forms.Label();
            this.FrontierId = new System.Windows.Forms.TextBox();
            this.InaraApi = new System.Windows.Forms.TextBox();
            this.ApiKeyLabel = new System.Windows.Forms.Label();
            this.InaraLink = new System.Windows.Forms.LinkLabel();
            this.Language = new System.Windows.Forms.TabPage();
            this.OpenLangPacks = new System.Windows.Forms.Button();
            this.LangLabel = new System.Windows.Forms.Label();
            this.LangComboBox = new System.Windows.Forms.ComboBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.Other.SuspendLayout();
            this.InaraSettings.SuspendLayout();
            this.Language.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Other);
            this.tabControl1.Controls.Add(this.InaraSettings);
            this.tabControl1.Controls.Add(this.Language);
            this.tabControl1.Location = new System.Drawing.Point(0, -1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(674, 486);
            this.tabControl1.TabIndex = 0;
            // 
            // Other
            // 
            this.Other.Controls.Add(this.ExpotType);
            this.Other.Controls.Add(this.comboBox1);
            this.Other.Controls.Add(this.EDDNcheck);
            this.Other.Controls.Add(this.UseRichPresence);
            this.Other.Controls.Add(this.OpenDialogButton);
            this.Other.Controls.Add(this.JournalPath);
            this.Other.Controls.Add(this.JournalPathLabel);
            this.Other.Location = new System.Drawing.Point(4, 22);
            this.Other.Name = "Other";
            this.Other.Size = new System.Drawing.Size(666, 460);
            this.Other.TabIndex = 0;
            this.Other.Text = "Settings";
            this.Other.UseVisualStyleBackColor = true;
            // 
            // ExpotType
            // 
            this.ExpotType.AutoSize = true;
            this.ExpotType.Location = new System.Drawing.Point(89, 144);
            this.ExpotType.Name = "ExpotType";
            this.ExpotType.Size = new System.Drawing.Size(64, 13);
            this.ExpotType.TabIndex = 6;
            this.ExpotType.Text = "Export Type";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Coriolis",
            "EDSY"});
            this.comboBox1.Location = new System.Drawing.Point(171, 141);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(127, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // EDDNcheck
            // 
            this.EDDNcheck.AutoSize = true;
            this.EDDNcheck.Location = new System.Drawing.Point(92, 118);
            this.EDDNcheck.Name = "EDDNcheck";
            this.EDDNcheck.Size = new System.Drawing.Size(206, 17);
            this.EDDNcheck.TabIndex = 4;
            this.EDDNcheck.Text = "Send my data to EDDN(not works yet)";
            this.EDDNcheck.UseVisualStyleBackColor = true;
            // 
            // UseRichPresence
            // 
            this.UseRichPresence.AutoSize = true;
            this.UseRichPresence.Location = new System.Drawing.Point(92, 94);
            this.UseRichPresence.Name = "UseRichPresence";
            this.UseRichPresence.Size = new System.Drawing.Size(129, 17);
            this.UseRichPresence.TabIndex = 3;
            this.UseRichPresence.Text = "DiscordRichPresence";
            this.UseRichPresence.UseVisualStyleBackColor = true;
            // 
            // OpenDialogButton
            // 
            this.OpenDialogButton.Location = new System.Drawing.Point(213, 67);
            this.OpenDialogButton.Name = "OpenDialogButton";
            this.OpenDialogButton.Size = new System.Drawing.Size(113, 23);
            this.OpenDialogButton.TabIndex = 2;
            this.OpenDialogButton.Text = "Open dialog";
            this.OpenDialogButton.UseVisualStyleBackColor = true;
            this.OpenDialogButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // JournalPath
            // 
            this.JournalPath.Location = new System.Drawing.Point(80, 41);
            this.JournalPath.Name = "JournalPath";
            this.JournalPath.Size = new System.Drawing.Size(246, 20);
            this.JournalPath.TabIndex = 1;
            // 
            // JournalPathLabel
            // 
            this.JournalPathLabel.AutoSize = true;
            this.JournalPathLabel.Location = new System.Drawing.Point(8, 44);
            this.JournalPathLabel.Name = "JournalPathLabel";
            this.JournalPathLabel.Size = new System.Drawing.Size(66, 13);
            this.JournalPathLabel.TabIndex = 0;
            this.JournalPathLabel.Text = "Journal Path";
            // 
            // InaraSettings
            // 
            this.InaraSettings.Controls.Add(this.FrontierIDlink);
            this.InaraSettings.Controls.Add(this.InaraCheck);
            this.InaraSettings.Controls.Add(this.FrontierIdLabel);
            this.InaraSettings.Controls.Add(this.FrontierId);
            this.InaraSettings.Controls.Add(this.InaraApi);
            this.InaraSettings.Controls.Add(this.ApiKeyLabel);
            this.InaraSettings.Controls.Add(this.InaraLink);
            this.InaraSettings.Location = new System.Drawing.Point(4, 22);
            this.InaraSettings.Name = "InaraSettings";
            this.InaraSettings.Size = new System.Drawing.Size(666, 460);
            this.InaraSettings.TabIndex = 1;
            this.InaraSettings.Text = "Inara";
            this.InaraSettings.UseVisualStyleBackColor = true;
            // 
            // InaraCheck
            // 
            this.InaraCheck.AutoSize = true;
            this.InaraCheck.Location = new System.Drawing.Point(88, 173);
            this.InaraCheck.Name = "InaraCheck";
            this.InaraCheck.Size = new System.Drawing.Size(144, 17);
            this.InaraCheck.TabIndex = 5;
            this.InaraCheck.Text = "Send my data to Inara.cz";
            this.InaraCheck.UseVisualStyleBackColor = true;
            // 
            // FrontierIdLabel
            // 
            this.FrontierIdLabel.AutoSize = true;
            this.FrontierIdLabel.Location = new System.Drawing.Point(85, 114);
            this.FrontierIdLabel.Name = "FrontierIdLabel";
            this.FrontierIdLabel.Size = new System.Drawing.Size(56, 13);
            this.FrontierIdLabel.TabIndex = 4;
            this.FrontierIdLabel.Text = "Frontier ID";
            this.FrontierIdLabel.Visible = false;
            // 
            // FrontierId
            // 
            this.FrontierId.Location = new System.Drawing.Point(88, 130);
            this.FrontierId.Name = "FrontierId";
            this.FrontierId.Size = new System.Drawing.Size(436, 20);
            this.FrontierId.TabIndex = 3;
            this.FrontierId.Visible = false;
            // 
            // InaraApi
            // 
            this.InaraApi.Location = new System.Drawing.Point(88, 59);
            this.InaraApi.Name = "InaraApi";
            this.InaraApi.Size = new System.Drawing.Size(436, 20);
            this.InaraApi.TabIndex = 2;
            // 
            // ApiKeyLabel
            // 
            this.ApiKeyLabel.AutoSize = true;
            this.ApiKeyLabel.Location = new System.Drawing.Point(85, 43);
            this.ApiKeyLabel.Name = "ApiKeyLabel";
            this.ApiKeyLabel.Size = new System.Drawing.Size(71, 13);
            this.ApiKeyLabel.TabIndex = 1;
            this.ApiKeyLabel.Text = "Inara API key";
            // 
            // InaraLink
            // 
            this.InaraLink.AutoSize = true;
            this.InaraLink.Location = new System.Drawing.Point(114, 82);
            this.InaraLink.Name = "InaraLink";
            this.InaraLink.Size = new System.Drawing.Size(107, 13);
            this.InaraLink.TabIndex = 0;
            this.InaraLink.TabStop = true;
            this.InaraLink.Text = "Get my Inara API key";
            this.InaraLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Inara_LinkClicked);
            // 
            // Language
            // 
            this.Language.Controls.Add(this.OpenLangPacks);
            this.Language.Controls.Add(this.LangLabel);
            this.Language.Controls.Add(this.LangComboBox);
            this.Language.Location = new System.Drawing.Point(4, 22);
            this.Language.Name = "Language";
            this.Language.Size = new System.Drawing.Size(666, 460);
            this.Language.TabIndex = 2;
            this.Language.Text = "Language";
            this.Language.UseVisualStyleBackColor = true;
            // 
            // OpenLangPacks
            // 
            this.OpenLangPacks.Location = new System.Drawing.Point(289, 77);
            this.OpenLangPacks.Name = "OpenLangPacks";
            this.OpenLangPacks.Size = new System.Drawing.Size(140, 36);
            this.OpenLangPacks.TabIndex = 2;
            this.OpenLangPacks.Text = "Open my langugage packs folder";
            this.OpenLangPacks.UseVisualStyleBackColor = true;
            this.OpenLangPacks.Click += new System.EventHandler(this.OpenLangPacks_Click);
            // 
            // LangLabel
            // 
            this.LangLabel.AutoSize = true;
            this.LangLabel.Location = new System.Drawing.Point(8, 53);
            this.LangLabel.Name = "LangLabel";
            this.LangLabel.Size = new System.Drawing.Size(90, 13);
            this.LangLabel.TabIndex = 1;
            this.LangLabel.Text = "Choose language";
            // 
            // LangComboBox
            // 
            this.LangComboBox.FormattingEnabled = true;
            this.LangComboBox.Location = new System.Drawing.Point(104, 50);
            this.LangComboBox.Name = "LangComboBox";
            this.LangComboBox.Size = new System.Drawing.Size(325, 21);
            this.LangComboBox.TabIndex = 0;
            this.LangComboBox.SelectedIndexChanged += new System.EventHandler(this.LangComboBox_SelectedIndexChanged);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(582, 484);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(92, 25);
            this.OkButton.TabIndex = 1;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.UserProfile;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 509);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "EDIC Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.Other.ResumeLayout(false);
            this.Other.PerformLayout();
            this.InaraSettings.ResumeLayout(false);
            this.InaraSettings.PerformLayout();
            this.Language.ResumeLayout(false);
            this.Language.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Other;
        private System.Windows.Forms.TabPage InaraSettings;
        private System.Windows.Forms.LinkLabel InaraLink;
        private System.Windows.Forms.TextBox InaraApi;
        private System.Windows.Forms.Label ApiKeyLabel;
        private System.Windows.Forms.Label JournalPathLabel;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button OpenDialogButton;
        private System.Windows.Forms.TextBox JournalPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label FrontierIdLabel;
        private System.Windows.Forms.TextBox FrontierId;
        private System.Windows.Forms.CheckBox InaraCheck;
        private System.Windows.Forms.LinkLabel FrontierIDlink;
        private System.Windows.Forms.CheckBox UseRichPresence;
        private System.Windows.Forms.CheckBox EDDNcheck;
        private System.Windows.Forms.TabPage Language;
        private System.Windows.Forms.Label LangLabel;
        private System.Windows.Forms.ComboBox LangComboBox;
        private System.Windows.Forms.Button OpenLangPacks;
        private System.Windows.Forms.Label ExpotType;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}