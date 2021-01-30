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

            BitmapDataLoader bL = new BitmapDataLoader();
            XMLDataLoader xL = new XMLDataLoader();
            JSONDataLoader jL = new JSONDataLoader();
            BitmapDataLoader2 bL2 = new BitmapDataLoader2();

            //aF.Add(bL);
            aF.Add(xL);
            //aF.Add(jL);
            //aF.Add(bL2);

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

        public IActionResult Forecast(string type, string time, string loaders, string p1, string p2)
        {

            //https://localhost:44336/forec?type=prec&time=2020-10-28T16:44:10&loaders=bx

            if (type == null || time == null)
            {
                throw new Exception("Vstupní parametry type a time musí být vyplněny!");
            }
            else
            {

                DateTime dateTime;

                if(time == "0")
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

                AvgForecast aF = new AvgForecast();

                if (loaders == null)
                {
                    BitmapDataLoader bL = new BitmapDataLoader();
                    XMLDataLoader xL = new XMLDataLoader();
                    JSONDataLoader jL = new JSONDataLoader();
                    BitmapDataLoader2 bL2 = new BitmapDataLoader2();

                    aF.Add(bL);
                    aF.Add(xL);
                    aF.Add(jL);
                    aF.Add(bL2);
                }
                else
                {
                    if(loaders.ToLower().Contains("b1"))
                    {
                        BitmapDataLoader bL = new BitmapDataLoader();
                        aF.Add(bL);
                    }

                    if (loaders.ToLower().Contains("b2"))
                    {
                        BitmapDataLoader2 bL2 = new BitmapDataLoader2();
                        aF.Add(bL2);
                    }

                    if (loaders.ToLower().Contains('x'))
                    {
                        XMLDataLoader xL = new XMLDataLoader();
                        aF.Add(xL);
                    }

                    if (loaders.ToLower().Contains('j'))
                    {
                        JSONDataLoader jL = new JSONDataLoader();
                        aF.Add(jL);
                    }

                    if(aF.GetNumberOfLoaders() == 0)
                    {
                        throw new Exception("Nebyl přiřazen žádný z datových zrdrojů!");
                    }

                }

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
                        string[] p1LatLon = p1.Replace(".",",").Split("a");
                        string[] p2LatLon = p2.Replace(".", ",").Split("a");

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

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
