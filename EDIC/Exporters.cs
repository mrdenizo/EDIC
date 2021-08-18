using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace EDIC
{
    public class Exporters
    {
        public static class EdsyExport
        {
            //EDSY convert not works now
            public static string Export(string json)
            {
                byte[] data = Gziper.Zip(json);
                string str = Convert.ToBase64String(data);
                return "http://edsy.org/#/I=" + str.Replace("=", "%3D");
                //TODO: make it works
            }
        }
        public static class CoriolisExporter
        {
            public static string Export(string json)
            {
                byte[] data = Gziper.Zip(json);
                string str = Convert.ToBase64String(data);
                return "https://coriolis.io/import?data=" + str.Replace("=", "%3D");
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
}
