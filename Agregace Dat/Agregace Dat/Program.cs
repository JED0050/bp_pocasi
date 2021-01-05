using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            BitmapDataLoader2 bL2 = new BitmapDataLoader2();
            XMLDataLoader xL = new XMLDataLoader();
            JSONDataLoader jL = new JSONDataLoader();

            //jL.GetPrecipitationBitmap(new DateTime(2020, 11, 26, 15, 40, 10));
            //jL.GetPrecipitationBitmap(new DateTime(2020, 11, 5, 15, 40, 10));

            //malá data test 1
            //735 s
            //790 s

            //velké data test 1
            //8308,6 s
            //6548,1 s

            //malá data test 2
            //460 s

            /*
            //DateTime dT = new DateTime(2020, 11, 4, 18, 00, 00);
            DateTime dT = new DateTime(2020, 11, 22, 21, 00, 00);

            Stopwatch s = new Stopwatch();
            s.Start();
            
            for(int i = 0; i < 40; i++)
            {
                jL.GetPrecipitationBitmap(dT);  //8308,667 s

                dT = dT.AddHours(3);
            }

            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds / 1000.0);

            s.Reset();
            s.Start();
            
            jL.LoadAllCache();

            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds / 1000.0);
            */
            /*
            Dictionary<string, int> dic = new Dictionary<string, int>();

            int x2 = 5;

            dic.Add("x", x2);

            int y2;

            dic.TryGetValue("x", out y2);

            Console.WriteLine(y2);
            y2 = 3;

            dic["x"] = y2;

            Console.WriteLine(dic["x"]);

            Console.ReadLine();
            */

            //jL.LoadAllBitmapsFromCache();

            //jL.SaveJsonToCache();

            bL.GetPrecipitationBitmap(DateTime.Now);
        }


    }
}
