﻿using System;
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

            //Debug.WriteLine(type);

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

        public static double GetFullPrecInPoint(DateTime time, PointLatLng point, string loaders, List<PointLatLng> bounds, ForecType forecType)
        {
            Bitmap bFor = GetBitmapFromServer(forecType.Type, time, loaders, bounds);

            //bFor.Save(@"C:\Users\Honza_PC\Desktop\testMap.bmp", ImageFormat.Bmp);

            int x = GetX(point.Lng, bFor.Width, bounds[0].Lng, bounds[1].Lng);
            int y = GetY(point.Lat, bFor.Height, bounds[0].Lat, bounds[1].Lat);

            Color c = Color.Transparent;

            try
            {
                c = bFor.GetPixel(x, y);
            }
            catch
            { }

            double precVal = forecType.GetForecValue(c);

            //Debug.WriteLine(forecType.Type);

            return precVal;
        }
    }
}