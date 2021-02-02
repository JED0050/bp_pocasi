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
        private Bitmap scaleBitmap;
        public delegate double getValDel(Color c);

        public ForecType(string type)
        {
            string scaleName = "";

            if (type == "prec")
            {
                Type = "prec";
                GetForecValue = GetPrecipitationValueFromPixel;
                Unit = "[mm]";
                CzForecType = "srážky";
                GraphMaxValue = 30;
                GraphMinValue = 0;

                scaleName = "škála_0.1_30.png";
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
            }

            string workingDirectory = Environment.CurrentDirectory;
            string dataFolder = Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Data\";

            scaleBitmap = new Bitmap(dataFolder + scaleName);


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

            double stepValue = 0.1;
            double minValue = 0.1;

            for (int i = 0; i < scaleBitmap.Width; i++)
            {

                if (pixelAlpha == scaleBitmap.GetPixel(i, 0))
                {
                    return minValue + stepValue * i;
                }

            }

            return 0;
        }


        public double GetTemperatureValueFromPixel(Color pixel)
        {
            Color pixelAlpha = Color.FromArgb(255, pixel.R, pixel.G, pixel.B);

            int stepValue = 1;
            int minValue = -30;

            for (int i = 0; i < scaleBitmap.Width; i++)
            {

                if (pixelAlpha == scaleBitmap.GetPixel(i, 0))
                {
                    return minValue + stepValue * i;
                }

            }

            return 0;
        }

    }
}
