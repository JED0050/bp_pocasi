using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;

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

        public Forecast GetForecastByTime(DateTime t, string url)
        {
            Forecast f = new Forecast();
            f.Time = t;

            string JSONtext = GetJSON(url);

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

            if (File.Exists(bitmapName))
            {
                return new Bitmap(bitmapName);
            }
            else
            {
                Bitmap forBitmap = new Bitmap(728, 528);

                List<string> locations = GetUrls();

                foreach (string loc in locations)
                {
                    Forecast f = GetForecastByTime(forTime, loc);

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
                    //DrawIntersectionCircle(10, x, y, forBitmap, Color.Red);
                    DrawIntersectionCircle(5, x, y, forBitmap, f.GetPrecipitationColor());

                }

                forBitmap.Save(bitmapName, ImageFormat.Bmp);

                return forBitmap;
            }

        }

        public List<string> GetUrls()
        {
            List<string> jsonUrls = new List<string>();
            
            if(File.Exists("JSON_links.txt"))
            {
                using(StreamReader sr = File.OpenText("JSON_links.txt"))
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
                string JSONText = File.ReadAllText("city.list.json");

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

                using(StreamWriter sw = File.CreateText("JSON_links.txt"))
                {
                    foreach(string link in jsonUrls)
                    {
                        sw.WriteLine(link);
                    }
                }


            }

            return jsonUrls;
        }
    }
}
