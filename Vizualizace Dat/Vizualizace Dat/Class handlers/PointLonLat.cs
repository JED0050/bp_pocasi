using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vizualizace_Dat
{
    public class PointLonLat
    {
        public double Lon { get; set; }
        public double Lat { get; set; }

        public PointLonLat(double lon, double lat)
        {
            Lon = lon;
            Lat = lat;
        }
    }
}
