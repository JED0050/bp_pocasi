using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AgregaceDatLib
{
    public class BitmapHelper
    {
        protected byte AvgCol(byte a, byte b)
        {
            return (byte)((a + b) / 2);
        }

        protected void DrawIntersectionCircle(int size, int x, int y, Bitmap forBitmap, Color newC)
        {
            for (double r = 1; r <= size; r += 1)
            {
                double rr = Math.Pow(r, 2);

                for (int i = x - (int)r; i <= x + r; i++)
                    for (int j = y - (int)r; j <= y + r; j++)
                        if (Math.Abs(Math.Pow(i - x, 2) + Math.Pow(j - y, 2) - rr) <= r)
                        {
                            Color avgC;

                            if (forBitmap.GetPixel(i, j) == Color.FromArgb(0, 0, 0, 0))     //nastavení barvy pokud zatím na daném pixelu žádná není
                            {
                                avgC = newC;
                            }
                            else                                                            //nastavení průměrné barvy (průnik barev)
                            {
                                Color oldC = forBitmap.GetPixel(i, j);                      
                                avgC = Color.FromArgb(AvgCol(oldC.R, newC.R), AvgCol(oldC.G, newC.G), AvgCol(oldC.B, newC.B));
                            }

                            forBitmap.SetPixel(i, j, avgC);
                        }
            }
        }

        protected void DrawCircle(int size, int x, int y, Bitmap forBitmap, Color newC)
        {
            for (double r = 1; r <= size; r += 1)
            {
                double rr = Math.Pow(r, 2);

                for (int i = x - (int)r; i <= x + r; i++)
                    for (int j = y - (int)r; j <= y + r; j++)
                        if (Math.Abs(Math.Pow(i - x, 2) + Math.Pow(j - y, 2) - rr) <= r)
                            forBitmap.SetPixel(i, j, newC);

            }

        }

    }

}
