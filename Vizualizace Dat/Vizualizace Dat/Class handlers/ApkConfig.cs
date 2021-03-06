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
        private static JsonObjClass apkConfigObj;

        static ApkConfig()
        {

            using (StreamReader file = File.OpenText(path))
            {
                apkConfigObj = JsonConvert.DeserializeObject<JsonObjClass>(file.ReadToEnd());
            }

        }

        public static string Loaders {

            get
            {
                return apkConfigObj.Loaders;
            }

            set
            {
                apkConfigObj.Loaders = value;

                ApkConfigObj = apkConfigObj;

            }
        }

        public static string ServerAddress
        {
            get
            {
                return apkConfigObj.ServerAddress;
            }

            set
            {
                apkConfigObj.ServerAddress = value;

                ApkConfigObj = apkConfigObj;

            }
        }

        public static int BitmapAlpha
        {
            get
            {
                return apkConfigObj.BitmapAlpha;
            }

            set
            {

                if (value > 255)
                {
                    apkConfigObj.BitmapAlpha = 255;
                }
                else if (value < 0)
                {
                    apkConfigObj.BitmapAlpha = 0;
                }
                else
                {
                    apkConfigObj.BitmapAlpha = value;
                }

                ApkConfigObj = apkConfigObj;

            }
        }

        public static int AnimMaxMove
        {
            get
            {
                return apkConfigObj.AnimMaxMove;
            }

            set
            {
                if (value > 144)
                {
                    apkConfigObj.AnimMaxMove = 144;
                }
                else if (value < 0)
                {
                    apkConfigObj.AnimMaxMove = 0;
                }
                else
                {
                    apkConfigObj.AnimMaxMove = value;
                }

                ApkConfigObj = apkConfigObj;
            }
        }

        public static int DblclickMaxData
        {
            get
            {
                return apkConfigObj.DblclickMaxData;
            }

            set
            {

                if (value > 40)
                {
                    apkConfigObj.DblclickMaxData = 40;
                }
                else if (value < 1)
                {
                    apkConfigObj.DblclickMaxData = 1;
                }
                else
                {
                    apkConfigObj.DblclickMaxData = value;
                }

                ApkConfigObj = apkConfigObj;

            }
        }

        public static string ForecastType
        {
            get
            {
                return apkConfigObj.ForecastType;
            }

            set
            {
                apkConfigObj.ForecastType = value;

                ApkConfigObj = apkConfigObj;
            }
        }

        private static JsonObjClass ApkConfigObj
        {
            get
            {
                return apkConfigObj;
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

    class JsonObjClass
    {
        public string Loaders { get; set; }
        public string ServerAddress { get; set; }
        public int BitmapAlpha { get; set; }
        public int AnimMaxMove { get; set; }
        public int DblclickMaxData { get; set; }
        public string ForecastType { get; set; }
    }

}
