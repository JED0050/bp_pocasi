using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DelaunayTriangulator;
using Newtonsoft.Json;

namespace AgregaceDatLib
{
    public class Forecast : Vertex
    {
        [XmlIgnore]
        [NonSerialized]
        private List<string> dataSources = new List<string>();

        public List<string> DataSources
        {
            get
            {
                return dataSources;
            }
        }

        public string Latitude { get; set; }                     //zeměpisná šířka "y"
               
        public string Longitude { get; set; }                  //zeměpisná délka "x"
       

        public DateTime Time { get; set; }

        public double Temperature { get; set; }     //teplota celsius
        public double Precipitation { get; set; }   //srážky = slabá (0,1 – 2,5), mírná	(2,6 – 8), silná (8 – 40), velmi silná (> 40)
        public double Humidity { get; set; }        //vlhkost
        public double Pressure { get; set; }        //tlak

        [XmlIgnore]
        [JsonIgnore]
        public double DLatitude
        {
            get
            {
                return Double.Parse(Latitude.Replace('.', ','));
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public double DLongitude
        {
            get
            {
                return Double.Parse(Longitude.Replace('.', ','));
            }
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

        public void AddDataSource(string source)
        {
            dataSources.Add(source);
        }

        [XmlIgnore]
        [JsonIgnore]
        public double GenericValue { get; set; }
    }
}
