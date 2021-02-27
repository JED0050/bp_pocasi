﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace AgregaceDatLib
{
    public class ColorValueHandler
    {
        private static Dictionary<Color, double> scalePrecDic;
        private static Dictionary<Color, double> scaleTempDic;

        static ColorValueHandler()
        {
            string workingDirectory = Environment.CurrentDirectory + @"\Data\";


            string scalePrecName = "škála_0.1_30.png";
            Bitmap scalePrecImage = new Bitmap(workingDirectory + scalePrecName);

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

            }


            string scaleTempName = "škála_-30_30.png";
            Bitmap scaleTempImage = new Bitmap(workingDirectory + scaleTempName);

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

            }

        }


        //škála pro srážky

        public static double GetPrecipitationValue(Color pixel)
        {
            if ((pixel.R == 0 && pixel.G == 0 && (pixel.B == 0 || pixel.B == 1) || (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)))
                return 0;

            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            if(scalePrecDic.ContainsKey(pixelAlpha))
            {
                return scalePrecDic[pixelAlpha];
            }
            else
            {
                Color c = GetClosestCol(pixelAlpha, ForecastTypes.PRECIPITATION);

                if(c == Color.Black)
                {
                    return 0;
                }
                else
                {
                    return scalePrecDic[c];
                }

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

            if (colorIndex > scalePrecDic.Keys.Count - 1)
                colorIndex = scalePrecDic.Keys.Count - 1;
            else if (colorIndex < 0)
                colorIndex = 0;

            return scalePrecDic.Keys.ElementAt(colorIndex);
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
                Color c = GetClosestCol(pixelAlpha, ForecastTypes.TEMPERATURE);

                if (c == Color.Black)
                {
                    return 0;
                }
                else
                {
                    return scaleTempDic[c];
                }
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

            if (colorIndex > scaleTempDic.Keys.Count - 1)
                colorIndex = scaleTempDic.Keys.Count - 1;
            else if (colorIndex < 0)
                colorIndex = 0;

            return scaleTempDic.Keys.ElementAt(colorIndex);
        }

        public static Color GetClosestCol(Color pixel, string type)
        {
            Color c = Color.Black;
            int dif = Math.Abs(pixel.R - c.R) + Math.Abs(pixel.G - c.G) + Math.Abs(pixel.B - c.B);

            Color[] colorsArray;

            if (type == "prec")
            {
                colorsArray = scalePrecDic.Keys.ToArray();
            }
            else
            {
                colorsArray = scaleTempDic.Keys.ToArray();
            }

            for (int i = 0; i < colorsArray.Length; i++)
            {

                Color p = colorsArray[i];

                int actDif = Math.Abs(pixel.R - p.R) + Math.Abs(pixel.G - p.G) + Math.Abs(pixel.B - p.B);

                if (actDif < dif)
                {
                    dif = actDif;
                    c = p;

                    if(dif == 0)
                    {
                        break;
                    }
                }

            }

            return c;
        }

        public static bool IsColorKnown(Color pixel, string type)
        {
            if(pixel.R == 0 && pixel.G == 0 && (pixel.B == 0 || pixel.B == 1))
            {
                return true;
            }
            else if(ForecastTypes.IsTypeKnown(type))
            {
                if(type == ForecastTypes.PRECIPITATION)
                {
                    return scalePrecDic.ContainsKey(pixel);
                }
                else
                {
                    return scaleTempDic.ContainsKey(pixel);
                }
            }
            else
            {
                return false;
            }
        }

        public static double GetValueForColorAndType(Color col, string type)
        {
            if(type == ForecastTypes.PRECIPITATION)
            {
                return GetPrecipitationValue(col);
            }
            else
            {
                return GetTemperatureValue(col);
            }
        }

        public static Color GetColorForValueAndType(double val, string type)
        {
            if (type == ForecastTypes.PRECIPITATION)
            {
                return GetPrecipitationColor(val);
            }
            else
            {
                return GetTemperatureColor(val);
            }
        }
    }
}
