using System;
using System.Drawing;

namespace AgregaceDatLib
{
    public class Forecast
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
                return Double.Parse(Latitude.Replace('.',','));
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

        internal void AddForecast(Forecast newF)
        {
            Temperature += newF.Temperature;
            Precipitation += newF.Precipitation;
        }

        internal void SetAvgForecast(int numOfFcs)
        {
            Temperature = Temperature / numOfFcs;
            Precipitation = Precipitation / numOfFcs;
        }

        public Color GetPrecipitationColor()
        {
            Color col = Color.FromArgb(0, 0, 0);

            if (Precipitation == 0)
                col = Color.FromArgb(0, 0, 0);
            else if (Precipitation > 0 && Precipitation <= 0.1)
                col = Color.FromArgb(48, 0, 168);
            else if (Precipitation > 0.1 && Precipitation <= 0.2)
                col = Color.FromArgb(0, 0, 252);
            else if (Precipitation > 0.2 && Precipitation <= 0.4)
                col = Color.FromArgb(0, 108, 192);
            else if (Precipitation > 0.4 && Precipitation <= 0.8)
                col = Color.FromArgb(0, 160, 0);
            else if (Precipitation > 0.8 && Precipitation <= 1)
                col = Color.FromArgb(0, 188, 0);
            else if (Precipitation > 1 && Precipitation <= 2)
                col = Color.FromArgb(52, 216, 0);
            else if (Precipitation > 2 && Precipitation <= 4)
                col = Color.FromArgb(156, 220, 0);
            else if (Precipitation > 4 && Precipitation <= 8)
                col = Color.FromArgb(224, 220, 0);
            else if (Precipitation > 8 && Precipitation <= 10)
                col = Color.FromArgb(252, 176, 0);
            else if (Precipitation > 10 && Precipitation <= 30)
                col = Color.FromArgb(252, 132, 0);
            else if (Precipitation > 30 && Precipitation <= 60)
                col = Color.FromArgb(252, 88, 0);
            else if (Precipitation > 60 && Precipitation <= 100)
                col = Color.FromArgb(252, 0, 0);
            else if (Precipitation > 100)
                col = Color.FromArgb(160, 0, 0);

            return col;
                
        }
    }
}
