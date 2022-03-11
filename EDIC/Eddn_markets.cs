using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Schema;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EDIC
{
    public class Eddn
    {
        public Eddn()
        {

        }
        private string GetTimeStamp()
        {
            //string time = $"{DateTime.Today.Year}.{DateTime.Today.Month}.{DateTime.Today.Day} {DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}";
            string time = DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss");
            return time;
        }
        private StreamWriter logger;
        public void SendPackage(EddnPackage package)
        {
            if (logger == null)
            {
                logger = File.CreateText("EddnLog " + GetTimeStamp() + ".log");
            }
            WebRequest request = WebRequest.Create("https://eddn.edcd.io:4430/upload/");
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(package).Replace("schemaRefVar", "$schemaRef"));
            request.ContentLength = data.Length;
            StreamWriter sw = new StreamWriter(request.GetRequestStream());
            sw.Write(JsonConvert.SerializeObject(package).Replace("schemaRefVar", "$schemaRef").Replace("\"zero0\"", "0").Replace("\"one1\"", "1").Replace("\"two2\"", "2").Replace("\"three3\"", "3"));
            sw.Close();
            try
            {
                WebResponse response = request.GetResponse();
                string dat = new StreamReader(response.GetResponseStream(), true).ReadToEnd();
                logger.Write("Sent: " + JsonConvert.SerializeObject(package) + "\nRecived: " + dat + "\n");
                response.Close();
            }
            catch(WebException e)
            {
                WebResponse response = e.Response;
                string dat = new StreamReader(response.GetResponseStream(), true).ReadToEnd();
                logger.Write("Sent: " + JsonConvert.SerializeObject(package) + "\nRecived: " + dat + " : WARNING: THIS IS ERROR\n");
                response.Close();
            }
        }
        public void CloseLogger()
        {
            if(logger != null)
            logger.Close();
        }
        public class EddnPackage
        {
            public string schemaRefVar;
            public Header header;
            public Message message;
            public class Header
            {
                public string uploaderID;
                public string softwareName;
                public string softwareVersion;
            }
            public class Message
            {
                public string systemName;
                public string stationName;
                public long marketId;
                //public DateTime timestamp;
                public string timestamp;
                public Commodities[] commodities;
                public Economies[] economies;
                public class Commodities
                {
                    public string name;
                    public long meanPrice;
                    public long buyPrice;
                    public long stock;
                    public StockBracket stockBracket;
                    public long sellPrice;
                    public long demand;
                    public StockBracket demandBracket;
                    public enum StockBracket
                    {
                        zero0,
                        one1,
                        two2,
                        three3
                    }
                }
                public class Economies
                {
                    public string name;
                    public double proportion;
                }
            }
        }
    }
}