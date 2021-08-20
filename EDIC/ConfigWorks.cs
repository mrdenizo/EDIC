using System;
using System.IO;
using Newtonsoft.Json;
using DiscordRPC;

namespace EDIC
{
    public class Config
    {
        //program config
        public string InaraApiKey = "";
        public string JournalPath = "";
        public string FrontierID = "";
        public bool DataToInara = false;
        public bool DiscordRpc = false;
        public bool Eddn = false;
        public string ChoosenLanguage = "";
        public bool Edsy = false;
    }
    public static class ConfigSaver
    {
        public static void SaveCfg(Config cfg)
        {
            //save config
            if (File.Exists("config.json")) File.Delete("config.json");
            StreamWriter sw = File.CreateText("config.json");
            sw.Write(JsonConvert.SerializeObject(cfg));
            sw.Close();
            sw.Dispose();
        }
        public static Config LoadCfg()
        {
            //load config
            Config cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            return cfg;
        }
    }
}
