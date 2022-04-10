using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Reflection;
using System.Windows.Forms;
using EDCSLogreader;
using Newtonsoft.Json;
using DiscordRPC;
using System.IO;
using Newtonsoft.Json.Linq;

namespace EDIC
{
    public partial class EDICmainForm : Form
    {
        public EDICmainForm()
        {
            InitializeComponent();
        }
        private Inara inara = new Inara();
        private Eddn eddn = new Eddn();
        public static string AppVer = "2.4.0";
        private LangPack lang = new LangPack();
        private long ShipID = 0;
        private EDCSLogreader.Events.LoadoutInfo ShipJSON;
        private string ship = "";
        private string StarSystem;
        private string StarPort;
        public Config config;
        private EliteDangerousAPI api;
        private DiscordRpcClient client;
        private cAPI capi;
        private DateTime StartTimeStamp = DateTime.UtcNow;
        private Dictionary<string, string> ShipNamePairs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "SideWinder", "Sidewinder" },
            { "Eagle", "Eagle" },
            { "Hauler", "Hauler" },
            { "Adder", "Adder" },
            { "Viper", "Viper MkIII" },
            { "CobraMkIII", "Cobra MkIII" },
            { "Type6", "Type-6 Transporter" },
            { "Dolphin", "Dolphin" },
            { "Type7", "Type-7 Transporter" },
            { "Asp", "Asp Explorer" },
            { "Vulture", "Vulture" },
            { "Empire_Trader", "Imperial Clipper" },
            { "Federation_Dropship", "Federal Dropship" },
            { "Orca", "Orca" },
            { "Type9", "Type-9 Heavy" },
            { "Python", "Python" },
            { "BelugaLiner", "Beluga Liner" },
            { "FerDeLance", "Fer-de-Lance" },
            { "Anaconda", "Anaconda" },
            { "Federation_Corvette", "Federal Corvette" },
            { "Cutter", "Imperial Cutter" },
            { "DiamondBack", "Diamondback Scout" },
            { "Empire_Courier", "Imperial Courier" },
            { "DiamondBackXL", "Diamondback Explorer" },
            { "Empire_Eagle", "Imperial Eagle" },
            { "Federation_Dropship_MkII", "Federal Assault Ship" },
            { "Federation_Gunship", "Federal Gunship" },
            { "Viper_MkIV", "Viper MkIV" },
            { "CobraMkIV", "Cobra" },
            { "Independant_Trader", "Keelback" },
            { "Asp_Scout", "Asp Scout" },
            { "Type9_Military", "Type-10" },
            { "Krait_MkII", "Krait MkII" },
            { "TypeX", "Alliance Chieftain" },
            { "TypeX_2", "Alliance Crusader" },
            { "TypeX_3", "Alliance Challenger" },
            { "Krait_Light", "Krait Phantom" },
            { "Mamba", "Mamba" },
            { "Unknown", "Unknown" }
        };

        private void Form1_Load(object sender, EventArgs e)
        {
            //loading
            LoadCfg();
            LoadTranslation();
            if (System.Diagnostics.Process.GetProcessesByName(this.Text).Length > 1)
            {
                MessageBox.Show("EDIC process is already running", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            LoadApp();
        }

        private void LoadTranslation()
        {
            this.Text = lang.lang["EDICFORM_TEXT"];
            fileToolStripMenuItem.Text = lang.lang["EDICFORM_MENUSTRIP_FILE_BUTTON"];
            settingsToolStripMenuItem.Text = lang.lang["EDICFORM_MENUSTRIP_FILE_SETTINGS"];
            ShipLabel.Text = lang.lang["EDICFORM_SHIPLABLESTART"];
            SysName.Text = lang.lang["EDICFORM_SYSTEMLABLESTART"];
            StarportName.Text = lang.lang["EDICFORM_STATIONLABLESTART"];
        }

        private void LoadCfg()
        {
            //reading config
            if (File.Exists("config.json"))
            {
                config = ConfigSaver.LoadCfg();
            }
            else
            {
                //writing standard journal path to config if not exists
                config = new Config();
                config.JournalPath = System.IO.Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "saved games\\Frontier Developments\\Elite Dangerous");
                //config.FrontierID = "put your frontier ID here";
                config.InaraApiKey = "put your Inara API here";
                config.DataToInara = false;
                config.DiscordRpc = false;
                config.ChoosenLanguage = "Language Packs\\English.json";
                config.Edsy = false;
                ConfigSaver.SaveCfg(config);
            }
            //checking languages
            if (!Directory.Exists("Language Packs"))
            {
                Directory.CreateDirectory("Language Packs");
            }
            if (!Directory.Exists("Plugins"))
            {
                Directory.CreateDirectory("Plugins");
            }
            if (!File.Exists("Language Packs\\English.json"))
            {
                using (StreamWriter sr = File.CreateText("Language Packs\\English.json"))
                {
                    sr.Write(JsonConvert.SerializeObject(new LangPack(), Formatting.Indented));
                }
            }
            lang = JsonConvert.DeserializeObject<LangPack>(File.ReadAllText(config.ChoosenLanguage));
        }

        private void LoadApp()
        {
            OAuth2 auth = OAuth2.Load();
            if (auth == null || !auth.Refresh())
            {
                var req = OAuth2.Authorize();
                auth = req.GetAuth();
            }
            auth.Save();
            capi = new cAPI(auth);
            api = new EliteDangerousAPI(new DirectoryInfo(config.JournalPath));
            api.Start();
            if (api.Commander != null)
            {
                CMDR_lable.Text = lang.lang["EDICFORM_CMDRLABELSTART"] + api.Commander.Commander;
                StarSystem = api.Location.StarSystem;
                SysLink.Text = api.Location.StarSystem;
                LastUpdated.Text = lang.lang["EDICFORM_LASTUPDATELABLESTART"] + HMS();
                if (config.DiscordRpc)
                {
                    client = new DiscordRpcClient("877505120856862750");
                    client.Initialize();
                    client.SetPresence(new RichPresence()
                    {
                        State = lang.lang["DISCORD_RPC_PLAYINGED"],
                        Details = "",
                        Timestamps = new Timestamps 
                        {
                            Start = StartTimeStamp,
                        }
                    });
                    client.UpdateStartTime();
                }
                else
                {
                    if (client != null && !client.IsDisposed)
                    {
                        client.ClearPresence();
                        client.Dispose();
                    }
                }
                //reading journal to get info about ship and cmdr location
                using (StreamReader sr = new StreamReader(File.Open(GetNewestFile(api.journalDirectory).FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    EDCSLogreader.Events.LoadoutInfo LastLoadoutInfo = new EDCSLogreader.Events.LoadoutInfo();
                    bool Docked = false;
                    string LastRightJSON = "";
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            string line = sr.ReadLine();
                            var loadout = JsonConvert.DeserializeObject<EDCSLogreader.Events.LoadoutInfo>(line);
                            var loacation = JsonConvert.DeserializeObject<EDCSLogreader.Events.LocationInfo>(line);
                            var dockEvent = JsonConvert.DeserializeObject<EDCSLogreader.Events.DockedInfo>(line);
                            var undockEvent = JsonConvert.DeserializeObject<EDCSLogreader.Events.UndockedInfo>(line);
                            if (loadout.Event == "Loadout")
                            {
                                LastLoadoutInfo = loadout;
                                LastRightJSON = line;
                            }
                            if (dockEvent.Event == "Docked")
                            {
                                Docked = true;
                            }
                            if (undockEvent.Event == "Undocked")
                            {
                                Docked = false;
                            }
                        }
                        catch
                        {

                        }
                    }
                    if (!string.IsNullOrEmpty(api.Location.Station)) Docked = true;
                    if (Docked)
                    {
                        StarPort = api.Location.Station;
                        StarportLink.Text = api.Location.Station;
                        StarportLink.Enabled = true;
                        if (config.DiscordRpc)
                        {
                            client.SetPresence(new RichPresence()
                            {
                                Details = lang.lang["DISCORD_RPC_DOCKEDAT"] + StarPort,
                                State = lang.lang["DISCORD_RPC_INSYSTEM"] + api.Location.StarSystem,
                                Assets = new Assets()
                                {
                                    LargeImageKey = LastLoadoutInfo.Ship.ToLower(),
                                },
                                Timestamps = new Timestamps
                                {
                                    Start = StartTimeStamp,
                                }
                            });
                        }
                    }
                    else
                    {
                        StarPort = "x";
                        StarportLink.Text = "x";
                        StarportLink.Enabled = false;
                        if (config.DiscordRpc)
                        {
                            client.SetPresence(new RichPresence()
                            {
                                Details = lang.lang["DISCORD_RPC_PLAYINGED"],
                                State = lang.lang["DISCORD_RPC_INSYSTEM"] + api.Location.StarSystem,
                                Assets = new Assets()
                                {
                                    LargeImageKey = LastLoadoutInfo.Ship.ToLower(),
                                },
                                Timestamps = new Timestamps
                                {
                                    Start = StartTimeStamp,
                                }
                            });
                        }
                    }
                    if (LastLoadoutInfo.Event != null)
                    {
                        ShipJSON = LastLoadoutInfo;
                        ShipID = LastLoadoutInfo.ShipId;
                        ship = LastLoadoutInfo.Ship;
                        if (string.IsNullOrWhiteSpace(LastLoadoutInfo.ShipName))
                        {
                            ShipLink.Text = ShipNamePairs[LastLoadoutInfo.Ship];
                        }
                        else
                        {
                            ShipLink.Text = LastLoadoutInfo.ShipName;
                        }
                    }
                    else
                    {
                        ShipJSON = new EDCSLogreader.Events.LoadoutInfo();
                        ShipID = 0;
                        ship = "unknown";
                        ShipLink.Text = "Unknown";
                    }
                }
                if (config.DataToInara)
                {
                    Package WealthPackage = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("setCommanderCredits", GetTimeStamp(), new CreditsEvent(api.Commander.Credits, api.Commander.Assets)) });
                    inara.SendPakage(WealthPackage);
                }
                //event hold
                //loadout event
                api.Events.LoadoutEvent += (send, ev) =>
                {
                    if (config.DiscordRpc)
                    {
                        client.SetPresence(new RichPresence()
                        {
                            Details = client.CurrentPresence.Details,
                            State = client.CurrentPresence.State,
                            Assets = new Assets()
                            {
                                LargeImageKey = ev.Ship.ToLower(),
                            },
                            Timestamps = new Timestamps
                            {
                                Start = StartTimeStamp,
                            }
                        });
                    }
                    SysLink.Invoke(new Action(() =>
                    {
                        ShipID = ev.ShipId;
                        ShipJSON = ev;
                        ship = ev.Ship[0].ToString().ToUpper() + ev.Ship.Substring(1, ev.Ship.Length - 1);
                        if (string.IsNullOrWhiteSpace(ev.ShipName))
                        {
                            ShipLink.Text = ShipNamePairs[ev.Ship];
                        }
                        else
                        {
                            ShipLink.Text = ev.ShipName;
                        }
                    }));
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("setCommanderShipLoadout", ev.timestamp, new SetShipLoadout(ev.Ship, ev.ShipId, new InaraSlefModules(ev).modules.ToArray())) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.ShipyardNewEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderShip", ev.timestamp, new AddCmdrShip(ev.ShipType, ev.NewShipID)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.ShipyardSellEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("delCommanderShip", ev.timestamp, new DelCmdrShip(ev.ShipType, ev.SellShipID)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.ShipyardTransferEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("setCommanderShipTransfer", ev.timestamp, new ShipTransfer(ev.ShipType, ev.ShipId, api.Location.StarSystem, api.Location.Station)) });
                        inara.SendPakage(package);
                    }
                };
                //For plugins event
                api.Events.AllEvent += (send, ev) =>
                {
                    if (!Directory.Exists("Plugins"))
                    {
                        Directory.CreateDirectory("Plugins");
                    }
                    foreach(string file in Directory.GetFiles("Plugins"))
                    {
                        if (Path.GetExtension(file) == ".dll")
                        {
                            FullTrustAssembliesSection section = new FullTrustAssembliesSection();
                            Assembly assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), file));
                            bool b = assembly.IsFullyTrusted;
                            foreach (Type t in assembly.GetExportedTypes())
                            {
                                try
                                {
                                    var c = Activator.CreateInstance(t);
                                    if (t.Name == "Program")
                                    { 
                                        t.InvokeMember("Main", BindingFlags.InvokeMethod, null, c, new object[] { ev });
                                    }
                                }
                                catch(Exception e)
                                {
                                    int FileID = 1;
                                    for(int i = 0; ; i++)
                                    {
                                        if (!File.Exists("CrashReport_" + i + ".txt"))
                                        {
                                            FileID = i;
                                            break;
                                        }
                                    }
                                    StreamWriter sw = File.CreateText("CrashReport_"  + FileID + ".txt");
                                    sw.Write($"Crash report {GetTimeStamp()}:\n{e.Message},\nmore info:\n{e.StackTrace}");
                                    sw.Close();
                                }
                            }
                        }
                    }
                };

                //materials event
                api.Events.MaterialsEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        List<SetMaterials> materials = new List<SetMaterials>();
                        foreach (EDCSLogreader.Events.MaterialsInfo.Material r in ev.Raw)
                        {
                            materials.Add(new SetMaterials(r.Name, r.Count));
                        }
                        foreach (EDCSLogreader.Events.MaterialsInfo.Material r in ev.Manufactured)
                        {
                            materials.Add(new SetMaterials(r.Name, r.Count));
                        }
                        foreach (EDCSLogreader.Events.MaterialsInfo.Material r in ev.Encoded)
                        {
                            materials.Add(new SetMaterials(r.Name, r.Count));
                        }
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEventMultyply[] { new InaraEventMultyply("setCommanderInventoryMaterials", ev.timestamp, materials.ToArray()) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.MaterialCollectedEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderInventoryMaterialsItem", ev.timestamp, new SetMaterials(ev.Name, ev.Count)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.MaterialTradeEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("delCommanderInventoryMaterialsItem", ev.timestamp, new SetMaterials(ev.Paid.Material, ev.Paid.Quantity)), new InaraEvent("addCommanderInventoryMaterialsItem", ev.timestamp, new SetMaterials(ev.Received.Material, ev.Received.Quantity)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.EngineerCraftEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        List<SetMaterials> mats = new List<SetMaterials>();
                        foreach(var mat in ev.Ingredients)
                        {
                            mats.Add(new SetMaterials(mat.Name, mat.Count));
                        }
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEventMultyply[] { new InaraEventMultyply("delCommanderInventoryMaterialsItem", ev.timestamp, mats.ToArray()) });
                        inara.SendPakage(package);
                    }
                };

                //traveling events
                api.Events.FSDJumpEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderTravelFSDJump", ev.timestamp, new TravelFSDjump(ev.StarSystem, ev.StarPos.ToArray(), ev.JumpDist, ship, ShipID)) });
                        inara.SendPakage(package);
                    }
                    SysLink.Invoke(new Action(() =>
                    {
                        SysLink.Text = ev.StarSystem;
                        StarSystem = ev.StarSystem;
                    }));
                    if (config.DiscordRpc)
                    {
                        client.SetPresence(new RichPresence()
                        {
                            Details = lang.lang["DISCORD_RPC_PLAYINGED"],
                            State = lang.lang["DISCORD_RPC_INSYSTEM"] + ev.StarSystem,
                            Assets = new Assets()
                            {
                                LargeImageKey = client.CurrentPresence.Assets.LargeImageKey,
                            },
                            Timestamps = new Timestamps
                            {
                                Start = StartTimeStamp,
                            }
                        });
                    }
                };
                api.Events.UndockedEvent += (send, ev) =>
                {
                    SysLink.Invoke(new Action(() =>
                    {
                        StarportLink.Enabled = false;
                        StarportLink.Text = "x";
                        StarPort = "x";

                        if (config.DiscordRpc)
                        {
                            client.SetPresence(new RichPresence()
                            {
                                Details = lang.lang["DISCORD_RPC_PLAYINGED"],
                                State = lang.lang["DISCORD_RPC_INSYSTEM"] + api.Location.StarSystem,
                                Assets = new Assets()
                                {
                                    LargeImageKey = client.CurrentPresence.Assets.LargeImageKey,
                                },
                                Timestamps = new Timestamps
                                {
                                    Start = StartTimeStamp,
                                }
                            });
                        }
                    }));
                };
                api.Events.DockedEvent += (send, ev) =>
                {
                    SysLink.Invoke(new Action(() =>
                    {
                        LastUpdated.Text = lang.lang["EDICFORM_LASTUPDATELABLESTART"] + HMS();
                        StarportLink.Enabled = true;
                        StarportLink.Text = ev.StationName;
                        StarPort = ev.StationName;

                        if (config.DiscordRpc)
                        {
                            client.SetPresence(new RichPresence()
                            {
                                Details = lang.lang["DISCORD_RPC_DOCKEDAT"] + StarPort,
                                State = lang.lang["DISCORD_RPC_INSYSTEM"] + api.Location.StarSystem,
                                Assets = new Assets()
                                {
                                    LargeImageKey = client.CurrentPresence.Assets.LargeImageKey,
                                },
                                Timestamps = new Timestamps
                                {
                                    Start = StartTimeStamp,
                                }
                            });
                        }
                    }));
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderTravelDock", ev.timestamp, new DockedToStation(ev.StarSystem, ev.StationName, ev.MarketId, ship, ShipID)) });
                        inara.SendPakage(package);
                    }
                    if (config.Eddn)
                    {
                        if (auth == null || !auth.Refresh())
                        {
                            var req = OAuth2.Authorize();
                            auth = req.GetAuth();
                        }
                        capi = new cAPI(auth);
                        var a = capi.GetMarket();
                        if (a.ContainsKey("commodities"))
                        {
                            List<Eddn.EddnPackage.Message.Economies> economies = new List<Eddn.EddnPackage.Message.Economies>();
                            List<Eddn.EddnPackage.Message.Commodities> commodities = new List<Eddn.EddnPackage.Message.Commodities>();
                            foreach (EDCSLogreader.Events.StationEconomy economy in ev.StationEconomies)
                            {
                                economies.Add(new Eddn.EddnPackage.Message.Economies() { name = economy.Name, proportion = economy.Proportion });
                            }
                            foreach (JObject commodity in a["commodities"])
                            {
                                Eddn.EddnPackage.Message.Commodities commodities1 = new Eddn.EddnPackage.Message.Commodities();
                                commodities1 = new Eddn.EddnPackage.Message.Commodities()
                                {
                                    name = commodity["name"].ToString(),
                                    meanPrice = long.Parse(commodity["meanPrice"].ToString()),
                                    buyPrice = long.Parse(commodity["buyPrice"].ToString()),
                                    stock = long.Parse(commodity["stock"].ToString()),
                                    sellPrice = long.Parse(commodity["sellPrice"].ToString()),
                                    demand = long.Parse(commodity["demand"].ToString())
                                };
                                switch (int.Parse(commodity["stockBracket"].ToString()))
                                {
                                    case 0:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.zero0;
                                        break;
                                    case 1:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.one1;
                                        break;
                                    case 2:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.two2;
                                        break;
                                    case 3:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.three3;
                                        break;
                                }
                                switch (int.Parse(commodity["demandBracket"].ToString()))
                                {
                                    case 0:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.zero0;
                                        break;
                                    case 1:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.one1;
                                        break;
                                    case 2:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.two2;
                                        break;
                                    case 3:
                                        commodities1.stockBracket = Eddn.EddnPackage.Message.Commodities.StockBracket.three3;
                                        break;
                                }
                                commodities.Add(commodities1);
                            }

                            Eddn.EddnPackage package = new Eddn.EddnPackage()
                            {
                                schemaRefVar = @"https://eddn.edcd.io/schemas/commodity/3",
                                header = new Eddn.EddnPackage.Header()
                                {
                                    uploaderID = api.Commander.FrontierID,
                                    softwareName = "Elite:Dangerous Inara connector",
                                    softwareVersion = AppVer,
                                },
                                message = new Eddn.EddnPackage.Message()
                                {
                                    systemName = api.Location.StarSystem,
                                    stationName = ev.StationName,
                                    marketId = ev.MarketId,
                                    timestamp = ev.timestamp,
                                    commodities = commodities.ToArray(),
                                    economies = economies.ToArray()
                                }
                            };
                            eddn.SendPackage(package);
                        }
                    }
                };

                //ranks reputation progress
                api.Events.ProgressEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEventMultyply[]
                        {
                            new InaraEventMultyply("setCommanderRankPilot", ev.timestamp, new PilotRankEvent[]
                            {
                                new PilotRankEvent(PilotRankEvent.RankName.combat, (float)api.Commander.ProgressRanks.Combat / 100, api.Commander.Ranks.Combat),
                                new PilotRankEvent(PilotRankEvent.RankName.trade, (float)api.Commander.ProgressRanks.Trade / 100, api.Commander.Ranks.Trade),
                                new PilotRankEvent(PilotRankEvent.RankName.explore, (float)api.Commander.ProgressRanks.Explore / 100, api.Commander.Ranks.Explore),
                                new PilotRankEvent(PilotRankEvent.RankName.soldier, (float)api.Commander.ProgressRanks.Soldier / 100, api.Commander.Ranks.Soldier),
                                new PilotRankEvent(PilotRankEvent.RankName.exobiologist, (float)api.Commander.ProgressRanks.Exobiologist / 100, api.Commander.Ranks.Exobiologist),
                                new PilotRankEvent(PilotRankEvent.RankName.cqc, (float)api.Commander.ProgressRanks.CQC / 100, api.Commander.Ranks.CQC),
                                new PilotRankEvent(PilotRankEvent.RankName.federation, (float)api.Commander.ProgressRanks.Federation / 100, api.Commander.Ranks.Federation),
                                new PilotRankEvent(PilotRankEvent.RankName.empire, (float)api.Commander.ProgressRanks.Empire / 100, api.Commander.Ranks.Empire),
                            })
                        });
                        inara.SendPakage(package);
                    }
                };
                api.Events.ReputationEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEventMultyply[]
                        {
                            new InaraEventMultyply("setCommanderReputationMajorFaction", ev.timestamp, new MajorFractionRep[]
                            {
                                new MajorFractionRep("alliance", (float)ev.Alliance / 100),
                                new MajorFractionRep("empire", (float)ev.Empire / 100),
                                new MajorFractionRep("federation", (float)ev.Federation / 100),
                            })
                        });
                        inara.SendPakage(package);
                    }
                };
                api.Events.RankEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEventMultyply[]
                        {
                            new InaraEventMultyply("setCommanderRankPilot", ev.timestamp, new PilotRankEvent[]
                            {
                                new PilotRankEvent(PilotRankEvent.RankName.combat, (float)api.Commander.ProgressRanks.Combat / 100, api.Commander.Ranks.Combat),
                                new PilotRankEvent(PilotRankEvent.RankName.trade, (float)api.Commander.ProgressRanks.Trade / 100, api.Commander.Ranks.Trade),
                                new PilotRankEvent(PilotRankEvent.RankName.explore, (float)api.Commander.ProgressRanks.Explore / 100, api.Commander.Ranks.Explore),
                                new PilotRankEvent(PilotRankEvent.RankName.soldier, (float)api.Commander.ProgressRanks.Soldier / 100, api.Commander.Ranks.Soldier),
                                new PilotRankEvent(PilotRankEvent.RankName.exobiologist, (float)api.Commander.ProgressRanks.Exobiologist / 100, api.Commander.Ranks.Exobiologist),
                                new PilotRankEvent(PilotRankEvent.RankName.cqc, (float)api.Commander.ProgressRanks.CQC / 100, api.Commander.Ranks.CQC),
                                new PilotRankEvent(PilotRankEvent.RankName.federation, (float)api.Commander.ProgressRanks.Federation / 100, api.Commander.Ranks.Federation),
                                new PilotRankEvent(PilotRankEvent.RankName.empire, (float)api.Commander.ProgressRanks.Empire / 100, api.Commander.Ranks.Empire),
                            })
                        });
                        inara.SendPakage(package);
                    }
                };
                api.Events.EngineerProgressEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        List<EngineerRank> ranks = new List<EngineerRank>();
                        foreach (EDCSLogreader.Events.EngineerProgressInfo.EngineerState r in ev.Engineers)
                        {
                            ranks.Add(new EngineerRank(r.Engineer, r.Progress, r.RankProgress));
                        }
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEventMultyply[] { new InaraEventMultyply("setCommanderRankEngineer", ev.timestamp, ranks.ToArray()) });
                        inara.SendPakage(package);
                    }
                };

                //combat events
                api.Events.EscapeInterdictionEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatInterdictionEscape", ev.timestamp, new EscapedInterection(api.Location.StarSystem, ev.Interdictor, ev.IsPlayer)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.InterdictionEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatInterdiction", ev.timestamp, new GotInterected(api.Location.StarSystem, ev.Interdicted, ev.IsPlayer, ev.Success)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.InterdictedEvent += (send, ev) =>
                {
                    if(config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatInterdicted", ev.timestamp, new GotInterected(api.Location.StarSystem, ev.Interdictor, ev.IsPlayer, ev.Submitted)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.DiedEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatDeath", ev.timestamp, new CombatDeath(api.Location.StarSystem, ev.KillerName)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.PVPKillEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(false, config.InaraApiKey, api.Commander.Commander, api.Commander.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatKill", ev.timestamp, new CommanderPVPkill(api.Location.StarSystem, ev.Victim)) });
                        inara.SendPakage(package);
                    }
                };
            }
            else
            {
                MessageBox.Show("Journal finding error: invalid path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetTimeStamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        private void EDICmainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (api != null)
            {
                api.Stop();
            }
            if (inara != null)
            {
                inara.CloseLogger();
            }
            if (eddn != null)
            {
                eddn.CloseLogger();
            }
        }

        private string HMS()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://inara.cz/starsystem/?search=" + StarSystem.Replace(' ', '+'));
            }
            catch
            {
                MessageBox.Show("Error with opening system on inara", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StarportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://inara.cz/station/?search=" + StarSystem.Replace(' ', '+') + "%20[" + StarPort.Replace(' ', '+') + "]");
            }
            catch
            {
                MessageBox.Show("Error with opening station on inara", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShipLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (config.Edsy)
            {
                System.Diagnostics.Process.Start(Exporters.EdsyExport.Export(ShipJSON));
            }
            else
            {
                System.Diagnostics.Process.Start(Exporters.CoriolisExporter.Export(ShipJSON));
            }
        }

        public static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            List<FileInfo> files = directory.GetFiles().ToList();
            for(int i = 0; i < files.Count; i++)
            {
                FileInfo file = files[i];
                if (!file.Name.StartsWith("Journal."))
                {
                    files.Remove(file);
                    i--;
                }
            }
            return files.ToArray().Union(directory.GetDirectories().Select(d => GetNewestFile(d))).OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime)).FirstOrDefault();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm(config);
            form.ShowDialog();
            config = form.cfg;
            ConfigSaver.SaveCfg(config);
            LoadCfg();
            api.Stop();
            api = null;
            LoadApp();
            LoadTranslation();
        }
    }
}
