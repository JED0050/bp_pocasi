﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace AgregaceDatLib
{
    public class BitmapLoader2 : DataLoader
    {
        public Bitmap GetBigPrecipitationBitmap(DateTime forTime)   //všechny data evropa
        {

            forTime = forTime.AddMinutes(30);
            forTime = new DateTime(forTime.Year, forTime.Month, forTime.Day, forTime.Hour, 0, 0);

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {
                string onlyDateName = f.Name.Substring(0, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if (dateTime == forTime)
                {
                    return new Bitmap(f.FullName);
                }
            }

            throw new Exception("Bitmapa pro požadovaný čas nebyla nalezena");
        }

        public Bitmap GetPrecipitationBitmap(DateTime forTime)  //celá ČR
        {
            PointLonLat topLeft = new PointLonLat(10.88, 51.88);
            PointLonLat botRight  = new PointLonLat(20.21, 47.09);

            return GetPrecipitationBitmap(forTime, topLeft, botRight);
        }

        public Bitmap GetPrecipitationBitmap(DateTime forTime, PointLonLat topLeft, PointLonLat botRight)
        {

            Bitmap image = GetBigPrecipitationBitmap(forTime);

            //56.13636792495879 -21.123962402343754 - levý horní bod
            //56.134837449127744 41.11633300781251 - pravý horní bod
            //33.25246979589199 29.696044921875 - pravý dolní bod
            //59.68195494710389 9.96940612792969 - střed horní bod

            Bitmap bigBitmap = ResizeBitmap(image, 980, 600);

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

            return ResizeBitmap(smallBitmap, 728, 528);
        }

        public void SaveNewDeleteOldBmps(int days)
        {
            //odstranění starých bitmap
            DateTime now = DateTime.Now;

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                string onlyDateName = f.Name.Substring(0, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if (dateTime < now) //smazání starých bitmap
                {
                    f.Delete();
                }
            }

            //vytvoření nových bitmap

            Bitmap allDataBitmap = new Bitmap(1,1);

            DateTime bitmapTime = now;

            bitmapTime = bitmapTime.AddHours(- bitmapTime.Hour % 6);

            for(int i = 0; i < 8; i++)
            {           
                string timePart = bitmapTime.ToString("yyMMdd_HH");    //201112_12
                string url = @"http://www.medard-online.cz/apipreview?run=" + timePart + @"&forecast=precip&layer=eu";
                
                try
                {
                    allDataBitmap = GetBitmap(url);
                    break;
                }
                catch
                {

                }

                bitmapTime = bitmapTime.AddHours(-6);
            }

            //

            for(int w = 0; w < allDataBitmap.Width; w++)
            {
                for(int h = 0; h < allDataBitmap.Height; h++)
                {
                    double prec = GetPrecFromColor(allDataBitmap.GetPixel(w,h));

                    Forecast f = new Forecast() { Precipitation = prec };

                    allDataBitmap.SetPixel(w,h, f.GetPrecipitationColor());
                }
            }


            if(allDataBitmap.Width == 1 || allDataBitmap.Height == 1)
            {
                throw new Exception("Nebyla nalezena žádná bitmapa s daty o počasí");
            }

            int sBW = allDataBitmap.Width / 10;
            int sBH = allDataBitmap.Height / 8;

            int nBW = 0;
            int nBH = 0;

            for (int i = 0; i < 78; i++)
            {

                if (bitmapTime >= now)
                {

                    Bitmap newBmp = new Bitmap(sBW, sBH);

                    int tmpX = 0;
                    for(int w = nBW; w < nBW + sBW; w++)
                    {

                        int tmpY = 0;

                        for (int h = nBH; h < nBH + sBH; h++)
                        {
                            Color c = allDataBitmap.GetPixel(w, h);

                            newBmp.SetPixel(tmpX, tmpY, c);

                            tmpY++;
                        }

                        tmpX++;
                    }

                    newBmp.Save(GetPathToDataDirectory(bitmapTime.ToString("yyyy-MM-dd-HH") + ".bmp"), ImageFormat.Bmp);

                }

                bitmapTime = bitmapTime.AddHours(1);

                nBW += sBW;

                if(nBW >= allDataBitmap.Width)
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

        private Bitmap ResizeBitmap(Bitmap b, float w, float h)
        {
            float width = w;
            float height = h;
            var brush = new SolidBrush(Color.Transparent);

            float scale = Math.Min(width / b.Width, height / b.Height);

            var bmp = new Bitmap((int)width, (int)height);
            var graph = Graphics.FromImage(bmp);

            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;

            var scaleWidth = (int)(b.Width * scale);
            var scaleHeight = (int)(b.Height * scale);

            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(b, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);

            return bmp;
        }

        private double GetPrecFromColor(Color pixel)
        {
            Color colorWithoutAlfa = Color.FromArgb(0, pixel.R, pixel.G, pixel.B);
            int colorValue = colorWithoutAlfa.ToArgb();

            double precipitation = 0;

            if (colorValue < 255)
                precipitation = 0;
            else
            {
                //double colDif = Color.FromArgb(0, 255, 73, 255).ToArgb() - Color.FromArgb(0, 0, 0, 255).ToArgb();
                //double valDif = 35.5;

                //precipitation = valDif * (colDif / colorValue);

                if (colorValue == 255)
                    precipitation = 0.1;
                else if (colorValue <= Color.FromArgb(0, 0, 56, 231).ToArgb())
                {
                    precipitation = 0.4;
                }
                else if (colorValue <= Color.FromArgb(0, 0, 100, 211).ToArgb())
                {
                    precipitation = 1;
                }
                else if (colorValue <= Color.FromArgb(0, 0, 184, 158).ToArgb())
                {
                    precipitation = 2.5;
                }
                else if (colorValue <= Color.FromArgb(0, 20, 230, 0).ToArgb())
                {
                    precipitation = 5.5;
                }
                else if (colorValue <= Color.FromArgb(0, 167, 230, 0).ToArgb())
                {
                    precipitation = 8.5;
                }
                else if (colorValue <= Color.FromArgb(0, 235, 185, 0).ToArgb())
                {
                    precipitation = 11.5;
                }
                else if (colorValue <= Color.FromArgb(0, 240, 131, 0).ToArgb())
                {
                    precipitation = 14.5;
                }
                else if (colorValue <= Color.FromArgb(0, 248, 66, 0).ToArgb())
                {
                    precipitation = 17.5;
                }
                else if (colorValue <= Color.FromArgb(0, 253, 19, 0).ToArgb())
                {
                    precipitation = 20.5;
                }
                else if (colorValue <= Color.FromArgb(0, 255, 0, 41).ToArgb())
                {
                    precipitation = 23.5;
                }
                else if (colorValue <= Color.FromArgb(0, 255, 0, 113).ToArgb())
                {
                    precipitation = 26.5;
                }
                else if (colorValue <= Color.FromArgb(0, 255, 0, 177).ToArgb())
                {
                    precipitation = 29.5;
                }
                else if (colorValue <= Color.FromArgb(0, 255, 0, 227).ToArgb())
                {
                    precipitation = 32.5;
                }
                else if (colorValue <= Color.FromArgb(0, 255, 73, 255).ToArgb())
                {
                    precipitation = 35.5;
                }

                //Console.WriteLine($"prec: {precipitation} cV: {colorValue} col: {pixel}");
            }

            //Console.WriteLine($"prec: {precipitation} cV: {colorValue} col: {pixel}");

            return precipitation;
        }
    }
}