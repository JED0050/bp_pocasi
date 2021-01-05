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
    public class JSONDataLoader : BitmapCustomDraw, DataLoader
    {
        //bounds
        private PointLonLat topLeft = new PointLonLat(10.88, 51.88);
        private PointLonLat botRight = new PointLonLat(20.21, 47.09);
        
        private int bitmapW = 728;
        private int bitmapH = 528;
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
                throw new Exception("Bitmap not found!");
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

            DateTime now = DateTime.Now.AddHours(-2);

            int index;

            if(!File.Exists(GetPathToDataDirectory(@"json_cache\" + "index.txt")))
            {
                index = 0;
            }
            else
            {
                string content = File.ReadAllText(GetPathToDataDirectory(@"json_cache\" + "index.txt"));
                index = int.Parse(content);
            }

            object lockObj = new object();

            Stopwatch s = new Stopwatch();
            s.Start();

            ConcurrentDictionary<string, Bitmap> bitmaps = new ConcurrentDictionary<string, Bitmap>();

            for (int i = index; i < locations.Count; )    //nemá smysl používat vlákna když se musí čekat 60s na stažení 60 souborů
            {
                int inc = 0;

                Parallel.For(0, keys.Count, parInd =>
                {
                    if (i + parInd >= locations.Count)
                    {
                        return;
                    }

                    try
                    {
                        using (var client = new WebClient())
                            JSONText = client.DownloadString(locations[i + parInd] + keys[parInd]);
                    }
                    catch
                    {
                        return;
                    }

                    List<Forecast> onePointForecasts = GetAllForecastsFronJSON(now, JSONText);

                    lock(lockObj)
                    {
                        inc++;

                        foreach (Forecast forecast in onePointForecasts)
                        {
                            string timePart = forecast.Time.ToString("yyyy-MM-dd-HH");
                            string fileName = timePart + "-" + "prec" + ".bmp";
                            string fullName = GetPathToDataDirectory(@"json_cache\" + fileName);

                            Bitmap cacheBmp;

                            if (!bitmaps.TryGetValue(fullName, out cacheBmp))
                            {
                                if (File.Exists(fullName))
                                {
                                    cacheBmp = new Bitmap(fullName);

                                    /*
                                    using (Image img = Image.FromFile(fullName))
                                    {
                                        cacheBmp = (Bitmap)img;
                                    }
                                    */

                                    /*
                                    using (Bitmap bmp = new Bitmap(fullName))
                                    {
                                        cacheBmp = bmp;

                                        bmp.Dispose();
                                        //bmp.Close();
                                    }*/
                                }
                                else
                                {
                                    cacheBmp = new Bitmap(bitmapW, bitmapH);
                                }
                            }

                            Color pixelColor;

                            if (forecast.Precipitation == 0)
                            {
                                pixelColor = Color.FromArgb(255, 0, 0, 1);   //pokud jsou pro daný bod srážky 0 nastavíme černý bod (0,0,1) (rozlišení místa po které neznáme srážky a místa kde jsou srážky 0)
                                //pixelColor = Color.Red;
                            }
                            else
                            {
                                pixelColor = forecast.GetPrecipitationColor();
                                //pixelColor = Color.Red;
                            }

                            //cacheBmp.SetPixel(5,5,Color.Red);
                            cacheBmp.SetPixel((int)forecast.x, (int)forecast.y, pixelColor);

                            bitmaps[fullName] = cacheBmp;

                        }
                    }
                });

                i += inc;

                c++;

                //Console.WriteLine(c + " " + i + " " + s.ElapsedMilliseconds);

                if (c % 8 == 0 || i == locations.Count)
                {
                    foreach (var bmp in bitmaps)
                    {
                        //bmp.Value.Save(bmp.Key, ImageFormat.Bmp);

                        /*
                        using (var ms = new MemoryStream())
                        {
                            Bitmap b = new Bitmap(bmp.Value);
                            b.Save(ms, ImageFormat.Bmp);
                        }
                        */

                        //System.Runtime.InteropServices.ExternalException: A generic error occurred in GDI+.

                        Bitmap b = new Bitmap(bmp.Value);

                        //Console.WriteLine(bmp.Key);

                        b.Save(bmp.Key, ImageFormat.Bmp);
                        //b.Dispose();
                    }

                    File.WriteAllText(GetPathToDataDirectory(@"json_cache\" + "index.txt"), i.ToString());

                    s.Stop();

                    if(s.ElapsedMilliseconds < 60_000)
                    {
                        int ms = 60_000 - (int)s.ElapsedMilliseconds;

                        Thread.Sleep(ms);   //limit 60 stažení za minutu ~ 20 minut pro stažení dat pro celou ČR na týden
                    }

                    s.Reset();
                    s.Start();

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

            DirectoryInfo dICache = new DirectoryInfo(GetPathToDataDirectory("json_cache"));
            foreach (var f in dICache.GetFiles("*.bmp"))
            {
                string onlyDateName = f.Name.Substring(0, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if (dateTime < DateTime.Now.AddDays(-1)) 
                {
                    f.Delete();
                }
                
                if(dateTime < lastUpdate)
                    lastUpdate = dateTime;
                
            }

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {
                string onlyDateName = f.Name.Substring(8, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if (dateTime < DateTime.Now.AddHours(-2)) //smazání starých bitmap
                {
                    f.Delete();
                }

            }

            if (lastUpdate < DateTime.Now.AddDays(-1) || dICache.GetFiles("*.bmp").Length == 0)
            {
                File.WriteAllText(GetPathToDataDirectory(@"json_cache\" + "index.txt"), "0");
                SaveJsonToCache();
                LoadAllBitmapsFromCache();
            }
            else if (File.Exists(GetPathToDataDirectory(@"json_cache\" + "index.txt")))
            {
                int numOfCreatedPoints = int.Parse(File.ReadAllText(GetPathToDataDirectory(@"json_cache\" + "index.txt")));

                if (numOfCreatedPoints < GetUrls().Count)
                {
                    SaveJsonToCache();
                    LoadAllBitmapsFromCache();
                }

            }

        }

        private List<Forecast> GetAllForecastsFronJSON(DateTime now, string JSONtext)
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
                f.SetXY(bitmapW, bitmapH);

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
        private void LoadAllBitmapsFromCache()
        {

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory("json_cache"));
            FileInfo[] files = dI.GetFiles("*.bmp");

            List<List<Forecast>> allForecasts = new List<List<Forecast>>();

            object lockObj = new object();

            //foreach(FileInfo file in files)
            Parallel.ForEach(files, file =>
            {
                List<Forecast> forecasts = new List<Forecast>();
                
                Bitmap cacheBmp = new Bitmap(file.FullName);

                bool hasRain = false;

                for(int x = 0; x < bitmapW; x++)
                {
                    for(int y = 0; y < bitmapH; y++)
                    {
                        if(cacheBmp.GetPixel(x,y).ToArgb() != -16777216)    //-16777216 reprezentuje (255, 0, 0, 0) při načtení bitmapy z disku má alfu vždy na 255
                        {

                            Forecast forecast = new Forecast();

                            forecast.Precipitation = ColorValueHandler.GetPrecipitationValue(cacheBmp.GetPixel(x, y));

                            if (forecast.Precipitation > 0)
                                hasRain = true;

                            forecast.x = x;
                            forecast.y = y;

                            string[] dateParts = file.Name.Split('.')[0].Split('-');

                            forecast.Time = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]), int.Parse(dateParts[3]), 0, 0);

                            forecasts.Add(forecast);
                        }
                    }
                }

                if(!hasRain)    //nemá smysl dělat triangulaci na bitmapě plné prázdných bodů (255, 0, 0, 0) - bez předpovědi a (255, 0, 0, 1) - srážky 0mm
                {
                    forecasts.Clear();
                }

                if(forecasts.Count == 0)
                {
                    Forecast forecast = new Forecast();

                    string[] dateParts = file.Name.Split('.')[0].Split('-');

                    forecast.Time = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]), int.Parse(dateParts[3]), 0, 0);

                    forecasts.Add(forecast);
                }

                //Console.WriteLine(forecasts.Count);

                allForecasts.Add(forecasts);

            });

            Parallel.ForEach(allForecasts, forecasts => {

                string bitmapName = "JSBitmap" + forecasts[0].Time.ToString("yyyy-MM-dd-HH") + ".bmp";
                string bitmapPath = GetPathToDataDirectory(bitmapName);

                Bitmap newBitmap = new Bitmap(bitmapW, bitmapH);

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
                                    newBitmap.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].GetPrecipitationColor(), forecasts[t.b].GetPrecipitationColor(), forecasts[t.c].GetPrecipitationColor()));
                            }
                        }
                    }
                }

                newBitmap.Save(bitmapPath, ImageFormat.Bmp);

            });
        }
    }
}
