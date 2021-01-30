using System;
using System.Drawing;
using DelaunayTriangulator;

namespace AgregaceDatLib
{
    public class Forecast : Vertex
    {

        public string City { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }        //zeměpisná šířka "y"
        public string Longitude { get; set; }       //zeměpisná délka "x"
        public DateTime Time { get; set; }
        public double Temperature { get; set; }     //teplota celsius
        public double Precipitation { get; set; }   //srážky = slabá (0,1 – 2,5), mírná	(2,6 – 8), silná (8 – 40), velmi silná (> 40)

        public double DLatitude
        {
            get
            {
                return Double.Parse(Latitude.Replace('.', ','));
            }
        }

        public double DLongitude
        {
            get
            {
                return Double.Parse(Longitude.Replace('.', ','));
            }
        }


        public override string ToString()
        {
            string output = "Country: " + Country + " City: " + City + " Time: " + Time + " Temperature: " + Temperature + " Precipitation: " + Precipitation;

            return output;
        }

        public void SetXY(int bW, int bH)
        {
            double lonDif = 20.21 - 10.06;
            double latDif = 51.88 - 47.09;

            double PixelLon = lonDif / bW;
            double PixelLat = latDif / bH;

            double bY = 51.88;
            double bX = 10.06;

            double locLon = DLongitude;
            double locLat = DLatitude;

            int x;
            for (x = 0; x < bW; x++)
            {
                if (bX >= locLon && locLon <= bX + PixelLon)
                    break;

                bX += PixelLon;
            }

            if (x == bW)
                x--;

            this.x = x;

            int y;
            for (y = 0; y < bH; y++)
            {
                if (bY - PixelLat <= locLat && locLat <= bY)
                    break;

                bY -= PixelLat;
            }

            if (y == bH)
                y--;

            this.y = y;
        }

    }
}
