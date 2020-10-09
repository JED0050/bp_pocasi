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
            XMLDataLoader xL = new XMLDataLoader();
            JSONDataLoader jL = new JSONDataLoader();
            
            AvgForecast aF = new AvgForecast();
            aF.Add(bL);
            aF.Add(xL);
            aF.Add(jL);

            Console.WriteLine("Srážky v Praze");  
            DateTime dT = new DateTime(2020,10,10,12,20,20);
            Console.WriteLine(aF.GetForecast(dT, 50.0680297, 12.8445248).Precipitation);
        }
    }
}
