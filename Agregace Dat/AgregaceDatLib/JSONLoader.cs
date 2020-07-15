using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace AgregaceDatLib
{
    public class JSONDataLoader : DataLoader
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
            /*
            try
            {
                return new Bitmap(@"JSBitmap" + forTime.ToString("yyyyMMddHH") + ".bmp");
            }
            catch { }
            */

            Bitmap forBitmap = new Bitmap(728, 528);

            List<string> locations = new List<string>();
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=49.5952379&lon=17.2094959&appid=ea63080a4f8e99972630d2671e3ef805");     //Olomouc
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=50.0598058&lon=14.325542&appid=ea63080a4f8e99972630d2671e3ef805");      //Praha
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=49.2021611&lon=16.5079211&appid=ea63080a4f8e99972630d2671e3ef805");     //Brno
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=49.8198311&lon=18.1673551&appid=ea63080a4f8e99972630d2671e3ef805");     //Ostrava
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=49.741787&lon=13.3018839&appid=ea63080a4f8e99972630d2671e3ef805");      //Plzeň
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=50.2140083&lon=15.7711033&appid=ea63080a4f8e99972630d2671e3ef805");     //Hradec Králové
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=50.2169842&lon=12.7942751&appid=ea63080a4f8e99972630d2671e3ef805");     //Karlovy Vary
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=50.7662021&lon=14.9796305&appid=ea63080a4f8e99972630d2671e3ef805");     //Liberec
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=50.0347806&lon=15.688131&appid=ea63080a4f8e99972630d2671e3ef805");      //Pardubice
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=50.651363&lon=13.9713279&appid=ea63080a4f8e99972630d2671e3ef805");      //Ústí nad Labem
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=49.4045045&lon=15.5105797&appid=ea63080a4f8e99972630d2671e3ef805");     //Jihlava
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=49.2311334&lon=17.6064674&appid=ea63080a4f8e99972630d2671e3ef805");     //Zlín
            locations.Add("https://api.openweathermap.org/data/2.5/forecast?lat=48.9764035&lon=14.4555279&appid=ea63080a4f8e99972630d2671e3ef805");     //České Budějovice


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

                for (double r = 1; r <= 30; r += 1)
                {
                    double rr = Math.Pow(r, 2);

                    for (int i = x - (int)r; i <= x + r; i++)
                        for (int j = y - (int)r; j <= y + r; j++)
                            if (Math.Abs(Math.Pow(i - x, 2) + Math.Pow(j - y, 2) - rr) <= r)
                                forBitmap.SetPixel(i, j, f.GetPrecipitationColor());
                }
                
            }

            //forBitmap.Save("JSBitmap" + forTime.ToString("yyyyMMddHH") + ".bmp", ImageFormat.Bmp);

            return forBitmap;
        }
    }
}
