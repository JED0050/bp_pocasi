using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vizualizace_Dat
{
    class ForecType
    {
        public delegate double getValDel(Color c);

        public ForecType(string type)
        {

            if(type == "prec")
            {
                Type = "prec";
                GetForecValue = GetPrecipitationValueFromPixel;
                Unit = "[mm]";
                CzForecType = "srážky";
                GraphMaxValue = 30;
                GraphMinValue = 0;
            }
            else if(type == "temp")
            {
                Type = "temp";
                GetForecValue = GetTemperatureValueFromPixel;
                Unit = "[°C]";
                CzForecType = "teplota";
                GraphMaxValue = 30;
                GraphMinValue = -30;
            }
        

        }

        public string Type { get; }

        public getValDel GetForecValue { get; }
        public string Unit { get; }
        public string CzForecType { get; }
        public double GraphMaxValue { get; }
        public double GraphMinValue { get; }

        public static double GetPrecipitationValueFromPixel(Color pixel)
        {
            double precipitation = 0;

            Color colorWithoutAlfa = Color.FromArgb(0, pixel.R, pixel.G, pixel.B);
            int colorValue = colorWithoutAlfa.ToArgb();

            /*
            srážky dle RGB barev

            index - R G B - srážky

            0 - 0 0 0 - 0 mm/h
            1 - 0 0 1 - 0 mm/h

            2 - 0 0 168 - 0.1 mm/h
            3 - 0 0 252 - 0.2 mm/h
            4 - 0 108 192 - 0.4 mm/h
            5 - 0 160 0 - 0.8 mm/h

            6 - 0 188 0 - 1 mm/h
            7 - 52 216 0 - 2 mm/h
            8 - 156 220 0 - 4 mm/h
            9 - 224 220 0 - 8 mm/h

            10 - 252 88 0 - 10 mm/h
            11 - 252 132 0 - 30 mm/h
            12 - 252 176 0 - 60 mm/h

            13 - 255 0 0 - 100 mm/h
            14 - 255 255 0 - 101+ mm/h
            */

            if (colorValue <= Color.FromArgb(0, 0, 0, 1).ToArgb())
            {
                precipitation = 0;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 0, 168).ToArgb())
            {
                precipitation = 0.1;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 0, 252).ToArgb())
            {
                precipitation = 0.2;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 108, 192).ToArgb())
            {
                precipitation = 0.4;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 160, 0).ToArgb())
            {
                precipitation = 0.8;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 188, 0).ToArgb())
            {
                precipitation = 1;
            }
            else if (colorValue <= Color.FromArgb(0, 52, 216, 0).ToArgb())
            {
                precipitation = 2;
            }
            else if (colorValue <= Color.FromArgb(0, 156, 220, 0).ToArgb())
            {
                precipitation = 4;
            }
            else if (colorValue <= Color.FromArgb(0, 224, 220, 0).ToArgb())
            {
                precipitation = 8;
            }
            else if (colorValue <= Color.FromArgb(0, 252, 88, 0).ToArgb())
            {
                precipitation = 10;
            }
            else if (colorValue <= Color.FromArgb(0, 252, 132, 0).ToArgb())
            {
                precipitation = 30;
            }
            else if (colorValue <= Color.FromArgb(0, 252, 176, 0).ToArgb())
            {
                precipitation = 60;
            }
            else if (colorValue <= Color.FromArgb(0, 255, 0, 0).ToArgb())
            {
                precipitation = 100;
            }
            else if (colorValue <= Color.FromArgb(0, 255, 255, 0).ToArgb())
            {
                precipitation = 101;
            }

            return precipitation;
        }


        public static double GetTemperatureValueFromPixel(Color pixel)
        {
            double temperature = 0;

            Color colorWithoutAlfa = Color.FromArgb(0, pixel.R, pixel.G, pixel.B);
            int colorValue = colorWithoutAlfa.ToArgb();


            /*
            
            index - R G B - teplota

            0 - 169 0 221 - -25
            1 - 90 0 228 - -20
            2 - 31 0 231 - -15
            3 - 0 43 236 - -10
            4 - 0 142 237 - -5
            5 - 0 241 237 - 0
            6 - 0 242 169 - 3
            7 - 0 236 115 - 5
            8 - 0 214 35 - 8
            9 - 12 214 0 - 10
            10 - 88 239 0 - 13
            11 - 145 248 231 - 15
            12 - 225 250 0 - 18
            13 - 251 225 0 - 20
            14 - 252 109 0 - 25
            15 - 252 110 0 - 30+

            */


            if (colorValue <= Color.FromArgb(0, 169, 0, 221).ToArgb() && colorValue > Color.FromArgb(0, 90, 0, 228).ToArgb() && pixel.G == 0)
            {
                temperature = -25;
            }
            else if (colorValue <= Color.FromArgb(0, 90, 0, 228).ToArgb() && colorValue > Color.FromArgb(0, 31, 0, 231).ToArgb() && pixel.G == 0)
            {
                temperature = -20;
            }
            else if (colorValue <= Color.FromArgb(0, 31, 0, 231).ToArgb() && colorValue > Color.FromArgb(0, 0, 43, 236).ToArgb() && pixel.G == 0)
            {
                temperature = -15;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 43, 236).ToArgb() && colorValue < Color.FromArgb(0, 0, 142, 237).ToArgb() && pixel.R == 0 && pixel.G < 199)
            {
                temperature = -10;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 142, 237).ToArgb() && colorValue < Color.FromArgb(0, 0, 241, 237).ToArgb() && pixel.R == 0 && pixel.G < 199)
            {
                temperature = -5;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 241, 237).ToArgb() && colorValue < Color.FromArgb(0, 0, 242, 169).ToArgb())
            {
                temperature = 0;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 242, 169).ToArgb() && colorValue > Color.FromArgb(0, 0, 236, 115).ToArgb())
            {
                temperature = 3;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 236, 115).ToArgb() && colorValue > Color.FromArgb(0, 0, 214, 35).ToArgb())
            {
                temperature = 5;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 214, 35).ToArgb() && colorValue < Color.FromArgb(0, 12, 214, 0).ToArgb())
            {
                temperature = 8;
            }
            else if (colorValue >= Color.FromArgb(0, 12, 214, 0).ToArgb() && colorValue < Color.FromArgb(0, 88, 239, 0).ToArgb())
            {
                temperature = 10;
            }
            else if (colorValue >= Color.FromArgb(0, 88, 239, 0).ToArgb() && colorValue < Color.FromArgb(0, 145, 248, 231).ToArgb())
            {
                temperature = 13;
            }
            else if (colorValue >= Color.FromArgb(0, 145, 248, 231).ToArgb() && colorValue < Color.FromArgb(0, 225, 250, 0).ToArgb())
            {
                temperature = 15;
            }
            else if (colorValue >= Color.FromArgb(0, 225, 250, 0).ToArgb() && colorValue < Color.FromArgb(0, 251, 225, 0).ToArgb())
            {
                temperature = 18;
            }
            else if (colorValue >= Color.FromArgb(0, 251, 225, 0).ToArgb() && colorValue < Color.FromArgb(0, 252, 109, 0).ToArgb())
            {
                temperature = 20;
            }
            else if (colorValue >= Color.FromArgb(0, 252, 109, 0).ToArgb() && colorValue < Color.FromArgb(0, 252, 110, 0).ToArgb())
            {
                temperature = 25;
            }
            else if (colorValue >= Color.FromArgb(0, 252, 110, 0).ToArgb())
            {
                temperature = 30;
            }

            //Debug.WriteLine(temperature + " " + pixel);

            return temperature;
        }

    }
}
