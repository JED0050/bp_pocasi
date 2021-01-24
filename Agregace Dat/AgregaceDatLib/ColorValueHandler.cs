using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class ColorValueHandler
    {

        //škála pro srážky

        public static double GetPrecipitationValue(Color pixel)
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

        public static Color GetPrecipitationColor(double prec)
        {
            Color col = Color.Transparent;

            if (prec == 0)
                col = Color.FromArgb(255, 0, 0, 0);
            else if (prec <= 0.1)
                col = Color.FromArgb(0, 0, 168);
            else if (prec <= 0.2)
                col = Color.FromArgb(0, 0, 252);
            else if (prec <= 0.4)
                col = Color.FromArgb(0, 108, 192);
            else if (prec <= 0.8)
                col = Color.FromArgb(0, 160, 0);
            else if (prec <= 1)
                col = Color.FromArgb(0, 188, 0);
            else if (prec <= 2)
                col = Color.FromArgb(52, 216, 0);
            else if (prec <= 4)
                col = Color.FromArgb(156, 220, 0);
            else if (prec <= 8)
                col = Color.FromArgb(224, 220, 0);
            else if (prec <= 10)
                col = Color.FromArgb(252, 88, 0);
            else if (prec <= 30)
                col = Color.FromArgb(252, 132, 0);
            else if (prec <= 60)
                col = Color.FromArgb(252, 88, 0);
            else if (prec <= 100)
                col = Color.FromArgb(255, 0, 0);
            else if (prec > 100)
                col = Color.FromArgb(255, 255, 0);

            return col;

        }


        //škála pro teplotu

        public static double GetTemperatureValue(Color pixel)
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

            return temperature;
        }

        public static Color GetTemperatureColor(double temp)
        {
            Color col = Color.Transparent;

            if (temp <= -25)
                col = Color.FromArgb(169, 0, 221);
            else if (temp <= -20)
                col = Color.FromArgb(90, 0, 228);
            else if (temp <= -15)
                col = Color.FromArgb(31, 0, 231);
            else if (temp <= -10)
                col = Color.FromArgb(0, 43, 236);
            else if (temp <= -5)
                col = Color.FromArgb(0, 142, 237);
            else if (temp <= 0)
                col = Color.FromArgb(0, 241, 237);
            else if (temp <= 3)
                col = Color.FromArgb(0, 242, 169);
            else if (temp <= 5)
                col = Color.FromArgb(0, 236, 115);
            else if (temp <= 8)
                col = Color.FromArgb(0, 214, 35);
            else if (temp <= 10)
                col = Color.FromArgb(12, 214, 0);
            else if (temp <= 13)
                col = Color.FromArgb(88, 239, 0);
            else if (temp <= 15)
                col = Color.FromArgb(145, 248, 231);
            else if (temp <= 18)
                col = Color.FromArgb(225, 250, 0);
            else if (temp <= 20)
                col = Color.FromArgb(251, 225, 0);
            else if (temp <= 25)
                col = Color.FromArgb(252, 109, 0);
            else if (temp > 25)
                col = Color.FromArgb(252, 110, 0);

            return col;

        }

    }
}
