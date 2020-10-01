using System;
using System.Drawing;
using System.IO;
using AgregaceDatLib;

namespace Agregace_Dat
{
    class Program
    {
        static void Main(string[] args)
        {

            BitmapDataLoader bL = new BitmapDataLoader();
            //bL.GetForecastBitmap(DateTime.Now);
            //Console.WriteLine(bL.GetForecastBitmap(DateTime.Now).GetPixel(100,100));

            XMLDataLoader xL = new XMLDataLoader();
            //xL.GetForecastBitmap(DateTime.Now);

            JSONDataLoader jL = new JSONDataLoader();
            //jL.GetForecastBitmap(DateTime.Now);
            
            AvgForecast aF = new AvgForecast();
            aF.Add(bL);
            aF.Add(xL);
            aF.Add(jL);

            //aF.GetAvgBitmap(DateTime.Now);

            Console.WriteLine("Srážky právě teď v Praze");  
            Console.WriteLine(aF.GetForecast(DateTime.Now, 50.0598058, 14.325542).Precipitation);
        }
    }
}
