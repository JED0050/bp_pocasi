using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class ColorValueHandler
    {
        private static Color[] scalePrecArray;
        private static Dictionary<Color, double> scalePrecDic;

        private static Color[] scaleTempArray;
        private static Dictionary<Color, double> scaleTempDic;

        static ColorValueHandler()
        {
            string workingDirectory = Environment.CurrentDirectory + @"\Data\";


            string scalePrecName = "škála_0.1_30.png";
            Bitmap scalePrecImage = new Bitmap(workingDirectory + scalePrecName);

            scalePrecArray = new Color[scalePrecImage.Width];
            scalePrecDic = new Dictionary<Color, double>();

            double minVal = 0.1;
            double step = 0.1;

            for (int i = 0; i < scalePrecImage.Width; i++)
            {
                Color pixel = scalePrecImage.GetPixel(i, 0);
                double value = minVal + i * step;

                if (!scalePrecDic.ContainsKey(pixel))
                {
                    scalePrecDic.Add(pixel, value);
                }

                scalePrecArray[i] = pixel;
            }


            string scaleTempName = "škála_-30_30.png";
            Bitmap scaleTempImage = new Bitmap(workingDirectory + scaleTempName);

            scaleTempArray = new Color[scaleTempImage.Width];
            scaleTempDic = new Dictionary<Color, double>();

            minVal = -30;
            step = 1;

            for (int i = 0; i < scaleTempImage.Width; i++)
            {
                Color pixel = scaleTempImage.GetPixel(i, 0);
                double value = minVal + i * step;

                if (!scaleTempDic.ContainsKey(pixel))
                {
                    scaleTempDic.Add(pixel, value);
                    //Debug.WriteLine(value);
                }

                scaleTempArray[i] = pixel;
            }

        }


        //škála pro srážky

        public static double GetPrecipitationValue(Color pixel)
        {
            if ((pixel.R == 0 && pixel.G == 0 && pixel.B == 0) || (pixel.R == 255 && pixel.G == 255 && pixel.B == 255))
                return 0;

            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            if(scalePrecDic.ContainsKey(pixelAlpha))
            {
                return scalePrecDic[pixelAlpha];
            }
            else
            {
                return 0;
            }
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

            return scalePrecArray[colorIndex];

        }


        //škála pro teplotu

        public static double GetTemperatureValue(Color pixel)
        {

            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            //int stepValue = 1;
            //int minValue = -30;

            if (scaleTempDic.ContainsKey(pixelAlpha))
            {
                return scaleTempDic[pixelAlpha];
            }
            else
            {
                return 0;
            }
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

            return scaleTempArray[colorIndex];

        }

    }
}
