using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using AgregaceDatLib;
using DLRadarBourkyLib;
using IDataLoaderAndHandlerLib.HandlersAndObjects;
using IDataLoaderAndHandlerLib.Interface;
using DLWeatherUnlockedLib;
using System.Linq;
using IDataLoaderAndHandlerLib.DelaunayTriangulator;
using System.IO;
using DLYrNoLib;
using DLMedardLib;
using DLOpenWeatherMapLib;
using System.Drawing.Drawing2D;

namespace TestConsoleApk
{
    class Program
    {

        static void Main(string[] args)
        {
            //ColorValueHandler.GetColorForValueAndType(5,"prec");
            //ColorValueHandler.GetColorForValueAndType(8, "prec");
            //ColorValueHandler.GetColorForValueAndType(7, "humi");
            //AvgForecast a = new AvgForecast("");

            //new OpenWeatherMapDataLoader().GetForecastBitmap(DateTime.Now, ForecastTypes.TEMPERATURE);

            //new MedardDataLoader().SaveNewDeleteOldBmps();
            new RadarBourkyDataLoader().SaveNewDeleteOldBmps();

            /*
            Bounds LoaderBounds = new Bounds() { TopLeftCorner = new PointLonLat(14.7432875, 50.23636), BotRightCorner = new PointLonLat(18.3313879, 48.7553031) };

            PointLonLat topLeft = LoaderBounds.TopLeftCorner;
            PointLonLat botRight = LoaderBounds.BotRightCorner;

            Bitmap dataBitmap = new Bitmap(728, 528);

            for (int x = 0; x < dataBitmap.Width; x++)
            {
                for (int y = 0; y < dataBitmap.Height; y++)
                {

                    if(y < 100)
                        dataBitmap.SetPixel(x, y, Color.Red);
                    else if (y < 300 && x < 300)
                        dataBitmap.SetPixel(x, y, Color.Yellow);
                    else if (y < 300 && x >= 300)
                        dataBitmap.SetPixel(x, y, Color.Blue);
                    else if (y < 500)
                        dataBitmap.SetPixel(x, y, Color.Green);
                    else if (y < 700)
                        dataBitmap.SetPixel(x, y, Color.White);
                    else
                        dataBitmap.SetPixel(x, y, Color.Blue);
                }
            }

            Bitmap newBitmap = HandleDiffBpms(new Bitmap("n3.bmp"), new Bounds(topLeft, botRight));

            newBitmap.Save("bmpTest02.bmp", ImageFormat.Bmp);
            
            */
        }

        public static Bitmap HandleDiffBpms(Bitmap dataBitmap, Bounds loaderBounds)
        {
            Bounds DefaultBounds = new Bounds() { TopLeftCorner = new PointLonLat(11.3151977, 51.1357528), BotRightCorner = new PointLonLat(25.9167339, 46.4158868) };
            //DefaultBounds = new Bounds() { TopLeftCorner = new PointLonLat(11, 51), BotRightCorner = new PointLonLat(16, 46) };
            //DefaultBounds = new Bounds() { TopLeftCorner = new PointLonLat(15, 50), BotRightCorner = new PointLonLat(18.5, 49) };
            //DefaultBounds = new Bounds() { TopLeftCorner = new PointLonLat(11, 51), BotRightCorner = new PointLonLat(16, 46) };
            DefaultBounds = new Bounds() { TopLeftCorner = new PointLonLat(16, 51.1357528), BotRightCorner = new PointLonLat(25.9167339, 46.4158868) };

            if (loaderBounds.TopLeftCorner.Lon <= DefaultBounds.TopLeftCorner.Lon && loaderBounds.TopLeftCorner.Lat >= DefaultBounds.TopLeftCorner.Lat && loaderBounds.BotRightCorner.Lon >= DefaultBounds.TopLeftCorner.Lon && loaderBounds.BotRightCorner.Lat <= DefaultBounds.TopLeftCorner.Lat)
            {
                //Bitmap bigBitmap = ResizeBitmap(fullBitmap, 980, 600);

                Point p1 = new Point();
                Point p2 = new Point();

                double lonDif = loaderBounds.BotRightCorner.Lon - loaderBounds.TopLeftCorner.Lon;
                double latDif = loaderBounds.TopLeftCorner.Lat - loaderBounds.BotRightCorner.Lat;

                double PixelLon = lonDif / dataBitmap.Width;
                double PixelLat = latDif / dataBitmap.Height;

                double bY = loaderBounds.TopLeftCorner.Lat;
                double bX = loaderBounds.TopLeftCorner.Lon;

                int x;
                for (x = 0; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.TopLeftCorner.Lon && DefaultBounds.TopLeftCorner.Lon <= bX + DefaultBounds.TopLeftCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p1.X = x;

                int y;
                for (y = 0; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.TopLeftCorner.Lat && DefaultBounds.TopLeftCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p1.Y = y;

                for (; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.BotRightCorner.Lon && DefaultBounds.BotRightCorner.Lon <= bX + DefaultBounds.BotRightCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p2.X = x;

                for (; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.BotRightCorner.Lat && DefaultBounds.BotRightCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p2.Y = y;

                Bitmap smallBitmap = new Bitmap(p2.X - p1.X, p2.Y - p1.Y);

                for (x = 0; x < smallBitmap.Width; x++)
                {
                    for (y = 0; y < smallBitmap.Height; y++)
                    {
                        Color c = dataBitmap.GetPixel(x + p1.X, y + p1.Y);

                        smallBitmap.SetPixel(x, y, c);
                    }
                }

                Bitmap newBitmap = new Bitmap(728, 528);

                Graphics g = Graphics.FromImage(newBitmap);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                g.DrawImage(smallBitmap, 0, 0, 728, 528);

                return newBitmap;
            }
            else if(loaderBounds.TopLeftCorner.Lon >= DefaultBounds.TopLeftCorner.Lon && loaderBounds.TopLeftCorner.Lat <= DefaultBounds.TopLeftCorner.Lat && loaderBounds.BotRightCorner.Lon <= DefaultBounds.TopLeftCorner.Lon && loaderBounds.BotRightCorner.Lat >= DefaultBounds.TopLeftCorner.Lat)
            {
                int xS = -1;
                int yS = -1;

                int counter = 0;

                Point p1 = new Point();
                Point p2 = new Point();

                double lonDif = DefaultBounds.BotRightCorner.Lon - DefaultBounds.TopLeftCorner.Lon;
                double latDif = DefaultBounds.TopLeftCorner.Lat - DefaultBounds.BotRightCorner.Lat;

                double PixelLon = lonDif / 728;
                double PixelLat = latDif / 528;

                double bY = DefaultBounds.TopLeftCorner.Lat;
                double bX = DefaultBounds.TopLeftCorner.Lon;

                while (bX < loaderBounds.BotRightCorner.Lon)
                {
                    if (bX >= loaderBounds.TopLeftCorner.Lon)
                    {
                        p2.X++;

                        if (xS == -1)
                        {
                            xS = counter;
                        }
                    }

                    if (counter <= 727)
                    {
                        p1.X = p2.X;
                    }

                    bX += PixelLon;
                    counter++;
                }

                counter = 0;

                while (bY > loaderBounds.BotRightCorner.Lat)
                {
                    if (bY <= loaderBounds.TopLeftCorner.Lat)
                    {
                        p2.Y++;

                        if (yS == -1)
                        {
                            yS = counter;
                        }
                    }

                    if (counter <= 527)
                    {
                        p1.Y = p2.Y;
                    }

                    bY -= PixelLat;
                    counter++;
                }

                Bitmap newBmp = new Bitmap(728, 528);

                Bitmap resizedBmp = new Bitmap(p1.X, p1.Y);
                using (Graphics g = Graphics.FromImage(resizedBmp))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(dataBitmap, 0, 0, p1.X, p1.Y);
                };

                for (int x = 0; x < p1.X; x++)
                {
                    for (int y = 0; y < p1.Y; y++)
                    {
                        newBmp.SetPixel(x + xS, y + yS, resizedBmp.GetPixel(x, y));
                    }
                }

                return newBmp;
            }
            else
            {
                //vyřezání části z bitmapy

                Point p1 = new Point();
                Point p2 = new Point();

                double lonDif = loaderBounds.BotRightCorner.Lon - loaderBounds.TopLeftCorner.Lon;
                double latDif = loaderBounds.TopLeftCorner.Lat - loaderBounds.BotRightCorner.Lat;

                double PixelLon = lonDif / dataBitmap.Width;
                double PixelLat = latDif / dataBitmap.Height;

                double bY = loaderBounds.TopLeftCorner.Lat;
                double bX = loaderBounds.TopLeftCorner.Lon;

                int x;
                for (x = 0; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.TopLeftCorner.Lon && DefaultBounds.TopLeftCorner.Lon <= bX + DefaultBounds.TopLeftCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p1.X = x;

                int y;
                for (y = 0; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.TopLeftCorner.Lat && DefaultBounds.TopLeftCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p1.Y = y;

                for (; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.BotRightCorner.Lon && DefaultBounds.BotRightCorner.Lon <= bX + DefaultBounds.BotRightCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p2.X = x;

                for (; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.BotRightCorner.Lat && DefaultBounds.BotRightCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p2.Y = y;

                int cutX = p2.X - p1.X;
                if (cutX == 0)
                {
                    cutX = 728;
                    p1.X = 0;
                }

                int cutY = p2.Y - p1.Y;
                if (cutY == 0)
                {
                    cutY = 528;
                    p1.Y = 0;
                }

                Bitmap smallBitmap = new Bitmap(cutX, cutY);

                for (x = 0; x < smallBitmap.Width; x++)
                {
                    for (y = 0; y < smallBitmap.Height; y++)
                    {
                        Color c = dataBitmap.GetPixel(x + p1.X, y + p1.Y);

                        smallBitmap.SetPixel(x, y, c);
                    }
                }

                smallBitmap.Save("ZcutBmp.bmp", ImageFormat.Bmp);

                //vložení této části do nové bitmapy

                int xS = -1;
                int yS = -1;

                int counter = 0;

                p1 = new Point();
                p2 = new Point();

                lonDif = DefaultBounds.BotRightCorner.Lon - DefaultBounds.TopLeftCorner.Lon;
                latDif = DefaultBounds.TopLeftCorner.Lat - DefaultBounds.BotRightCorner.Lat;

                PixelLon = lonDif / 728;
                PixelLat = latDif / 528;

                bY = DefaultBounds.TopLeftCorner.Lat;
                bX = DefaultBounds.TopLeftCorner.Lon;

                while (bX < loaderBounds.BotRightCorner.Lon)
                {
                    if (bX >= loaderBounds.TopLeftCorner.Lon)
                    {
                        p2.X++;

                        if (xS == -1)
                        {
                            xS = counter;
                        }
                    }

                    if (counter <= 727)
                    {
                        p1.X = p2.X;
                    }

                    bX += PixelLon;
                    counter++;
                }

                counter = 0;

                while (bY > loaderBounds.BotRightCorner.Lat)
                {
                    if (bY <= loaderBounds.TopLeftCorner.Lat)
                    {
                        p2.Y++;

                        if (yS == -1)
                        {
                            yS = counter;
                        }
                    }

                    if (counter <= 527)
                    {
                        p1.Y = p2.Y;
                    }

                    bY -= PixelLat;
                    counter++;
                }

                Bitmap newBmp = new Bitmap(728, 528);

                Bitmap resizedBmp = new Bitmap(p1.X, p1.Y);
                using (Graphics g = Graphics.FromImage(resizedBmp))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(smallBitmap, 0, 0, p1.X, p1.Y);
                };

                for (x = 0; x < p1.X; x++)
                {
                    for (y = 0; y < p1.Y; y++)
                    {
                        newBmp.SetPixel(x + xS, y + yS, resizedBmp.GetPixel(x, y));
                    }
                }

                newBmp.Save("ZfinBmp.bmp", ImageFormat.Bmp);

                return newBmp;
            }

        }

        public static void DrawBiggerExampleBitmaps()
        {
            //DrawTriangulationExample();
            //DrawExampleBitmaps();

            Bitmap bNew = new Bitmap(10, 10);

            bNew.SetPixel(3, 3, Color.Red);
            bNew.Save("BNew.bmp", ImageFormat.Bmp);

            //bNew.Dispose();

            Bitmap bLoad = new Bitmap("BNew.bmp");
            bLoad.SetPixel(6, 6, Color.Blue);

            bLoad.Save("BNew2.bmp", ImageFormat.Bmp);

            bLoad.Dispose();

            DirectoryInfo dI = new DirectoryInfo(@"D:\Škola - programy\Bakalářka - Předpověď počasí\bp_pocasi\Agregace Dat\TestConsoleApk\TestConsoleApk\bin\Debug\netcoreapp2.1\");
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                if (f.Name == "BNew.bmp")
                {
                    f.Delete();
                }
            }

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Console.WriteLine(bLoad.GetPixel(x, y));
                }
            }
        }

        public static void DrawExampleBitmaps()
        {
            Dictionary<DateTime, List<Forecast>> fcs = new WeatherUnlockedDataLoader().GetWeekForecast();

            List<DateTime> keys = fcs.Keys.ToList();

            List<Forecast> forecasts = fcs[keys[4]];

            Bitmap bmp1 = new Bitmap(728, 528);
            Bitmap bmp2 = new Bitmap(728, 528);
            Bitmap bmp3 = new Bitmap(728, 528);
            Bitmap bmp4 = new Bitmap(728, 528);

            for (int x = 0; x < 728; x++)
            {
                for (int y = 0; y < 528; y++)
                {
                    bmp1.SetPixel(x, y, Color.Black);
                    bmp2.SetPixel(x, y, Color.Black);
                    bmp3.SetPixel(x, y, Color.Black);
                    bmp4.SetPixel(x, y, Color.Black);
                }
            }

            Console.WriteLine("b1");

            foreach (Forecast point in forecasts)
            {
                Console.WriteLine(point.Temperature.Value);

                Color c = ColorValueHandler.GetTemperatureColor(point.Temperature.Value);

                try
                {
                    for(int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bmp1.SetPixel((int)point.x + i, (int)point.y + j, c);
                        }
                    }
                }
                catch
                {

                }
            }

            Console.WriteLine("b2 a b3");

            Triangulator angulator = new Triangulator();
            List<Vertex> vertexes = forecasts.ConvertAll(x => (Vertex)x);
            List<Triad> triangles = angulator.Triangulation(vertexes, true);

            for (int i = 0; i < triangles.Count; i++)
            {
                Triad t = triangles[i];

                Point p1 = new Point((int)forecasts[t.a].x, (int)forecasts[t.a].y);
                Point p2 = new Point((int)forecasts[t.b].x, (int)forecasts[t.b].y);
                Point p3 = new Point((int)forecasts[t.c].x, (int)forecasts[t.c].y);

                DrawLineInt(bmp2, p1, p2);
                DrawLineInt(bmp2, p2, p3);
                DrawLineInt(bmp2, p3, p1);

                Point[] arP = new Point[] { p1, p2, p3 };

                int xMin = Math.Min(p1.X, Math.Min(p2.X, p3.X));
                int xMax = Math.Max(p1.X, Math.Max(p2.X, p3.X));
                int yMin = Math.Min(p1.Y, Math.Min(p2.Y, p3.Y));
                int yMax = Math.Max(p1.Y, Math.Max(p2.Y, p3.Y));

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        Point newPoint = new Point(x, y);

                        if (PointInTriangle(newPoint, p1, p2, p3))
                        {
                            bmp3.SetPixel(x, y, GetCollorInTriangle(newPoint, p1, p2, p3, forecasts[t.a].Temperature.Value, forecasts[t.b].Temperature.Value, forecasts[t.c].Temperature.Value, ForecastTypes.TEMPERATURE));
                        }
                    }
                }
            }

            Console.WriteLine("b4");

            for (int x = 0; x < 728; x++)
            {
                for (int y = 0; y < 528; y++)
                {
                    bmp4.SetPixel(x, y, bmp2.GetPixel(x, y));

                    Color c = bmp1.GetPixel(x, y);

                    if (c.R != 0 || c.G != 0 || c.B != 0)
                    {
                        bmp4.SetPixel(x, y, c);
                    }
                }
            }

            bmp1.Save("b1.png", ImageFormat.Png);
            bmp2.Save("b2.png", ImageFormat.Png);
            bmp3.Save("b3.png", ImageFormat.Png);
            bmp4.Save("b4.png", ImageFormat.Png);
        }

        public static void DrawTriangulationExample()
        {

            Bitmap b = new Bitmap(300, 300);

            for(int x = 0; x < 300; x++)
            {
                for (int y = 0; y < 300; y++)
                {
                    b.SetPixel(x, y, Color.Black);
                }
            }

            List<Vertex> points = new List<Vertex>();

            Random rand = new Random();

            for(int i = 0; i < 500; i++)
            {
                int x = rand.Next(300);
                int y = rand.Next(300);

                points.Add(new Vertex(x, y));
            }

            Triangulator angulator = new Triangulator();
            List<Vertex> vertexes = points;
            List<Triad> triangles = angulator.Triangulation(vertexes, true);

            for (int i = 0; i < triangles.Count; i++)
            {
                Triad t = triangles[i];

                Point p1 = new Point((int)vertexes[t.a].x, (int)vertexes[t.a].y);
                Point p2 = new Point((int)vertexes[t.b].x, (int)vertexes[t.b].y);
                Point p3 = new Point((int)vertexes[t.c].x, (int)vertexes[t.c].y);

                DrawLineInt(b, p1, p2);
                DrawLineInt(b, p2, p3);
                DrawLineInt(b, p3, p1); 
            }

            b.Save("bT.png", ImageFormat.Png);
        }

        public static bool PointInTriangle(Point pt, Point v1, Point v2, Point v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        public static float sign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static Color GetCollorInTriangle(Point p, Point v1, Point v2, Point v3, double val1, double val2, double val3, string type)
        {
            double dis1 = Math.Sqrt(Math.Pow(v1.X - p.X, 2) + Math.Pow(v1.Y - p.Y, 2));
            double dis2 = Math.Sqrt(Math.Pow(v2.X - p.X, 2) + Math.Pow(v2.Y - p.Y, 2));
            double dis3 = Math.Sqrt(Math.Pow(v3.X - p.X, 2) + Math.Pow(v3.Y - p.Y, 2));

            double w1, w2, w3;

            if (dis1 > 0)
                w1 = 1 / dis1;
            else
                w1 = 0;

            if (dis2 > 0)
                w2 = 1 / dis2;
            else
                w2 = 0;

            if (dis3 > 0)
                w3 = 1 / dis3;
            else
                w3 = 0;

            double avgVal = (w1 * val1 + w2 * val2 + w3 * val3) / (w1 + w2 + w3);

            return ColorValueHandler.GetColorForValueAndType(avgVal, type);
        }

        public static void DrawLineInt(Bitmap bmp, Point p1, Point p2)
        {
            Pen blackPen = new Pen(Color.White, 1);

            // Draw line to screen.
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawLine(blackPen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }
    }
}
