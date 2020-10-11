using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Vizualizace_Dat
{
    class BitmapHandler
    {
        
        public static double GetPrecipitationFromPixel(Color pixel)
        {
            double precipitation = 0;

            /*
            srážky dle RGB barev
            index - R G B - srážky
            0 - 0 0 0 - 0 mm/h
            4 - 56 0 112 - 0 mm/h
            8 - 48 0 168 - 0.1 mm/h
            12 - 0 0 252 - 0.2 mm/h
            16 - 0 108 192 - 0.4 mm/h
            20 - 0 160 0 - 0.8 mm/h
            24 - 0 188 0 - 1 mm/h
            28 - 52 216 0 - 2 mm/h
            32 - 156 220 0 - 4 mm/h
            36 - 224 220 0 - 8 mm/h
            40 - 252 176 0 - 10 mm/h
            44 - 252 132 0 - 30 mm/h
            48 - 252 88 0 - 60 mm/h
            52 - 252 0 0 - 100 mm/h
            56 - 160 0 0 - 100+ mm/h
            60 - 255 255 255 - 100+ mm/h
            */

            if (ColInRange(0, 56, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(0, 167, pixel.B))
                precipitation = 0;
            else if (ColInRange(0, 48, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(168, 251, pixel.B))
                precipitation = 0.1;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(0, 107, pixel.G) && ColInRange(193, 252, pixel.B))
                precipitation = 0.2;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(108, 159, pixel.G) && ColInRange(1, 192, pixel.B))
                precipitation = 0.4;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(160, 187, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 0.8;
            else if (ColInRange(0, 51, pixel.R) && ColInRange(188, 215, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 1;
            else if (ColInRange(52, 155, pixel.R) && ColInRange(216, 219, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 2;
            else if (ColInRange(156, 223, pixel.R) && ColInRange(220, 220, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 4;
            else if (ColInRange(224, 251, pixel.R) && ColInRange(177, 220, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 8;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(133, 176, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 10;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(89, 132, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 30;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(1, 88, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 60;
            else if (ColInRange(150, 255, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 100;

            return precipitation;
        }

        private static bool ColInRange(int min, int max, int val)
        {
            return (min <= val) && (val <= max);
        }

        public static double GetLon(int x, int bW)
        {
            double lonDif = 20.21 - 10.06;
            double pixelLon = lonDif / bW;
            double leftCorner = 10.06;

            return Math.Round(leftCorner + x * pixelLon, 6);
        }

        public static double GetLat(int y, int bH)
        {
            double latDif = 51.88 - 47.09;
            double pixelLat = latDif / bH;
            double leftCorner = 51.88;

            return Math.Round(leftCorner - y * pixelLat, 6);
        }

        public static string GetAdressFromLonLat(double lon, double lat)
        {
            //Geocoder geocoder = new Geocoder("AIzaSyB-dtdURExB2aYlfNr071TDkx3ZNxZ5jD8");
            string output = "";

            string jsonUrl = "https://api.opencagedata.com/geocode/v1/json?key=d4727b73fca44443ab1ac1581abad71a&q=" + lat + "%2C" + lon + "8&pretty=1";
            string jsonText = "";

            using (var client = new WebClient())
            {
                jsonText = client.DownloadString(jsonUrl);
            }

            dynamic jsonGeo = JObject.Parse(jsonText);

            JArray jResAr = (JArray)jsonGeo["results"];

            foreach(var el in jResAr)
            {
                dynamic jsonElement = JObject.Parse(el.ToString());

                string state_code = jsonElement.components.country_code;
                string state_fullName = jsonElement.components.country;
                string county = jsonElement.components.county;
                string municipality = jsonElement.components.municipality;
                string state = jsonElement.components.state;
                string village = jsonElement.components.village;
                string road = jsonElement.components.road;
                string suburb = jsonElement.components.suburb;
                string city = jsonElement.components.city;

                string cityOrVil = "";

                if (city != null && city.Length >= 2)
                    cityOrVil = "\nMěsto: " + city;
                else if (village != null && village.Length >= 2)
                    cityOrVil = "\nVesnice: " + village;

                output = "Zamě: " + state_code + "\nKraj: " + county + cityOrVil;
            }

            return output;
        }
    }
}
