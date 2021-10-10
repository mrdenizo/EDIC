using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDIC
{
    class cAPI
    {
        private const string ProfileURL = "https://companion.orerve.net/profile";
        private const string MarketURL = "https://companion.orerve.net/market";
        private const string ShipyardURL = "https://companion.orerve.net/shipyard";
        private const string JournalURL = "https://companion.orerve.net/journal";


        public OAuth2 OAuth { get; private set; }

        public cAPI(OAuth2 auth)
        {
            OAuth = auth;
        }

        private JObject Get(string url)
        {
            for (; ; )
            {
                var req = OAuth.CreateRequest(url);
                req.Method = "GET";
                try
                {
                    using (var response = req.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            using (var textreader = new StreamReader(stream))
                            {
                                using (var jsonreader = new JsonTextReader(textreader))
                                {
                                    return JObject.Load(jsonreader);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    cAPI capi;
                    OAuth2 auth = OAuth2.Load();
                    var reqest = OAuth2.Authorize();
                    auth = reqest.GetAuth();
                    auth.Save();
                    this.OAuth = auth;
                    
                }
            }
        }

        public JObject GetProfile()
        {
            return Get(ProfileURL);
        }

        public JObject GetMarket()
        {
            return Get(MarketURL);
        }

        public JObject GetShipyard()
        {
            return Get(ShipyardURL);
        }
    }
}
