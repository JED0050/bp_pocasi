using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class BitmapDataLoader : DataLoader
    {

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

        public Bitmap GetForecastBitmap(DateTime forTime)
        {

            string bitmapName = "RadarBitmap" + forTime.ToString("yyyyMMddHH") + ".bmp";
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
                    }
                }
            }
        }

    }
}
