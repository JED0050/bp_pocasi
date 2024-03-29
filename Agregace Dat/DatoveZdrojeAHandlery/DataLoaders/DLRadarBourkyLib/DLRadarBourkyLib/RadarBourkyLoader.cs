﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using IDataLoaderAndHandlerLib.Interface;
using IDataLoaderAndHandlerLib.HandlersAndObjects;
using System.Collections.Generic;
using System.Reflection;

namespace DLRadarBourkyLib
{
    public class RadarBourkyDataLoader : DataLoaderHandler, DataLoader
    {
        //bounds
        private PointLonLat topLeft;// = new PointLonLat(10.06, 51.88);
        private PointLonLat botRight;// = new PointLonLat(20.21, 47.09);

        public string LOADER_NAME;// = "RadarBourky";

        private static Dictionary<Color, double> scaleDic = new Dictionary<Color, double>();

        public RadarBourkyDataLoader()
        {
            string dataDir = Environment.CurrentDirectory + @"\Data\";
            string loaderDir = dataDir + @"Radar.bourky\";

            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
                Directory.CreateDirectory(loaderDir);
                Directory.CreateDirectory(loaderDir + @"\scales");
            }
            else if (!Directory.Exists(loaderDir))
            {
                Directory.CreateDirectory(loaderDir);
                Directory.CreateDirectory(loaderDir + @"\scales");
            }
            else if (!Directory.Exists(loaderDir + @"\scales"))
            {
                Directory.CreateDirectory(loaderDir + @"\scales");
            }

            string jsonConfifFile = GetPathToDataDirectory("loaderConfig.json");

            if (!File.Exists(jsonConfifFile))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string scaleAssembylName = "DLRadarBourkyLib.Resources.loaderConfig.json";

                using (var stream = assembly.GetManifestResourceStream(scaleAssembylName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        File.WriteAllText(jsonConfifFile, reader.ReadToEnd());
                    }
                }
            }

            string scalePath = GetPathToDataDirectory(@"scales/scale.png");

            if (!File.Exists(scalePath))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string scaleAssembylName = "DLRadarBourkyLib.Resources.scale.png";

                using (var stream = assembly.GetManifestResourceStream(scaleAssembylName))
                {
                    Image.FromStream(stream).Save(scalePath, ImageFormat.Png);
                }
            }

            dataLoaderConfig = GetDataLoaderConfigFile();

            topLeft = dataLoaderConfig.TopLeftCornerLonLat;
            botRight = dataLoaderConfig.BotRightCornerLonLat;

            LOADER_NAME = dataLoaderConfig.DataLoaderName;
        }
        public Bitmap GetBitmap(string url)
        {

            Bitmap bitmap;

            using (WebClient client = new WebClient())
            {
                Stream stream = client.OpenRead(url);
                bitmap = new Bitmap(stream);

                //stream.Flush();
                //stream.Close();
                //client.Dispose();
            }

            return bitmap;

        }

        public Bitmap GetForecastBitmap(DateTime forTime, string type)
        {
            if (type != ForecastTypes.PRECIPITATION)
                throw new Exception("Datový zdroj poskytuje pouze data o srážkách!");

            forTime = forTime.AddMinutes(- forTime.Minute % 10);

            if (forTime > DateTime.Now.AddHours(6))
            {
                throw new Exception("zvolený čas je příliš vysoký, služba poskytuje pouze předpovědi aktuální čas");
            }
            else if(forTime < DateTime.Now.AddHours(-25))
            {
                throw new Exception("zvolený čas je příliš nízký, služba poskytuje pouze předpovědi aktuální čas");
            }

            try
            {
                string bitmapName = GetBitmapName(ForecastTypes.PRECIPITATION, forTime);
                string bitmapPath = GetPathToDataDirectory(bitmapName);

                return new Bitmap(bitmapPath);
            }
            catch
            {
                DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));

                FileInfo[] fileInfos = dI.GetFiles("*.bmp");

                DateTime lastBmpTime = GetDateTimeFromBitmapName(fileInfos[fileInfos.Length - 1].Name);

                if(forTime > lastBmpTime && (forTime - lastBmpTime).TotalHours <= 6.1)
                {
                    return new Bitmap(fileInfos[fileInfos.Length - 1].FullName);
                }
            }

            throw new Exception("Požadovaná bitmapa srážek nebyla nalezena");
        }

        protected override string GetPathToDataDirectory(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\Radar.bourky\" + fileName;
        }

        private void ClearBitmap(Bitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for(int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    double precVal = GetPrecipitationFromPixel(c);

                    bitmap.SetPixel(x, y, ColorValueHandler.GetPrecipitationColor(precVal));
                }
            }
        }

        private void CreateBitmap()
        {
            DateTime now = DateTime.Now.AddHours(- dataLoaderConfig.MaximumHoursBack);

            DateTime forTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            Bitmap radarBitmap;

            now = DateTime.Now.AddHours(6);

            while (forTime <= now)
            {
                string bitmapName = GetBitmapName(ForecastTypes.PRECIPITATION, TimeZoneInfo.ConvertTimeFromUtc(forTime, TimeZoneInfo.Local));
                string bitmapPath = GetPathToDataDirectory(bitmapName);

                Bounds loaderBounds = new Bounds(topLeft, botRight);

                if (!File.Exists(bitmapPath))
                {
                    try
                    {
                        //TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.Utc);

                        radarBitmap = GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.ToString("yyyyMMdd.HHmm") + ".0.png");

                        //string bitmapName = GetBitmapName(ForecastTypes.PRECIPITATION, forTime.AddHours(1)); //UTC na UTC+1
                        //string bitmapPath = GetPathToDataDirectory(bitmapName);

                        ClearBitmap(radarBitmap);

                        Bitmap newBmp = GetPartOfBitmap(radarBitmap, loaderBounds);

                        newBmp.Save(bitmapPath, ImageFormat.Bmp);
                        newBmp.Dispose();
                    }
                    catch
                    { }
                }

                forTime = forTime.AddMinutes(10);
            }

        }

        public void SaveNewDeleteOldBmps() //1 hodina +- (nepoužitelnéprakticky krom aktuálního počasí)
        {
            Debug.WriteLine(LOADER_NAME + ": Zahájení vytváření bitmap");

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                DateTime dateTime = GetDateTimeFromBitmapName(f.Name);

                if (dateTime < DateTime.Now.AddHours(-24)) //smazání starých bitmap
                {
                    f.Delete();
                }
            }

            dataLoaderConfig = GetDataLoaderConfigFile();

            if (IsReadyToDownloadData(dataLoaderConfig))
            {
                CreateScaleDic();

                CreateBitmap();

                dataLoaderConfig.LastUpdateDateTime = DateTime.Now;
                CreateNewConfigFile(dataLoaderConfig);
            }

            Debug.WriteLine(LOADER_NAME + ": Dokončení vytváření bitmap");
        }

        private double GetPrecipitationFromPixel(Color pixel)
        {
            if (scaleDic.ContainsKey(pixel))
            {
                return scaleDic[pixel];
            }
            else
            {
                return 0;
            }
        }

        private void CreateScaleDic()
        {
            if (scaleDic.Keys.Count == 0)
            {

                Bitmap scaleBmp = new Bitmap(GetPathToDataDirectory(@"scales/scale.png"));

                /*
                srážky dle RGB barev

                index - R G B - srážky

                0 - 0 0 0 - 0 mm/h
                4 - 56 0 112 - 0.05mm/h

                8 - 48 0 168 - 0.1 mm/h
                12 - 0 0 252 - 0.2 mm/h
                16 - 0 108 192 - 0.4 mm/h
                20 - 0 160 0 - 0.8 mm/h

                24 - 0 188 0 - 1 mm/h
                28 - 52 216 0 - 2 mm/h
                32 - 156 220 0 - 4 mm/h
                36 - 224 220 0 - 8 mm/h

                40 - 252 176 0 - 10 mm/h
                44 - 252 132 0 - 20 mm/h
                48 - 252 88 0 - 40 mm/h

                52 - 252 0 0 - 80 mm/h
                56 - 160 0 0 - 100 mm/h
                60 - 255 255 255 - 100+ mm/h
                */

                double val = 0.05;

                int step = 30;

                for (int i = 0; i < scaleBmp.Width; i += step)
                {
                    if (i == 5 * step)
                        val = 1;
                    else if (i == 9 * step)
                        val = 10;
                    else if (i >= 13 * step)
                        val = 100;

                    scaleDic.Add(scaleBmp.GetPixel(i, 0), val);

                    val *= 2;
                }
            }
        }

        private new string GetBitmapName(string type, DateTime time)
        {
            return type + "-" + time.ToString("yyyy-MM-dd-HH-mm") + ".bmp";
        }

        protected new DateTime GetDateTimeFromBitmapName(string name)
        {
            string onlyDateName = name.Split('.')[0];

            string[] timeParts = onlyDateName.Split("-");

            DateTime dateTime = new DateTime(int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), int.Parse(timeParts[4]), int.Parse(timeParts[5]), 0);

            return dateTime;
        }

        public Forecast GetForecastPoint(DateTime forTime, PointLonLat location)
        {
            Forecast forecast = new Forecast();

            forecast.Longitude = location.Lon.ToString();
            forecast.Latitude = location.Lat.ToString();
            forecast.Time = forTime;
            forecast.AddDataSource(LOADER_NAME);

            forecast.Precipitation = GetValueFromBitmapTypeAndBounds(GetForecastBitmap(forTime, ForecastTypes.PRECIPITATION), DefaultBounds, location, ForecastTypes.PRECIPITATION);

            forecast.Humidity = null;
            forecast.Pressure = null;
            forecast.Temperature = null;

            return forecast;
        }

        public DataLoaderConfig GetLoaderConfigFile()
        {
            return dataLoaderConfig;
        }
    }
}
