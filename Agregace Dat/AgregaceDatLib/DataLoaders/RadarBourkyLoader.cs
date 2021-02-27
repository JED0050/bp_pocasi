using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class RadarBourkyDataLoader : BitmapCustomDraw, DataLoader
    {
        //bounds
        private PointLonLat topLeft = new PointLonLat(10.88, 51.88);
        private PointLonLat botRight = new PointLonLat(20.21, 47.09);

        public string LOADER_NAME = "RadarBourky";

        public RadarBourkyDataLoader()
        {
            if (!Directory.Exists(GetPathToDataDirectory("")))
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"Radar.bourky\";

                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);

                    Directory.CreateDirectory(loaderDir);
                }
                else if (!Directory.Exists(loaderDir))
                {
                    Directory.CreateDirectory(loaderDir);
                }
            }
        }
        public Bitmap GetBitmap(string url)
        {

            Bitmap bitmap;

            using (WebClient client = new WebClient())
            {
                Stream stream = client.OpenRead(url);
                bitmap = new Bitmap(stream);

                //stream.Flush();
                //stream.Close();
                //client.Dispose();
            }

            return bitmap;

        }

        public Bitmap GetForecastBitmap(DateTime forTime, string type)
        {
            if (type != ForecastTypes.PRECIPITATION)
                throw new Exception("Datový zdroj poskytuje pouze data o srážkách!");

            forTime = forTime.AddMinutes(- forTime.Minute % 10);

            if (forTime > DateTime.Now.AddHours(6))
            {
                throw new Exception("zvolený čas je příliš vysoký, služba poskytuje pouze předpovědi aktuální čas");
            }
            else if(forTime < DateTime.Now.AddHours(-6))
            {
                throw new Exception("zvolený čas je příliš nízký, služba poskytuje pouze předpovědi aktuální čas");
            }

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));

            FileInfo[] fileInfos = dI.GetFiles("*.bmp");

            string bitmapPath = "";

            TimeSpan minTS = forTime - DateTime.Now.AddHours(-8);

            for (int i = 0; i < fileInfos.Length; i++)
            {
                DateTime dT = GetDateTimeFromBitmapName(fileInfos[i].Name);
                TimeSpan actTS = dT - forTime;

                if (i == 0)
                {
                    minTS = actTS;
                    bitmapPath = fileInfos[i].FullName;
                }
                else
                {
                    if (Math.Abs(actTS.TotalMinutes) < Math.Abs(minTS.TotalMinutes))
                    {
                        minTS = actTS;
                        bitmapPath = fileInfos[i].FullName;
                    }
                }
            }

            if(Math.Abs(minTS.TotalHours) < 7)
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                throw new Exception("Požadovaná bitmapa srážek nebyla nalezena");
            }
        }

        private string GetPathToDataDirectory(string fileName)
        {
            //string workingDirectory = Environment.CurrentDirectory;
            //return Directory.GetParent(workingDirectory).FullName + @"\Data\Radar.bourky\" + fileName;

            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\Radar.bourky\" + fileName;
        }

        private void ClearBitmap(Bitmap bitmap)
        {
            for(int x = 0; x < bitmap.Width; x++)
            {
                for(int y = 0; y < bitmap.Height; y++)
                {
                    if(x >= 0 && x <= 345 && y >= 510 && y <= bitmap.Height)
                    {
                        bitmap.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        Color c = bitmap.GetPixel(x, y);

                        if (c == Color.Empty || c == Color.FromArgb(255, 0, 0, 0) || c == Color.FromArgb(255, 196, 196, 196) || c == Color.FromArgb(255, 255, 255, 255))
                        {
                            bitmap.SetPixel(x, y, Color.Black);
                        }
                        else
                        {
                            double precVal = GetPrecipitationFromPixel(c);

                            if(precVal == 0)
                            {
                                bitmap.SetPixel(x, y, Color.Black);
                            }
                            else
                            {
                                bitmap.SetPixel(x, y, ColorValueHandler.GetPrecipitationColor(precVal));
                            }
                            
                        }
                    }
                }
            }
        }

        private void CreateBitmap(DateTime forTime)
        {
            forTime = forTime.AddMinutes(-forTime.Minute % 10);

            Bitmap radarBitmap;

            int c = 0;

            while (true)
            {

                try
                {
                    radarBitmap = GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.ToString("yyyyMMdd.HHmm") + ".0.png");

                    break;
                }
                catch
                {
                    forTime = forTime.AddMinutes(-10);
                    c++;
                }

                if (c > 108)
                {
                    throw new Exception("nebyla nalezena bitmapa z daty pro zvolený čas");
                }
            }

            string bitmapName = GetBitmapName(ForecastTypes.PRECIPITATION, forTime);
            string bitmapPath = GetPathToDataDirectory(bitmapName);

            ClearBitmap(radarBitmap);
            radarBitmap.Save(bitmapPath, ImageFormat.Bmp);
            radarBitmap.Dispose();
        }

        public void SaveNewDeleteOldBmps() //1 hodina +- (nepoužitelnéprakticky krom aktuálního počasí)
        {
            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                DateTime dateTime = GetDateTimeFromBitmapName(f.Name);

                if (dateTime < DateTime.Now.AddHours(-7)) //smazání starých bitmap
                {
                    f.Delete();
                }
            }

            CreateBitmap(DateTime.Now);
        }

        private double GetPrecipitationFromPixel(Color pixel)
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
            else if (ColInRange(0, 0, pixel.R) && ColInRange(107, 159, pixel.G) && ColInRange(0, 192, pixel.B))
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
            else if (ColInRange(250, 255, pixel.R) && ColInRange(133, 176, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 10;
            else if (ColInRange(250, 255, pixel.R) && ColInRange(89, 132, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 30;
            else if (ColInRange(250, 255, pixel.R) && ColInRange(1, 88, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 60;
            else if (ColInRange(150, 255, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 100;

            return precipitation;
        }

        private bool ColInRange(int min, int max, int val)
        {
            return (min <= val) && (val <= max);
        }

        public Forecast GetForecastPoint(DateTime forTime, PointLonLat location)
        {
            Forecast forecast = new Forecast();

            forecast.Longitude = location.Lon.ToString();
            forecast.Latitude = location.Lat.ToString();
            forecast.Time = forTime;
            forecast.AddDataSource(LOADER_NAME);

            forecast.Precipitation = GetValueFromBitmapTypeAndPoints(GetForecastBitmap(forTime, ForecastTypes.PRECIPITATION), topLeft, botRight, location, ForecastTypes.PRECIPITATION);

            forecast.Humidity = null;
            forecast.Pressure = null;
            forecast.Temperature = null;

            return forecast;
        }
    }
}
