using System;
using System.Drawing;
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

            for(int i = forTime.Hour; i > 0; i--)
            {
                try
                {
                    return GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.Year + forTime.ToString("MM") + forTime.ToString("dd") + "." + i.ToString().PadLeft(2,'0') + "00.0.png");
                }
                catch { }
            }

            forTime.AddDays(-1);

            for (int i = 24; i > 0; i--)
            {
                try
                {
                    return GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.Year + forTime.ToString("MM") + forTime.ToString("dd") + "." + i.ToString().PadLeft(2, '0') + "00.0.png");
                }
                catch { }
            }

            return GetBitmap("http://radar.bourky.cz/data/pacz2gmaps.z_max3d." + forTime.Year + forTime.ToString("MM") + forTime.ToString("dd") + ".0000.0.png");
        }
    }
}
