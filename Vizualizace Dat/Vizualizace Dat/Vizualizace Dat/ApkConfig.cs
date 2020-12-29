using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vizualizace_Dat
{
    public class ApkConfig
    {
        private static string fullPath = AppDomain.CurrentDomain.BaseDirectory;
        private static string path = Path.GetFullPath(Path.Combine(fullPath, @"..\..\loadersConfig.json"));

        public static string Loaders {
            get
            {
                JsonObjClass obj;

                using (StreamReader file = File.OpenText(path))
                {
                    obj = JsonConvert.DeserializeObject<JsonObjClass>(file.ReadToEnd());
                }

                return obj.Loaders;  
            }

            set
            {
                JsonObjClass obj;

                
                using (StreamReader file = File.OpenText(path))
                {
                    obj = JsonConvert.DeserializeObject<JsonObjClass>(file.ReadToEnd());
                }
                
                obj.Loaders = value;

                using (StreamWriter file = File.CreateText(path))
                {
                    string jsonData = JsonConvert.SerializeObject(obj);

                    file.Write(jsonData);
                }

            }
        }

        public static string ServerAddress
        {
            get
            {
                JsonObjClass obj;

                using (StreamReader file = File.OpenText(path))
                {
                    obj = JsonConvert.DeserializeObject<JsonObjClass>(file.ReadToEnd());
                }

                return obj.ServerAddress;
            }

            set
            {
                JsonObjClass obj;

                using (StreamReader file = File.OpenText(path))
                {
                    obj = JsonConvert.DeserializeObject<JsonObjClass>(file.ReadToEnd());
                }

                obj.ServerAddress = value;

                using (StreamWriter file = File.CreateText(path))
                {
                    string jsonData = JsonConvert.SerializeObject(obj);

                    file.Write(jsonData);
                }

            }
        }

    }

    public class JsonObjClass
    {

        public string Loaders { get; set; }
        public string ServerAddress { get; set; }

    }
}
