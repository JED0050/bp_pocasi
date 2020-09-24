using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            if(File.Exists(bitmapName))
            {
                return new Bitmap(bitmapName);
            }
            else
            {
                Bitmap forBitmap = new Bitmap(728, 528);

                List<string> locations = GetUrls();

                using (var client = new WebClient())
                {

                    //Parallel.ForEach(locations, loc =>
                    foreach (string loc in locations)
                    {
                        string xmlText = "";

                        try
                        {
                            xmlText = client.DownloadString(loc);
                        }
                        catch { }

                        Forecast f = GetForecastByTime(forTime, xmlText);

                        if (f.Temperature == -1)    //neplatná předpověď, webová služba vrátila error místo XML dat
                        {
                            //Console.WriteLine("N " + loc);
                            //return;
                            continue;
                        }/*
                    else
                    {
                        Console.WriteLine("A " + loc);
                    }*/

                        double lonDif = 20.21 - 10.06;
                        double latDif = 51.88 - 47.09;

                        double PixelLon = lonDif / forBitmap.Width;
                        double PixelLat = latDif / forBitmap.Height;

                        double bY = 51.88;
                        double bX = 10.06;

                        double locLon = f.DLongitude;
                        double locLat = f.DLatitude;

                        int x;
                        for (x = 0; x < forBitmap.Width; x++)
                        {
                            if (bX >= locLon && locLon <= bX + locLon)
                                break;

                            bX += PixelLon;
                        }

                        int y;
                        for (y = 0; y < forBitmap.Height; y++)
                        {
                            if (bY - PixelLat <= locLat && locLat <= bY)
                                break;

                            bY -= PixelLat;
                        }

                        //tmpB.SetPixel(x, y, f.GetPrecipitationColor());

                        DrawIntersectionCircle(10, x, y, forBitmap, f.GetPrecipitationColor());
                        //DrawIntersectionCircle(10, x, y, forBitmap, Color.Red);

                    }//);
                }

                forBitmap.Save(bitmapName, ImageFormat.Bmp);

                return forBitmap;
            }

        }

        private List<string> GetUrls()
        {
            string allUrls = "http://fil.nrk.no/yr/viktigestader/verda.txt";
            string fileName = "XML_Links.txt";
            List<string> czechForecast = new List<string>();

            if (File.Exists(fileName))
            {
                using (StreamReader sr = File.OpenText(fileName))
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

                        if (url.Length > 10)
                        {
                            czechForecast.Add(url);
                        }

                    }
                    catch
                    {
                        return;
                    }
                });

                using (StreamWriter sw = File.CreateText(fileName))
                {
                    foreach (string line in czechForecast)
                        sw.WriteLine(line);
                }
            }

            return czechForecast;
        }
    }

}
