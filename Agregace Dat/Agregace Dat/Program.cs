using System;
using AgregaceDatLib;

namespace Agregace_Dat
{
    class Program
    {
        static void Main(string[] args)
        {
            //XML

            XMLDataLoader xL = new XMLDataLoader("http://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml");

            //string xmlFile = loader.GetXML();
            //Console.WriteLine(xmlFile);

            Console.WriteLine("Teplota v Praze v tuto chvíli");
            Console.WriteLine("Teplota: " + xL.GetForecastByTime(DateTime.Now).Temperature);


            Console.WriteLine("Teplota v Praze za 2 dny v 7:15");
            DateTime tommorow = DateTime.Now.AddDays(2);
            TimeSpan hours = new TimeSpan(7, 15, 0);
            tommorow = tommorow.Date + hours;
            Console.WriteLine("Teplota: " + xL.GetForecastByTime(tommorow).Temperature);


            //JSON

            string lat = "50.5";   //Praha = DMS latitude longitude coordinates for Prague are: 50°5'16.94"N, 14°25'14.74"E.
            string lon = "14.25";
            string apiKey = "ea63080a4f8e99972630d2671e3ef805";

            JSONDataLoader jL = new JSONDataLoader("https://api.openweathermap.org/data/2.5/forecast?lat=" + lat + "&lon=" + lon + "&appid=" + apiKey);

            Forecast jF = jL.GetForecastByTime(DateTime.Now);

            Console.WriteLine("Teplota na souřadnicích lat={0}, lon={1} v tuto chvíli", lat, lon);
            Console.WriteLine("Teplota: " + jF.Temperature);
            Console.WriteLine("Srážky na souřadnicích lat={0}, lon={1} v tuto chvíli", lat, lon);
            Console.WriteLine("Srážky: " + jF.Precipitation);


            //BITMAP

            DateTime now = DateTime.Now;

            BitmapDataLoader bL = new BitmapDataLoader("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + now.Year + now.ToString("MM") + now.ToString("dd") + ".1020.0.png");   //728 x 528

            Console.WriteLine("Srážky dnes v Praze dle vybraného radaru v 10:20");
            Console.WriteLine("Srážky: " + bL.GetForecastByTime(DateTime.Now).Precipitation);




            Console.WriteLine();
            Console.WriteLine();

            //Třída na agregaci dat
            //Zjištění zjištění průměrných srážek dle 3 různých datových zdrojů
            //Místo Praha, čas teď

            AvgForecast forecastFactory = new AvgForecast();

            forecastFactory.Add(new XMLDataLoader("http://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml"));
            forecastFactory.Add(new JSONDataLoader("https://api.openweathermap.org/data/2.5/forecast?lat=" + lat + "&lon=" + lon + "&appid=" + apiKey));
            forecastFactory.Add(new BitmapDataLoader("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + now.Year + now.ToString("MM") + now.ToString("dd") + ".1020.0.png"));

            Console.WriteLine("Průměrné srážky v tuto chvíli v Praze: " + forecastFactory.GetForecast(DateTime.Now, "-").Precipitation);

        }
    }
}
