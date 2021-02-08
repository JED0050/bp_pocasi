using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vizualizace_Dat
{
    class ForecType
    {
        private Dictionary<Color, double> scaleDic = new Dictionary<Color, double>();
        public delegate double getValDel(Color c);

        public ForecType(string type)
        {
            string scaleName = "";
            double minVal = 0;
            double step = 0;

            if (type == "prec")
            {
                Type = "prec";
                GetForecValue = GetPrecipitationValueFromPixel;
                Unit = "[mm]";
                CzForecType = "srážky";
                GraphMaxValue = 30;
                GraphMinValue = 0;

                scaleName = "škála_0.1_30.png";

                minVal = 0.1;
                step = 0.1;
            }
            else if(type == "temp")
            {
                Type = "temp";
                GetForecValue = GetTemperatureValueFromPixel;
                Unit = "[°C]";
                CzForecType = "teplota";
                GraphMaxValue = 30;
                GraphMinValue = -30;

                scaleName = "škála_-30_30.png";

                minVal = -30;
                step = 1;
            }

            string workingDirectory = Environment.CurrentDirectory;
            string dataFolder = Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Data\";
            Bitmap scaleBitmap = new Bitmap(dataFolder + scaleName);

            for (int i = 0; i < scaleBitmap.Width; i++)
            {
                Color pixel = scaleBitmap.GetPixel(i, 0);
                double value = minVal + i * step;

                if (!scaleDic.ContainsKey(pixel))
                {
                    scaleDic.Add(pixel, value);
                }
            }
        }

        public string Type { get; }

        public getValDel GetForecValue { get; }
        public string Unit { get; }
        public string CzForecType { get; }
        public double GraphMaxValue { get; }
        public double GraphMinValue { get; }

        public double GetPrecipitationValueFromPixel(Color pixel)
        {
            if ((pixel.R == 0 && pixel.G == 0 && pixel.B == 0) || (pixel.R == 255 && pixel.G == 255 && pixel.B == 255))
                return 0;

            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            if (scaleDic.ContainsKey(pixelAlpha))
            {
                return scaleDic[pixelAlpha];
            }
            else
            {
                return 0;
            }
        }


        public double GetTemperatureValueFromPixel(Color pixel)
        {
            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            if (scaleDic.ContainsKey(pixelAlpha))
            {
                return scaleDic[pixelAlpha];
            }
            else
            {
                return 0;
            }
        }

    }
}
