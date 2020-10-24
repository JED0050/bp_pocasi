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

        public IActionResult Forecast(string type, string time)
        {

            if (type == null || time == null)
            {
                throw new Exception("Oba vstupní parametry musí být vyplněny!");
            }
            else
            {
                List<int> timeParts = time.Split('.').Select(Int32.Parse).ToList();

                DateTime dateTime;

                try
                {
                    dateTime = new DateTime(timeParts[0], timeParts[1], timeParts[2], timeParts[3], timeParts[4], 0);  //rok mesic den hodina minuta sekunda
                }
                catch
                {
                    throw new Exception("Formát času není správně vyplněn! (yyyy.mm.dd.hh.mm)");
                }

                if (type.ToLower() == "prec")
                {

                    //string workingDirectory = Environment.CurrentDirectory;
                    //string p = workingDirectory + @"\Data\Yr.no\" + "radar.xml";

                    BitmapDataLoader bL = new BitmapDataLoader();
                    XMLDataLoader xL = new XMLDataLoader();
                    JSONDataLoader jL = new JSONDataLoader();

                    AvgForecast aF = new AvgForecast();
                    aF.Add(bL);
                    aF.Add(xL);
                    //aF.Add(jL);

                    Bitmap precBitmap = aF.GetAvgBitmap(dateTime);

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
