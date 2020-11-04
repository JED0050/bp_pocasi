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

            f.City = jsonForecast.city.name;
            f.Country = jsonForecast.city.country;
            f.Latitude = jsonForecast.city.coord.lat;
            f.Longitude = jsonForecast.city.coord.lon;

            JArray jsonForecastArray = (JArray)jsonForecast["list"];

            int counter = 0;

            /*
            //zaokrouhlení na celou hodinu
            t.AddMinutes(30);
            t = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0);
            */

            foreach (var el in jsonForecastArray)
            {
                dynamic jsonElement = JObject.Parse(el.ToString());

                if (counter == 0)
                {
                    f.Temperature = Double.Parse(jsonElement.main.temp.ToString().Replace('.', ',')) - 273.15;  //Kelvin na celsius

                    try
                    {
                        f.Precipitation = Double.Parse(jsonElement.rain.GetValue("3h").ToString().Replace('.', ','));   //jsonElement.main.rain.3h ?
                    }
                    catch
                    {
                        f.Precipitation = 0;
                    }

                    counter++;
                    continue;
                }

                DateTime from = DateTime.Parse(jsonElement.dt_txt.ToString());
                DateTime to = from.AddHours(3); //předpověď v 3 hodinovém okně

                if (t >= from && t <= to)
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
            DateTime updatedTime = forTime;

            if (updatedTime.Hour % 3 < 1)
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
                DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory("json_cache"));
                FileInfo[] files = dI.GetFiles("*.txt");

                Bitmap forBitmap = new Bitmap(728, 528);

                List<Forecast> forecasts = new List<Forecast>();

                Parallel.ForEach(files, file =>
                {
                    string JSONText = "";

                    try
                    {
                        using (StreamReader sr = file.OpenText())
                        {
                            string s = "";
                            while ((s = sr.ReadLine()) != null)
                            {
                                JSONText += s;
                            }
                        }
                    }
                    catch
                    {
                        return;
                    }

                    if (JSONText == "")
                        return;

                    Forecast f = GetForecastByTime(updatedTime, JSONText);

                    forecasts.Add(f);
                });

                foreach (Forecast f in forecasts)
                    f.SetXY(forBitmap);

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
                                forBitmap.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].GetPrecipitationColor(), forecasts[t.b].GetPrecipitationColor(), forecasts[t.c].GetPrecipitationColor()));
                        }
                    }
                }

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

                    if (jsonElement.country.ToString() == "CZ")
                    {
                        string lat = jsonElement.coord.lat;
                        string lon = jsonElement.coord.lon;

                        string link = "https://api.openweathermap.org/data/2.5/forecast?lat=" + lat + "&lon=" + lon + "&appid=ea63080a4f8e99972630d2671e3ef805";

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

            List<string> locations = GetUrls();
            string JSONText;

            Stopwatch s = new Stopwatch();
            s.Start();

            foreach(string loc in locations)    //nemá smysl používat vlákna když se musí čekat 60s na stažení 60 souborů
            {
                try
                {
                    using (var client = new WebClient())
                        JSONText = client.DownloadString(loc);
                }
                catch
                {
                    continue;
                }

                string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH") + "-" + c + ".txt";

                using (StreamWriter sW = File.CreateText(GetPathToDataDirectory(@"json_cache\" + fileName)))
                {
                    sW.Write(JSONText);
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

        public void SaveNewDeleteOldBmps(int days)
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

            if(lastUpdate < DateTime.Now.AddDays(-1) || dI.GetFiles("*.txt").Length == 0)
            {
                SaveJsonToCache();
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
                else if (dateTime > DateTime.Now.AddDays(1))  //přemazání bitmap
                {
                    f.Delete();
                }

            }

            
            int hours = days * 24;

            DateTime now = DateTime.Now;

            for (int i = 0; i < hours; i++)
            {
                DateTime time = now.AddHours(i);

                if (time.Hour % 3 == 0)
                {
                    GetPrecipitationBitmap(time);
                }

            }
            
        }
    }
}
