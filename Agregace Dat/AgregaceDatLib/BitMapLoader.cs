using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class BitmapDataLoader : DataLoader
    {
        public BitmapDataLoader()
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

        public Bitmap GetPrecipitationBitmap(DateTime forTime)
        {

            string bitmapName = "RadarBitmap" + forTime.ToString("yyyy-MM-dd-HH") + ".bmp";
            string bitmapPath = GetPathToDataDirectory(bitmapName);

            if (File.Exists(bitmapPath))
            {
                return new Bitmap(bitmapPath);
            }
            else
            {
                Bitmap radarBitmap;

                for (int i = forTime.Hour; i > 0; i--)
                {

                    for(int j = forTime.Minute; j >= 0; j--)
                    {
                        try
                        {
                            radarBitmap = GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.Year + forTime.ToString("MM") + forTime.ToString("dd") + "." + i.ToString().PadLeft(2, '0') + j.ToString().PadLeft(2, '0') + ".0.png");
                            ClearBitmap(radarBitmap);

                            radarBitmap.Save(bitmapPath, ImageFormat.Bmp);

                            return radarBitmap;
                        }
                        catch { }
                    }
                    
                }

                forTime.AddDays(-1);

                for (int i = 24; i > 0; i--)
                {
                    for (int j = 59; j >= 0; j--)
                    {
                        try
                        {
                            radarBitmap = GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.Year + forTime.ToString("MM") + forTime.ToString("dd") + "." + i.ToString().PadLeft(2, '0') + j.ToString().PadLeft(2, '0') + ".0.png");
                            ClearBitmap(radarBitmap);

                            radarBitmap.Save(bitmapPath, ImageFormat.Bmp);

                            return radarBitmap;
                        }
                        catch { }
                    }
                }

                radarBitmap = GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.Year + forTime.ToString("MM") + forTime.ToString("dd") + ".0000.0.png");
                ClearBitmap(radarBitmap);

                radarBitmap.Save(bitmapPath, ImageFormat.Bmp);

                return radarBitmap;
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
                        bitmap.SetPixel(x, y, Color.Transparent);
                    }
                    else
                    {
                        Color c = bitmap.GetPixel(x, y);

                        if (c == Color.Empty || c == Color.FromArgb(255, 0, 0, 0) || c == Color.FromArgb(255, 196, 196, 196) || c == Color.FromArgb(255, 255, 255, 255))
                        {
                            bitmap.SetPixel(x, y, Color.Transparent);
                        }
                        else
                        {
                            double precVal = GetPrecipitationFromPixel(c);

                            if(precVal == 0)
                            {
                                bitmap.SetPixel(x, y, Color.Transparent);
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

        public void SaveNewDeleteOldBmps() //1 hodina +- (nepoužitelnéprakticky krom aktuálního počasí)
        {
            DateTime now = DateTime.Now;

            DirectoryInfo dI = new DirectoryInfo(GetPathToDataDirectory(""));
            foreach (var f in dI.GetFiles("*.bmp"))
            {

                string onlyDateName = f.Name.Substring(11, 13);

                string[] timeParts = onlyDateName.Split("-");

                DateTime dateTime = new DateTime(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), 0, 0);

                if (dateTime < now) //smazání starých bitmap
                {
                    f.Delete();
                }
            }

            GetPrecipitationBitmap(now);
            GetPrecipitationBitmap(now.AddHours(1));
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

        public Bitmap GetTemperatureBitmap(DateTime forTime)
        {
            throw new NotImplementedException();    //služba poskytuje pouze srážky na aktuální čas
        }
    }
}
