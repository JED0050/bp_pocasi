using System;
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
        private static Dictionary<Color, double> scalePresDic;
        private static Dictionary<Color, double> scaleHumiDic;

        static ColorValueHandler()
        {
            string workingDirectory = Environment.CurrentDirectory + @"\Data\";


            string scalePrecName = "scale_prec.png";
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


            string scaleTempName = "scale_temp.png";
            Bitmap scaleTempImage = new Bitmap(workingDirectory + scaleTempName);

            scaleTempDic = new Dictionary<Color, double>();

            minVal = -50;
            step = 1;

            for (int i = 0; i < scaleTempImage.Width; i++)
            {
                Color pixel = scaleTempImage.GetPixel(i, 0);
                double value = minVal + i * step;

                scaleTempDic.Add(pixel, value);
            }


            string scalePresName = "scale_pres.png";
            Bitmap scalePresImage = new Bitmap(workingDirectory + scalePresName);

            scalePresDic = new Dictionary<Color, double>();

            minVal = 870;
            step = 1;

            for (int i = 0; i < scalePresImage.Width; i++)
            {
                Color pixel = scalePresImage.GetPixel(i, 0);
                double value = minVal + i * step;

                scalePresDic.Add(pixel, value);

            }


            string scaleHumiName = "scale_humi.png";
            Bitmap scaleHumiImage = new Bitmap(workingDirectory + scaleHumiName);

            scaleHumiDic = new Dictionary<Color, double>();

            minVal = 1;
            step = 1;

            for (int i = 0; i < scaleHumiImage.Width; i++)
            {
                Color pixel = scaleHumiImage.GetPixel(i, 0);
                double value = minVal + i * step;

                scaleHumiDic.Add(pixel, value);
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
            prec = Math.Round(prec, 1);

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
            double minVal = -50;
            double maxVal = 50;

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

        //škála pro tlak

        public static double GetPressureValue(Color pixel)
        {
            if ((pixel.R == 0 && pixel.G == 0 && (pixel.B == 0 || pixel.B == 1) || (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)))
                return 870;

            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            if (scalePresDic.ContainsKey(pixelAlpha))
            {
                return scalePresDic[pixelAlpha];
            }
            else
            {
                Color c = GetClosestCol(pixelAlpha, ForecastTypes.PRESSURE);

                return scalePresDic[c];
            }
        }

        public static Color GetPressureColor(double pres)
        {
            double minVal = 870;
            double maxVal = 1084;

            if (pres < minVal)
            {
                pres = minVal;
            }
            else if (pres > maxVal)
            {
                pres = maxVal;
            }

            int colorIndex = (int)Math.Round(pres) - (int)(minVal);

            return scalePresDic.Keys.ElementAt(colorIndex);
        }

        //škála pro vlhkost vzduchu

        public static double GetHumidityValue(Color pixel)
        {
            if ((pixel.R == 0 && pixel.G == 0 && (pixel.B == 0 || pixel.B == 1) || (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)))
                return 1;

            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            if (scaleHumiDic.ContainsKey(pixelAlpha))
            {
                return scaleHumiDic[pixelAlpha];
            }
            else
            {
                Color c = GetClosestCol(pixelAlpha, ForecastTypes.HUMIDITY);

                return scaleHumiDic[c];
            }
        }

        public static Color GetHumidityColor(double humi)
        {
            double minVal = 1;
            double maxVal = 100;

            if (humi < minVal)
            {
                humi = minVal;
            }
            else if (humi > maxVal)
            {
                humi = maxVal;
            }

            int colorIndex = (int)Math.Round(humi) - (int)(minVal);

            return scaleHumiDic.Keys.ElementAt(colorIndex);
        }

        public static Color GetClosestCol(Color pixel, string type)
        {
            Color c = Color.Black;
            int dif = Math.Abs(pixel.R - c.R) + Math.Abs(pixel.G - c.G) + Math.Abs(pixel.B - c.B);

            Color[] colorsArray;

            if (type == ForecastTypes.PRECIPITATION)
            {
                colorsArray = scalePrecDic.Keys.ToArray();
            }
            else if (type == ForecastTypes.TEMPERATURE)
            {
                colorsArray = scaleTempDic.Keys.ToArray();
            }
            else if (type == ForecastTypes.PRESSURE)
            {
                colorsArray = scalePresDic.Keys.ToArray();
            }
            else if (type == ForecastTypes.HUMIDITY)
            {
                colorsArray = scaleHumiDic.Keys.ToArray();
            }
            else
            {
                throw new Exception("Neznámý typ předpovědi");
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
                else if (type == ForecastTypes.TEMPERATURE)
                {
                    return scaleTempDic.ContainsKey(pixel);
                }
                else if (type == ForecastTypes.PRESSURE)
                {
                    return scalePresDic.ContainsKey(pixel);
                }
                else
                {
                    return scaleHumiDic.ContainsKey(pixel);
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
            else if (type == ForecastTypes.TEMPERATURE)
            {
                return GetTemperatureValue(col);
            }
            else if (type == ForecastTypes.PRESSURE)
            {
                return GetPressureValue(col);
            }
            else
            {
                return GetHumidityValue(col);
            }
        }

        public static Color GetColorForValueAndType(double val, string type)
        {
            if (type == ForecastTypes.PRECIPITATION)
            {
                return GetPrecipitationColor(val);
            }
            else if (type == ForecastTypes.TEMPERATURE)
            {
                return GetTemperatureColor(val);
            }
            else if (type == ForecastTypes.PRESSURE)
            {
                return GetPressureColor(val);
            }
            else
            {
                return GetHumidityColor(val);
            }
        }
    }
}
