using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace AgregaceDatLib.DataLoaders
{
    public class WeatherUnlockedLoader : BitmapCustomDraw, DataLoader
    {
        //bounds
        private PointLonLat topLeft = new PointLonLat(10.88, 51.88);
        private PointLonLat botRight = new PointLonLat(20.21, 47.09);

        public string LOADER_NAME = "WeatherUnlocked";

        public WeatherUnlockedLoader()
        {
            if (!Directory.Exists(GetPathToDataDirectory("")))
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"WeatherUnlocked\";

                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);

                    Directory.CreateDirectory(loaderDir);
                }
                else if (!Directory.Exists(loaderDir))
                {
                    Directory.CreateDirectory(loaderDir);
                }
            }
        }

        private string GetPathToDataDirectory(string fileName)
        {
            //string workingDirectory = Environment.CurrentDirectory;
            //return Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Data\Yr.no\" + fileName;

            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\WeatherUnlocked\" + fileName;
        }

        public Forecast GetForecast(DateTime forTime, PointLonLat point)
        {
            throw new NotImplementedException();
        }

        public Bitmap GetPrecipitationBitmap(DateTime forTime)
        {
            throw new NotImplementedException();
        }

        public Bitmap GetTemperatureBitmap(DateTime forTime)
        {
            throw new NotImplementedException();
        }

        public void SaveNewDeleteOldBmps()
        {
            throw new NotImplementedException();
        }
    }
}
