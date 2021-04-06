using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using IDataLoaderAndHandlerLib.Interface;
using IDataLoaderAndHandlerLib.HandlersAndObjects;

namespace DLMedardLib
{
    public class MedardDataLoader : DataLoaderHandler, DataLoader
    {
        //bounds
        private PointLonLat topLeft;// = new PointLonLat(-21.123962402343754, 56.13636792495879);
        private PointLonLat botRight;// = new PointLonLat(29.696044921875, 33.25246979589199);

        public string LOADER_NAME;// = "Medard-online";

        private static List<Color> scaleTempArray;
        private static List<Color> scalePrecArray;

        public MedardDataLoader()
        {

            if (!Directory.Exists(GetPathToDataDirectory("")))
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"medard-online\";

                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);

                    Directory.CreateDirectory(loaderDir);
                }
                else if(!Directory.Exists(loaderDir))
                {
                    Directory.CreateDirectory(loaderDir);
                }
            }

            dataLoaderConfig = GetDataLoaderConfigFile();

            topLeft = dataLoaderConfig.TopLeftCornerLonLat;
            botRight = dataLoaderConfig.BotRightCornerLonLat;

            LOADER_NAME = dataLoaderConfig.DataLoaderName;

            if (scaleTempArray == null)
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"medard-online\scales\";

                string scaleTempName = "scale_temp.png";
                Bitmap scaleTempImage = new Bitmap(loaderDir + scaleTempName);

                scaleTempArray = new List<Color>();

                scaleTempArray.Add(scaleTempImage.GetPixel(0, 0));

                for (int i = 1; i < scaleTempImage.Width; i++)
                {
                    Color actPixel = scaleTempImage.GetPixel(i, 0);

                    if(actPixel != scaleTempArray[scaleTempArray.Count - 1])   //vložení pouze odlišných pixelů (škála z webu má více stejných pixelů pod sebou)
                    {
                        scaleTempArray.Add(actPixel);
                        //Debug.WriteLine(actPixel);
                    }
                }

                scaleTempImage.Dispose();

                //Debug.WriteLine(scaleTempArray.Count);
            }

            if(scalePrecArray == null)
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"medard-online\scales\";

                string scalePrecName = "scale_prec.png";
                Bitmap scalePrecImage = new Bitmap(loaderDir + scalePrecName);

                scalePrecArray = new List<Color>();

                scalePrecArray.Add(scalePrecImage.GetPixel(0, 0));

                for (int i = 1; i < scalePrecImage.Width; i++)
                {
                    Color actPixel = scalePrecImage.GetPixel(i, 0);

                    if (actPixel != scalePrecArray[scalePrecArray.Count - 1])   //vložení pouze odlišných pixelů (škála z webu má více stejných pixelů pod sebou)
                    {
                        scalePrecArray.Add(actPixel);
                        //Debug.WriteLine(actPixel);
                    }
                }

                scalePrecImage.Dispose();

                //Debug.WriteLine(scalePrecArray.Count);
            }

        }

        public Bitmap GetForecastBitmap(DateTime forTime, string type)
        {
            forTime = forTime.AddMinutes(30);
            forTime = new DateTime(forTime.Year, forTime.Month, forTime.Day, forTime.Hour, 0, 0);

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles($"{type}-*.bmp"))
            {
                DateTime dateTime = GetDateTimeFromBitmapName(f.Name);

                if (dateTime == forTime)
                {
                    return new Bitmap(f.FullName);
                }
            }

            throw new Exception("Bitmapa pro požadovaný čas nebyla nalezena");
        }

        public Bitmap GetPartOfBigBimtap(Bitmap fullBitmap)
        {

            //56.13636792495879 -21.123962402343754 - levý horní bod
            //56.134837449127744 41.11633300781251 - pravý horní bod
            //33.25246979589199 29.696044921875 - pravý dolní bod
            //59.68195494710389 9.96940612792969 - střed horní bod

            Bitmap bigBitmap = ResizeBitmap(fullBitmap, 980, 600);

            Point p1 = new Point();
            Point p2 = new Point();

            double lonDif = botRight.Lon - topLeft.Lon;
            double latDif = topLeft.Lat - botRight.Lat;

            double PixelLon = lonDif / bigBitmap.Width;
            double PixelLat = latDif / bigBitmap.Height;

            double bY = topLeft.Lat;
            double bX = topLeft.Lon;

            int x;
            for (x = 0; x < bigBitmap.Width; x++)
            {
                if (bX >= DefaultBounds.TopLeftCorner.Lon && DefaultBounds.TopLeftCorner.Lon <= bX + DefaultBounds.TopLeftCorner.Lon)
                    break;

                bX += PixelLon;
            }

            p1.X = x;

            int y;
            for (y = 0; y < bigBitmap.Height; y++)
            {
                if (bY - PixelLat <= DefaultBounds.TopLeftCorner.Lat && DefaultBounds.TopLeftCorner.Lat <= bY)
                    break;

                bY -= PixelLat;
            }

            p1.Y = y;

            //bY = 59.68195494710389;
            //bX = -21.123962402343754;

            for (; x < bigBitmap.Width; x++)
            {
                if (bX >= DefaultBounds.BotRightCorner.Lon && DefaultBounds.BotRightCorner.Lon <= bX + DefaultBounds.BotRightCorner.Lon)
                    break;

                bX += PixelLon;
            }

            p2.X = x;

            for (; y < bigBitmap.Height; y++)
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
                    Color c = bigBitmap.GetPixel(x + p1.X, y + p1.Y);

                    smallBitmap.SetPixel(x, y, c);
                }
            }

            return ResizeBitmap(smallBitmap, 728, 528);
        }

        public void SaveNewDeleteOldBmps()  //4 dny +-
        {
            Debug.WriteLine(LOADER_NAME + ": Zahájení vytváření bitmap");

            //odstranění starých bitmap
            DateTime now = DateTime.Now;

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                DateTime dateTime = GetDateTimeFromBitmapName(f.Name);

                if (dateTime < now.AddHours(-24)) //smazání starých bitmap
                {
                    f.Delete();
                }
            }

            dataLoaderConfig = GetDataLoaderConfigFile();

            if (IsReadyToDownloadData(dataLoaderConfig))
            {
                //vytvoření nových bitmap

                Bitmap allDataPrecipBitmap = new Bitmap(1, 1);
                Bitmap allDataTempBitmap = new Bitmap(1, 1);

                DateTime bitmapTime = now;

                bitmapTime = bitmapTime.AddHours(-bitmapTime.Hour % 6);

                for (int i = 0; i < 8; i++)
                {
                    string timePart = bitmapTime.ToString("yyMMdd_HH");    //201112_12

                    string baseUrl = @"http://www.medard-online.cz/apipreview?run=";
                    string precipUrl = baseUrl + timePart + @"&forecast=precip&layer=eu";
                    string tempUrl = baseUrl + timePart + @"&forecast=temp&layer=eu";

                    try
                    {
                        allDataPrecipBitmap = GetBitmap(precipUrl);
                        allDataTempBitmap = GetBitmap(tempUrl);
                        break;
                    }
                    catch
                    {

                    }

                    bitmapTime = bitmapTime.AddHours(-6);
                }

                if (allDataPrecipBitmap.Width == 1 || allDataPrecipBitmap.Height == 1 || allDataTempBitmap.Width == 1 || allDataTempBitmap.Height == 1)
                {
                    throw new Exception("Nebyla nalezena žádná bitmapa s daty o počasí");
                }

                //přebarvení kvůli použití různých škál


                for (int w = 0; w < allDataPrecipBitmap.Width; w++)
                {
                    for (int h = 0; h < allDataPrecipBitmap.Height; h++)
                    {
                        Color pixel = allDataPrecipBitmap.GetPixel(w, h);

                        if (!(pixel.R == 0 && pixel.G == 0 && pixel.B == 0))
                        {
                            double prec = GetPrecFromColor(pixel);
                            allDataPrecipBitmap.SetPixel(w, h, ColorValueHandler.GetPrecipitationColor(prec));
                        }

                        pixel = allDataTempBitmap.GetPixel(w, h);

                        if (!(pixel.R == 0 && pixel.G == 0 && pixel.B == 0))
                        {
                            double temp = GetTempFromColor(pixel);
                            allDataTempBitmap.SetPixel(w, h, ColorValueHandler.GetTemperatureColor(temp));

                            //Debug.WriteLine(temp);
                        }

                    }
                }


                int sBW = allDataPrecipBitmap.Width / 10;
                int sBH = allDataPrecipBitmap.Height / 8;

                int nBW = 0;
                int nBH = 0;

                for (int i = 0; i < 78; i++)
                {

                    if (bitmapTime >= now)
                    {

                        Bitmap newPrecBmp = new Bitmap(sBW, sBH);
                        Bitmap newTempBmp = new Bitmap(sBW, sBH);

                        int tmpX = 0;
                        for (int w = nBW; w < nBW + sBW; w++)
                        {

                            int tmpY = 0;

                            for (int h = nBH; h < nBH + sBH; h++)
                            {
                                Color c = allDataPrecipBitmap.GetPixel(w, h);
                                newPrecBmp.SetPixel(tmpX, tmpY, c);

                                c = allDataTempBitmap.GetPixel(w, h);
                                newTempBmp.SetPixel(tmpX, tmpY, c);

                                tmpY++;
                            }

                            tmpX++;
                        }

                        newPrecBmp = GetPartOfBigBimtap(newPrecBmp);
                        newTempBmp = GetPartOfBigBimtap(newTempBmp);

                        newPrecBmp.Save(GetPathToDataDirectory($"{ForecastTypes.PRECIPITATION}-" + bitmapTime.ToString("yyyy-MM-dd-HH") + ".bmp"), ImageFormat.Bmp);
                        newTempBmp.Save(GetPathToDataDirectory($"{ForecastTypes.TEMPERATURE}-" + bitmapTime.ToString("yyyy-MM-dd-HH") + ".bmp"), ImageFormat.Bmp);

                    }

                    bitmapTime = bitmapTime.AddHours(1);

                    nBW += sBW;

                    if (nBW >= allDataPrecipBitmap.Width)
                    {
                        nBW = 0;
                        nBH += sBH;
                    }
                }

                dataLoaderConfig.LastUpdateDateTime = DateTime.Now;
                CreateNewConfigFile(dataLoaderConfig);
            }

            Debug.WriteLine(LOADER_NAME + ": Dokončení vytváření bitmap");

        }

        public Bitmap GetBitmap(string url)
        {

            Bitmap bitmap;

            using (WebClient client = new WebClient())
            {
                Stream stream = client.OpenRead(url);
                bitmap = new Bitmap(stream);

            }

            return bitmap;

        }

        protected override string GetPathToDataDirectory(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\medard-online\" + fileName;
        }

        private Bitmap ResizeBitmap(Bitmap b, float w, float h)
        {
            Bitmap newBitmap = new Bitmap((int)w, (int)h);

            Graphics g = Graphics.FromImage(newBitmap);

            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.DrawImage(b, 0, 0, (int)w, (int)h);

            return newBitmap;
        }

        private double GetPrecFromColor(Color pixel)
        {
            //Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            if(pixel.R == 0 && pixel.G == 0 && pixel.B == 0)
            {
                return 0;
            }

            double minValue = 1.5;
            double maxValue = 23.5;

            double stepValue = (maxValue - minValue) / (scalePrecArray.Count - 1);

            int dif = 255 * 3;
            int index = 0;

            for (int i = 0; i < scalePrecArray.Count; i++)
            {

                int actDif = Math.Abs(pixel.R - scalePrecArray[i].R) + Math.Abs(pixel.G - scalePrecArray[i].G) + Math.Abs(pixel.B - scalePrecArray[i].B);

                if (actDif < dif)
                {
                    dif = actDif;
                    index = i;

                    if (actDif == 0)
                    {
                        break;
                    }
                }


            }
            
            return minValue + index * stepValue;
        }

        private double GetTempFromColor(Color pixel)
        {
            //Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            double minValue = -25;
            double maxValue = 25;

            double stepValue = (maxValue - minValue) / (scaleTempArray.Count - 1);

            int dif = 255 * 3;
            int index = 0;

            for (int i = 0; i < scaleTempArray.Count; i++)
            {

                int actDif = Math.Abs(pixel.R - scaleTempArray[i].R) + Math.Abs(pixel.G - scaleTempArray[i].G) + Math.Abs(pixel.B - scaleTempArray[i].B);

                if(actDif < dif)
                {
                    dif = actDif;
                    index = i;

                    if(actDif == 0)
                    {
                        break;
                    }
                }

            }

            return minValue + index * stepValue;
        }

        public Forecast GetForecastPoint(DateTime forTime, PointLonLat location)
        {
            Forecast forecast = new Forecast();

            forecast.Longitude = location.Lon.ToString();
            forecast.Latitude = location.Lat.ToString();
            forecast.Time = forTime;
            forecast.AddDataSource(LOADER_NAME);

            Point targetPoint = GetPointFromBoundsAndTarget(new Size(728,528), DefaultBounds, location);

            forecast.Precipitation = GetValueFromBitmapAndPoint(GetForecastBitmap(forTime, ForecastTypes.PRECIPITATION), targetPoint, ForecastTypes.PRECIPITATION);
            forecast.Temperature = GetValueFromBitmapAndPoint(GetForecastBitmap(forTime, ForecastTypes.TEMPERATURE), targetPoint, ForecastTypes.TEMPERATURE);

            forecast.Humidity = null;
            forecast.Pressure = null;

            return forecast;
        }

        public DataLoaderConfig GetLoaderConfigFile()
        {
            return dataLoaderConfig;
        }
    }
}
