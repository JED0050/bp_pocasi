using DelaunayTriangulator;
using Newtonsoft.Json.Linq;
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

namespace AgregaceDatLib
{
    public class WeatherUnlockedDataLoader : BitmapCustomDraw, DataLoader
    {
        //bounds
        private PointLonLat topLeft = new PointLonLat(10.88, 51.88);
        private PointLonLat botRight = new PointLonLat(20.21, 47.09);

        public string LOADER_NAME = "WeatherUnlocked";

        public WeatherUnlockedDataLoader()
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
            DateTime updatedTime = GetValidTime(forTime);

            string bitmapName = GetBitmapName(ForecastTypes.PRECIPITATION, updatedTime);
            string bitmapPath = GetPathToDataDirectory(bitmapName);

            if (File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                throw new Exception("Bitmapa srážek pro požadovaný čas nebyla nalezena!");
            }
        }

        public Bitmap GetTemperatureBitmap(DateTime forTime)
        {
            DateTime updatedTime = GetValidTime(forTime);

            string bitmapName = GetBitmapName(ForecastTypes.TEMPERATURE, updatedTime);
            string bitmapPath = GetPathToDataDirectory(bitmapName);

            if (File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                throw new Exception("Bitmapa srážek pro požadovaný čas nebyla nalezena!");
            }
        }

        public void SaveNewDeleteOldBmps()
        {
            CreateFullBmps();
        }

        public List<Forecast> GetAllForecastsFromUrl(DateTime t, string JSONtext, Point pixelCoord)
        {
            List<Forecast> forecasts = new List<Forecast>();

            //JSONForecast jF = JsonConvert.DeserializeObject<JSONForecast>(JSONtext);

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

                    DateTime actTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, 0, 0);


                    if(actTime >= t)
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

        private Dictionary<string, Bitmap> GetPixelBmps()
        {
            Dictionary<string, Bitmap> bmps = new Dictionary<string, Bitmap>();

            int bmpW = 728;
            int bmpH = 528;

            double lonDif = Math.Abs(topLeft.Lon - botRight.Lon);
            double latDif = Math.Abs(topLeft.Lat - botRight.Lat);

            double PixelLon = lonDif / bmpW;
            double PixelLat = latDif / bmpH;

            //double bY = topLeft.Lat;
            //double bX = topLeft.Lon;

            double locLon = topLeft.Lon;
            double locLat = topLeft.Lat;

            int c = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int x = 0; x <= bmpW; x+=50)
            {
                if (x >= bmpW)
                    x = bmpW - 1;

                for (int y = 0; y <= bmpH; y+=50)
                {
                    if (y >= bmpH)
                        y = bmpH - 1;

                    string lon = (locLon + x * PixelLon).ToString().Replace(",",".");
                    string lat = (locLat - y * PixelLat).ToString().Replace(",", ".");

                    string url = @"http://api.weatherunlocked.com/api/forecast/" + lat + "," + lon + "?app_id=79ef9432&app_key=904d54d78eec41c2c55ed93cbaf7c7ca";
                    string content = "";

                    if(c >= 75)
                    {
                        if(stopwatch.ElapsedMilliseconds <= 60_000)
                        {
                            Thread.Sleep(60_000 - (int)stopwatch.ElapsedMilliseconds);
                        }

                        stopwatch.Reset();
                        stopwatch.Start();

                        c = 0;
                    }

                    List<Forecast> forecasts;

                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            content = client.DownloadString(url);
                        }

                        forecasts = GetAllForecastsFromUrl(DateTime.Now.AddHours(-3), content, new Point(x, y));
                    }
                    catch
                    {
                        continue;
                    }

                    c++;

                    foreach (Forecast forecast in forecasts)
                    {
                        string tempBmpName = GetBitmapName(ForecastTypes.TEMPERATURE, forecast.Time);

                        if(!bmps.ContainsKey(tempBmpName))
                        {
                            Bitmap bmp = new Bitmap(bmpW, bmpH);

                            bmps.Add(tempBmpName, bmp);
                        }

                        bmps[tempBmpName].SetPixel((int)forecast.x, (int)forecast.y, ColorValueHandler.GetColorForValueAndType(forecast.Temperature, ForecastTypes.TEMPERATURE));


                        string precBitmap = GetBitmapName(ForecastTypes.PRECIPITATION, forecast.Time);

                        if (!bmps.ContainsKey(precBitmap))
                        {
                            Bitmap bmp = new Bitmap(bmpW, bmpH);

                            bmps.Add(precBitmap, bmp);
                        }

                        bmps[precBitmap].SetPixel((int)forecast.x, (int)forecast.y, ColorValueHandler.GetColorForValueAndType(forecast.Precipitation, ForecastTypes.PRECIPITATION));
                    }
                }
            }

            return bmps;
        }

        private void CreateFullBmps()
        {
            var dicBmps = GetPixelBmps();

            Parallel.ForEach(dicBmps.Keys, bmpName => {

                Bitmap bmp = dicBmps[bmpName];
                string bitmapPath = GetPathToDataDirectory(bmpName);

                List<Forecast> forecasts = new List<Forecast>();

                string forecType = bmpName.Split("-")[0];

                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color pixel = bmp.GetPixel(x, y);

                        if (!(pixel.R == 0 && pixel.G == 0 && pixel.B == 0))    //pixel není prázdný je to bod s přepdovědí
                        {
                            Forecast forecast = new Forecast();

                            forecast.x = x;
                            forecast.y = y;

                            forecast.GenericValue = ColorValueHandler.GetValueForColorAndType(pixel, forecType);

                            forecasts.Add(forecast);
                        }
                    }
                }

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

                        for (int x = xMin; x < xMax; x++)
                        {
                            for (int y = yMin; y < yMax; y++)
                            {
                                Point newPoint = new Point(x, y);

                                if (PointInTriangle(newPoint, p1, p2, p3))
                                    bmp.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].GenericValue, forecasts[t.b].GenericValue, forecasts[t.c].GenericValue, forecType));
                            }
                        }
                    }
                }

                bmp.Save(bitmapPath, ImageFormat.Bmp);
                bmp.Dispose();
            });

            foreach(string name in dicBmps.Keys)
            {
                dicBmps[name].Dispose();
            }
        }

        

    }
}
