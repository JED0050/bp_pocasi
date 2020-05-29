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

            string lat = "50";
            string lon = "150";
            string apiKey = "ea63080a4f8e99972630d2671e3ef805";

            JSONDataLoader jL = new JSONDataLoader("https://api.openweathermap.org/data/2.5/forecast?lat=" + lat + "&lon=" + lon + "&appid=" + apiKey);

            Forecast jF = jL.GetForecastByTime(DateTime.Now);

            Console.WriteLine("Teplota na souřadnicích lat={0}, lon={1} v tuto chvíli", lat, lon);
            Console.WriteLine("Teplota: " + jF.Temperature);
            Console.WriteLine("Srážky na souřadnicích lat={0}, lon={1} v tuto chvíli", lat, lon);
            Console.WriteLine("Sráž: " + jF.Precipitation);



        }
    }
}
