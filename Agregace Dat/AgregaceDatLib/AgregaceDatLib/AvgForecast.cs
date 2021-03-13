using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;
using IDataLoaderAndHandlerLib.Interface;
using IDataLoaderAndHandlerLib.HandlersAndObjects;
using DLMedardLib;
using DLOpenWeatherMapLib;
using DLRadarBourkyLib;
using DLWeatherUnlockedLib;
using DLYrNoLib;
using System.Diagnostics;

namespace AgregaceDatLib
{
    public class AvgForecast
    {
        private PointLonLat topLeftBound = new PointLonLat(4.1303633, 55.1995133);
        private PointLonLat botRightBound = new PointLonLat(37.9033283, 41.6999200);

        private List<DataLoader> dLs;

        public AvgForecast()
        {
            dLs = new List<DataLoader>();
        }

        public AvgForecast(List<DataLoader> defDLs)
        {
            dLs = defDLs;
        }

        public AvgForecast(string loaders)
        {
            dLs = new List<DataLoader>();

            if (loaders == null || loaders.Length == 0)
            {
                RadarBourkyDataLoader bL = new RadarBourkyDataLoader();
                YrNoDataLoader xL = new YrNoDataLoader();
                OpenWeatherMapDataLoader jL = new OpenWeatherMapDataLoader();
                MedardDataLoader bL2 = new MedardDataLoader();
                WeatherUnlockedDataLoader wL = new WeatherUnlockedDataLoader();

                dLs.Add(bL);
                dLs.Add(xL);
                dLs.Add(jL);
                dLs.Add(bL2);
                dLs.Add(wL);
            }
            else
            {
                string[] arLoaders = loaders.ToLower().Split(",");

                foreach (string loader in arLoaders)
                {
                    if (loader == "rb")
                    {
                        RadarBourkyDataLoader bL = new RadarBourkyDataLoader();
                        dLs.Add(bL);
                    }

                    if (loader == "mdrd")
                    {
                        MedardDataLoader bL2 = new MedardDataLoader();
                        dLs.Add(bL2);
                    }

                    if (loader == "yrno")
                    {
                        YrNoDataLoader xL = new YrNoDataLoader();
                        dLs.Add(xL);
                    }

                    if (loader == "owm")
                    {
                        OpenWeatherMapDataLoader jL = new OpenWeatherMapDataLoader();
                        dLs.Add(jL);
                    }

                    if (loader == "weun")
                    {
                        WeatherUnlockedDataLoader wL = new WeatherUnlockedDataLoader();
                        dLs.Add(wL);
                    }
                }

                if (dLs.Count == 0)
                {
                    throw new Exception("Nebyl přiřazen žádný z vámi zadanách datových zrdrojů!");
                }
            }
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
            Debug.WriteLine("AgregaceDat: Zahájení vytváření bitmap");

            Parallel.ForEach(dLs, dL =>
            {
                dL.SaveNewDeleteOldBmps();
            });

            Debug.WriteLine("AgregaceDat: Dokončení vytváření bitmap");
        }

        
        public Bitmap GetAvgForecBitmap(DateTime time, string type)
        {

            Bitmap avgBitmap = new Bitmap(728, 528);

            List<Bitmap> loaderBitmaps = new List<Bitmap>();

            foreach (DataLoader dL in dLs)
            {
                try
                {
                    loaderBitmaps.Add(dL.GetForecastBitmap(time, type));
                }
                catch
                {
                    continue;
                }
            }

            if(loaderBitmaps.Count == 0)
            {
                throw new Exception("Žádný ze zvolených datových zdrojů neobsahuje bitmapu se zvoleným časem a typem předpovědi! Zkuste změnit čas případně zadaný typ.");
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
                        double valSum = 0;

                        foreach (Bitmap b in loaderBitmaps)
                        {
                            valSum += ColorValueHandler.GetValueForColorAndType(b.GetPixel(x, y), type);
                        }

                        avgBitmap.SetPixel(x, y, ColorValueHandler.GetColorForValueAndType(valSum / loaderBitmaps.Count, type));
                    }
                }

                return avgBitmap;
            }
            
        }

        public Bitmap GetAvgForecBitmap(DateTime time, string type, PointLonLat topLeft, PointLonLat botRight)   //vyříznutí části mapy
        {
            if(topLeft.Lon > botRight.Lon || topLeft.Lat < botRight.Lat)
            {
                throw new Exception("Hranice mapy jsou zadány špatně. První bod (topLeft) se musí dle zeměpisných souřadnic nacházet vlevo nahoře od druhého budu (botRight).");
            }
            else if(topLeft.Lon < 4.1303633 || topLeft.Lat > 55.1995133 || botRight.Lon > 37.9033283 || botRight.Lat < 41.6999200)
            {
                throw new Exception("Hranice mapy jsou zadány špatně. Body topLeft a botRight se musí nacházet uvnitř plochy ohraničené body P1 (lat=55.1995133, lon=4.1303633) a P2 (lat=41.6999200, lon=37.9033283).");
            }

            Bitmap bigBitmap = GetAvgForecBitmap(time,type);

            Point p1 = new Point();
            Point p2 = new Point();

            double lonDif = Math.Abs(botRightBound.Lon - topLeftBound.Lon);
            double latDif = Math.Abs(topLeftBound.Lat - botRightBound.Lat);

            double PixelLon = lonDif / bigBitmap.Width;
            double PixelLat = latDif / bigBitmap.Height;

            double bY = topLeftBound.Lat;
            double bX = topLeftBound.Lon;

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

            //bY = topLeftBound.Lat;
            //bX = topLeftBound.Lon;

            for (; x < bigBitmap.Width; x++)
            {
                if (bX >= botRight.Lon && botRight.Lon <= bX + botRight.Lon)
                    break;

                bX += PixelLon;
            }

            p2.X = x;

            for (; y < bigBitmap.Height; y++)
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
                    forecasts.Add(dL.GetForecastPoint(time, point));

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
                Forecast avgForecast = new Forecast();
                avgForecast.Time = forecasts[0].Time;
                avgForecast.Longitude = forecasts[0].Longitude;
                avgForecast.Latitude = forecasts[0].Latitude;

                int tempC = 0;
                int precC = 0;
                int presC = 0;
                int humiC = 0;

                for (int i = 0; i < numOfValidForecasts; i++)
                {
                    if(forecasts[i].Temperature.HasValue)
                    {
                        avgForecast.Temperature += forecasts[i].Temperature;
                        tempC++;
                    }

                    if (forecasts[i].Precipitation.HasValue)
                    {
                        avgForecast.Precipitation += forecasts[i].Precipitation;
                        precC++;
                    }

                    if (forecasts[i].Pressure.HasValue)
                    {
                        avgForecast.Pressure += forecasts[i].Pressure;
                        presC++;
                    }

                    if (forecasts[i].Humidity.HasValue)
                    {
                        avgForecast.Humidity += forecasts[i].Humidity;
                        humiC++;
                    }



                    if (forecasts[i].Time < avgForecast.Time)
                    {
                        avgForecast.Time = forecasts[i].Time;
                    }

                    avgForecast.AddDataSource(forecasts[i].DataSources[0]);
                }

                if(tempC == 0)
                {
                    avgForecast.Temperature = null;
                }
                else if(tempC > 1)
                {
                    avgForecast.Temperature = Math.Round(avgForecast.Temperature.Value / tempC, 4);
                }

                if (precC == 0)
                {
                    avgForecast.Precipitation = null;
                }
                else if(precC > 1)
                {
                    avgForecast.Precipitation = Math.Round(avgForecast.Precipitation.Value / precC, 4);
                }

                if (humiC == 0)
                {
                    avgForecast.Humidity = null;
                }
                else if (humiC > 1)
                {
                    avgForecast.Humidity = Math.Round(avgForecast.Humidity.Value / humiC, 4);
                }

                if (presC == 0)
                {
                    avgForecast.Pressure = null;
                }
                else if (presC > 1)
                {
                    avgForecast.Pressure = Math.Round(avgForecast.Pressure.Value / presC, 4);
                }

                //avgForecast.Longitude = avgForecast.Longitude.Replace(',', '.');
                //avgForecast.Latitude = avgForecast.Latitude.Replace(',', '.');    

                return avgForecast;
            }
        }
    }
}
