using System;

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
    }
}
