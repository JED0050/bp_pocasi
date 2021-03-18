using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using IDataLoaderAndHandlerLib.Interface;
using IDataLoaderAndHandlerLib.HandlersAndObjects;
using System.Collections.Generic;

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
            if (!Directory.Exists(GetPathToDataDirectory("")))
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"Radar.bourky\";

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
            else if(forTime < DateTime.Now.AddHours(-6))
            {
                throw new Exception("zvolený čas je příliš nízký, služba poskytuje pouze předpovědi aktuální čas");
            }

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));

            FileInfo[] fileInfos = dI.GetFiles("*.bmp");

            string bitmapPath = "";

            TimeSpan minTS = forTime - DateTime.Now.AddHours(-8);

            for (int i = 0; i < fileInfos.Length; i++)
            {
                DateTime dT = GetDateTimeFromBitmapName(fileInfos[i].Name);
                TimeSpan actTS = dT - forTime;

                if (i == 0)
                {
                    minTS = actTS;
                    bitmapPath = fileInfos[i].FullName;
                }
                else
                {
                    if (Math.Abs(actTS.TotalMinutes) < Math.Abs(minTS.TotalMinutes))
                    {
                        minTS = actTS;
                        bitmapPath = fileInfos[i].FullName;
                    }
                }
            }

            if(Math.Abs(minTS.TotalHours) <= 6)
            {
                Console.WriteLine(bitmapPath);

                return new Bitmap(bitmapPath);
            }
            else
            {
                throw new Exception("Požadovaná bitmapa srážek nebyla nalezena");
            }
        }

        protected override string GetPathToDataDirectory(string fileName)
        {
            //string workingDirectory = Environment.CurrentDirectory;
            //return Directory.GetParent(workingDirectory).FullName + @"\Data\Radar.bourky\" + fileName;

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

                try
                {
                    radarBitmap = GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.ToString("yyyyMMdd.HHmm") + ".0.png");

                    string bitmapName = GetBitmapName(ForecastTypes.PRECIPITATION, forTime.AddHours(1)); //UTC na UTC+1
                    string bitmapPath = GetPathToDataDirectory(bitmapName);

                    ClearBitmap(radarBitmap);

                    int xS = -1;
                    int yS = -1;

                    int xC = 0;
                    int yC = 0;

                    double lonDif = defaultBotRightBound.Lon - defaultTopLeftBound.Lon;
                    double latDif = defaultTopLeftBound.Lat - defaultBotRightBound.Lat;

                    double PixelLon = lonDif / 728;
                    double PixelLat = latDif / 528;

                    double bY = defaultTopLeftBound.Lat;
                    double bX = defaultTopLeftBound.Lon;

                    for (int x = 0; x < 728; x++)
                    {
                        if (bX >= botRight.Lon)
                            break;

                        if (bX >= topLeft.Lon)
                        {
                            xC++;

                            if (xS == -1)
                                xS = x;
                        }

                        bX += PixelLon;
                    }

                    for (int y = 0; y < 528; y++)
                    {
                        if (bY <= botRight.Lat)
                            break;

                        if (bY <= topLeft.Lat)
                        {
                            yC++;

                            if (yS == -1)
                                yS = y;
                        }

                        bY -= PixelLat;
                    }

                    Bitmap newBmp = new Bitmap(728, 528);

                    Bitmap resizedBmp = new Bitmap(xC, yC);
                    using (Graphics g = Graphics.FromImage(resizedBmp))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.DrawImage(radarBitmap, 0, 0, xC, yC);
                    };

                    for (int x = 0; x < xC; x++)
                    {
                        for (int y = 0; y < yC; y++)
                        {
                            newBmp.SetPixel(x + xS, y + yS, resizedBmp.GetPixel(x, y));
                        }
                    }

                    newBmp.Save(bitmapPath, ImageFormat.Bmp);
                    newBmp.Dispose();

                    resizedBmp.Dispose();

                    radarBitmap.Dispose();
                }
                catch
                { }

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

                if (dateTime < DateTime.Now.AddHours(-6)) //smazání starých bitmap
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

            forecast.Precipitation = GetValueFromBitmapTypeAndPoints(GetForecastBitmap(forTime, ForecastTypes.PRECIPITATION), topLeft, botRight, location, ForecastTypes.PRECIPITATION);

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
