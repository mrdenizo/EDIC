using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Threading.Tasks;

namespace EDIC
{
    public class Exporters
    {
        public static class EdsyExport
        {
            public static string Export(EliteAPI.Events.LoadoutInfo json)
            {
                byte[] data = Gziper.Zip(JsonConvert.SerializeObject(json));
                string str = Convert.ToBase64String(data);
                return "http://edsy.org/#/I=" + str.Replace("+", "-").Replace("/", "_").Replace("=", "%3D");
            }
        }
        public static class CoriolisExporter
        {
            public static string Export(EliteAPI.Events.LoadoutInfo json)
            {
                byte[] data = Gziper.Zip(JsonConvert.SerializeObject(json));
                string str = Convert.ToBase64String(data);
                return "https://coriolis.io/import?data=" + str.Replace("+", "-").Replace("/", "_").Replace("=", "%3D");
            }
        }
        private class Gziper
        {
            private static void CopyTo(Stream src, Stream dest)
            {
                byte[] bytes = new byte[4096];

                int cnt;

                while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
                {
                    dest.Write(bytes, 0, cnt);
                }
            }
            public static byte[] Zip(string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);

                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        CopyTo(msi, gs);
                    }

                    return mso.ToArray();
                }
            }
        }
    }

    public class ToSLEF 
    {
        public ShipHeader header;
        public LoadOutData data;
        public ToSLEF(string ShipType, EliteAPI.Events.LoadoutInfo info)
        {
            this.header = new ShipHeader("Elite:Dangerous Inara connector", "1.0.2", "https://github.com/mrdenizo/EDIC");
            List<Module> modules = new List<Module>();
            foreach(EliteAPI.Events.Module module in info.Modules)
            {
                if(module.Engineering != null)
                {
                    modules.Add(new EngineeredModule(module.Slot, module.Item, new EngineeringModule(module.Engineering.BlueprintName, module.Engineering.Level, module.Engineering.Quality, module.Engineering.ExperimentalEffect)));
                }
                else
                {
                    modules.Add(new ShipModule(module.Slot, module.Item));
                }
            }
            this.data = new LoadOutData(ShipType, modules);
        }
        public class ShipHeader
        {
            public string appName;
            public string appVersion;
            public string appURL;
            public ShipHeader(string appName, string appVersion, string appURL)
            {
                this.appName = appName;
                this.appVersion = appVersion;
                this.appURL = appURL;
            }
        }
        public class LoadOutData
        {
            public string Ship;
            public Module[] Modules;
            public LoadOutData(string Ship, List<Module> Modules)
            {
                this.Ship = Ship;
                this.Modules = Modules.ToArray();
            }
        }
        public abstract class Module
        {
            public string Slot;
            public string Item;
        }
        public class ShipModule : Module
        {
            public ShipModule(string Slot, string Item)
            {
                this.Slot = Slot;
                this.Item = Item;
            }
        }
        public class EngineeredModule : Module
        {
            public EngineeringModule Engineering;
            public EngineeredModule(string Slot, string Item, EngineeringModule Engineering)
            {
                this.Slot = Slot;
                this.Item = Item;
                this.Engineering = Engineering;
            }
        }
        public class EngineeringModule
        {
            public string BlueprintName;
            public long Level;
            public double Quality;
            public string ExperimentalEffect;
            public EngineeringModule(string BlueprintName, long Level, double Quality, string ExperimentalEffect)
            {
                this.BlueprintName = BlueprintName;
                this.Level = Level;
                this.Quality = Quality;
                this.ExperimentalEffect = ExperimentalEffect;
            }
        }
    }
}
