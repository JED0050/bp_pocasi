using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace AgregaceDatLib
{
    public class ColorValueHandler
    {
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

    }
}
