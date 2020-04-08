using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OofDetector.Helpers
{
    class Checks
    {
        public static bool IsNvidia()
        {
            ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");

            bool cardFound = false;
            foreach (ManagementObject obj in objvide.Get())
                if (obj["Name"].ToString().ToLower().Contains("nvidia")) cardFound = true;

            return cardFound;
        }
        public static void PrepHighlightDir(string highlightPath) //Throws FileNotFoundException
        {
            if (!Directory.Exists(highlightPath)) Directory.CreateDirectory(highlightPath);
            else if (Directory.GetFiles(highlightPath).Length > 0)
            {
                Directory.Delete(highlightPath);
                Directory.CreateDirectory(highlightPath);
            }
        }
        public static void PrepTarkovConfig(string confPath) //Throws FileNotFoundException
        {
            if (!File.Exists(confPath)) throw new FileNotFoundException();
            string confPlain;
            using (StreamReader confReader = new StreamReader(confPath))
            {
                confPlain = confReader.ReadToEnd();
                confReader.Close();
            }
            JObject confObj = JObject.Parse(confPlain);
            JProperty nvProp = confObj.Property("NVidiaHighlightsEnabled");
            bool isEnabled = nvProp.Value.Value<bool>();

            if (!isEnabled)
            {
                nvProp.Value = true;
                string updatedPlain = JsonConvert.SerializeObject(confObj, Formatting.Indented);
                confObj = null;
                File.Delete(confPath);
                using (StreamWriter confWriter = new StreamWriter(confPath))
                {
                    confWriter.WriteLine(updatedPlain);
                    confWriter.Flush();
                }
            }
        }
    }
}
