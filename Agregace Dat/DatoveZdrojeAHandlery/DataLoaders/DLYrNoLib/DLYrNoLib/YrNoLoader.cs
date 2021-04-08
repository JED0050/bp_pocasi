using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using IDataLoaderAndHandlerLib.Interface;
using IDataLoaderAndHandlerLib.DelaunayTriangulator;
using IDataLoaderAndHandlerLib.HandlersAndObjects;
using System.Reflection;

namespace DLYrNoLib
{

    public class YrNoDataLoader : DataLoaderHandler, DataLoader
    {
        //bounds
        private PointLonLat topLeft = DefaultBounds.TopLeftCorner;
        private PointLonLat botRight = DefaultBounds.BotRightCorner;

        public string LOADER_NAME;// = "Yr.No";

        public YrNoDataLoader()
        {
            string dataDir = Environment.CurrentDirectory + @"\Data\";
            string loaderDir = dataDir + @"Yr.no\";

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
                string scaleAssembylName = "DLYrNoLib.Resources.loaderConfig.json";

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
                DateTime closestTime = DateTime.Now.AddHours(-100);
                TimeSpan closestTimeSpan = forTime - closestTime;
                double minHourSpan = Math.Abs(closestTimeSpan.TotalHours);
                string bitmapFullName = "";

                DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
                foreach (var f in dI.GetFiles($"{type}-*.bmp"))
                {
                    DateTime dateTime = GetDateTimeFromBitmapName(f.Name);

                    TimeSpan actTimeSpan = forTime - dateTime;
                    double actHourSpan = Math.Abs(actTimeSpan.TotalHours);

                    if (actHourSpan < minHourSpan)
                    {
                        bitmapFullName = f.FullName;

                        if(actHourSpan <= 1)
                        {
                            return new Bitmap(bitmapFullName);
                        }

                        minHourSpan = actHourSpan;
                    }
                }

                if(minHourSpan <= 6)
                {
                    return new Bitmap(bitmapFullName);
                }

                throw new Exception("Bitmapa počasí pro požadovaný čas nebyla nalezena!");
            }

        }

        protected override string GetPathToDataDirectory(string fileName)
        {
            //string workingDirectory = Environment.CurrentDirectory;
            //return Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Data\Yr.no\" + fileName;

            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\Yr.no\" + fileName;
        }

        public void SaveNewDeleteOldBmps()  //8 dnů +-
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

        public List<Forecast> GetAllForecastsFromUrl(DateTime minimalTime, string xmlText, Point point)
        {
            List<Forecast> forecasts = new List<Forecast>();

            if (xmlText == "")
                return forecasts;

            TextReader tr = new StringReader(xmlText);

            XDocument xmlDoc = XDocument.Load(tr);

            bool precFromNext = false;

            double humi = 0;
            double temp = 0;
            double pres = 0;

            DateTime actTime = DateTime.Now;

            foreach (var timeSlot in xmlDoc.Descendants("time"))
            {
                if(precFromNext)
                {
                    precFromNext = false;

                    Forecast f = new Forecast();

                    f.x = point.X;
                    f.y = point.Y;

                    f.Time = actTime;

                    f.Temperature = temp;
                    f.Humidity = humi;
                    f.Pressure = pres;

                    if(timeSlot.Element("location").Element("precipitation") == null)
                    {
                        f.Precipitation = 0;
                    }
                    else
                    {
                        f.Precipitation = double.Parse(timeSlot.Element("location").Element("precipitation").Attribute("value").Value, CultureInfo.InvariantCulture);
                    }

                    forecasts.Add(f);

                    continue;
                }

                //nemusí se parsovat do local time protože ve formátu času je Z na konci
                //DateTime from = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(timeSlot.Attribute("from").Value.ToString()), TimeZoneInfo.Local);
                //DateTime to = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(timeSlot.Attribute("to").Value.ToString()), TimeZoneInfo.Local);

                DateTime from = DateTime.Parse(timeSlot.Attribute("from").Value.ToString());
                DateTime to = DateTime.Parse(timeSlot.Attribute("to").Value.ToString());

                if (from == to)
                {
                    if(from < minimalTime)
                    {
                        continue;
                    }

                    temp = double.Parse(timeSlot.Element("location").Element("temperature").Attribute("value").Value, CultureInfo.InvariantCulture);
                    humi = double.Parse(timeSlot.Element("location").Element("humidity").Attribute("value").Value, CultureInfo.InvariantCulture);
                    pres = double.Parse(timeSlot.Element("location").Element("pressure").Attribute("value").Value, CultureInfo.InvariantCulture);

                    actTime = from;

                    precFromNext = true;
                }

            }

            return forecasts;
        }


        private DateTime GetValidTime(DateTime forTime)
        {
            forTime = forTime.AddMinutes(30);

            DateTime updatedTime = new DateTime(forTime.Year, forTime.Month, forTime.Day, forTime.Hour, 0, 0);

            return updatedTime;
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

        private Dictionary<DateTime, List<Forecast>> GetWeekForecast()
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

            int pixelGap = 25;

            bool doXBreak = false;
            bool doYBreak = false;

            DateTime minimalDateTime = DateTime.Now.AddHours(- dataLoaderConfig.MaximumHoursBack);

            for (int x = 0; x < bmpW + pixelGap; x += pixelGap)
            {
                if (x >= bmpW)
                {
                    doXBreak = true;
                    x = bmpW - 1;
                }

                for (int y = 0; y <= bmpH + pixelGap; y += pixelGap)
                {
                    if (y >= bmpH)
                    {
                        doYBreak = true;
                        y = bmpH - 1;
                    }

                    string lon = (locLon + x * PixelLon).ToString().Replace(",", ".");
                    string lat = (locLat - y * PixelLat).ToString().Replace(",", ".");

                    string url = @"https://api.met.no/weatherapi/locationforecast/2.0/classic?lat=" + lat + "&lon=" + lon;

                    string content = "";

                    try
                    {
                        //Thread.Sleep(100);

                        using (WebClient client = new WebClient())
                        {
                            client.Headers.Add("User-Agent: Other");

                            content = client.DownloadString(url);
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine($"{LOADER_NAME}: drop na {url}, {e.Message}");

                        //continue;
                    }

                    List<Forecast> forecasts = GetAllForecastsFromUrl(minimalDateTime, content, new Point(x, y));

                    foreach (Forecast forecast in forecasts)
                    {

                        if (!dicForecasts.ContainsKey(forecast.Time))
                        {
                            List<Forecast> newForecasts = new List<Forecast>();

                            dicForecasts.Add(forecast.Time, newForecasts);
                        }

                        dicForecasts[forecast.Time].Add(forecast);

                    }

                    //Debug.WriteLine($"x: {x} y: {y}");

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

            //Debug.WriteLine("DONE");

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

        public DataLoaderConfig GetLoaderConfigFile()
        {
            return dataLoaderConfig;
        }
    }
}

