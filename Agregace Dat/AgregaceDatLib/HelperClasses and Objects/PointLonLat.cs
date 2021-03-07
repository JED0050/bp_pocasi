using System;
using System.Collections.Generic;
using System.Text;

namespace AgregaceDatLib
{
    public class PointLonLat
    {
        public double Lon { get; set; }
        public double Lat { get; set; }

        public PointLonLat()
        {

        }

        public PointLonLat(double lon, double lat)
        {
            Lon = lon;
            Lat = lat;
        }
    }
}
