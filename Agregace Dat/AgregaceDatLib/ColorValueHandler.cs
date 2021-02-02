using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class ColorValueHandler
    {
        private static string workingDirectory = Environment.CurrentDirectory + @"\Data\";

        private static string scalePrecName = "škála_0.1_30.png";
        private static Bitmap scalePrecImage = new Bitmap(workingDirectory + scalePrecName);

        private static string scaleTempName = "škála_-30_30.png";
        private static Bitmap scaleTempImage = new Bitmap(workingDirectory + scaleTempName);



        //škála pro srážky

        public static double GetPrecipitationValue(Color pixel)
        {
            if ((pixel.R == 0 && pixel.G == 0 && pixel.B == 0) || (pixel.R == 255 && pixel.G == 255 && pixel.B == 255))
                return 0;

            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            double stepValue = 0.1;
            double minValue = 0.1;

            for (int i = 0; i < scalePrecImage.Width; i++)
            {

                if (pixelAlpha == scalePrecImage.GetPixel(i, 0))
                {
                    return minValue + stepValue * i;
                }

            }

            return 0;
        }

        public static Color GetPrecipitationColor(double prec)
        {
            //return Color.Red;

            if (prec < 0.1)
            {
                return Color.Black;
            }

            double minVal = 0.1;
            double maxVal = 30;

            if (prec > maxVal)
                prec = maxVal;

            int colorIndex = (int)Math.Round(prec * 10) - (int)(minVal * 10);

            //Debug.WriteLine("p: " + prec + " " + colorIndex);

            return scalePrecImage.GetPixel(colorIndex, 0);

        }


        //škála pro teplotu

        public static double GetTemperatureValue(Color pixel)
        {
            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            int stepValue = 1;
            int minValue = -30;

            for(int i = 0; i < scaleTempImage.Width; i++)
            {

                if(pixelAlpha == scaleTempImage.GetPixel(i,0))
                {
                    return minValue + stepValue * i;
                }

            }

            return 0;
        }

        public static Color GetTemperatureColor(double temp)
        {

            double minVal = -30;
            double maxVal = 30;

            if (temp < minVal)
                temp = minVal;
            else if (temp > maxVal)
                temp = maxVal;

            int colorIndex = (int)Math.Round(temp) + (-(int)minVal);

            //Debug.WriteLine("t: " + temp + " " + colorIndex);

            return scaleTempImage.GetPixel(colorIndex, 0);

        }

    }
}
