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
using System.Globalization;

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
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                AvgForecast aF = new AvgForecast();

                RadarBourkyDataLoader bL = new RadarBourkyDataLoader();
                YrNoDataLoader xL = new YrNoDataLoader();
                OpenWeatherMapDataLoader jL = new OpenWeatherMapDataLoader();
                MedardDataLoader bL2 = new MedardDataLoader();
                WeatherUnlockedDataLoader weunL = new WeatherUnlockedDataLoader();

                aF.Add(bL);
                aF.Add(xL);
                aF.Add(jL);
                aF.Add(bL2);
                aF.Add(weunL);

                aF.SaveForecastBitmaps();

                Debug.WriteLine(stopwatch.ElapsedMilliseconds); //565162 ms = 9.5 minut bez YrNo

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return this.Content("{\"message\": \"" + e.Message + "\"}", "text/json");
            }
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
            try
            {
                //https://localhost:44336/bmp?type=prec&time=2021-05-26T18:30:00&loaders=owm,yrno&p1=49.621559;17.1507294&p2=49.5211889;17.4213141

                if (type == null)
                {
                    throw new Exception("Vstupní parametry type musí být vyplněn!");
                }
                else
                {

                    DateTime dateTime;

                    if (time == "0" || time == null)
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
                            throw new Exception("Formát času není správně vyplněn! Použíjte prosím formát ISO 8601 (yyyy-MM-ddTHH:mm:ss) nebo znak 0 pro aktuální čas.");
                        }
                    }

                    AvgForecast aF = new AvgForecast(loaders);

                    type = type.ToLower();

                    if (ForecastTypes.IsTypeKnown(type))
                    {
                        Bitmap precBitmap;

                        if (p1 == null || p2 == null)
                        {
                            precBitmap = aF.GetAvgForecBitmap(dateTime, type);
                        }
                        else
                        {
                            string[] p1LatLon = p1.Split(",");
                            string[] p2LatLon = p2.Split(",");

                            PointLonLat point1 = new PointLonLat(double.Parse(p1LatLon[1], CultureInfo.InvariantCulture), double.Parse(p1LatLon[0], CultureInfo.InvariantCulture));
                            PointLonLat point2 = new PointLonLat(double.Parse(p2LatLon[1], CultureInfo.InvariantCulture), double.Parse(p2LatLon[0], CultureInfo.InvariantCulture));

                            precBitmap = aF.GetAvgForecBitmap(dateTime, type, point1, point2);
                        }

                        var bitmapBytes = BitmapToBytes(precBitmap);

                        return File(bitmapBytes, "image/jpeg");
                    }
                }

                throw new Exception("Požadovaná bitmapa nenalezena! Zkuste prosím změnit datový zdroj, čas, hranice nebo typ předpovědi.");
            }
            catch(Exception e)
            {
                return this.Content("{\"message\": \"" + e.Message + "\"}", "text/json");
            }
        }

        public IActionResult XMLForecast(string time, string loaders, string lon, string lat)
        {
            try
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
            catch(Exception e)
            {
                return this.Content("{\"message\": \"" + e.Message + "\"}", "text/json");
            }
        }

        public IActionResult JSONForecast(string time, string loaders, string lon, string lat)
        {
            try
            {
                Forecast forecast = GetForecastFromTimeAndPoint(time, loaders, lon, lat);

                string jsonString = JsonConvert.SerializeObject(forecast);

                return this.Content(jsonString, "text/json");
            }
            catch(Exception e)
            {
                return this.Content("{\"message\": \"" + e.Message + "\"}", "text/json");
            }
        }

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        private Forecast GetForecastFromTimeAndPoint(string time, string loaders, string lon, string lat)
        {
            PointLonLat point = new PointLonLat(double.Parse(lon, CultureInfo.InvariantCulture), double.Parse(lat, CultureInfo.InvariantCulture));

            AvgForecast aF = new AvgForecast(loaders);

            return aF.GetForecastFromTimeAndPoint(DateTime.Parse(time), point);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
