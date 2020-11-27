using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using DelaunayTriangulator;

namespace AgregaceDatLib
{

    public class XMLDataLoader : BitmapHelper, DataLoader
    {
        //bounds
        private PointLonLat topLeft = new PointLonLat(10.88, 51.88);
        private PointLonLat botRight = new PointLonLat(20.21, 47.09);

        public XMLDataLoader()
        {
            if (!Directory.Exists(GetPathToDataDirectory("")))
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
            }
        }
        public Forecast GetForecastByTime(DateTime t, string xmlText)
        {
            Forecast f = new Forecast();
            f.Time = t;

            if (xmlText == "")
            {
                f.Temperature = -1;
                return f;
            }

            TextReader tr = new StringReader(xmlText);

            XDocument xmlDoc = XDocument.Load(tr);


            foreach (var weatherData in xmlDoc.Descendants("weatherdata"))
            {
                int counter = 0;

                foreach (var location in xmlDoc.Descendants("location"))
                {
                    if (counter == 0)
                    {
                        f.Country = location.Element("country").Value;
                        f.City = location.Element("name").Value;

                        counter++;
                    } else
                    {

                        f.Latitude = location.Attribute("latitude").Value;
                        f.Longitude = location.Attribute("longitude").Value;
                    }
                }

                counter = 0;

                foreach (var forecast in xmlDoc.Descendants("forecast"))
                {
                    foreach (var tabular in xmlDoc.Descendants("tabular"))
                    {
                        foreach (var time in xmlDoc.Descendants("time"))
                        {
                            if (counter == 0)    //nastavení první hodnoty pokud není žádná jiná známá
                            {
                                counter++;

                                f.Temperature = Double.Parse(time.Element("temperature").Attribute("value").Value.Replace('.', ','));
                                f.Precipitation = Double.Parse(time.Element("precipitation").Attribute("value").Value.Replace('.', ','));

                                continue;
                            }

                            DateTime from = DateTime.Parse(time.Attribute("from").Value);
                            DateTime to = DateTime.Parse(time.Attribute("to").Value);

                            if (t >= from && t <= to)
                            {
                                f.Temperature = Double.Parse(time.Element("temperature").Attribute("value").Value.Replace('.', ','));
                                f.Precipitation = Double.Parse(time.Element("precipitation").Attribute("value").Value.Replace('.', ','));

                                break;
                            }

                        }
                    }
                }
            }


            return f;
        }


        public Bitmap GetPrecipitationBitmap(DateTime forTime)
        {
            DateTime updatedTime = forTime;

            if (updatedTime.Hour % 6 < 3)
            {
                updatedTime = updatedTime.AddHours(-(updatedTime.Hour % 6));
            }
            else
            {
                updatedTime = updatedTime.AddHours(6 - updatedTime.Hour % 6);
            }

            string bitmapName = "XMLBitmap" + updatedTime.ToString("yyyy-MM-dd-HH") + ".bmp";
            string bitmapPath = GetPathToDataDirectory(bitmapName);

            if(File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                Bitmap forBitmap = new Bitmap(728, 528);
                int bW = forBitmap.Width;
                int bH = forBitmap.Height;

                List<string> locations = GetUrls();

                List<Forecast> forecasts = new List<Forecast>();

                object lockObj = new object();

                Parallel.ForEach(locations, loc =>
                //foreach(string loc in locations)    
                {
                    string xmlText = "";

                    try
                    {
                        using (var client = new WebClient())    //volání download stringu na jedné instanci a její zamykání je pomalejší než vytváření spousty instancí
                            xmlText = client.DownloadString(loc);

                    }
                    catch
                    {
                        return;
                        //continue;
                    }

                    Forecast f = GetForecastByTime(updatedTime, xmlText);

                    if (f.Temperature == -1)    //neplatná předpověď, webová služba vrátila error místo XML dat
                    {
                        return;
                        //continue;
                    }

                    f.SetXY(bW, bH);

                    lock (lockObj)
                    {
                        forecasts.Add(f);
                    }

                });

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

                forBitmap.Save(bitmapPath, ImageFormat.Bmp);

                return forBitmap;
            }

        }

        private List<string> GetUrls()
        {
            string allUrls = "http://fil.nrk.no/yr/viktigestader/verda.txt";

            string fileName = "XML_Links.txt";
            string filePath = GetPathToDataDirectory(fileName);

            List<string> czechForecast = new List<string>();

            if (File.Exists(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (String.IsNullOrWhiteSpace(line))
                            continue;

                        czechForecast.Add(line);
                    }
                }
            }
            else
            {
                string webContent = "";

                using (var client = new WebClient())
                {
                    webContent = client.DownloadString(allUrls);
                }

                string[] lines = webContent.Split("\r\n");

                Parallel.ForEach(lines, (line, state) =>
                {

                    try
                    {
                        string[] lineParts = line.Split('\t');

                        //index 12 - latitude
                        //index 13 - longitude
                        //index 15-17 - web url (17 - english url)

                        double lat = double.Parse(lineParts[12].Replace(".", ","));
                        double lon = double.Parse(lineParts[13].Replace(".", ","));


                        if (lat >= botRight.Lat && lat <= topLeft.Lat && lon >= topLeft.Lon && lon <= botRight.Lon)    //hranice mapy 
                        {
                            string url = lineParts[17];

                            czechForecast.Add(url);
                        }

                    }
                    catch
                    {
                        return;
                    }
                });

                using (StreamWriter sw = File.CreateText(filePath))
                {
                    foreach (string line in czechForecast)
                        sw.WriteLine(line);
                }
            }

            return czechForecast;
        }

        private string GetPathToDataDirectory(string fileName)
        {
            //string workingDirectory = Environment.CurrentDirectory;
            //return Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Data\Yr.no\" + fileName;

            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\Yr.no\" + fileName;
        }

        public void SaveNewDeleteOldBmps()  //8 dnů +-
        {

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {
                
                string onlyDateName = f.Name.Substring(9, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if(dateTime < DateTime.Now) //smazání starých bitmap
                {
                    f.Delete();
                }
            }

            LoadAllBitmapsFromUrls();

        }

        public List<Forecast> GetAllForecastsFromUrl(DateTime t, string xmlText)
        {
            List<Forecast> forecasts = new List<Forecast>();

            if (xmlText == "")
            {
                return forecasts;
            }

            TextReader tr = new StringReader(xmlText);

            XDocument xmlDoc = XDocument.Load(tr);

            foreach (var weatherData in xmlDoc.Descendants("weatherdata"))
            {
                int counter = 0;

                string lon = "";
                string lat = "";

                
                foreach (var location in xmlDoc.Descendants("location"))
                {
                    if (counter == 0)
                    {
                        //f.Country = location.Element("country").Value;
                        //f.City = location.Element("name").Value;

                        counter++;
                    }
                    else
                    {

                        lat = location.Attribute("latitude").Value;
                        lon = location.Attribute("longitude").Value;

                        break;
                    }
                }
                

                foreach (var forecast in xmlDoc.Descendants("forecast"))
                {
                    foreach (var tabular in xmlDoc.Descendants("tabular"))
                    {
                        foreach (var time in xmlDoc.Descendants("time"))
                        {

                            DateTime actTime = DateTime.Parse(time.Attribute("from").Value);

                            if (actTime < t)
                            {
                                continue;
                            }

                            Forecast f = new Forecast();

                            f.Longitude = lon;
                            f.Latitude = lat;
                            f.Time = actTime;
                            f.Temperature = Double.Parse(time.Element("temperature").Attribute("value").Value.Replace('.', ','));
                            f.Precipitation = Double.Parse(time.Element("precipitation").Attribute("value").Value.Replace('.', ','));

                            forecasts.Add(f);


                        }
                    }
                }
            }

            return forecasts;
        }

        public void LoadAllBitmapsFromUrls()
        {
            DateTime now = DateTime.Now.AddHours(-2);

            int bW = 728;
            int bH = 528;

            List<string> locations = GetUrls();

            List<List<Forecast>> allForecasts = new List<List<Forecast>>();

            object lockObj = new object();

            Parallel.ForEach(locations, loc =>
            //foreach(string loc in locations)    
            {
                string xmlText = "";

                try
                {
                    using (var client = new WebClient())    //volání download stringu na jedné instanci a její zamykání je pomalejší než vytváření spousty instancí
                        xmlText = client.DownloadString(loc);

                }
                catch
                {
                    return;
                    //continue;
                }

                List<Forecast> newForecasts = GetAllForecastsFromUrl(now, xmlText);

                foreach (Forecast f in newForecasts)
                    f.SetXY(bW, bH);

                if (newForecasts.Count == 0)    //neplatná předpověď, webová služba vrátila error místo XML dat
                {
                    return;
                    //continue;
                }


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

                string bitmapName = "XMLBitmap" + forecasts[0].Time.ToString("yyyy-MM-dd-HH") + ".bmp";
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

        }
    }
}
