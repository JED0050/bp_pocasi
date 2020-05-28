using System;
using AgregaceDatLib;

namespace Agregace_Dat
{
    class Program
    {
        static void Main(string[] args)
        {

            XMLDataLoader loader = new XMLDataLoader("http://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml");

            //string xmlFile = loader.GetXML();
            //Console.WriteLine(xmlFile);

            Console.WriteLine("Teplota v Praze v tuto chvíli");
            Console.WriteLine("Teplota: " + loader.GetForecastByTimeInPrague(DateTime.Now).Temperature);


            Console.WriteLine("Teplota v Praze za 2 dny v 7:15");
            DateTime tommorow = DateTime.Now.AddDays(2);
            TimeSpan hours = new TimeSpan(7, 15, 0);
            tommorow = tommorow.Date + hours;
            Console.WriteLine("Teplota: " + loader.GetForecastByTimeInPrague(tommorow).Temperature);


        }
    }
}
