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
        private static string path = Path.GetFullPath(Path.Combine(fullPath, @"..\..\apkConfig.json"));

        public static string Loaders {

            get
            {
                return ApkConfigObj.Loaders;
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
                return ApkConfigObj.ServerAddress;
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

        public static int BitmapAlpha
        {
            get
            {
                return ApkConfigObj.BitmapAlpha;
            }

            set
            {
                JsonObjClass obj;

                using (StreamReader file = File.OpenText(path))
                {
                    obj = JsonConvert.DeserializeObject<JsonObjClass>(file.ReadToEnd());
                }

                if (value > 255)
                {
                    obj.BitmapAlpha = 255;
                }
                else if (value < 0)
                {
                    obj.BitmapAlpha = 0;
                }
                else
                {
                    obj.BitmapAlpha = value;
                }

                using (StreamWriter file = File.CreateText(path))
                {
                    string jsonData = JsonConvert.SerializeObject(obj);

                    file.Write(jsonData);
                }

            }
        }

        private static JsonObjClass ApkConfigObj
        {
            get
            {
                JsonObjClass obj;

                using (StreamReader file = File.OpenText(path))
                {
                    obj = JsonConvert.DeserializeObject<JsonObjClass>(file.ReadToEnd());
                }

                return obj;
            }

            set
            {
                JsonObjClass obj = value;

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
        public int BitmapAlpha { get; set; }
    }

}
