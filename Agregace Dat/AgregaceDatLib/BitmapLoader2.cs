using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace AgregaceDatLib
{
    public class BitmapLoader2 : DataLoader
    {
        public Bitmap GetPrecipitationBitmap(DateTime forTime)
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
    }
}
