﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using IDataLoaderAndHandlerLib.DelaunayTriangulator;
using IDataLoaderAndHandlerLib.HandlersAndObjects;
using IDataLoaderAndHandlerLib.Interface;
using System.Reflection;

namespace DLWeatherUnlockedLib
{
    public class WeatherUnlockedDataLoader : DataLoaderHandler, DataLoader
    {
        //bounds
        private PointLonLat topLeft = DefaultBounds.TopLeftCorner;
        private PointLonLat botRight = DefaultBounds.BotRightCorner;

        public string LOADER_NAME;// = "WeatherUnlocked";

        public WeatherUnlockedDataLoader()
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

            string jsonConfifFile = GetPathToDataDirectory("loaderConfig.json");

            if (!File.Exists(jsonConfifFile))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string scaleAssembylName = "DLWeatherUnlockedLib.Resources.loaderConfig.json";

                using (var stream = assembly.GetManifestResourceStream(scaleAssembylName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        File.WriteAllText(jsonConfifFile, reader.ReadToEnd());
                    }
                }
            }

            dataLoaderConfig = GetDataLoaderConfigFile();

            //topLeft = dataLoaderConfig.TopLeftCornerLonLat;
            //botRight = dataLoaderConfig.BotRightCornerLonLat;

            LOADER_NAME = dataLoaderConfig.DataLoaderName;
        }

        protected override string GetPathToDataDirectory(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;

            return workingDirectory + @"\Data\WeatherUnlocked\" + fileName;
        }

        public Forecast GetForecastPoint(DateTime forTime, PointLonLat location)
        {
            forTime = GetValidTime(forTime);

            Forecast forecast = new Forecast();

            forecast.Longitude = location.Lon.ToString();
            forecast.Latitude = location.Lat.ToString();
            forecast.Time = forTime;
            forecast.AddDataSource(LOADER_NAME);

            Point targetPoint = GetPointFromBoundsAndTarget(new Size(728, 528), DefaultBounds, location);

            forecast.Precipitation = GetValueFromBitmapAndPoint(GetForecastBitmap(forTime, ForecastTypes.PRECIPITATION), targetPoint, ForecastTypes.PRECIPITATION);
            forecast.Temperature = GetValueFromBitmapAndPoint(GetForecastBitmap(forTime, ForecastTypes.TEMPERATURE), targetPoint, ForecastTypes.TEMPERATURE);
            forecast.Humidity = GetValueFromBitmapAndPoint(GetForecastBitmap(forTime, ForecastTypes.HUMIDITY), targetPoint, ForecastTypes.HUMIDITY);
            forecast.Pressure = GetValueFromBitmapAndPoint(GetForecastBitmap(forTime, ForecastTypes.PRESSURE), targetPoint, ForecastTypes.PRESSURE);

            return forecast;
        }

        public void SaveNewDeleteOldBmps()
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
                CreateFullBmps();

                dataLoaderConfig.LastUpdateDateTime = DateTime.Now;
                CreateNewConfigFile(dataLoaderConfig);
            }

            Debug.WriteLine(LOADER_NAME + ": Dokončení vytváření bitmap");
        }

        public List<Forecast> GetAllForecastsFromUrl(DateTime t, string JSONtext, Point pixelCoord)
        {
            List<Forecast> forecasts = new List<Forecast>();

            if (JSONtext == "")
                return forecasts;

            dynamic jsonForecast = JObject.Parse(JSONtext);

            JArray jsonArDays = (JArray)jsonForecast["Days"];

            foreach (dynamic day in jsonArDays)
            {
                JArray jsonArTimeSlots = (JArray)day["Timeframes"];

                string dateString = day.date;

                DateTime dateTime = DateTime.Parse(dateString, CultureInfo.CreateSpecificCulture("cs-CZ")); 

                foreach (dynamic timeSlot in jsonArTimeSlots)
                {
                    int hour = timeSlot.time / 100;

                    //DateTime actTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, 0, 0);
                    DateTime actTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, 0, 0), TimeZoneInfo.Local);

                    if (actTime >= t)
                    {
                        Forecast f = new Forecast();

                        f.x = pixelCoord.X;
                        f.y = pixelCoord.Y;

                        f.Time = GetValidTime(actTime);

                        f.Temperature = timeSlot.temp_c;
                        f.Humidity = timeSlot.humid_pct;
                        f.Precipitation = timeSlot.precip_mm;
                        f.Pressure = timeSlot.slp_mb;

                        forecasts.Add(f);
                    }
                }        
            }

            return forecasts;
        }

        private DateTime GetValidTime(DateTime forTime)
        {
            forTime = forTime.AddMinutes(30);

            DateTime updatedTime = new DateTime(forTime.Year, forTime.Month, forTime.Day, forTime.Hour, 0, 0);

            if (updatedTime.Hour % 3 < 2)
            {
                updatedTime = updatedTime.AddHours(-(updatedTime.Hour % 3));
            }
            else
            {
                updatedTime = updatedTime.AddHours(3 - updatedTime.Hour % 3);
            }

            return updatedTime.AddHours(1);
        }

        public Dictionary<DateTime, List<Forecast>> GetWeekForecast()
        {
            Dictionary<DateTime, List<Forecast>> dicForecasts = new Dictionary<DateTime, List<Forecast>>();

            int bmpW = 728;
            int bmpH = 528;

            double lonDif = Math.Abs(topLeft.Lon - botRight.Lon);
            double latDif = Math.Abs(topLeft.Lat - botRight.Lat);

            double PixelLon = lonDif / bmpW;
            double PixelLat = latDif / bmpH;

            double locLon = topLeft.Lon;
            double locLat = topLeft.Lat;

            int c = 0;

            int apiMinuteLimit = dataLoaderConfig.MaximumDownloadsPerMinute;

            Stopwatch apiLimitStopwatch = new Stopwatch();
            apiLimitStopwatch.Start();

            int pixelGap = 25;

            List<string> apiPart = new List<string>();
            apiPart.Add("?app_id=79ef9432&app_key=904d54d78eec41c2c55ed93cbaf7c7ca");
            apiPart.Add("?app_id=c0e28ff1&app_key=71ef299d81c23d71cd36fcb8ee8691ba");

            int apiPartIndex = 0;

            bool doXBreak = false;
            bool doYBreak = false;

            DateTime minimalDateTime = DateTime.Now.AddHours(-dataLoaderConfig.MaximumHoursBack);

            for (int x = 0; x < bmpW + pixelGap; x+=pixelGap)
            {
                if (x >= bmpW)
                {
                    doXBreak = true;
                    x = bmpW - 1;
                }

                for (int y = 0; y <= bmpH + pixelGap; y+=pixelGap)
                {
                    if (y >= bmpH)
                    {
                        doYBreak = true;
                        y = bmpH - 1;
                    }

                    string lon = (locLon + x * PixelLon).ToString().Replace(",",".");
                    string lat = (locLat - y * PixelLat).ToString().Replace(",", ".");

                    string url = @"http://api.weatherunlocked.com/api/forecast/" + lat + "," + lon + apiPart[apiPartIndex];
                    
                    string content = "";

                    if(c >= apiMinuteLimit * apiPart.Count)
                    {
                        if(apiLimitStopwatch.ElapsedMilliseconds <= 60_000)
                        {
                            Thread.Sleep(60_000 - (int)apiLimitStopwatch.ElapsedMilliseconds);

                            Debug.WriteLine("Čekáme");
                        }

                        apiLimitStopwatch.Reset();
                        apiLimitStopwatch.Start();

                        c = 0;
                    }

                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            content = client.DownloadString(url);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"{LOADER_NAME}: drop na {url}, {e.Message}");

                        //continue;
                    }

                    List<Forecast> forecasts = GetAllForecastsFromUrl(minimalDateTime, content, new Point(x, y));

                    foreach (Forecast forecast in forecasts)
                    {

                        if(!dicForecasts.ContainsKey(forecast.Time))
                        {
                            List<Forecast> newForecasts = new List<Forecast>();

                            dicForecasts.Add(forecast.Time, newForecasts);
                        }

                        dicForecasts[forecast.Time].Add(forecast);

                    }

                    c++;
                    apiPartIndex++;

                    if (apiPartIndex > apiPart.Count - 1)
                        apiPartIndex = 0;

                    //Debug.WriteLine($"x: {x} y: {y} c: {c} apiPart: {apiPartIndex}");

                    if (doYBreak)
                    {
                        doYBreak = false;
                        break;
                    }
                }

                Debug.Indent();
                Debug.WriteLine($"{LOADER_NAME}: zpracováno {x}x / {bmpW - 1}x");
                Debug.Unindent();

                if (doXBreak)
                    break;
            }


            DateTime lastDate = DateTime.Now;

            foreach(DateTime date in dicForecasts.Keys)
            {
                if (date > lastDate)
                    lastDate = date;
            }

            if (lastDate != DateTime.Now)
                dicForecasts.Remove(lastDate);  //odstranění poslední předpovědi, je bez dat


            return dicForecasts;
        }

        private void CreateFullBmps()
        {
            var dicBmps = GetWeekForecast();

            Parallel.ForEach(dicBmps.Keys, bmpTime => {
                List<Forecast> forecasts = dicBmps[bmpTime];

                Bitmap tempBmp = new Bitmap(728, 528);
                Bitmap precBmp = new Bitmap(728, 528);
                Bitmap presBmp = new Bitmap(728, 528);
                Bitmap humiBmp = new Bitmap(728, 528);

                string tempBmpFullName = GetPathToDataDirectory(GetBitmapName(ForecastTypes.TEMPERATURE, bmpTime));
                string precBmpFullName = GetPathToDataDirectory(GetBitmapName(ForecastTypes.PRECIPITATION, bmpTime));
                string presBmpFullName = GetPathToDataDirectory(GetBitmapName(ForecastTypes.PRESSURE, bmpTime));
                string humiBmpFullName = GetPathToDataDirectory(GetBitmapName(ForecastTypes.HUMIDITY, bmpTime));

                if (forecasts.Count >= 3)   //pokud je počet bodů < 3 tak vrátíme prázdné počasí (nelze udělat trojuhleník)
                {
                    Triangulator angulator = new Triangulator();
                    List<Vertex> vertexes = forecasts.ConvertAll(x => (Vertex)x);
                    List<Triad> triangles = angulator.Triangulation(vertexes, true);

                    for (int i = 0; i < triangles.Count; i++)
                    {
                        Triad t = triangles[i];

                        Point p1 = new Point((int)forecasts[t.a].x, (int)forecasts[t.a].y);
                        Point p2 = new Point((int)forecasts[t.b].x, (int)forecasts[t.b].y);
                        Point p3 = new Point((int)forecasts[t.c].x, (int)forecasts[t.c].y);

                        Point[] arP = new Point[] { p1, p2, p3 };

                        int xMin = Math.Min(p1.X, Math.Min(p2.X, p3.X));
                        int xMax = Math.Max(p1.X, Math.Max(p2.X, p3.X));
                        int yMin = Math.Min(p1.Y, Math.Min(p2.Y, p3.Y));
                        int yMax = Math.Max(p1.Y, Math.Max(p2.Y, p3.Y));

                        for (int x = xMin; x <= xMax; x++)
                        {
                            for (int y = yMin; y <= yMax; y++)
                            {
                                Point newPoint = new Point(x, y);

                                if (PointInTriangle(newPoint, p1, p2, p3))
                                {
                                    tempBmp.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].Temperature.Value, forecasts[t.b].Temperature.Value, forecasts[t.c].Temperature.Value, ForecastTypes.TEMPERATURE));
                                    precBmp.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].Precipitation.Value, forecasts[t.b].Precipitation.Value, forecasts[t.c].Precipitation.Value, ForecastTypes.PRECIPITATION));
                                    presBmp.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].Pressure.Value, forecasts[t.b].Pressure.Value, forecasts[t.c].Pressure.Value, ForecastTypes.PRESSURE));
                                    humiBmp.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].Humidity.Value, forecasts[t.b].Humidity.Value, forecasts[t.c].Humidity.Value, ForecastTypes.HUMIDITY));
                                }
                            }
                        }
                    }
                }

                tempBmp.Save(tempBmpFullName, ImageFormat.Bmp);
                tempBmp.Dispose();

                precBmp.Save(precBmpFullName, ImageFormat.Bmp);
                precBmp.Dispose();

                presBmp.Save(presBmpFullName, ImageFormat.Bmp);
                presBmp.Dispose();

                humiBmp.Save(humiBmpFullName, ImageFormat.Bmp);
                humiBmp.Dispose();

            });
        }

        public Bitmap GetForecastBitmap(DateTime forTime, string type)
        {
            DateTime updatedTime = GetValidTime(forTime);

            string bitmapName = GetBitmapName(type, updatedTime);
            string bitmapPath = GetPathToDataDirectory(bitmapName);

            if (File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                throw new Exception("Bitmapa počasí pro požadovaný čas nebyla nalezena!");
            }
        }

        public DataLoaderConfig GetLoaderConfigFile()
        {
            return dataLoaderConfig;
        }
    }
}
