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

            //BitmapDataLoader bL = new BitmapDataLoader();
            XMLDataLoader xL = new XMLDataLoader();
            JSONDataLoader jL = new JSONDataLoader();

            //aF.Add(bL);
            aF.Add(xL);
            aF.Add(jL);

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

        public IActionResult Forecast(string type, string time, string loaders)
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
                        string[] dateTimeParts = time.Split('T');   //ISO 8601 = yyyy-MM-ddTHH:mm:ss

                        List<int> date = dateTimeParts[0].Split('-').Select(Int32.Parse).ToList();
                        List<int> hours = dateTimeParts[1].Split(':').Select(Int32.Parse).ToList();

                        dateTime = new DateTime(date[0], date[1], date[2], hours[0], hours[1], hours[2]);  //rok mesic den hodina minuta sekunda
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
                    //JSONDataLoader jL = new JSONDataLoader();

                    aF.Add(bL);
                    aF.Add(xL);
                    //aF.Add(jL);
                }
                else
                {
                    if(loaders.ToLower().Contains('b'))
                    {
                        BitmapDataLoader bL = new BitmapDataLoader();
                        aF.Add(bL);
                    }

                    if (loaders.ToLower().Contains('x'))
                    {
                        XMLDataLoader xL = new XMLDataLoader();
                        aF.Add(xL);
                    }

                    if (loaders.ToLower().Contains('j'))
                    {
                        //JSONDataLoader jL = new JSONDataLoader();
                        //aF.Add(jL);
                    }

                }

                if (type.ToLower() == "prec")
                {

                    Bitmap precBitmap = aF.GetAvgForecBitmap(dateTime);

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
