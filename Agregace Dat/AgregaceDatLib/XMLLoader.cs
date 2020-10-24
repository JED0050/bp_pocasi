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


        public Bitmap GetForecastBitmap(DateTime forTime)
        {
            string bitmapName = "XMLBitmap" + forTime.ToString("yyyyMMddHH") + ".bmp";
            string bitmapPath = GetPathToDataDirectory(bitmapName);

            if(File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                Bitmap forBitmap = new Bitmap(728, 528);

                List<string> locations = GetUrls();

                List<Forecast> forecasts = new List<Forecast>();

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

                    Forecast f = GetForecastByTime(forTime, xmlText);

                    if (f.Temperature == -1)    //neplatná předpověď, webová služba vrátila error místo XML dat
                    {
                        return;
                        //continue;
                    }

                    forecasts.Add(f);

                });

                foreach(Forecast f in forecasts)
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
                        string url = line.Substring(line.IndexOf("http://www.yr.no/place/Czech_Republic"));

                        if (url.Length > 30)
                        {
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
    }

}
