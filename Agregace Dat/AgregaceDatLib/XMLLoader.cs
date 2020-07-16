using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace AgregaceDatLib
{

    public class XMLDataLoader : BitmapHelper, DataLoader
    {

        public string GetXML(string url)
        {
            string xmlText = "";

            using (var client = new WebClient())
            {
                xmlText = client.DownloadString(url);
            }

            return xmlText;
        }

        public Forecast GetForecastByTime(DateTime t, string url)
        {
            Forecast f = new Forecast();
            f.Time = t;

            TextReader tr = new StringReader(GetXML(url));


            XDocument xmlDoc = XDocument.Load(tr);


            foreach (var weatherData in xmlDoc.Descendants("weatherdata"))
            {
                int counter = 0;

                foreach (var location in xmlDoc.Descendants("location"))
                {
                    if (counter == 0)
                    {
                        f.Country = location.Element("country").Value;
                        f.City = location.Element("name").Value;

                        counter++;
                    } else
                    {

                        f.Latitude = location.Attribute("latitude").Value;
                        f.Longitude = location.Attribute("longitude").Value;
                    }
                }

                counter = 0;

                foreach (var forecast in xmlDoc.Descendants("forecast"))
                {
                    foreach (var tabular in xmlDoc.Descendants("tabular"))
                    {
                        foreach (var time in xmlDoc.Descendants("time"))
                        {
                            if (counter == 0)    //nastavení první hodnoty pokud není žádná jiná známá
                            {
                                counter++;

                                f.Temperature = Double.Parse(time.Element("temperature").Attribute("value").Value.Replace('.', ','));
                                f.Precipitation = Double.Parse(time.Element("precipitation").Attribute("value").Value.Replace('.', ','));

                                continue;
                            }

                            DateTime from = DateTime.Parse(time.Attribute("from").Value);
                            DateTime to = DateTime.Parse(time.Attribute("to").Value);

                            if (t >= from && t <= to)
                            {
                                f.Temperature = Double.Parse(time.Element("temperature").Attribute("value").Value.Replace('.', ','));
                                f.Precipitation = Double.Parse(time.Element("precipitation").Attribute("value").Value.Replace('.', ','));

                                break;
                            }

                        }
                    }
                }
            }


            return f;
        }


        public Bitmap GetForecastBitmap(DateTime forTime)
        {
            Bitmap forBitmap = new Bitmap(728, 528);

            List<string> locations = new List<string>();
            locations.Add("https://www.yr.no/place/Czech_Republic/Olomouc/Olomouc/forecast.xml");           //Olomouc
            locations.Add("https://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml");             //Praha
            locations.Add("https://www.yr.no/place/Czech_Republic/South_Moravia/Brno/forecast.xml");        //Brno
            locations.Add("https://www.yr.no/place/Czech_Republic/Moravia-Silesia/Ostrava/forecast.xml");   //Ostrava
            locations.Add("https://www.yr.no/place/Czech_Republic/Plze%C5%88/Plze%C5%88/forecast.xml");     //Plzeň
            locations.Add("https://www.yr.no/place/Czech_Republic/Hradec_Kr%C3%A1lov%C3%A9/Hradec_Kr%C3%A1lov%C3%A9/forecast.xml"); //Hradec Králové
            locations.Add("https://www.yr.no/place/Czech_Republic/Karlovy_Vary/Karlovy_Vary/forecast.xml"); //Karlovy Vary
            locations.Add("https://www.yr.no/place/Czech_Republic/Liberec/Liberec/forecast.xml");           //Liberec
            locations.Add("https://www.yr.no/place/Czech_Republic/Pardubice/Pardubice/forecast.xml");       //Pardubice
            locations.Add("https://www.yr.no/place/Czech_Republic/%C3%9Ast%C3%AD_nad_Labem/%C3%9Ast%C3%AD_nad_Labem/forecast.xml"); //Ústí nad Labem
            locations.Add("https://www.yr.no/place/Czech_Republic/Vyso%C4%8Dina/Jihlava/forecast.xml");     //Jihlava
            locations.Add("https://www.yr.no/place/Czech_Republic/Zl%C3%ADn/Zl%C3%ADn/forecast.xml");       //Zlín
            locations.Add("https://www.yr.no/place/Czech_Republic/South_Bohemia/Okres_%C4%8Cesk%C3%A9_Bud%C4%9Bjovice/forecast.xml");//České Budějovice

            foreach (string loc in locations)
            {
                Forecast f = GetForecastByTime(forTime, loc);

                double lonDif = 20.21 - 10.06;
                double latDif = 51.88 - 47.09;

                double PixelLon = lonDif / forBitmap.Width;
                double PixelLat = latDif / forBitmap.Height;

                double bY = 51.88;
                double bX = 10.06;

                double locLon = f.DLongitude;
                double locLat = f.DLatitude;

                int x;
                for(x = 0 ; x < forBitmap.Width; x++)
                {
                    if (bX >= locLon && locLon <= bX + locLon)
                        break;

                    bX += PixelLon;
                }

                int y;
                for (y = 0; y < forBitmap.Height; y++)
                {
                    if (bY - PixelLat <= locLat && locLat <= bY)
                            break;   

                    bY -= PixelLat;
                }

                //tmpB.SetPixel(x, y, f.GetPrecipitationColor());

                DrawIntersectionCircle(40, x, y, forBitmap, f.GetPrecipitationColor());

            }


            //forBitmap.Save("XMLBitmap" + forTime.ToString("yyyyMMddHH") + ".bmp", ImageFormat.Bmp);

            return forBitmap;

        }

        
    }

}
