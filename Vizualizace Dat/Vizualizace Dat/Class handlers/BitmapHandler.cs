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
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Vizualizace_Dat
{
    class BitmapHandler
    {
        public static string baseUrl = ApkConfig.ServerAddress;

        public static PointLatLng defaultTopLeftBound = ApkConfig.TopLeftCorner; //new PointLatLng(55.1995133, 4.1303633);
        public static PointLatLng defaultBotRightBound = ApkConfig.BotRightCorner; //new PointLatLng(41.6999200, 37.9033283);
        public static double GetLon(int x, int bW)
        {
            double lonDif = defaultBotRightBound.Lng - defaultTopLeftBound.Lng;
            double pixelLon = lonDif / bW;
            double leftCorner = defaultTopLeftBound.Lng;

            return Math.Round(leftCorner + x * pixelLon, 6);
        }

        public static double GetLat(int y, int bH)
        {
            double latDif = defaultTopLeftBound.Lat - defaultBotRightBound.Lat;
            double pixelLat = latDif / bH;
            double leftCorner = defaultTopLeftBound.Lat;

            return Math.Round(leftCorner - y * pixelLat, 6);
        }

        public static int GetX(double lon, int bW, double bX, double bX2)
        {
            if (lon < defaultTopLeftBound.Lng || lon > defaultBotRightBound.Lng)
                return -1;

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
            if (lat > defaultTopLeftBound.Lat || lat < defaultBotRightBound.Lat)
                return -1;

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
            string time = dateTime.ToString("yyyy-MM-ddTHH:mm:00");

            string pointUrl;

            if((bounds.Count < 2) || (bounds[0].Lng == defaultTopLeftBound.Lng && bounds[0].Lat == defaultTopLeftBound.Lat && bounds[1].Lng == defaultBotRightBound.Lng && bounds[1].Lat == defaultBotRightBound.Lat))
            {
                pointUrl = "";
            }
            else
            {
                pointUrl = "&p1=" + bounds[0].Lat.ToString().Replace(",",".") + "," + bounds[0].Lng.ToString().Replace(",", ".") + "&p2=" + bounds[1].Lat.ToString().Replace(",", ".") + "," + bounds[1].Lng.ToString().Replace(",", ".");
            }

            string precUrl = $"bmp?type={type}&time={time}&loaders={loaders}" + pointUrl;
            string fullUrl = baseUrl + precUrl;

            Bitmap precBitmap;

            Debug.WriteLine(fullUrl);

            using (WebClient wc = new WebClient())
            {
                try
                {
                    using (Stream s = wc.OpenRead(fullUrl))
                    {
                        precBitmap = new Bitmap(s);
                    }
                }
                catch
                {
                    string exceptionMessage = "";

                    try
                    {
                        wc.Encoding = System.Text.Encoding.UTF8;

                        string serverJsonException = wc.DownloadString(fullUrl);

                        dynamic jsonFile = JObject.Parse(serverJsonException);

                        exceptionMessage = jsonFile.message.ToString();
                    }
                    catch
                    {
                        exceptionMessage = "Na server se nepodařilo připojit, zadáváte chybnou adresu nebo je server aktuálně vypnutý.";
                    }

                    throw new Exception(exceptionMessage);
                }
            }

            //Debug.WriteLine(fullUrl);

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

        public static List<PointLatLng> GetBounds(GMapControl gMap)
        {
            List<PointLatLng> bounds = new List<PointLatLng>();

            if (gMap.Zoom <= 8)
            {
                bounds.Add(defaultTopLeftBound);
                bounds.Add(defaultBotRightBound);

                return bounds;
            }

            var topLeft = gMap.FromLocalToLatLng(0, 0);
            var botRight = gMap.FromLocalToLatLng(gMap.Width, gMap.Height);

            bounds.Add(topLeft);
            bounds.Add(botRight);

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

        public static double GetFullPrecInPoint(DateTime time, PointLatLng point, string loaders, List<PointLatLng> bounds, ForecType forecType, Bitmap bFor = null)
        {
            if (bFor == null)
                bFor = GetBitmapFromServer(forecType.Type, time, loaders, bounds);

            //bFor.Save(@"C:\Users\Honza_PC\Desktop\testMap.bmp", ImageFormat.Bmp);

            int x = GetX(point.Lng, bFor.Width, bounds[0].Lng, bounds[1].Lng);
            int y = GetY(point.Lat, bFor.Height, bounds[0].Lat, bounds[1].Lat);

            Color c = Color.Transparent;

            try
            {
                c = bFor.GetPixel(x, y);
            }
            catch
            {
                throw new Exception("Zvolený bod se nenachází ve zpracovávané oblasti!");
            }

            c = bFor.GetPixel(x, y);

            double precVal = forecType.GetForecValue(c);

            //Debug.WriteLine(forecType.Type);

            return precVal;
        }

        public static PointLatLng GetPointCoordFromCityName(string cityName)
        {
            string url = @"https://api.mapbox.com/geocoding/v5/mapbox.places/" + cityName + @".json?access_token=sk.eyJ1IjoiZ2FhcmEzc2FuIiwiYSI6ImNrbjA5OW4zdjBhZHgyb3BiNDZkZmpucnUifQ.hcAnYzBqYxfx3zvZfZnMug&cachebuster=1617364889072&autocomplete=true";

            string jsonContent = "";

            using(WebClient client = new WebClient())
            {
                try
                {
                    jsonContent = client.DownloadString(url);
                }
                catch
                {
                    throw new Exception("Název místa obsahuje nepovolené znaky!");
                }
            }

            try
            {
                dynamic jObjGeo = JObject.Parse(jsonContent);
                JArray jArFeatures = (JArray)jObjGeo["features"];
                JArray jArCenter = (JArray)jArFeatures[0]["center"];

                double lon = (double)jArCenter[0];
                double lat = (double)jArCenter[1];

                return new PointLatLng(lat, lon);
            }
            catch
            {
                throw new Exception("Místo se nepodařilo zpracovat, zkuste jiné místo!");
            }
        }


    }
}
