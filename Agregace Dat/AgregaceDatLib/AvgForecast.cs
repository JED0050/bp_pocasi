using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;

namespace AgregaceDatLib
{
    public class AvgForecast : BitmapHelper
    {
        private List<DataLoader> dLs;

        public AvgForecast()
        {
            dLs = new List<DataLoader>();
        }

        public AvgForecast(List<DataLoader> defDLs)
        {
            dLs = defDLs;
        }

        public void Add(DataLoader newDl)
        {
            dLs.Add(newDl);
        }

        public void Remove(DataLoader remDl)
        {
            dLs.Remove(remDl);
        }

        public void SaveForecastBitmaps(int days)
        {

            Parallel.ForEach(dLs, dL =>
            {
                dL.SaveNewDeleteOldBmps(days);
            });

        }

        
        public Bitmap GetAvgForecBitmap(DateTime time)
        {

            Bitmap avgBitmap = new Bitmap(728, 528);

            List<Bitmap> loaderBitmaps = new List<Bitmap>();

            foreach (DataLoader dL in dLs)
            {
                try
                {
                    loaderBitmaps.Add(dL.GetPrecipitationBitmap(time));
                }
                catch
                {
                    continue;
                }
            }

            for (int y = 0; y < avgBitmap.Height; y++)
            {
                for(int x = 0; x < avgBitmap.Width; x++)
                {
                    double precSum = 0;

                    foreach(Bitmap b in loaderBitmaps)
                    {
                        precSum += new BitmapForecast(b).GetPrecipitationFromPixel(b.GetPixel(x,y));
                    }

                    avgBitmap.SetPixel(x, y, new Forecast() { Precipitation = precSum / loaderBitmaps.Count }.GetPrecipitationColor());
                }
            }

            //avgBitmap.Save("ForecastBitmap" + time.ToString("yyyyMMddHH") + ".bmp", ImageFormat.Bmp);

            return avgBitmap;
        }

        public Bitmap GetAvgForecBitmap(DateTime time, PointLonLat topLeft, PointLonLat botRight)   //vyříznutí části mapy
        {
            Bitmap bigBitmap = GetAvgForecBitmap(time);

            Point p1 = new Point();
            Point p2 = new Point();

            double lonDif = 20.21 - 10.06;
            double latDif = 51.88 - 47.09;

            double PixelLon = lonDif / bigBitmap.Width;
            double PixelLat = latDif / bigBitmap.Height;

            double bY = 51.88;
            double bX = 10.06;

            int x;
            for (x = 0; x < bigBitmap.Width; x++)
            {
                if (bX >= topLeft.Lon && topLeft.Lon <= bX + topLeft.Lon)
                    break;

                bX += PixelLon;
            }

            p1.X = x;

            int y;
            for (y = 0; y < bigBitmap.Height; y++)
            {
                if (bY - PixelLat <= topLeft.Lat && topLeft.Lat <= bY)
                    break;

                bY -= PixelLat;
            }

            p1.Y = y;

            bY = 51.88;
            bX = 10.06;

            for (x = 0; x < bigBitmap.Width; x++)
            {
                if (bX >= botRight.Lon && botRight.Lon <= bX + botRight.Lon)
                    break;

                bX += PixelLon;
            }

            p2.X = x;

            for (y = 0; y < bigBitmap.Height; y++)
            {
                if (bY - PixelLat <= botRight.Lat && botRight.Lat <= bY)
                    break;

                bY -= PixelLat;
            }

            p2.Y = y;

            Bitmap smallBitmap = new Bitmap(p2.X - p1.X, p2.Y - p1.Y);

            for (x = 0; x < smallBitmap.Width; x++)
            {
                for (y = 0; y < smallBitmap.Height; y++)
                {
                    Color c = bigBitmap.GetPixel(x + p1.X, y + p1.Y);

                    smallBitmap.SetPixel(x, y, c);
                }
            }

            return smallBitmap;
        }

        public Forecast GetForecast(DateTime time, double lat, double lon)
        {
            BitmapForecast bF = new BitmapForecast(GetAvgForecBitmap(time));

            Forecast f = bF.GetForecast(time, lat, lon);

            //Console.WriteLine(f.GetPrecipitationColor());

            return f;
        }

    }
}
