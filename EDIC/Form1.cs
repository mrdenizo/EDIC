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
using EliteAPI;
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

        private LangPack lang = new LangPack();
        private long ShipID = 0;
        private EliteAPI.Events.LoadoutInfo ShipJSON;
        private string ship = "";
        private string StarSystem;
        private string StarPort;
        public Config config;
        private EliteDangerousAPI api;
        private DiscordRpcClient client;
        private cAPI capi;

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
                config.FrontierID = "put your frontier ID here";
                config.InaraApiKey = "put your Inara API here";
                config.DataToInara = false;
                config.DiscordRpc = false;
                config.ChoosenLanguage = "Language Packs\\English.json";
                config.Edsy = false;
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
            api.Start(false);
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
                        Assets = new Assets()
                        {
                            LargeImageKey = "sidewinder",
                        }
                    }) ;
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
                using (StreamReader sr = new StreamReader(File.Open(GetNewestFile(api.JournalDirectory).FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    EliteAPI.Events.LoadoutInfo LastLoadoutInfo = new EliteAPI.Events.LoadoutInfo();
                    bool Docked = false;
                    string LastRightJSON = "";
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        var loadout = JsonConvert.DeserializeObject<EliteAPI.Events.LoadoutInfo>(line);
                        var loacation = JsonConvert.DeserializeObject<EliteAPI.Events.LocationInfo>(line);
                        var dockEvent = JsonConvert.DeserializeObject<EliteAPI.Events.DockedInfo>(line);
                        var undockEvent = JsonConvert.DeserializeObject<EliteAPI.Events.UndockedInfo>(line);
                        if (loadout.Ship != null && loadout.Modules != null)
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
                                    LargeImageKey = "sidewinder",
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
                                    LargeImageKey = "sidewinder",
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
                            ShipLink.Text = LastLoadoutInfo.Ship[0].ToString().ToUpper() + LastLoadoutInfo.Ship.Substring(1, LastLoadoutInfo.Ship.Length - 1);
                        }
                        else
                        {
                            ShipLink.Text = LastLoadoutInfo.ShipName;
                        }
                    }
                    else
                    {
                        ShipJSON = new EliteAPI.Events.LoadoutInfo();
                        ShipID = 0;
                        ship = "unknown";
                        ShipLink.Text = "Unknown";
                    }
                }
                //event hold
                //loadout event
                api.Events.LoadoutEvent += (send, ev) =>
                {
                    SysLink.Invoke(new Action(() =>
                    {
                        ShipID = ev.ShipId;
                        ShipJSON = ev;
                        ship = ev.Ship[0].ToString().ToUpper() + ev.Ship.Substring(1, ev.Ship.Length - 1);
                        if (string.IsNullOrWhiteSpace(ev.ShipName))
                        {
                            ShipLink.Text = ev.Ship[0].ToString().ToUpper() + ev.Ship.Substring(1, ev.Ship.Length - 1);
                        }
                        else
                        {
                            ShipLink.Text = ev.ShipName;
                        }
                    }));
                };
                api.Events.ShipyardTransferEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("setCommanderShipTransfer", GetTimeStamp(), new ShipTransfer(ev.ShipType, ev.ShipId, api.Location.StarSystem, api.Location.Station, ev.MarketId, ev.TransferTime)) });
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
                                    t.InvokeMember("Main", BindingFlags.InvokeMethod, null, c, new object[] { ev });
                                }
                                catch(Exception e)
                                {
                                    int FileID = 1;
                                    for(int i = 0; ; i++)
                                    {
                                        if (!File.Exists("CrashReport_" + i))
                                        {
                                            FileID = i;
                                            break;
                                        }
                                    }
                                    StreamWriter sw = File.CreateText("CrashReport_"  + FileID);
                                    sw.Write($"Crash report {GetTimeStamp()}:\n{e.Message}, more info:\n{e.StackTrace}");
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
                        foreach (EliteAPI.Events.Raw r in ev.Raw)
                        {
                            materials.Add(new SetMaterials(r.Name, r.Count));
                        }
                        foreach (EliteAPI.Events.Encoded r in ev.Manufactured)
                        {
                            materials.Add(new SetMaterials(r.Name, r.Count));
                        }
                        foreach (EliteAPI.Events.Encoded r in ev.Encoded)
                        {
                            materials.Add(new SetMaterials(r.Name, r.Count));
                        }
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEventMultyply[] { new InaraEventMultyply("setCommanderInventoryMaterials", GetTimeStamp(), materials.ToArray()) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.MaterialCollectedEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderInventoryMaterialsItem", GetTimeStamp(), new SetMaterials(ev.Name, ev.Count)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.MaterialTradeEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("delCommanderInventoryMaterialsItem", GetTimeStamp(), new SetMaterials(ev.Paid.Material, ev.Paid.Quantity)), new InaraEvent("addCommanderInventoryMaterialsItem", GetTimeStamp(), new SetMaterials(ev.Received.Material, ev.Received.Quantity)) });
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
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEventMultyply[] { new InaraEventMultyply("delCommanderInventoryMaterialsItem", GetTimeStamp(), mats.ToArray()) });
                        inara.SendPakage(package);
                    }
                };

                //traveling events
                api.Events.FSDJumpEvent += (send, ev) =>
                {
                    var keys = api.Commander.Statistics.BankAccount.Keys;
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderTravelFSDJump", GetTimeStamp(), new TravelFSDjump(ev.StarSystem, ev.StarPos.ToArray(), ev.JumpDist, ship, ShipID)) });
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
                                LargeImageKey = "sidewinder",
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
                                    LargeImageKey = "sidewinder",
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
                                    LargeImageKey = "sidewinder",
                                }
                            });
                        }
                    }));
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderTravelDock", GetTimeStamp(), new DockedToStation(ev.StarSystem, ev.StationName, ev.MarketId, ship, ShipID)) });
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
                        List<Eddn.EddnPackage.Message.Economies> economies = new List<Eddn.EddnPackage.Message.Economies>();
                        List<Eddn.EddnPackage.Message.Commodities> commodities = new List<Eddn.EddnPackage.Message.Commodities>();
                        foreach(EliteAPI.Events.StationEconomy economy in ev.StationEconomies)
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
                            schemaRefVar = @"https://eddn.edcd.io/schemas/commodity/3/test",
                            header = new Eddn.EddnPackage.Header()
                            {
                                uploaderID = config.FrontierID,
                                softwareName = "Elite:Dangerous Inara connector",
                                softwareVersion = "1.2.0",
                            },
                            message = new Eddn.EddnPackage.Message()
                            {
                                systemName = api.Location.StarSystem,
                                stationName = ev.StationName,
                                marketId = ev.MarketId,
                                timestamp = DateTime.UtcNow,
                                commodities = commodities.ToArray(),
                                economies = economies.ToArray()
                            }
                        };
                        eddn.SendPackage(package);
                    }
                };

                //game start\stop
                api.Events.ShutdownEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("setCommanderCredits", GetTimeStamp(), new CreditsEvent(api.Commander.Credits, api.Commander.Statistics.BankAccount["Current_Wealth"])) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.LoadGameEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("setCommanderCredits", GetTimeStamp(), new CreditsEvent(api.Commander.Credits, api.Commander.Statistics.BankAccount["Current_Wealth"])) });
                        inara.SendPakage(package);
                    }
                };

                //ranks reputation progress
                api.Events.ProgressEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        float rank = (float)api.Commander.FederationRankProgress / 100;
                        long value = api.Commander.FederationRank;
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEventMultyply[]
                        {
                            new InaraEventMultyply("setCommanderRankPilot", GetTimeStamp(), new PilotRankEvent[]
                            {
                                new PilotRankEvent(PilotRankEvent.RankName.combat, (float)api.Commander.CombatRankProgress / 100, api.Commander.CombatRank),
                                new PilotRankEvent(PilotRankEvent.RankName.trade, (float)api.Commander.CombatRankProgress / 100, api.Commander.TradeRank),
                                new PilotRankEvent(PilotRankEvent.RankName.explore, (float)api.Commander.ExplorationRankProgress / 100, api.Commander.ExplorationRank),
                                new PilotRankEvent(PilotRankEvent.RankName.cqc, (float)api.Commander.CqcRankProgress / 100, api.Commander.CqcRank),
                                new PilotRankEvent(PilotRankEvent.RankName.federation, (float)api.Commander.FederationRankProgress / 100, api.Commander.FederationRank),
                                new PilotRankEvent(PilotRankEvent.RankName.empire, (float)api.Commander.EmpireRankProgress / 100, api.Commander.EmpireRank),
                            })
                        });
                        inara.SendPakage(package);
                    }
                };
                api.Events.ReputationEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEventMultyply[]
                        {
                            new InaraEventMultyply("setCommanderReputationMajorFaction", GetTimeStamp(), new MajorFractionRep[]
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
                        float rank = (float)api.Commander.FederationRankProgress / 100;
                        long value = api.Commander.FederationRank;
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEventMultyply[]
                        {
                            new InaraEventMultyply("setCommanderRankPilot", GetTimeStamp(), new PilotRankEvent[]
                            {
                                new PilotRankEvent(PilotRankEvent.RankName.combat, (float)api.Commander.CombatRankProgress / 100, api.Commander.CombatRank),
                                new PilotRankEvent(PilotRankEvent.RankName.trade, (float)api.Commander.CombatRankProgress / 100, api.Commander.TradeRank),
                                new PilotRankEvent(PilotRankEvent.RankName.explore, (float)api.Commander.ExplorationRankProgress / 100, api.Commander.ExplorationRank),
                                new PilotRankEvent(PilotRankEvent.RankName.cqc, (float)api.Commander.CqcRankProgress / 100, api.Commander.CqcRank),
                                new PilotRankEvent(PilotRankEvent.RankName.federation, (float)api.Commander.FederationRankProgress / 100, api.Commander.FederationRank),
                                new PilotRankEvent(PilotRankEvent.RankName.empire, (float)api.Commander.EmpireRankProgress / 100, api.Commander.EmpireRank),
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
                        foreach (EliteAPI.Events.Engineer r in ev.Engineers)
                        {
                            ranks.Add(new EngineerRank(r.EngineerEngineer, r.Progress, r.RankProgress));
                        }
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEventMultyply[] { new InaraEventMultyply("setCommanderRankEngineer", GetTimeStamp(), ranks.ToArray()) });
                        inara.SendPakage(package);
                    }
                };

                //combat events
                api.Events.EscapeInterdictionEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatInterdictionEscape", GetTimeStamp(), new EscapedInterection(api.Location.StarSystem, ev.Interdictor, ev.IsPlayer)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.InterdictionEvent += (send, ev) => 
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatInterdiction", GetTimeStamp(), new GotInterected(api.Location.StarSystem, ev.Interdicted, ev.IsPlayer, ev.Success)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.InterdictedEvent += (send, ev) =>
                {
                    if(config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatInterdicted", GetTimeStamp(), new GotInterected(api.Location.StarSystem, ev.Interdictor, ev.IsPlayer, ev.Submitted)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.DiedEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatDeath", GetTimeStamp(), new CombatDeath(api.Location.StarSystem, ev.KillerName)) });
                        inara.SendPakage(package);
                    }
                };
                api.Events.PVPKillEvent += (send, ev) =>
                {
                    if (config.DataToInara)
                    {
                        Package package = new Package(new Header(true, config.InaraApiKey, api.Commander.Commander, config.FrontierID), new InaraEvent[] { new InaraEvent("addCommanderCombatKill", GetTimeStamp(), new CommanderPVPkill(api.Location.StarSystem, ev.Victim)) });
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
            return DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
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
