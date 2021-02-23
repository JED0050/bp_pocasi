using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webova_Sluzba.Models;
using AgregaceDatLib;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Webova_Sluzba.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Loader()
        {
            AvgForecast aF = new AvgForecast();

            RadarBourkyDataLoader bL = new RadarBourkyDataLoader();
            YrNoDataLoader xL = new YrNoDataLoader();
            OpenWeatherMapDataLoader jL = new OpenWeatherMapDataLoader();
            MedardDataLoader bL2 = new MedardDataLoader();

            aF.Add(bL);
            aF.Add(xL);
            //aF.Add(jL);
            aF.Add(bL2);

            aF.SaveForecastBitmaps();

            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Cíl bakalářské práce.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Různé kontaktní údaje.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult BitmapForecast(string type, string time, string loaders, string p1, string p2)
        {

            //https://localhost:44336/bmp?type=prec&time=2021-05-26T18:30:00&loaders=owm,yrno&p1=49.621559;17.1507294&p2=49.5211889;17.4213141

            if (type == null)
            {
                throw new Exception("Vstupní parametry type musí být vyplněn!");
            }
            else
            {

                DateTime dateTime;

                if(time == "0" || time == null)
                {
                    dateTime = DateTime.Now;
                }
                else
                {

                    try
                    {
                        dateTime = DateTime.Parse(time);    //ISO 8601 = yyyy-MM-ddTHH:mm:ss
                    }
                    catch
                    {
                        throw new Exception("Formát času není správně vyplněn! (yyyy-MM-ddTHH:mm:ss)");
                    }
                }

                AvgForecast aF = SetLoadersForAvgForecast(loaders);

                type = type.ToLower();

                if (type == "prec" || type ==  "temp")
                {
                    Bitmap precBitmap;

                    if (p1 == null || p2 == null)
                    {
                        precBitmap = aF.GetAvgForecBitmap(dateTime, type);
                    }
                    else
                    {
                        string[] p1LatLon = p1.Replace(".",",").Split(";");
                        string[] p2LatLon = p2.Replace(".", ",").Split(";");

                        PointLonLat point1 = new PointLonLat(double.Parse(p1LatLon[1]), double.Parse(p1LatLon[0]));
                        PointLonLat point2 = new PointLonLat(double.Parse(p2LatLon[1]), double.Parse(p2LatLon[0]));

                        precBitmap = aF.GetAvgForecBitmap(dateTime, type, point1, point2);

                    }

                    var bitmapBytes = BitmapToBytes(precBitmap);

                    return File(bitmapBytes, "image/jpeg");
                }


            }

            throw new Exception("Požadovaná bitmapa nenalezena!");
        }

        public IActionResult XMLForecast(string time, string loaders, string lon, string lat)
        {
            Forecast forecast = GetForecastFromTimeAndPoint(time, loaders, lon, lat);

            XmlSerializer serializer = new XmlSerializer(typeof(Forecast));

            string xmlString = "";

            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, forecast);
                xmlString = textWriter.ToString();
            }

            return this.Content(xmlString, "text/xml");
        }

        public IActionResult JSONForecast(string time, string loaders, string lon, string lat)
        {
            Forecast forecast = GetForecastFromTimeAndPoint(time, loaders, lon, lat);

            string jsonString = JsonConvert.SerializeObject(forecast);

            return this.Content(jsonString, "text/json");
        }

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private AvgForecast SetLoadersForAvgForecast(string loaders)
        {
            AvgForecast aF = new AvgForecast();

            if (loaders == null)
            {
                RadarBourkyDataLoader bL = new RadarBourkyDataLoader();
                YrNoDataLoader xL = new YrNoDataLoader();
                OpenWeatherMapDataLoader jL = new OpenWeatherMapDataLoader();
                MedardDataLoader bL2 = new MedardDataLoader();

                aF.Add(bL);
                aF.Add(xL);
                aF.Add(jL);
                aF.Add(bL2);
            }
            else
            {
                string[] arLoaders = loaders.ToLower().Split(",");

                foreach(string loader in arLoaders)
                {
                    if (loader == "rb")
                    {
                        RadarBourkyDataLoader bL = new RadarBourkyDataLoader();
                        aF.Add(bL);
                    }

                    if (loader == "mdrd")
                    {
                        MedardDataLoader bL2 = new MedardDataLoader();
                        aF.Add(bL2);
                    }

                    if (loader == "yrno")
                    {
                        YrNoDataLoader xL = new YrNoDataLoader();
                        aF.Add(xL);
                    }

                    if (loader == "owm")
                    {
                        OpenWeatherMapDataLoader jL = new OpenWeatherMapDataLoader();
                        aF.Add(jL);
                    }
                }

                if (aF.GetNumberOfLoaders() == 0)
                {
                    throw new Exception("Nebyl přiřazen žádný z datových zrdrojů!");
                }

            }

            return aF;
        }

        private Forecast GetForecastFromTimeAndPoint(string time, string loaders, string lon, string lat)
        {
            PointLonLat point = new PointLonLat(double.Parse(lon.Replace(".", ",")), double.Parse(lat.Replace(".", ",")));

            AvgForecast aF = SetLoadersForAvgForecast(loaders);

            return aF.GetForecastFromTimeAndPoint(DateTime.Parse(time), point);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
