using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AgregaceDatLib
{
    public class BitmapCustomDraw
    {

        private float sign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
        protected bool PointInTriangle(Point pt, Point v1, Point v2, Point v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        protected Color GetCollorInTriangle(Point p, Point v1, Point v2, Point v3, Color c1, Color c2, Color c3)
        {
            double dis1 = Math.Sqrt(Math.Pow(v1.X - p.X, 2) + Math.Pow(v1.Y - p.Y, 2));
            double dis2 = Math.Sqrt(Math.Pow(v2.X - p.X, 2) + Math.Pow(v2.Y - p.Y, 2));
            double dis3 = Math.Sqrt(Math.Pow(v3.X - p.X, 2) + Math.Pow(v3.Y - p.Y, 2));

            double w1, w2, w3;

            if (dis1 > 0)
                w1 = 1 / dis1;
            else
                w1 = 0;

            if (dis2 > 0)
                w2 = 1 / dis2;
            else
                w2 = 0;

            if (dis3 > 0)
                w3 = 1 / dis3;
            else
                w3 = 0;

            double val1 = ColorValueHandler.GetPrecipitationValue(c1);
            double val2 = ColorValueHandler.GetPrecipitationValue(c2);
            double val3 = ColorValueHandler.GetPrecipitationValue(c3);

            double avgVal = (w1 * val1 + w2 * val2 + w3 * val3) / (w1 + w2 + w3);

            return ColorValueHandler.GetPrecipitationColor(avgVal);
        }
        protected Color GetCollorInTriangleOld(Point p, Point v1, Point v2, Point v3, Color c1, Color c2, Color c3)
        {
            double dis1 = Math.Sqrt(Math.Pow(v1.X - p.X, 2) + Math.Pow(v1.Y - p.Y, 2));
            double dis2 = Math.Sqrt(Math.Pow(v2.X - p.X, 2) + Math.Pow(v2.Y - p.Y, 2));
            double dis3 = Math.Sqrt(Math.Pow(v3.X - p.X, 2) + Math.Pow(v3.Y - p.Y, 2));

            double w1, w2, w3;

            if (dis1 > 0)
                w1 = 1 / dis1;
            else
                w1 = 0;

            if (dis2 > 0)
                w2 = 1 / dis2;
            else
                w2 = 0;

            if (dis3 > 0)
                w3 = 1 / dis3;
            else
                w3 = 0;


            double r = (w1 * c1.R + w2 * c2.R + w3 * c3.R) / (w1 + w2 + w3);

            if (r > 255)
                r = 255;
            else if (r < 0)
                r = 0;

            double g = (w1 * c1.G + w2 * c2.G + w3 * c3.G) / (w1 + w2 + w3);

            if (g > 255)
                g = 255;
            else if (g < 0)
                g = 0;

            double b = (w1 * c1.B + w2 * c2.B + w3 * c3.B) / (w1 + w2 + w3);

            if (b > 255)
                b = 255;
            else if (b < 0)
                b = 0;


            return Color.FromArgb((int)r, (int)g, (int)b);
        }

    }

}
