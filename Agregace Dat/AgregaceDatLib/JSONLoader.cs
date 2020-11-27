﻿using Newtonsoft.Json.Linq;
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

namespace AgregaceDatLib
{
    public class JSONDataLoader : BitmapHelper, DataLoader
    {
        //bounds
        private PointLonLat topLeft = new PointLonLat(10.88, 51.88);
        private PointLonLat botRight = new PointLonLat(20.21, 47.09);
        public JSONDataLoader()
        {
            if (!Directory.Exists(GetPathToDataDirectory(@"json_cache\")))
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"Openweathermap\";
                string cacheDir = loaderDir + @"json_cache\";

                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                    Directory.CreateDirectory(loaderDir);
                    Directory.CreateDirectory(cacheDir);
                }
                else if (!Directory.Exists(loaderDir))
                {
                    Directory.CreateDirectory(loaderDir);
                    Directory.CreateDirectory(cacheDir);
                }
                else if(!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
            }
        }
        public string GetJSON(string url)
        {
            string JSONText = "";

            using (var client = new WebClient())
            {
                JSONText = client.DownloadString(url);
            }

            return JSONText;
        }

        public Forecast GetForecastByTime(DateTime t, string JSONtext)
        {
            Forecast f = new Forecast();
            f.Time = t;

            //JSONForecast jF = JsonConvert.DeserializeObject<JSONForecast>(JSONtext);

            dynamic jsonForecast = JObject.Parse(JSONtext);

            //f.City = jsonForecast.city.name;
            //f.Country = jsonForecast.city.country;
            f.Latitude = jsonForecast.city.coord.lat;
            f.Longitude = jsonForecast.city.coord.lon;

            JArray jsonForecastArray = (JArray)jsonForecast["list"];

            foreach (var el in jsonForecastArray)
            {
                dynamic jsonElement = JObject.Parse(el.ToString());

                DateTime elementTime = DateTime.Parse(jsonElement.dt_txt.ToString());

                if (t == elementTime)
                {
                    f.Temperature = Double.Parse(jsonElement.main.temp.ToString().Replace('.', ',')) - 273.15;

                    try
                    {
                        f.Precipitation = Double.Parse(jsonElement.rain.GetValue("3h").ToString().Replace('.', ','));
                    }
                    catch
                    {
                        f.Precipitation = 0;
                    }

                    break;

                }


            }


            return f;
        }

        public Bitmap GetPrecipitationBitmap(DateTime forTime)
        {
            DateTime updatedTime = new DateTime(forTime.AddMinutes(30).Year, forTime.AddMinutes(30).Month, forTime.AddMinutes(30).Day, forTime.AddMinutes(30).Hour, 0, 0);

            if (updatedTime.Hour % 3 < 2)
            {
                updatedTime = updatedTime.AddHours(-(updatedTime.Hour % 3));
            }
            else
            {
                updatedTime = updatedTime.AddHours(3 - updatedTime.Hour % 3);
            }

            string bitmapName = "JSBitmap" + updatedTime.ToString("yyyy-MM-dd-HH") + ".bmp";
            string bitmapPath = GetPathToDataDirectory(bitmapName);


            if (File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                //Stopwatch st = new Stopwatch();
                //st.Start();

                DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory("json_cache"));
                FileInfo[] files = dI.GetFiles("*.txt");

                Bitmap forBitmap = new Bitmap(728, 528);

                List<Forecast> forecasts = new List<Forecast>();

                int bH = forBitmap.Height;
                int bW = forBitmap.Width;

                //st.Stop();
                //Console.WriteLine("0: " + st.ElapsedMilliseconds / 1000.0);
                //st.Reset();
                //st.Start();

                object lockObj = new object();

                //foreach(FileInfo file in files)
                Parallel.ForEach(files, file =>
                {
                    string JSONText = "";

                    try
                    {
                        using (StreamReader sr = file.OpenText())
                        {
                            JSONText = sr.ReadToEnd();
                        }
                    }
                    catch
                    {
                        return;
                    }

                    Forecast forecast = GetForecastByTime(updatedTime, JSONText);
                    forecast.SetXY(bW, bH);

                    lock (lockObj)
                    {
                        bool dup = false;

                        foreach(Forecast oldForecast in forecasts)
                        {
                            if (forecast.x == oldForecast.x && forecast.y == oldForecast.y)
                            {
                                dup = true;
                                //Console.WriteLine("DUP!");
                                break;
                            }
                        }

                        if(!dup)
                            forecasts.Add(forecast);
                    }

                });

                //st.Stop();

                //Console.WriteLine("1: " + st.ElapsedMilliseconds / 1000.0 + " " + forecasts.Count);

                //st.Reset();
                //st.Start();

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
                                forBitmap.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].GetPrecipitationColor(), forecasts[t.b].GetPrecipitationColor(), forecasts[t.c].GetPrecipitationColor()));
                        }
                    }
                }

                //st.Stop();

                //Console.WriteLine("2: " + st.ElapsedMilliseconds);

                forBitmap.Save(bitmapPath, ImageFormat.Bmp);

                return forBitmap;
            }

        }

        public List<string> GetUrls()
        {
            List<string> jsonUrls = new List<string>();

            string fileName = "JSON_links.txt";
            string filePath = GetPathToDataDirectory(fileName);

            if (File.Exists(filePath))
            {
                using(StreamReader sr = File.OpenText(filePath))
                {
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (String.IsNullOrWhiteSpace(line))
                                continue;

                            jsonUrls.Add(line);
                        }
                    }
                }
            }
            else
            {
                string JSONText = File.ReadAllText(GetPathToDataDirectory("city.list.json"));

                JArray jsCityList = JArray.Parse(JSONText);

                Parallel.ForEach(jsCityList, el =>
                {
                    dynamic jsonElement = JObject.Parse(el.ToString());

                    string sLat = jsonElement.coord.lat;
                    string sLon = jsonElement.coord.lon;

                    double lat = double.Parse(sLat.Replace(".", ","));
                    double lon = double.Parse(sLon.Replace(".", ","));

                    if (lat >= botRight.Lat && lat <= topLeft.Lat && lon >= topLeft.Lon && lon <= botRight.Lon)    //hranice mapy 
                    {
                        //string link = "https://api.openweathermap.org/data/2.5/forecast?lat=" + sLat + "&lon=" + sLon + "&appid=ea63080a4f8e99972630d2671e3ef805";
                        string link = "https://api.openweathermap.org/data/2.5/forecast?id=" + jsonElement.id + "&appid=";

                        jsonUrls.Add(link);
                    }


                });

                using(StreamWriter sw = File.CreateText(filePath))
                {
                    foreach(string link in jsonUrls)
                    {
                        sw.WriteLine(link);
                    }
                }


            }

            return jsonUrls;
        }

        private void SaveJsonToCache()
        {
            int c = 0;

            List<string> keys = new List<string>();
            keys.Add("ea63080a4f8e99972630d2671e3ef805");
            keys.Add("621f630a81701821a6309170b5ec310f");
            keys.Add("58efa6edb4c39e14d88acd636986dec5");
            keys.Add("5c6924655aea1fde4c178d274f5b6afc");

            List<string> locations = GetUrls();
            string JSONText;

            Stopwatch s = new Stopwatch();
            s.Start();

            string timePart = DateTime.Now.ToString("yyyy-MM-dd-HH");

            for(int i = 0; i < locations.Count; )    //nemá smysl používat vlákna když se musí čekat 60s na stažení 60 souborů - 25 minut běh pro celou ČR
            {
                foreach(string key in keys)
                {

                    try
                    {
                        using (var client = new WebClient())
                            JSONText = client.DownloadString(locations[i] + key);
                    }
                    catch
                    {
                        continue;
                    }

                    string fileName = timePart + "-" + i + ".txt";

                    using (StreamWriter sW = File.CreateText(GetPathToDataDirectory(@"json_cache\" + fileName)))
                    {
                        sW.Write(JSONText);
                    }

                    i++;
                }

                c++;

                if(c % 60 == 0)
                {
                    s.Stop();

                    if(s.ElapsedMilliseconds < 60_000)
                    {
                        int ms = 60_000 - (int)s.ElapsedMilliseconds;

                        Thread.Sleep(ms);   //limit 60 stažení za minutu ~ 20 minut pro stažení dat pro celou ČR na týden
                    }

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
            DateTime lastUpdate = DateTime.Now;

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory("json_cache"));
            foreach (var f in dI.GetFiles("*.txt"))
            {
                string onlyDateName = f.Name.Substring(0, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if (dateTime < DateTime.Now.AddDays(-1)) 
                {
                    f.Delete();
                }
                
                lastUpdate = dateTime;
                
            }
            
            bool newCacheCreated = false;
            if(lastUpdate < DateTime.Now.AddDays(-1) || dI.GetFiles("*.txt").Length == 0)
            {
                SaveJsonToCache();
                newCacheCreated = true;
            }


            dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {
                string onlyDateName = f.Name.Substring(8, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if (dateTime < DateTime.Now) //smazání starých bitmap
                {
                    f.Delete();
                }

            }

            if(newCacheCreated)
            {
                LoadAllBitmapsFromCache();
            }
        }

        public List<Forecast> GetAllForecastsFromCache(DateTime now, string JSONtext)
        {
            List<Forecast> forecasts = new List<Forecast>();

            //JSONForecast jF = JsonConvert.DeserializeObject<JSONForecast>(JSONtext);

            dynamic jsonForecast = JObject.Parse(JSONtext);

            JArray jsonForecastArray = (JArray)jsonForecast["list"];

            string lon = jsonForecast.city.coord.lon;
            string lat = jsonForecast.city.coord.lat;

            foreach (var el in jsonForecastArray)
            {
                Forecast f = new Forecast();
                f.Latitude = lat;
                f.Longitude = lon;

                dynamic jsonElement = JObject.Parse(el.ToString());
                DateTime elementTime = DateTime.Parse(jsonElement.dt_txt.ToString());

                if (elementTime < now)
                    continue;

                f.Time = elementTime;

                f.Temperature = Double.Parse(jsonElement.main.temp.ToString().Replace('.', ',')) - 273.15;

                try
                {
                    f.Precipitation = Double.Parse(jsonElement.rain.GetValue("3h").ToString().Replace('.', ','));
                }
                catch
                {
                    f.Precipitation = 0;
                }

                forecasts.Add(f);
            }


            return forecasts;
        }
        public void LoadAllBitmapsFromCache()
        {

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory("json_cache"));
            FileInfo[] files = dI.GetFiles("*.txt");

            DateTime now = DateTime.Now.AddHours(-2);

            List<List<Forecast>> allForecasts = new List<List<Forecast>>();

            int bH = 528;
            int bW = 728;

            object lockObj = new object();

            //foreach(FileInfo file in files)
            Parallel.ForEach(files, file =>
            {
                string JSONText = "";

                try
                {
                    using (StreamReader sr = file.OpenText())
                    {
                        JSONText = sr.ReadToEnd();
                    }
                }
                catch
                {
                    return;
                }

                List<Forecast> newForecasts = GetAllForecastsFromCache(now, JSONText);

                foreach (Forecast f in newForecasts)
                    f.SetXY(bW, bH);

                lock (lockObj)
                {
                    bool dup = false;

                    foreach (List<Forecast> oldForecasts in allForecasts)
                    {
                        if (oldForecasts[0].x == newForecasts[0].x && oldForecasts[0].y == newForecasts[0].y)
                        {
                            dup = true;
                        }

                        if (dup)
                            break;
                    }

                    if (!dup)
                        allForecasts.Add(newForecasts);
                }

            });

            List<List<Forecast>> sortedForecasts = new List<List<Forecast>>();

            for (int j = 0; j < allForecasts[0].Count; j++)
            {
                List<Forecast> forecasts = new List<Forecast>();

                foreach (List<Forecast> pointFullForecast in allForecasts)
                {
                    forecasts.Add(pointFullForecast[j]);
                }

                sortedForecasts.Add(forecasts);
            }

            Parallel.ForEach(sortedForecasts, forecasts => {

                string bitmapName = "JSBitmap" + forecasts[0].Time.ToString("yyyy-MM-dd-HH") + ".bmp";
                string bitmapPath = GetPathToDataDirectory(bitmapName);

                Triangulator angulator = new Triangulator();

                List<Vertex> vertexes = forecasts.ConvertAll(x => (Vertex)x);

                List<Triad> triangles = angulator.Triangulation(vertexes, true);

                Bitmap newBitmap = new Bitmap(bW, bH);

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
                                newBitmap.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].GetPrecipitationColor(), forecasts[t.b].GetPrecipitationColor(), forecasts[t.c].GetPrecipitationColor()));
                        }
                    }
                }

                newBitmap.Save(bitmapPath, ImageFormat.Bmp);

            });

            /*
            for(int j = 0; j < allForecasts[0].Count; j++)
            {
                List<Forecast> forecasts = new List<Forecast>();

                foreach(List<Forecast> pointFullForecast in allForecasts)
                {
                    forecasts.Add(pointFullForecast[j]);
                }
                
                string bitmapName = "JSBitmap" + forecasts[0].Time.ToString("yyyy-MM-dd-HH") + ".bmp";
                string bitmapPath = GetPathToDataDirectory(bitmapName);

                Triangulator angulator = new Triangulator();

                List<Vertex> vertexes = forecasts.ConvertAll(x => (Vertex)x);

                List<Triad> triangles = angulator.Triangulation(vertexes, true);

                forBitmap = new Bitmap(728, 528);

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
                                forBitmap.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].GetPrecipitationColor(), forecasts[t.b].GetPrecipitationColor(), forecasts[t.c].GetPrecipitationColor()));
                        }
                    }
                }

                forBitmap.Save(bitmapPath, ImageFormat.Bmp);
            }*/
        }
    }
}
