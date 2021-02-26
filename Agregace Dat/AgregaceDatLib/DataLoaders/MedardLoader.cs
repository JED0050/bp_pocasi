using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace AgregaceDatLib
{
    public class MedardDataLoader : DataLoader
    {
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

            if(scaleTempArray == null)
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"medard-online\scales\";

                string scaleTempName = "temp_scale.png";
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

                //Debug.WriteLine(scaleTempArray.Count);
            }

            if(scalePrecArray == null)
            {
                string dataDir = Environment.CurrentDirectory + @"\Data\";
                string loaderDir = dataDir + @"medard-online\scales\";

                string scalePrecName = "prec_scale.png";
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

                Debug.WriteLine(scalePrecArray.Count);
            }

        }

        public Bitmap GetBigForecastBitmap(DateTime forTime, string type)
        {
            forTime = forTime.AddMinutes(30);
            forTime = new DateTime(forTime.Year, forTime.Month, forTime.Day, forTime.Hour, 0, 0);

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {
                if (f.Name.StartsWith(type + "-"))
                {

                    string onlyDateName = f.Name.Split('.')[0];

                    string[] timeParts = onlyDateName.Split("-");

                    DateTime dateTime = new DateTime(int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), int.Parse(timeParts[4]), 0, 0);

                    if (dateTime == forTime)
                    {
                        return new Bitmap(f.FullName);
                    }

                }
            }

            throw new Exception("Bitmapa pro požadovaný čas nebyla nalezena");
        }

        public Bitmap GetBigPrecipitationBitmap(DateTime forTime)   //všechny data srážek evropa
        {
            return GetBigForecastBitmap(forTime, ForecastTypes.PRECIPITATION);
        }

        public Bitmap GetBigTemperatureBitmap(DateTime forTime)   //všechny data srážek evropa
        {
            return GetBigForecastBitmap(forTime, ForecastTypes.TEMPERATURE);
        }

        public Bitmap GetPrecipitationBitmap(DateTime forTime)  //celá ČR
        {
            PointLonLat topLeft = new PointLonLat(10.88, 51.88);
            PointLonLat botRight  = new PointLonLat(20.21, 47.09);

            return GetPartOfBigBimtap(forTime, topLeft, botRight, GetBigPrecipitationBitmap(forTime), ForecastTypes.PRECIPITATION);
        }

        public Bitmap GetPartOfBigBimtap(DateTime forTime, PointLonLat topLeft, PointLonLat botRight, Bitmap fullBitmap, string type)
        {

            //56.13636792495879 -21.123962402343754 - levý horní bod
            //56.134837449127744 41.11633300781251 - pravý horní bod
            //33.25246979589199 29.696044921875 - pravý dolní bod
            //59.68195494710389 9.96940612792969 - střed horní bod

            Bitmap bigBitmap = ResizeBitmap(fullBitmap, 980, 600, type);

            Point p1 = new Point();
            Point p2 = new Point();

            double lonDif = 41.11633300781251 - (-21.123962402343754);
            double latDif = 59.68195494710389 - 33.25246979589199;

            double PixelLon = lonDif / bigBitmap.Width;
            double PixelLat = latDif / bigBitmap.Height;

            double bY = 59.68195494710389;
            double bX = -21.123962402343754;

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

            bY = 59.68195494710389;
            bX = -21.123962402343754;

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

            return ResizeBitmap(smallBitmap, 728, 528, type);
        }

        public void SaveNewDeleteOldBmps()  //4 dny +-
        {
            //odstranění starých bitmap
            DateTime now = DateTime.Now;

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                string onlyDateName = f.Name.Split('.')[0];

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), int.Parse(timeParts[4]), 0, 0);

                if (dateTime < now) //smazání starých bitmap
                {
                    f.Delete();
                }
            }

            //vytvoření nových bitmap

            Bitmap allDataPrecipBitmap = new Bitmap(1,1);
            Bitmap allDataTempBitmap = new Bitmap(1, 1);

            DateTime bitmapTime = now;

            bitmapTime = bitmapTime.AddHours(- bitmapTime.Hour % 6);

            for(int i = 0; i < 8; i++)
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
                for(int h = 0; h < allDataPrecipBitmap.Height; h++)
                {
                    Color pixel = allDataPrecipBitmap.GetPixel(w, h);

                    if(!(pixel.R == 0 && pixel.G == 0 && pixel.B == 0))
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
                    for(int w = nBW; w < nBW + sBW; w++)
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

                    newPrecBmp.Save(GetPathToDataDirectory($"{ForecastTypes.PRECIPITATION}-" + bitmapTime.ToString("yyyy-MM-dd-HH") + ".bmp"), ImageFormat.Bmp);
                    newTempBmp.Save(GetPathToDataDirectory($"{ForecastTypes.TEMPERATURE}-" + bitmapTime.ToString("yyyy-MM-dd-HH") + ".bmp"), ImageFormat.Bmp);

                }

                bitmapTime = bitmapTime.AddHours(1);

                nBW += sBW;

                if(nBW >= allDataPrecipBitmap.Width)
                {
                    nBW = 0;
                    nBH += sBH;
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

            }

            return bitmap;

        }

        private string GetPathToDataDirectory(string fileName)
        {
            //string workingDirectory = Environment.CurrentDirectory;
            //return Directory.GetParent(workingDirectory).FullName + @"\Data\Radar.bourky\" + fileName;

            string workingDirectory = Environment.CurrentDirectory;
            return workingDirectory + @"\Data\medard-online\" + fileName;
        }

        private Bitmap ResizeBitmap(Bitmap b, float w, float h, string type)
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

            double stepValue = 0.33;
            double minValue = 0.1;

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
                        //Debug.WriteLine("BINGO");
                        break;
                    }
                }


            }
            
            return minValue + index * stepValue;
        }

        private double GetTempFromColor(Color pixel)
        {
            //Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            double stepValue = 0.2;
            int minValue = -28;

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
                        //Debug.WriteLine("BINGO");
                        break;
                    }
                }

            }

            return minValue + index * stepValue;
        }
        public Bitmap GetTemperatureBitmap(DateTime forTime)
        {
            PointLonLat topLeft = new PointLonLat(10.88, 51.88);
            PointLonLat botRight = new PointLonLat(20.21, 47.09);

            return GetPartOfBigBimtap(forTime, topLeft, botRight, GetBigTemperatureBitmap(forTime), ForecastTypes.TEMPERATURE);
        }

        public Forecast GetForecast(DateTime forTime, PointLonLat point)
        {
            throw new NotImplementedException();
        }
    }
}
