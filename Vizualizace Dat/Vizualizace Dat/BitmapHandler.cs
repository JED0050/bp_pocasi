using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Xml.Linq;

namespace Vizualizace_Dat
{
    class BitmapHandler
    {
        public static string baseUrl = ApkConfig.ServerAddress;
        public static double GetPrecipitationFromPixel(Color pixel)
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

        public static double GetTemperatureValue(Color pixel)
        {
            double temperature = 0;

            Color colorWithoutAlfa = Color.FromArgb(0, pixel.R, pixel.G, pixel.B);
            int colorValue = colorWithoutAlfa.ToArgb();


            /*
            
            index - R G B - teplota

            0 - 169 0 221 - -25
            1 - 90 0 228 - -20
            2 - 31 0 231 - -15
            3 - 0 43 236 - -10
            4 - 0 142 237 - -5
            5 - 0 241 237 - 0
            6 - 0 242 169 - 3
            7 - 0 236 115 - 5
            8 - 0 214 35 - 8
            9 - 12 214 0 - 10
            10 - 88 239 0 - 13
            11 - 145 248 231 - 15
            12 - 225 250 0 - 18
            13 - 251 225 0 - 20
            14 - 252 109 0 - 25
            15 - 252 110 0 - 30+

            */


            if (colorValue <= Color.FromArgb(0, 169, 0, 221).ToArgb() && colorValue > Color.FromArgb(0, 90, 0, 228).ToArgb() && pixel.G == 0)
            {
                temperature = -25;
            }
            else if (colorValue <= Color.FromArgb(0, 90, 0, 228).ToArgb() && colorValue > Color.FromArgb(0, 31, 0, 231).ToArgb() && pixel.G == 0)
            {
                temperature = -20;
            }
            else if (colorValue <= Color.FromArgb(0, 31, 0, 231).ToArgb() && colorValue > Color.FromArgb(0, 0, 43, 236).ToArgb() && pixel.G == 0)
            {
                temperature = -15;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 43, 236).ToArgb() && colorValue < Color.FromArgb(0, 0, 142, 237).ToArgb() && pixel.R == 0 && pixel.G < 199)
            {
                temperature = -10;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 142, 237).ToArgb() && colorValue < Color.FromArgb(0, 0, 241, 237).ToArgb() && pixel.R == 0 && pixel.G < 199)
            {
                temperature = -5;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 241, 237).ToArgb() && colorValue < Color.FromArgb(0, 0, 242, 169).ToArgb())
            {
                temperature = 0;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 242, 169).ToArgb() && colorValue > Color.FromArgb(0, 0, 236, 115).ToArgb())
            {
                temperature = 3;
            }
            else if (colorValue <= Color.FromArgb(0, 0, 236, 115).ToArgb() && colorValue > Color.FromArgb(0, 0, 214, 35).ToArgb())
            {
                temperature = 5;
            }
            else if (colorValue >= Color.FromArgb(0, 0, 214, 35).ToArgb() && colorValue < Color.FromArgb(0, 12, 214, 0).ToArgb())
            {
                temperature = 8;
            }
            else if (colorValue >= Color.FromArgb(0, 12, 214, 0).ToArgb() && colorValue < Color.FromArgb(0, 88, 239, 0).ToArgb())
            {
                temperature = 10;
            }
            else if (colorValue >= Color.FromArgb(0, 88, 239, 0).ToArgb() && colorValue < Color.FromArgb(0, 145, 248, 231).ToArgb())
            {
                temperature = 13;
            }
            else if (colorValue >= Color.FromArgb(0, 145, 248, 231).ToArgb() && colorValue < Color.FromArgb(0, 225, 250, 0).ToArgb())
            {
                temperature = 15;
            }
            else if (colorValue >= Color.FromArgb(0, 225, 250, 0).ToArgb() && colorValue < Color.FromArgb(0, 251, 225, 0).ToArgb())
            {
                temperature = 18;
            }
            else if (colorValue >= Color.FromArgb(0, 251, 225, 0).ToArgb() && colorValue < Color.FromArgb(0, 252, 109, 0).ToArgb())
            {
                temperature = 20;
            }
            else if (colorValue >= Color.FromArgb(0, 252, 109, 0).ToArgb() && colorValue < Color.FromArgb(0, 252, 110, 0).ToArgb())
            {
                temperature = 25;
            }
            else if (colorValue >= Color.FromArgb(0, 252, 110, 0).ToArgb())
            {
                temperature = 30;
            }

            return temperature;
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

        public static int GetX(double lon, int bW, double bX, double bX2)
        {
            //double lonDif = 20.21 - 10.06;
            //double bX = 10.06;

            double lonDif = Math.Abs(bX2 - bX);
            double pixelLon = lonDif / bW;

            int x;
            for (x = 0; x < bW; x++)
            {
                if (bX >= lon && lon <= bX + lon)
                    break;

                bX += pixelLon;
            }

            return x;
        }

        public static int GetY(double lat, int bH, double bY, double bY2)
        {
            //double latDif = 51.88 - 47.09;
            //double bY = 51.88;

            double latDif = Math.Abs(bY - bY2);
            double pixelLat = latDif / bH;

            int y;
            for (y = 0; y < bH - 1; y++)
            {
                if (bY - pixelLat <= lat && lat <= bY)
                    break;

                bY -= pixelLat;
            }

            return y;
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

        public static Bitmap GetBitmapFromServer(string type, DateTime dateTime, string loaders, List<PointLatLng> bounds)
        {
            string time = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");

            string pointUrl;

            if((bounds.Count < 2) || (bounds[0].Lng == 10.06 && bounds[0].Lat == 51.88 && bounds[1].Lng == 20.21 && bounds[1].Lat == 47.09))
            {
                pointUrl = "";
            }
            else
            {
                pointUrl = "&p1=" + bounds[0].Lat + "a" + bounds[0].Lng + "&p2=" + bounds[1].Lat + "a" + bounds[1].Lng;
            }

            string precUrl = $"forec?type={type}&time={time}&loaders={loaders}" + pointUrl;
            string fullUrl = baseUrl + precUrl;

            Bitmap precBitmap;

            using (WebClient wc = new WebClient())
            {
                using (Stream s = wc.OpenRead(fullUrl))
                {
                    precBitmap = new Bitmap(s);
                }
            }

            return precBitmap;
        }

        public static List<PointLatLng> GetRoutePoints(PointLatLng start, PointLatLng end)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            
            points.Add(start);

            try
            {
                string jsonText = "";

                string str_origin = "origin=" + Math.Round(start.Lat, 6).ToString().Replace(',', '.') + "," + Math.Round(start.Lng, 6).ToString().Replace(',', '.');
                string str_dest = "destination=" + Math.Round(end.Lat, 6).ToString().Replace(',', '.') + "," + Math.Round(end.Lng, 6).ToString().Replace(',', '.');
                string sensor = "sensor=false";
                string parameters = str_origin + "&" + str_dest + "&" + sensor;
                string output = "json";
                string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" + parameters + "&key=" + "AIzaSyB-dtdURExB2aYlfNr071TDkx3ZNxZ5jD8";

                using (var client = new WebClient())
                {
                    jsonText = client.DownloadString(url);
                }

                dynamic jsonGeo = JObject.Parse(jsonText);
                JArray jRoutesAr = (JArray)jsonGeo["routes"];
                JArray jLegsAr = (JArray)jRoutesAr[0]["legs"];
                JArray jStepsAr = (JArray)jLegsAr[0]["steps"];

                foreach (var step in jStepsAr)
                {
                    dynamic jsonElement = JObject.Parse(step.ToString());

                    double latD = double.Parse(jsonElement.start_location.lat.ToString().Replace('.', ','));
                    double lonD = double.Parse(jsonElement.start_location.lng.ToString().Replace('.', ','));

                    points.Add(new PointLatLng(latD, lonD));
                }
            }
            catch
            {

            }

            points.Add(end);

            return points;
        }

        public static List<PointLatLng> GetRoutePoints(string xmlContent)
        {
            List<PointLatLng> points = new List<PointLatLng>();

            XDocument xmlDoc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.topografix.com/GPX/1/1";
                
            foreach(XElement x in xmlDoc.Descendants(ns + "trkpt"))
            {
                double latD = double.Parse(x.Attribute("lat").Value.Replace('.', ','));
                double lonD = double.Parse(x.Attribute("lon").Value.Replace('.', ','));

                points.Add(new PointLatLng(latD, lonD));
            }

            return points;
        }

        public static List<PointLatLng> GetBounds(int zoom, PointLatLng center)
        {
            List<PointLatLng> bounds = new List<PointLatLng>();

            if (zoom < 8)
            {
                bounds.Add(new PointLatLng(51.88, 10.06));
                bounds.Add(new PointLatLng(47.09, 20.21));

                return bounds;
            }

            double degreePerPixel = (156543.04 * Math.Cos(center.Lat * Math.PI / 180)) / (111325 * Math.Pow(2, zoom));

            double mHalfWidthInDegrees = 728 * degreePerPixel / 0.9;
            double mHalfHeightInDegrees = 528 * degreePerPixel / 1.7;

            double mNorth = center.Lat + mHalfHeightInDegrees;
            double mWest = center.Lng - mHalfWidthInDegrees;
            double mSouth = center.Lat - mHalfHeightInDegrees;
            double mEast = center.Lng + mHalfWidthInDegrees;

            bounds.Add(new PointLatLng(mNorth, mWest));
            bounds.Add(new PointLatLng(mSouth, mEast));

            return bounds;
        }

        public static double GetDistance(PointLatLng p1, PointLatLng p2)
        {
            //https://www.movable-type.co.uk/scripts/latlong.html

            double lat1 = p1.Lat;
            double lon1 = p1.Lng;

            double lat2 = p2.Lat;
            double lon2 = p2.Lng;

            double R = 6371e3; // metres
            double φ1 = lat1 * Math.PI / 180; // φ, λ in radians
            double φ2 = lat2 * Math.PI / 180;
            double Δφ = (lat2 - lat1) * Math.PI / 180;
            double Δλ = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
              Math.Cos(φ1) * Math.Cos(φ2) *
              Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double d = R * c; // in metres

            return d;
        }

        public static double GetFullPrecInPoint(DateTime time, PointLatLng point, string type, string loaders, List<PointLatLng> bounds)
        {
            Bitmap bFor = BitmapHandler.GetBitmapFromServer(type, time, loaders, bounds);

            int x = BitmapHandler.GetX(point.Lng, bFor.Width, bounds[0].Lng, bounds[1].Lng);
            int y = BitmapHandler.GetY(point.Lat, bFor.Height, bounds[0].Lat, bounds[1].Lat);

            Color c = Color.Transparent;

            try
            {
                c = bFor.GetPixel(x, y);
            }
            catch
            { }

            double precVal = BitmapHandler.GetPrecipitationFromPixel(c);

            return precVal;
        }
    }
}
