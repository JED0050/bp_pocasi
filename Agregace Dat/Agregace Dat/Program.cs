using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
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
            /*
            AvgForecast aF = new AvgForecast();
            aF.Add(bL);
            aF.Add(xL);
            aF.Add(jL);
            
            Console.WriteLine("Srážky v Praze");
            DateTime dT = new DateTime(2020,10,10,12,20,20);
            Console.WriteLine(aF.GetForecast(dT, 50.0680297, 12.8445248).Precipitation);
            */

            BitmapDataLoader2 bL2 = new BitmapDataLoader2();
            //bL2.SaveNewDeleteOldBmps(7);
            
            //Bitmap bmp = bL2.GetPrecipitationBitmap(DateTime.Now, new PointLonLat(10.88,51.88), new PointLonLat(20.21,47.09));
            //bmp.Save("t.bmp", ImageFormat.Bmp);
        }


    }
}
