using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static EDCSLogreader.Events;
using System.Text;
using System.Threading.Tasks;

namespace EDCSLogreader
{
    public class EliteDangerousAPI
    {
        public CommanderE Commander { private set; get; } = new CommanderE();
        public Location Location { private set; get; } = new Location();
        public DirectoryInfo journalDirectory { private set; get; }
        public CommonEventsInvoke Events = new CommonEventsInvoke();
        private bool Enabled;
        private FileSystemWatcher watcher;
        private StreamReader sr;
        public EliteDangerousAPI(DirectoryInfo journalDirectory)
        {
            this.journalDirectory = journalDirectory;
            sr = new StreamReader(File.Open(GetNewestFile(journalDirectory).FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            watcher = new FileSystemWatcher(journalDirectory.FullName, GetNewestFile(journalDirectory).Name) { EnableRaisingEvents = false };
        }
        public void Start() 
        {
            Enabled = true;
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                CommonEvent ev = JsonConvert.DeserializeObject<CommonEvent>(s);
                if (ev.Event == "Commander")
                {
                    CommanderInfo info = JsonConvert.DeserializeObject<CommanderInfo>(s);
                    this.Commander.Commander = info.Name;
                    this.Commander.FrontierID = info.FID;
                }
                else if (ev.Event == "Location")
                {
                    LocationInfo info = JsonConvert.DeserializeObject<LocationInfo>(s);
                    Location.StarSystem = info.StarSystem;
                    if (info.Docked)
                        Location.Station = info.StationName;
                    else
                        Location.Station = "";
                }
                else if (ev.Event == "Materials")
                {
                    MaterialsInfo info = JsonConvert.DeserializeObject<MaterialsInfo>(s);
                    Events.RaiseMaterialsEvent(this, info);
                }
                else if (ev.Event == "FSDJump")
                {
                    FSDJumpInfo info = JsonConvert.DeserializeObject<FSDJumpInfo>(s);
                    Location.StarSystem = info.StarSystem;
                }
                else if (ev.Event == "Docked")
                {
                    DockedInfo info = JsonConvert.DeserializeObject<DockedInfo>(s);
                    Location.Station = info.StationName;
                }
                else if (ev.Event == "Undocked")
                {
                    UndockedInfo info = JsonConvert.DeserializeObject<UndockedInfo>(s);
                    Location.Station = null;
                }
                else if (ev.Event == "Rank")
                {
                    RankInfo info = JsonConvert.DeserializeObject<RankInfo>(s);
                    Commander.Ranks = info;
                }
                else if (ev.Event == "Progress")
                {
                    ProgressInfo info = JsonConvert.DeserializeObject<ProgressInfo>(s);
                    Commander.ProgressRanks = info;
                }
                else if (ev.Event == "Reputation")
                {
                    ReputationInfo info = JsonConvert.DeserializeObject<ReputationInfo>(s);
                }
                else if (ev.Event == "LoadGame")
                {
                    LoadGameInfo info = JsonConvert.DeserializeObject<LoadGameInfo>(s);
                    Commander.Credits = info.Credits;
                }
                else if (ev.Event == "Statistics")
                {
                    StatisticsInfo info = JsonConvert.DeserializeObject<StatisticsInfo>(s);
                    Commander.Assets = info.Bank_Account.Current_Wealth;
                }
            }
            sr.ReadToEnd();
            /*watcher.Changed += (s, e) =>
            {
                
            };*/
            Events.RaiseAllEvent(this, JsonConvert.SerializeObject(new CommanderInfo() { Event = "Commander", FID = Commander.FrontierID, Name = Commander.Commander }));
            Events.RaiseRankEvent(this, Commander.Ranks);
            new System.Threading.Thread(() => ThreadP()).Start();
            //watcher.EnableRaisingEvents = true;
        }

        private void ThreadP()
        {
            while (Enabled)
            {
                if (!sr.EndOfStream)
                {
                    string even = sr.ReadLine();
                    CommonEvent ev = JsonConvert.DeserializeObject<CommonEvent>(even);
                    if (ev.Event == "Docked")
                    {
                        DockedInfo info = JsonConvert.DeserializeObject<DockedInfo>(even);
                        Location.Station = info.StationName;
                        Events.RaiseDockEvent(this, info);
                    }
                    if (ev.Event == "Undocked")
                    {
                        UndockedInfo info = JsonConvert.DeserializeObject<UndockedInfo>(even);
                        Location.Station = null;
                        Events.RaiseUndockedEvent(this, info);
                    }
                    if (ev.Event == "Loadout")
                    {
                        LoadoutInfo info = JsonConvert.DeserializeObject<LoadoutInfo>(even);
                        Events.RaiseLoadoutEvent(this, info);
                    }
                    if (ev.Event == "FSDJump")
                    {
                        FSDJumpInfo info = JsonConvert.DeserializeObject<FSDJumpInfo>(even);
                        Location.StarSystem = info.StarSystem;
                        Events.RaiseFSDJumpEvent(this, info);
                    }
                    if (ev.Event == "ShipyardTransfer")
                    {
                        ShipyardTransferInfo info = JsonConvert.DeserializeObject<ShipyardTransferInfo>(even);
                        Events.RaiseShipyardTransferEvent(this, info);
                    }
                    if (ev.Event == "MaterialCollected")
                    {
                        MaterialCollectedInfo info = JsonConvert.DeserializeObject<MaterialCollectedInfo>(even);
                        Events.RaiseMaterialCollectedEvent(this, info);
                    }
                    if (ev.Event == "MaterialTrade")
                    {
                        MaterialTradeInfo info = JsonConvert.DeserializeObject<MaterialTradeInfo>(even);
                        Events.RaiseMaterialTradeEvent(this, info);
                    }
                    if (ev.Event == "EngineerCraft")
                    {
                        EngineerCraftInfo info = JsonConvert.DeserializeObject<EngineerCraftInfo>(even);
                        Events.RaiseEngineerCraftEvent(this, info);
                    }
                    if (ev.Event == "Rank")
                    {
                        RankInfo info = JsonConvert.DeserializeObject<RankInfo>(even);
                        Commander.Ranks = info;
                        Events.RaiseRankEvent(this, info);
                    }
                    if (ev.Event == "Progress")
                    {
                        ProgressInfo info = JsonConvert.DeserializeObject<ProgressInfo>(even);
                        Commander.ProgressRanks = info;
                        Events.RaiseProgressEvent(this, info);
                    }
                    if (ev.Event == "Reputation")
                    {
                        ReputationInfo info = JsonConvert.DeserializeObject<ReputationInfo>(even);
                        Commander.CommanderReputationMajor = info;
                        Events.RaiseReputationEvent(this, info);
                    }
                    if (ev.Event == "EngineerProgress")
                    {
                        EngineerProgressInfo info = JsonConvert.DeserializeObject<EngineerProgressInfo>(even);
                        Events.RaiseEngineerProgressEvent(this, info);
                    }
                    if (ev.Event == "EscapeInterdiction")
                    {
                        EscapeInterdictionInfo info = JsonConvert.DeserializeObject<EscapeInterdictionInfo>(even);
                        Events.RaiseEscapeInterdictionEvent(this, info);
                    }
                    if (ev.Event == "Interdicted")
                    {
                        InterdictedInfo info = JsonConvert.DeserializeObject<InterdictedInfo>(even);
                        Events.RaiseInterdictedEvent(this, info);
                    }
                    if (ev.Event == "Interdiction")
                    {
                        InterdictionInfo info = JsonConvert.DeserializeObject<InterdictionInfo>(even);
                        Events.RaiseInterdictionEvent(this, info);
                    }
                    if (ev.Event == "Died")
                    {
                        try
                        {
                            DiedInfo info = JsonConvert.DeserializeObject<DiedInfo>(even);
                            Events.RaiseDiedEvent(this, info);
                        }
                        catch
                        {
                            //that was wing kill
                        }
                    }
                    if (ev.Event == "PVPKill")
                    {
                        PVPKillInfo info = JsonConvert.DeserializeObject<PVPKillInfo>(even);
                        Events.RaisePVPKillEvent(this, info);
                    }
                    if (ev.Event == "ShipyardNew")
                    {
                        ShipyardNewInfo info = JsonConvert.DeserializeObject<ShipyardNewInfo>(even);
                        Events.RaiseShipyardNewEvent(this, info);
                    }
                    if (ev.Event == "ShipyardSell")
                    {
                        ShipyardSellInfo info = JsonConvert.DeserializeObject<ShipyardSellInfo>(even);
                        Events.RaiseShipyardSellEvent(this, info);
                    }
                    Events.RaiseAllEvent(this, even);
                }
                System.Threading.Thread.Sleep(250);
            }
        }

        public void Stop()
        {
            //watcher.EnableRaisingEvents = false;
            Enabled = false;
            sr.Close();
        }

        private static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            List<FileInfo> files = directory.GetFiles().ToList();
            for (int i = 0; i < files.Count; i++)
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
        public class CommonEventsInvoke : Events
        {
            public event LoadoutEventHalder LoadoutEvent;
            public delegate void LoadoutEventHalder(object sender, LoadoutInfo e);
            internal void RaiseLoadoutEvent(object sender, LoadoutInfo e) => LoadoutEvent?.Invoke(sender, e);

            public event DockEventHalder DockedEvent;
            public delegate void DockEventHalder(object sender, DockedInfo e);
            internal void RaiseDockEvent(object sender, DockedInfo e) => DockedEvent?.Invoke(sender, e);

            public event UndockedEventHalder UndockedEvent;
            public delegate void UndockedEventHalder(object sender, UndockedInfo e);
            internal void RaiseUndockedEvent(object sender, UndockedInfo e) => UndockedEvent?.Invoke(sender, e);

            public event FSDJumpEventHalder FSDJumpEvent;
            public delegate void FSDJumpEventHalder(object sender, FSDJumpInfo e);
            internal void RaiseFSDJumpEvent(object sender, FSDJumpInfo e) => FSDJumpEvent?.Invoke(sender, e);

            public event ShipyardTransferHalder ShipyardTransferEvent;
            public delegate void ShipyardTransferHalder(object sender, ShipyardTransferInfo e);
            internal void RaiseShipyardTransferEvent(object sender, ShipyardTransferInfo e) => ShipyardTransferEvent?.Invoke(sender, e);

            public event MaterialsEventHalder MaterialsEvent;
            public delegate void MaterialsEventHalder(object sender, MaterialsInfo e);
            internal void RaiseMaterialsEvent(object sender, MaterialsInfo e) => MaterialsEvent?.Invoke(sender, e);

            public event MaterialCollectedHalder MaterialCollectedEvent;
            public delegate void MaterialCollectedHalder(object sender, MaterialCollectedInfo e);
            internal void RaiseMaterialCollectedEvent(object sender, MaterialCollectedInfo e) => MaterialCollectedEvent?.Invoke(sender, e);

            public event MaterialTradeHalder MaterialTradeEvent;
            public delegate void MaterialTradeHalder(object sender, MaterialTradeInfo e);
            internal void RaiseMaterialTradeEvent(object sender, MaterialTradeInfo e) => MaterialTradeEvent?.Invoke(sender, e);

            public event EngineerCraftHalder EngineerCraftEvent;
            public delegate void EngineerCraftHalder(object sender, EngineerCraftInfo e);
            internal void RaiseEngineerCraftEvent(object sender, EngineerCraftInfo e) => EngineerCraftEvent?.Invoke(sender, e);

            public event RankHalder RankEvent;
            public delegate void RankHalder(object sender, RankInfo e);
            internal void RaiseRankEvent(object sender, RankInfo e) => RankEvent?.Invoke(sender, e);

            public event ProgressHalder ProgressEvent;
            public delegate void ProgressHalder(object sender, ProgressInfo e);
            internal void RaiseProgressEvent(object sender, ProgressInfo e) => ProgressEvent?.Invoke(sender, e);

            public event ReputationHalder ReputationEvent;
            public delegate void ReputationHalder(object sender, ReputationInfo e);
            internal void RaiseReputationEvent(object sender, ReputationInfo e) => ReputationEvent?.Invoke(sender, e);

            public event EngineerProgressHalder EngineerProgressEvent;
            public delegate void EngineerProgressHalder(object sender, EngineerProgressInfo e);
            internal void RaiseEngineerProgressEvent(object sender, EngineerProgressInfo e) => EngineerProgressEvent?.Invoke(sender, e);

            public event EscapeInterdictionHalder EscapeInterdictionEvent;
            public delegate void EscapeInterdictionHalder(object sender, EscapeInterdictionInfo e);
            internal void RaiseEscapeInterdictionEvent(object sender, EscapeInterdictionInfo e) => EscapeInterdictionEvent?.Invoke(sender, e);

            public event InterdictedHalder InterdictedEvent;
            public delegate void InterdictedHalder(object sender, InterdictedInfo e);
            internal void RaiseInterdictedEvent(object sender, InterdictedInfo e) => InterdictedEvent?.Invoke(sender, e);

            public event InterdictionHalder InterdictionEvent;
            public delegate void InterdictionHalder(object sender, InterdictionInfo e);
            internal void RaiseInterdictionEvent(object sender, InterdictionInfo e) => InterdictionEvent?.Invoke(sender, e);

            public event DiedHalder DiedEvent;
            public delegate void DiedHalder(object sender, DiedInfo e);
            internal void RaiseDiedEvent(object sender, DiedInfo e) => DiedEvent?.Invoke(sender, e);

            public event PVPKillHalder PVPKillEvent;
            public delegate void PVPKillHalder(object sender, PVPKillInfo e);
            internal void RaisePVPKillEvent(object sender, PVPKillInfo e) => PVPKillEvent?.Invoke(sender, e);

            public event ShipyardSellHalder ShipyardSellEvent;
            public delegate void ShipyardSellHalder(object sender, ShipyardSellInfo e);
            internal void RaiseShipyardSellEvent(object sender, ShipyardSellInfo e) => ShipyardSellEvent?.Invoke(sender, e);

            public event ShipyardNewHalder ShipyardNewEvent;
            public delegate void ShipyardNewHalder(object sender, ShipyardNewInfo e);
            internal void RaiseShipyardNewEvent(object sender, ShipyardNewInfo e) => ShipyardNewEvent?.Invoke(sender, e);



            public event AllEventHalder AllEvent;
            public delegate void AllEventHalder(object sender, string e);
            internal void RaiseAllEvent(object sender, string e) => AllEvent?.Invoke(sender, e);
        }
    }
    public class CommanderE
    {
        public string Commander;
        public string FrontierID;
        public RankInfo Ranks;
        public ProgressInfo ProgressRanks;
        public ReputationInfo CommanderReputationMajor;
        public long Credits;
        public long Assets;
    }
    public class Location
    {
        public string StarSystem;
        public string Station;
    }
    public class Events
    {
        public class CommonEvent
        {
            [JsonProperty("event")]
            public string Event;
            public string timestamp;
        }
        public class CommanderInfo : CommonEvent
        {
            public string FID;
            public string Name;
        }
        public class LocationInfo : CommonEvent
        {
            public string StarSystem;
            public bool Docked;
            public string StationName;
        }
        public class MaterialsInfo : CommonEvent
        {
            public Material[] Raw;
            public Material[] Manufactured;
            public Material[] Encoded;
            public class Material
            {
                public string Name;
                public string Name_Localised;
                public int Count;
            }
        }
        public class MaterialCollectedInfo : CommonEvent
        {
            public string Category;
            public string Name;
            public int Count;
        }
        public class MaterialTradeInfo : CommonEvent
        {
            [JsonProperty("MarketID")]
            public long MarketId;
            public MaterialAction Paid;
            public MaterialAction Received;
            public class MaterialAction
            {
                public string Material;
                public string Material_Localised;
                public string Category;
                public int Quantity;
            }
        }
        public class DockedInfo : CommonEvent
        {
            public string StationName;
            //public bool Taxi;
            public bool Multicrew;
            public string StarSystem;
            [JsonProperty("MarketID")]
            public long MarketId;
            public StationEconomy[] StationEconomies;
        }
        public class ShipyardTransferInfo : CommonEvent
        {
            public string ShipType;
            [JsonProperty("ShipID")]
            public long ShipId;
            public string System;
            public float Distance;
            public long TransferPrice;
        }
        public class FSDJumpInfo : CommonEvent
        {
            //public bool Taxi;
            public bool Multicrew;
            public string StarSystem;
            public long SystemAddress;
            public double[] StarPos;
            public float JumpDist;
        }
        public class ProgressInfo : CommonEvent
        {
            public int Combat;
            public int Trade;
            public int Explore;
            public int Soldier;
            public int Exobiologist;
            public int Empire;
            public int Federation;
            public int CQC;
        }
        public class RankInfo : CommonEvent
        {
            public int Combat;
            public int Trade;
            public int Explore;
            public int Soldier;
            public int Exobiologist;
            public int Empire;
            public int Federation;
            public int CQC;
        }
        public class ReputationInfo : CommonEvent
        {
            public float Empire;
            public float Federation;
            public float Alliance;
        }
        public class EngineerProgressInfo : CommonEvent
        {
            public EngineerState[] Engineers;
            public class EngineerState
            {
                public string Engineer;
                [JsonProperty("EngineerID")]
                public long EngineerId;
                public string Progress;
                public int RankProgress;
                public int Rank;
            }
        }
        public class StationEconomy
        {
            public string Name;
            public string Name_Localised;
            public float Proportion;
        }
        public class UndockedInfo : CommonEvent
        {
            public string StationName;
            //public bool Taxi;
            public bool Multicrew;
            public long MarketID;
        }
        public class EngineerCraftInfo : CommonEvent
        {
            public string Slot;
            public string Module;
            public string ApplyExperimentalEffect;
            public MaterialsInfo.Material[] Ingredients;
            public string Engineer;
            [JsonProperty("EngineerID")]
            public long EngineerId;
            [JsonProperty("BlueprintID")]
            public long BlueprintId;
            public string BlueprintName;
            public int Level;
            public float Quality;
            public string ExperimentalEffect;
        }
        public class LoadGameInfo : CommonEvent
        {
            public long Credits;
        }
        public class StatisticsInfo : CommonEvent
        {
            public BankAccount Bank_Account;
            public class BankAccount
            {
                public long Current_Wealth;
            }
        }
        public class EscapeInterdictionInfo : CommonEvent
        {
            public string Interdictor;
            public bool IsPlayer;
        }
        public class InterdictedInfo : CommonEvent
        {
            public bool Submitted;
            public string Interdictor;
            public bool IsPlayer;
        }
        public class InterdictionInfo : CommonEvent
        {
            public bool Success;
            public string Interdicted;
            public bool IsPlayer;
        }
        public class DiedInfo : CommonEvent
        {
            public string KillerName;
        }
        public class PVPKillInfo : CommonEvent
        {
            public string Victim;
        }
        public class ShipyardNewInfo : CommonEvent
        {
            public string ShipType;
            public long NewShipID;
        }
        public class ShipyardSellInfo : CommonEvent
        {
            public string ShipType;
            public long SellShipID;
            public long ShipPrice;
            public long MarketID;
        }
        public class LoadoutInfo : CommonEvent
        {
            public string Ship;
            [JsonProperty("ShipID")]
            public int ShipId;
            public string ShipName;
            public string ShipIdent;
            public int HullValue;
            public int ModulesValue;
            public float HullHealth;
            public bool Hot;
            public int Rebuy;
            public List<Module> Modules;

            public class Module
            {
                public string Slot;
                public string Item;
                public bool On;
                public int Priority;
                public int AmmoInClip;
                public int AmmoInHopper;
                public float Health;
                public long Value;
                public Engineering Engineering;
            }
            public class Engineering
            {
                public string Engineer;
                public long EngineerId;
                public long BlueprintId;
                public string BlueprintName;
                public int Level;
                public float Quality;
                public string ExperimentalEffect;
                public string ExperimentalEffectLocalised;
                public List<EngineeringModifier> Modifiers;
            }
            public class EngineeringModifier
            {
                public string Label;
                public float Value;
                public float OriginalValue;
                public int LessIsGood;
            }
        }
    }
}
