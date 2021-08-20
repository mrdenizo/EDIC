using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDIC
{
    public class LangPack
    {
        public Dictionary<string, string> lang = new Dictionary<string, string>();
        public LangPack()
        {
            //Main Form translation
            lang.Add("EDICFORM_MENUSTRIP_FILE_BUTTON", "File"); //Menu strip file button text
            lang.Add("EDICFORM_MENUSTRIP_FILE_SETTINGS", "Settings"); //Menu strip setting text

            lang.Add("EDICFORM_CMDRLABELSTART", "CMDR: "); //Cmdr lable start
            lang.Add("EDICFORM_SHIPLABLESTART", "Ship:"); //Ship name start
            lang.Add("EDICFORM_SYSTEMLABLESTART", "System:"); //System name start
            lang.Add("EDICFORM_STATIONLABLESTART", "Starport:"); //Station name start

            lang.Add("EDICFORM_LASTUPDATELABLESTART", "Last updated:"); //Last updated
            lang.Add("EDICFORM_FORCEUPDATEBUTTON", "Force update"); //Force update button text
            lang.Add("EDICFORM_FORCEUPDATEBUTTONPAUSE", "Pause "); //Pause (time) text on button

            lang.Add("EDICFORM_TEXT", "EDIC"); //Main form capture text


            //Settings Form translation
            lang.Add("SETTINGSFORM_TABS_SETTINGSTAB", "Settings");//Settings tab text
            lang.Add("SETTINGSFORM_TABS_INARA", "Inara"); //Inara tab text
            lang.Add("SETTINGSFORM_TABS_LANG", "Language"); //Language tab text

            lang.Add("SETTINGSFORM_JOURNALPATHLABEL", "Journal Path"); //Journal path text 
            lang.Add("SETTINGSFORM_JOURNALDIALOGOPEN", "Open dialog"); //Open dialog button
            lang.Add("SETTINGSFORM_CHECK_DISCORDRPC", "DiscordRichPresence"); //Discord RPC checkbox
            lang.Add("SETTINGSFORM_EXPORTTYPELABEL", "Export Type"); //export type label
            lang.Add("SETTINGSFORM_CHECK_SENDDATATOEDDN", "Send my data to EDDN(not works yet)"); //Send my data to EDDN checkbox

            lang.Add("SETTINGSFORM_INARAAPIKEYLABEL", "Inara API key"); //Inara API key text
            lang.Add("SETTINGSFORM_GETMYINARAAPIKYLINK", "Get my Inara API key"); //Get my inara API key link text
            lang.Add("SETTINGSFORM_FRONTIERIDLABEL", "Frontier ID"); //Frontier ID text
            lang.Add("SETTINGSFORM_GETMYFROMTIERIDLINK", "Get my frontier ID"); //Get my frontier ID key link text
            lang.Add("SETTINGSFORM_CHECK_SENDINARA", "Send my data to Inara.cz"); //Send my data to Inara checkbox

            lang.Add("SETTINGSFORM_CHOOSELANGUAGELABEL", "Choose language"); //Choose langugage text
            lang.Add("SETTINGSFORM_OPENMYPACKAGEFOLDER", "Open my langugage packs folder"); //Open my language packs folder button text

            lang.Add("SETTINGSFORM_OKBUTTON", "OK"); //OK button
            lang.Add("SETTINGSFORM_TEXT", "EDIC Settings"); //Settings form capture

            //Discord RPC
            lang.Add("DISCORD_RPC_DOCKEDAT", "Docked at: "); //Discord RPC when docked text
            lang.Add("DISCORD_RPC_INSYSTEM", "In system: "); //Discord RPC system marker
            lang.Add("DISCORD_RPC_PLAYINGED", "Playing Elite:Dangerous"); //Shown in RPC state when not docked
        }
    }
}
