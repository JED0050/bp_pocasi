using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DelaunayTriangulator;

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

        public Bitmap GetForecastBitmap(DateTime forTime)
        {
            string bitmapName = "JSBitmap" + forTime.ToString("yyyyMMddHH") + ".bmp";
            string bitmapPath = GetPathToDataDirectory(bitmapName);


            if (File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                Bitmap forBitmap = new Bitmap(728, 528);

                List<string> locations = GetUrls();
                List<Forecast> forecasts = new List<Forecast>();

                Parallel.ForEach(locations, loc =>
                {
                    string JSONText;

                    try
                    {
                        using (var client = new WebClient())
                            JSONText = client.DownloadString(loc);
                    }
                    catch
                    {
                        return;
                    }

                    Forecast f = GetForecastByTime(forTime, JSONText);

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

        private string GetPathToDataDirectory(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;

            return Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Data\Openweathermap\" + fileName;
        }
    }
}
