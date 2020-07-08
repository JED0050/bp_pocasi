﻿using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class BitmapDataLoader : DataLoader
    {
        public string UrlAdress { get; set; }

        public double PixelLon { get; set; }
        public double PixelLat { get; set; }

        public BitmapDataLoader(string url)
        {
            UrlAdress = url;
        }

        public double GetPrecipitationFromPixel(Color pixel)
        {
            double precipitation = 0;

            /*
            srážky dle RGB barev

            index - R G B - srážky

            0 - 0 0 0 - 0 mm/h
            4 - 56 0 112 - 0 mm/h

            8 - 48 0 168 - 0.1 mm/h
            12 - 0 0 252 - 0.2 mm/h
            16 - 0 108 192 - 0.4 mm/h
            20 - 0 160 0 - 0.8 mm/h

            24 - 0 188 0 - 1 mm/h
            28 - 52 216 0 - 2 mm/h
            32 - 156 220 0 - 4 mm/h
            36 - 224 220 0 - 8 mm/h

            40 - 252 176 0 - 10 mm/h
            44 - 252 132 0 - 30 mm/h
            48 - 252 88 0 - 60 mm/h

            52 - 252 0 0 - 100 mm/h
            56 - 160 0 0 - 100+ mm/h
            60 - 255 255 255 - 100+ mm/h

            */

            if (ColInRange(0, 56, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(0, 167, pixel.B))
                precipitation = 0;
            else if (ColInRange(0, 48, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(168, 251, pixel.B))
                precipitation = 0.1;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(0, 107, pixel.G) && ColInRange(193, 252, pixel.B))
                precipitation = 0.2;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(108, 159, pixel.G) && ColInRange(1, 192, pixel.B))
                precipitation = 0.4;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(160, 187, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 0.8;
            else if (ColInRange(0, 51, pixel.R) && ColInRange(188, 215, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 1;
            else if (ColInRange(52, 155, pixel.R) && ColInRange(216, 219, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 2;
            else if (ColInRange(156, 223, pixel.R) && ColInRange(220, 220, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 4;
            else if (ColInRange(224, 251, pixel.R) && ColInRange(177, 220, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 8;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(133, 176, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 10;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(89, 132, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 30;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(1, 88, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 60;
            else if (ColInRange(150, 255, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 100;

            return precipitation;
        }

        private bool ColInRange(int min, int max, int val)
        {
            return (min <= val) && (val <= max);
        }

        private void SetPixelCoord(int w, int h)
        {
            //levý horní roh, 51.88 10.06
            //lavý doní roh, 47.09 10.6
            //pravý horní roh, 51.88 20.21
            //pravý doní roh, 47.09 20.21

            double lonDif = 20.21 - 10.06;
            double latDif = 51.88 - 47.09;


            PixelLon = lonDif / w;
            PixelLat = latDif / h;

        }

        public Bitmap GetBitmap()
        {

            Bitmap bitmap;

            using (WebClient client = new WebClient())
            {
                Stream stream = client.OpenRead(UrlAdress);
                bitmap = new Bitmap(stream);

                //stream.Flush();
                //stream.Close();
                //client.Dispose();
            }

            return bitmap;

        }

        public Forecast GetForecastByTime(DateTime t)
        {
            Bitmap bitmap = GetBitmap();

            SetPixelCoord(bitmap.Width, bitmap.Height);

            Forecast f = new Forecast();
            f.Time = t;

            //Praha
            double targetLat = 50.5;
            double targetLon = 14.25;

            //pravý horní roh bitmapy
            double bY = 51.88;
            double bX = 10.06;

            for (int y = 0; y < bitmap.Height; y++)
            {
                bX = 10.06;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bX <= targetLon && targetLon <= bX + PixelLon && bY - PixelLat <= targetLat && targetLat <= bY)
                    {
                        /*
                        Console.WriteLine("Bingo!");
                        Console.WriteLine("cyklus x: {0} y: {1}", x, y);
                        Console.WriteLine("bitmapa x: {0} y: {1}", bX, bY);
                        Console.WriteLine("praha x: {0} y: {1}", targetLon, targetLat);
                        */

                        f.Precipitation = GetPrecipitationFromPixel(bitmap.GetPixel(x, y));
                    }

                    bX += PixelLon;
                }

                bY -= PixelLat;

            }

            return f;
        }
    }
}