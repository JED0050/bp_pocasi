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
using IDataLoaderAndHandlerLib.HandlersAndObjects;
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

                AvgForecast aF = new AvgForecast("");

                aF.SaveForecastBitmaps();

                Debug.WriteLine(stopwatch.ElapsedMilliseconds); //565162 ms = 9.5 minut bez YrNo

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return this.Content("{\"message\": \"" + e.Message + "\"}", "text/json");
            }
        }

        public IActionResult Bounds()
        {
            string jsonContent = BoundsJsonHandler.LoadBoundsAsString();

            return this.Content(jsonContent, "text/json");
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Cíl webové služby.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Různé kontaktní údaje.";

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
                        else if(p1 != null && p2 != null)
                        {
                            string[] p1LatLon = p1.Split(",");
                            string[] p2LatLon = p2.Split(",");

                            PointLonLat point1 = new PointLonLat(double.Parse(p1LatLon[1], CultureInfo.InvariantCulture), double.Parse(p1LatLon[0], CultureInfo.InvariantCulture));
                            PointLonLat point2 = new PointLonLat(double.Parse(p2LatLon[1], CultureInfo.InvariantCulture), double.Parse(p2LatLon[0], CultureInfo.InvariantCulture));

                            precBitmap = aF.GetAvgForecBitmap(dateTime, type, point1, point2);
                        }
                        else
                        {
                            throw new Exception("Zadejte prosím oba body ohraničující mapu pro vrácení vámi požadovaného výřezu, případně nezadávejte žádný bod pro získání mapy v plné velikosti.");
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

        public IActionResult XMLForecast(string time, string loaders, string lon, string lat, string hourDif, string numOfFcs)
        {
            try
            {
                if (lon == null || lat == null)
                {
                    throw new Exception("Parametry 'lon' a 'lat' musí být vyplněny.");
                }
                else
                {
                    DateTime startTime;

                    if (time == "0" || time == null)
                    {
                        startTime = DateTime.Now;
                    }
                    else
                    {
                        try
                        {
                            startTime = DateTime.Parse(time);
                        }
                        catch
                        {
                            throw new Exception("Formát času není správně vyplněn! Použíjte prosím formát ISO 8601 (yyyy-MM-ddTHH:mm:ss) nebo znak 0 pro aktuální čas.");
                        }
                    }

                    startTime = startTime.AddMinutes(30);
                    startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0);

                    if (hourDif == null && numOfFcs == null)
                    {
                        Forecast forecast = GetForecastFromTimeAndPoint(startTime, loaders, lon, lat);
                        forecast.Time = startTime;

                        ForecastXMLSerializeObject f = new ForecastXMLSerializeObject(forecast);

                        XmlSerializer serializer = new XmlSerializer(typeof(ForecastXMLSerializeObject));

                        string xmlString = "";

                        using (StringWriter textWriter = new StringWriter())
                        {
                            serializer.Serialize(textWriter, f);
                            xmlString = textWriter.ToString();
                        }

                        return this.Content(xmlString, "text/xml");
                    }
                    else if (hourDif != null && numOfFcs != null)
                    {

                        List<Forecast> forecasts = new List<Forecast>();

                        for (int i = 0; i < int.Parse(numOfFcs); i++)
                        {

                            DateTime actTime = startTime.AddHours(i * int.Parse(hourDif));

                            Forecast forecast = GetForecastFromTimeAndPoint(actTime, loaders, lon, lat);
                            forecast.Time = actTime;

                            forecasts.Add(forecast);
                        }


                        ForecastXMLSerializeObject f = new ForecastXMLSerializeObject(forecasts);

                        XmlSerializer serializer = new XmlSerializer(typeof(ForecastXMLSerializeObject));

                        string xmlString = "";

                        using (StringWriter textWriter = new StringWriter())
                        {
                            serializer.Serialize(textWriter, f);
                            xmlString = textWriter.ToString();
                        }

                        return this.Content(xmlString, "text/xml");
                    }
                    else
                    {
                        throw new Exception("Pro zjištění předpovědi na jeden den nesmí být parametry 'hourDif' a 'numOfFcs' vyplněny. Naopak pro zjištění více předpovědi musí být tyto parametry vplněny oba.");
                    }
                }
            }
            catch(Exception e)
            {
                return this.Content("{\"message\": \"" + e.Message + "\"}", "text/json");
            }
        }

        public IActionResult JSONForecast(string time, string loaders, string lon, string lat, string hourDif, string numOfFcs)
        {
            try
            {
                if(lon == null || lat == null)
                {
                    throw new Exception("Parametry 'lon' a 'lat' musí být vyplněny.");
                }
                else
                {
                    DateTime startTime;

                    if (time == "0" || time == null)
                    {
                        startTime = DateTime.Now;
                    }
                    else
                    {
                        try
                        {
                            startTime = DateTime.Parse(time);
                        }
                        catch
                        {
                            throw new Exception("Formát času není správně vyplněn! Použíjte prosím formát ISO 8601 (yyyy-MM-ddTHH:mm:ss) nebo znak 0 pro aktuální čas.");
                        }
                    }

                    startTime = startTime.AddMinutes(30);
                    startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0);

                    if (hourDif == null && numOfFcs == null)
                    {
                        Forecast forecast = GetForecastFromTimeAndPoint(startTime, loaders, lon, lat);
                        forecast.Time = startTime;

                        ForecastJSONSerializeObject f = new ForecastJSONSerializeObject(forecast);

                        string jsonString = JsonConvert.SerializeObject(f);

                        return this.Content(jsonString, "text/json");
                    }
                    else if (hourDif != null && numOfFcs != null)
                    {
                        List<Forecast> forecasts = new List<Forecast>();

                        for (int i = 0; i < int.Parse(numOfFcs); i++)
                        {

                            DateTime actTime = startTime.AddHours(i * int.Parse(hourDif));

                            Forecast forecast = GetForecastFromTimeAndPoint(actTime, loaders, lon, lat);
                            forecast.Time = actTime;

                            forecasts.Add(forecast);
                        }

                        ForecastJSONSerializeObject f = new ForecastJSONSerializeObject(forecasts);

                        string jsonString = JsonConvert.SerializeObject(f);

                        return this.Content(jsonString, "text/json");
                    }
                    else
                    {
                        throw new Exception("Pro zjištění předpovědi na jeden den nesmí být parametry 'hourDif' a 'numOfFcs' vyplněny. Naopak pro zjištění více předpovědi musí být tyto parametry vplněny oba.");
                    }
                }
            }
            catch(Exception e)
            {
                return this.Content("{\"message\": \"" + e.Message + "\"}", "text/json");
            }
        }

        public IActionResult ScaleImage(string type)
        {
            try
            {
                if(type != null)
                {
                    type = type.ToLower();
                }

                if(ForecastTypes.IsTypeKnown(type))
                {
                    string workingDirectory = Environment.CurrentDirectory;
                    string fullFilePath = workingDirectory + @"\Data\" + $"scale_{type}.png";

                    Bitmap scaleImage = new Bitmap(fullFilePath);

                    var bitmapBytes = BitmapToBytes(scaleImage);
                    return File(bitmapBytes, "image/jpeg");
                }
                else
                {
                    throw new Exception("Zadaný typ škály neexistuje. Zadejte prosím jednu z možností 'prec', 'temp', 'humi' nebo 'pres'.");
                }
            }
            catch (Exception e)
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

        private Forecast GetForecastFromTimeAndPoint(DateTime time, string loaders, string lon, string lat)
        {
            PointLonLat point = new PointLonLat(double.Parse(lon, CultureInfo.InvariantCulture), double.Parse(lat, CultureInfo.InvariantCulture));

            AvgForecast aF = new AvgForecast(loaders);

            return aF.GetForecastFromTimeAndPoint(time, point);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
