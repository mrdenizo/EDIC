using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EDIC
{
    public class Inara
    {
        private StreamWriter logger;
        public Inara()
        {
            logger = File.CreateText("InaraLog " + GetTimeStamp() + ".log");
        }
        private string GetTimeStamp()
        {
            string time = $"{DateTime.Today.Year}.{DateTime.Today.Month}.{DateTime.Today.Day} {DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
            return time;
        }
        public void CloseLogger()
        {
            logger.Close();
        }
        public void SendPakage(Package package)
        {
            WebRequest request = WebRequest.Create("https://inara.cz/inapi/v1/");
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(package));
            request.ContentLength = data.Length;
            StreamWriter sw = new StreamWriter(request.GetRequestStream());
            sw.Write(JsonConvert.SerializeObject(package));
            sw.Close();
            WebResponse response = request.GetResponse();
            string dat = new StreamReader(response.GetResponseStream(), true).ReadToEnd();
            logger.Write("Sent: " + JsonConvert.SerializeObject(package) + "\nRecived: " + dat + "\n");
            response.Close();
        }
    }
    public class Package
    {
        public Header header;
        public AInaraEvent[] events;
        public Package(Header header, AInaraEvent[] events)
        {
            this.header = header;
            this.events = events;
        }
    }
    public class Header
    {
        public string appName;
        public string appVersion;
        public bool isDeveloped;
        public string APIkey;
        public string commanderName;
        public string commanderFrontierID;
        public Header(bool isDeveloped, string APIkey, string commanderName, string commanderFrontierID)
        {
            this.appName = "Elite:Dangerous Inara connector";
            this.appVersion = "1.0.2";
            this.isDeveloped = isDeveloped;
            this.APIkey = APIkey;
            this.commanderName = commanderName;
            this.commanderFrontierID = commanderFrontierID;
        }
    }
    public abstract class AInaraEvent
    {
        public string eventName;
        public string eventTimestamp;
    }
    public class InaraEvent : AInaraEvent
    {
        public IEventData eventData;
        public InaraEvent(string eventName, string eventTimestamp, IEventData eventData) 
        {
            this.eventName = eventName;
            this.eventTimestamp = eventTimestamp;
            this.eventData = eventData;
        }
    }
    public class InaraEventMultyply : AInaraEvent
    {
        public IEventData[] eventData;
        public InaraEventMultyply(string eventName, string eventTimestamp, IEventData[] eventData)
        {
            this.eventName = eventName;
            this.eventTimestamp = eventTimestamp;
            this.eventData = eventData;
        }
    }
    public interface IEventData
    {

    }

    //ranks and reputation
    public class EngineerRank : IEventData
    {
        public string engineerName;
        public string rankStage;
        public long? rankValue;
        public EngineerRank(string engineerName, string rankStage, long? rankValue)
        {
            this.engineerName = engineerName;
            this.rankStage = rankStage;
            this.rankValue = rankValue;
        }
    }
    public class PilotRankEvent : IEventData
    {
        public string rankName;
        public long rankValue;
        public float rankProgress;
        public PilotRankEvent(RankName rankName, float rankProgress, long rankValue)
        {
            switch (rankName)
            {
                case RankName.combat:
                    this.rankName = "combat";
                    break;
                case RankName.trade:
                    this.rankName = "trade";
                    break;
                case RankName.explore:
                    this.rankName = "explore";
                    break;
                case RankName.cqc:
                    this.rankName = "cqc";
                    break;
                case RankName.federation:
                    this.rankName = "federation";
                    break;
                case RankName.empire:
                    this.rankName = "empire";
                    break;
            }
            this.rankValue = rankValue;
            this.rankProgress = rankProgress;
        }
        public enum RankName
        {
            combat,
            trade,
            explore,
            cqc,
            federation,
            empire
        }
    }
    class MajorFractionRep : IEventData
    {
        public string majorfactionName;
        public float majorfactionReputation;
        public MajorFractionRep(string majorfactionName, float majorfactionReputation)
        {
            this.majorfactionName = majorfactionName;
            this.majorfactionReputation = majorfactionReputation;
        }
    }

    //credits
    public class CreditsEvent : IEventData
    {
        public long commanderCredits;
        public long commanderAssets;
        public CreditsEvent(long commanderCredits, long commanderAssets)
        {
            this.commanderCredits = commanderCredits;
            this.commanderAssets = commanderAssets;
        }
    }

    //traveling
    public class TravelFSDjump : IEventData
    {
        public string starsystemName;
        public double[] starsystemCoords;
        public double jumpDistance;
        public string shipType;
        public long shipGameID;
        public TravelFSDjump(string starsystemName, double[] starsystemCoords, double jumpDistance, string shipType, long shipGameID)
        {
            this.starsystemName = starsystemName;
            this.starsystemCoords = starsystemCoords;
            this.jumpDistance = jumpDistance;
            this.shipType = shipType;
            this.shipGameID = shipGameID;
        }
    }
    public class DockedToStation : IEventData
    {
        public string starsystemName;
        public string stationName;
        public long marketID;
        public string shipType;
        public long shipGameID;
        public DockedToStation(string starsystemName, string stationName, long marketID, string shipType, long shipGameID)
        {
            this.starsystemName = starsystemName;
            this.stationName = stationName;
            this.marketID = marketID;
            this.shipType = shipType;
            this.shipGameID = shipGameID;
        }
    }
    public class ShipTransfer : IEventData
    {
        public string shipType;
        public long shipGameID;
        public string starsystemName;
        public string stationName;
        public long marketID;
        public long transferTime;
        public ShipTransfer(string shipType, long shipGameID, string starsystemName, string stationName, long marketID, long transferTime)
        {
            this.shipType = shipType;
            this.shipGameID = shipGameID;
            this.starsystemName = starsystemName;
            this.stationName = stationName;
            this.marketID = marketID;
            this.transferTime = transferTime;
        }
    }

    //combat
    public class CombatDeath : IEventData
    {
        public string starsystemName;
        public string opponentName;
        public CombatDeath(string starsystemName, string opponentName)
        {
            this.starsystemName = starsystemName;
            this.opponentName = opponentName;
        }
    }
    //not used now ;)
    public class CombatDeathWing : IEventData
    {
        public string starsystemName;
        public string[] wingOpponentNames;
        public CombatDeathWing(string starsystemName, string[] wingOpponentNames)
        {
            this.starsystemName = starsystemName;
            this.wingOpponentNames = wingOpponentNames;
        }
    }
    public class GotInterected : IEventData
    {
        public string starsystemName;
        public string opponentName;
        public bool isPlayer;
        public bool isSubmit;
        public GotInterected(string starsystemName, string opponentName, bool isPlayer, bool isSubmit)
        {
            this.starsystemName = starsystemName;
            this.opponentName = opponentName;
            this.isPlayer = isPlayer;
            this.isSubmit = isSubmit;
        }
    }
    public class InterectedSomeOne : IEventData
    {
        public string starsystemName;
        public string opponentName;
        public bool isPlayer;
        public bool isSuccess;
        public InterectedSomeOne(string starsystemName, string opponentName, bool isPlayer, bool isSuccess)
        {
            this.starsystemName = starsystemName;
            this.opponentName = opponentName;
            this.isPlayer = isPlayer;
            this.isSuccess = isSuccess;
        }
    }
    public class EscapedInterection : IEventData
    {
        public string starsystemName;
        public string opponentName;
        public bool isPlayer;
        public EscapedInterection(string starsystemName, string opponentName, bool isPlayer)
        {
            this.starsystemName = starsystemName;
            this.opponentName = opponentName;
            this.isPlayer = isPlayer;
        }
    }
    public class CommanderPVPkill : IEventData
    {
        public string starsystemName;
        public string opponentName;
        public CommanderPVPkill(string starsystemName, string opponentName)
        {
            this.starsystemName = starsystemName;
            this.opponentName = opponentName;
        }
    }

    //materials
    public  class SetMaterials : IEventData
    {
        public string itemName;
        public long itemCount;
        public SetMaterials(string itemName, long itemCount)
        {
            this.itemName = itemName;
            this.itemCount = itemCount;
        }
    }
}
