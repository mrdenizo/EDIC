using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Threading.Tasks;
using EDCSLogreader;

namespace EDIC
{
    public class Exporters
    {
        public static class EdsyExport
        {
            public static string Export(Events.LoadoutInfo json)
            {
                byte[] data = Gziper.Zip(JsonConvert.SerializeObject(json));
                string str = Convert.ToBase64String(data);
                return "http://edsy.org/#/I=" + str.Replace("+", "-").Replace("/", "_").Replace("=", "%3D");
            }
        }
        public static class CoriolisExporter
        {
            public static string Export(Events.LoadoutInfo json)
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

    public class InaraSlefModules
    {
        public InaraSlefModules(EDCSLogreader.Events.LoadoutInfo info)
        {
            modules = new List<InaraSlefModuleDefault>();
            foreach(EDCSLogreader.Events.LoadoutInfo.Module m in info.Modules)
            {
                if (m.Engineering != null)
                    modules.Add(new InaraSlefModuleEngineered() { slotName = m.Slot, itemName = m.Item, itemValue = m.Value,
                    itemHealth = m.Health, isOn = m.On, isHot = info.Hot, itemPriority = m.Priority, itemAmmoClip = m.AmmoInClip, itemAmmoHopper = m.AmmoInHopper, engineering = EngineeringFromLoadout(m)});
                else
                modules.Add(new InaraSlefModuleDefault() { slotName = m.Slot, itemName = m.Item, itemValue = m.Value,
                    itemHealth = m.Health, isOn = m.On, isHot = info.Hot, itemPriority = m.Priority, itemAmmoClip = m.AmmoInClip, itemAmmoHopper = m.AmmoInHopper});
            }
        }
        private InaraSlefModuleEngineered.Engineering EngineeringFromLoadout(EDCSLogreader.Events.LoadoutInfo.Module info) 
        {
            InaraSlefModuleEngineered.Engineering engineering = new InaraSlefModuleEngineered.Engineering() { blueprintName = info.Engineering.BlueprintName, 
                blueprintLevel = info.Engineering.Level, blueprintQuality = info.Engineering.Quality, experimentalEffect = info.Engineering.ExperimentalEffect, modifiers = ModifiersFromLoadout(info) };
            return engineering;
        }
        private InaraSlefModuleEngineered.Engineering.Modifier[] ModifiersFromLoadout(EDCSLogreader.Events.LoadoutInfo.Module info)
        {
            List<InaraSlefModuleEngineered.Engineering.Modifier> modifiers = new List<InaraSlefModuleEngineered.Engineering.Modifier>();
            foreach(EDCSLogreader.Events.LoadoutInfo.EngineeringModifier modifier in info.Engineering.Modifiers)
            {
                modifiers.Add(new InaraSlefModuleEngineered.Engineering.Modifier() { name = modifier.Label, value = modifier.Value, originalValue = modifier.OriginalValue, lessIsGood = modifier.LessIsGood != 0 });
            }
            return modifiers.ToArray();
        }

        public List<InaraSlefModuleDefault> modules;
        public class InaraSlefModuleDefault
        {
            public string slotName;
            public string itemName;
            public long itemValue;
            public double itemHealth;
            public bool isOn;
            public bool isHot;
            public int itemPriority;
            public int itemAmmoClip;
            public int itemAmmoHopper;
        }
        public class InaraSlefModuleEngineered : InaraSlefModuleDefault
        {
            public Engineering engineering;
            public class Engineering
            {
                public string blueprintName;
                public int blueprintLevel;
                public float blueprintQuality;
                public string experimentalEffect;
                public Modifier[] modifiers;
                public class Modifier
                {
                    public string name;
                    public float value;
                    public float originalValue;
                    public bool lessIsGood;
                }
            }
        }
    }
}
