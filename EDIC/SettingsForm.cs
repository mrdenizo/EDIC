using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDIC
{
    public partial class SettingsForm : Form
    {
        private LangPack lang = new LangPack();
        public Config cfg;
        public SettingsForm(Config cfg)
        {
            this.cfg = cfg;
            InitializeComponent();
            InaraApi.Text = cfg.InaraApiKey;
            //FrontierId.Text = cfg.FrontierID;
            JournalPath.Text = cfg.JournalPath;
            InaraCheck.Checked = cfg.DataToInara;
            UseRichPresence.Checked = cfg.DiscordRpc;
            EDDNcheck.Checked = cfg.Eddn;
            comboBox1.SelectedIndex = cfg.Edsy ? 1 : 0;
        }

        private void LoadTranslation()
        {
            this.Text = lang.lang["SETTINGSFORM_TEXT"];
            Other.Text = lang.lang["SETTINGSFORM_TABS_SETTINGSTAB"];
            InaraSettings.Text = lang.lang["SETTINGSFORM_TABS_INARA"];
            Language.Text = lang.lang["SETTINGSFORM_TABS_LANG"];
            JournalPathLabel.Text = lang.lang["SETTINGSFORM_JOURNALPATHLABEL"];
            OpenDialogButton.Text = lang.lang["SETTINGSFORM_JOURNALDIALOGOPEN"];
            UseRichPresence.Text = lang.lang["SETTINGSFORM_CHECK_DISCORDRPC"];
            EDDNcheck.Text = lang.lang["SETTINGSFORM_CHECK_SENDDATATOEDDN"];
            ApiKeyLabel.Text = lang.lang["SETTINGSFORM_INARAAPIKEYLABEL"];
            InaraLink.Text = lang.lang["SETTINGSFORM_GETMYINARAAPIKYLINK"];
            //FrontierIdLabel.Text = lang.lang["SETTINGSFORM_FRONTIERIDLABEL"];
            //FrontierIDlink.Text = lang.lang["SETTINGSFORM_GETMYFROMTIERIDLINK"];
            InaraCheck.Text = lang.lang["SETTINGSFORM_CHECK_SENDINARA"];
            LangLabel.Text = lang.lang["SETTINGSFORM_CHOOSELANGUAGELABEL"];
            OpenLangPacks.Text = lang.lang["SETTINGSFORM_OPENMYPACKAGEFOLDER"];
            OkButton.Text = lang.lang["SETTINGSFORM_OKBUTTON"];
            ExpotType.Text = lang.lang["SETTINGSFORM_EXPORTTYPELABEL"];
        }

        private void Inara_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://inara.cz/settings-api/");
        }

        private void FrontierIDlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://user.frontierstore.net/user/info");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                JournalPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //cfg.FrontierID = FrontierId.Text;
            cfg.InaraApiKey = InaraApi.Text;
            cfg.JournalPath = JournalPath.Text;
            cfg.DataToInara = InaraCheck.Checked;
            cfg.DiscordRpc = UseRichPresence.Checked;
            cfg.Eddn = EDDNcheck.Checked;
            cfg.ChoosenLanguage = "Language Packs\\" + LangComboBox.SelectedItem + ".json";
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            List<string> files = Directory.GetFiles("Language Packs").ToList();
            for(int i = 0; i < files.Count; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            LangComboBox.Items.AddRange(files.ToArray());
            LangComboBox.SelectedItem = Path.GetFileNameWithoutExtension(cfg.ChoosenLanguage);
            lang = JsonConvert.DeserializeObject<LangPack>(File.ReadAllText(cfg.ChoosenLanguage));
            LoadTranslation();
        }

        private void OpenLangPacks_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "Language Packs"));
        }

        private void LangComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            cfg.ChoosenLanguage = "Language Packs\\" + LangComboBox.SelectedItem + ".json";
            lang = JsonConvert.DeserializeObject<LangPack>(File.ReadAllText(cfg.ChoosenLanguage));
            LoadTranslation();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)
            {
                cfg.Edsy = false;
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                cfg.Edsy = true;
            }
        }
    }
}
