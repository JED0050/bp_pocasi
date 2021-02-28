using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DelaunayTriangulator;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Collections.Concurrent;

namespace AgregaceDatLib
{
    public class OpenWeatherMapDataLoader : BitmapCustomDraw, DataLoader
    {
        //bounds
        private PointLonLat topLeft = new PointLonLat(10.88, 51.88);
        private PointLonLat botRight = new PointLonLat(20.21, 47.09);

        public string LOADER_NAME = "OpenWeatherMap";
        
        private int bitmapW = 728;
        private int bitmapH = 528;
        public OpenWeatherMapDataLoader()
        {
            if (!Directory.Exists(GetPathToDataDirectory(@"json_cache\")))
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"Openweathermap\";

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
            //return Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Data\Openweathermap\" + fileName;

            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\Openweathermap\" + fileName;
        }

        public void SaveNewDeleteOldBmps() //7dnů +-
        {
            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                DateTime dateTime = GetDateTimeFromBitmapName(f.Name);

                if (dateTime < DateTime.Now.AddHours(-2)) //smazání starých bitmap
                {
                    f.Delete();
                }

            }

            CreateFullBmps();

        }

        private List<Forecast> GetAllForecastsFromJSON(DateTime now, string JSONtext, Point point)
        {
            List<Forecast> forecasts = new List<Forecast>();

            if (JSONtext == "")
                return forecasts;

            dynamic jsonForecast = JObject.Parse(JSONtext);

            JArray jsonForecastArray = (JArray)jsonForecast["list"];

            foreach (var timeSlot in jsonForecastArray)
            {
                Forecast f = new Forecast();
                f.x = point.X;
                f.y = point.Y;

                dynamic jsonElement = JObject.Parse(timeSlot.ToString());
                DateTime elementTime = DateTime.Parse(jsonElement.dt_txt.ToString());

                if (elementTime < now)
                    continue;

                f.Time = elementTime;

                f.Temperature = jsonElement.main.temp - 273.15;
                f.Humidity = jsonElement.main.humidity;
                f.Pressure = jsonElement.main.pressure;

                dynamic jsonWeather = JObject.Parse(((JArray)timeSlot["weather"])[0].ToString());
                string weatherState = jsonWeather.main.ToString().ToLower();

                if(weatherState == "rain")
                {
                    f.Precipitation = jsonElement.rain.GetValue("3h");
                }
                else
                {
                    f.Precipitation = 0;
                }
                
                forecasts.Add(f);
            }

            return forecasts;
        }

        private DateTime GetValidTime(DateTime forTime)
        {
            DateTime updatedTime = forTime.AddMinutes(30);

            if (updatedTime.Hour % 3 < 2)
            {
                updatedTime = updatedTime.AddHours(-(updatedTime.Hour % 3));
            }
            else
            {
                updatedTime = updatedTime.AddHours(3 - updatedTime.Hour % 3);
            }

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

            forecast.Precipitation = GetValueFromBitmapTypeAndPoints(GetForecastBitmap(forTime, ForecastTypes.PRECIPITATION), topLeft, botRight, location, ForecastTypes.PRECIPITATION);
            forecast.Temperature = GetValueFromBitmapTypeAndPoints(GetForecastBitmap(forTime, ForecastTypes.TEMPERATURE), topLeft, botRight, location, ForecastTypes.TEMPERATURE);
            forecast.Humidity = GetValueFromBitmapTypeAndPoints(GetForecastBitmap(forTime, ForecastTypes.HUMIDITY), topLeft, botRight, location, ForecastTypes.HUMIDITY);
            forecast.Pressure = GetValueFromBitmapTypeAndPoints(GetForecastBitmap(forTime, ForecastTypes.PRESSURE), topLeft, botRight, location, ForecastTypes.PRESSURE);

            return forecast;
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
                throw new Exception("Bitmapa počasí pro daný čas nebyla nalezena!");
            }
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

            int c = 0;

            int apiMinuteLimit = 60;

            Stopwatch apiLimitStopwatch = new Stopwatch();
            apiLimitStopwatch.Start();

            int pixelGap = 25;

            List<string> apiPart = new List<string>();
            apiPart.Add("ea63080a4f8e99972630d2671e3ef805");
            apiPart.Add("621f630a81701821a6309170b5ec310f");
            apiPart.Add("58efa6edb4c39e14d88acd636986dec5");
            apiPart.Add("5c6924655aea1fde4c178d274f5b6afc");

            int apiPartIndex = 0;

            bool doXBreak = false;
            bool doYBreak = false;

            DateTime minimalDateTime = DateTime.Now.AddHours(-3);

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

                    string url = @"https://api.openweathermap.org/data/2.5/forecast?lat=" + lat + "&lon=" + lon + "&appid=" + apiPart[apiPartIndex];

                    string content = "";

                    if (c >= apiMinuteLimit * apiPart.Count)
                    {
                        if (apiLimitStopwatch.ElapsedMilliseconds <= 60_000)
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
                    catch
                    {
                        Debug.WriteLine($"Drop na {url}");
                    }

                    List<Forecast> forecasts = GetAllForecastsFromJSON(minimalDateTime, content, new Point(x, y));

                    foreach (Forecast forecast in forecasts)
                    {

                        if (!dicForecasts.ContainsKey(forecast.Time))
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

                    if (doYBreak)
                    {
                        doYBreak = false;
                        break;
                    }
                }

                if (doXBreak)
                    break;
            }

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
    }
}
