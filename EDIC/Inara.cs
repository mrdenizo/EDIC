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
        public Config config;
        public Inara(Config config)
        {
            this.config = config;
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
            response.Close();
        }
    }
    public class Package
    {
        public Header header;
        public InaraEvent[] events;
        public Package(Header header, InaraEvent[] events)
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
            this.appVersion = "0.0.1";
            this.isDeveloped = isDeveloped;
            this.APIkey = APIkey;
            this.commanderName = commanderName;
            this.commanderFrontierID = commanderFrontierID;
        }
    }
    public class InaraEvent
    {
        public string eventName;
        public string eventTimestamp;
        public IEventData eventData;
        public InaraEvent(string eventName, string eventTimestamp, IEventData eventData) 
        {
            this.eventName = eventName;
            this.eventTimestamp = eventTimestamp;
            this.eventData = eventData;
        }
    }
    public interface IEventData
    {

    }

    public class CreditsEvent : IEventData
    {
        public int commanderCredits;
        public int commanderAssets;
        public CreditsEvent(int commanderCredits, int commanderAssets)
        {
            this.commanderCredits = commanderCredits;
            this.commanderAssets = commanderAssets;
        }
    }
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
}
