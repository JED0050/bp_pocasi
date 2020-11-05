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

        public Forecast GetForecast(DateTime time, double lat, double lon)
        {
            BitmapForecast bF = new BitmapForecast(GetAvgForecBitmap(time));

            Forecast f = bF.GetForecast(time, lat, lon);

            //Console.WriteLine(f.GetPrecipitationColor());

            return f;
        }
        
    }
}
