using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;

namespace AgregaceDatLib
{
    public class AvgForecast : BitmapCustomDraw
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

        public int GetNumberOfLoaders()
        {
            return dLs.Count;
        }

        public void SaveForecastBitmaps()
        {

            Parallel.ForEach(dLs, dL =>
            {
                dL.SaveNewDeleteOldBmps();
            });

        }

        
        public Bitmap GetAvgForecBitmap(DateTime time, string type)
        {

            Bitmap avgBitmap = new Bitmap(728, 528);

            List<Bitmap> loaderBitmaps = new List<Bitmap>();

            foreach (DataLoader dL in dLs)
            {
                try
                {
                    if(type == "prec")
                    {
                        loaderBitmaps.Add(dL.GetPrecipitationBitmap(time));
                    }
                    else if(type == "temp")
                    {
                        loaderBitmaps.Add(dL.GetTemperatureBitmap(time));
                    }

                }
                catch
                {
                    continue;
                }
            }

            if(loaderBitmaps.Count == 0)
            {
                throw new Exception("");
            }
            else if (loaderBitmaps.Count == 1)
            {
                return loaderBitmaps[0];
            }
            else
            {
                for (int y = 0; y < avgBitmap.Height; y++)
                {
                    for (int x = 0; x < avgBitmap.Width; x++)
                    {
                        double tempValSum = 0;

                        foreach (Bitmap b in loaderBitmaps)
                        {
                            if (type == "prec")
                            {
                                tempValSum += ColorValueHandler.GetPrecipitationValue(b.GetPixel(x, y));
                            }
                            else if (type == "temp")
                            {
                                tempValSum += ColorValueHandler.GetTemperatureValue(b.GetPixel(x, y));
                            }
                        }

                        if (type == "prec")
                        {
                            avgBitmap.SetPixel(x, y, ColorValueHandler.GetPrecipitationColor(tempValSum / loaderBitmaps.Count));
                        }
                        else if (type == "temp")
                        {
                            avgBitmap.SetPixel(x, y, ColorValueHandler.GetTemperatureColor(tempValSum / loaderBitmaps.Count));
                        }

                    }
                }

                return avgBitmap;
            }
            
        }

        public Bitmap GetAvgForecBitmap(DateTime time, string type, PointLonLat topLeft, PointLonLat botRight)   //vyříznutí části mapy
        {
            Bitmap bigBitmap = GetAvgForecBitmap(time,type);

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

            //return smallBitmap;

            Bitmap newBitmap = new Bitmap(728, 528);

            Graphics g = Graphics.FromImage(newBitmap);

            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.DrawImage(smallBitmap, 0, 0, 728, 528);

            return newBitmap;
        }

        public Forecast GetForecastFromTimeAndPoint(DateTime time, PointLonLat point)
        {
            List<Forecast> forecasts = new List<Forecast>();

            foreach (DataLoader dL in dLs)
            {
                try
                {
                    forecasts.Add(dL.GetForecast(time, point));

                }
                catch
                {
                    continue;
                }
            }

            int numOfValidForecasts = forecasts.Count;

            if (numOfValidForecasts == 0)
            {
                throw new Exception("Nebylo určeno žádné počasí z datových zdrojů");
            }
            else if(numOfValidForecasts == 1)
            {
                forecasts[0].Longitude = forecasts[0].Longitude.Replace(',', '.');
                forecasts[0].Latitude = forecasts[0].Latitude.Replace(',', '.');

                return forecasts[0];
            }
            else
            {
                Forecast avgForecast = forecasts[0];

                for(int i = 1; i < numOfValidForecasts; i++)
                {
                    avgForecast.Temperature += forecasts[i].Temperature;
                    avgForecast.Precipitation += forecasts[i].Precipitation;
                    avgForecast.Humidity += forecasts[i].Humidity;
                    avgForecast.Pressure += forecasts[i].Pressure;

                    if(forecasts[i].Time < avgForecast.Time)
                    {
                        avgForecast.Time = forecasts[i].Time;
                    }

                    avgForecast.AddDataSource(forecasts[i].DataSources[0]);
                }

                avgForecast.Temperature /= numOfValidForecasts;
                avgForecast.Precipitation /= numOfValidForecasts;
                avgForecast.Humidity /= numOfValidForecasts;
                avgForecast.Pressure /= numOfValidForecasts;

                avgForecast.Longitude = avgForecast.Longitude.Replace(',', '.');
                avgForecast.Latitude = avgForecast.Latitude.Replace(',', '.');    

                return avgForecast;
            }
        }
    }
}
