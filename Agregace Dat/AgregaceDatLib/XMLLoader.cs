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
                try
                {
                    xmlText = client.DownloadString(url);
                }
                catch { }
                
            }

            return xmlText;
        }

        public Forecast GetForecastByTime(DateTime t, string url)
        {
            Forecast f = new Forecast();
            f.Time = t;

            string xmlText = GetXML(url);

            TextReader tr = new StringReader(xmlText);

            if (xmlText == "")
            {
                f.Temperature = -1;
                return f;
            }

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

            string links = @"http://www.yr.no/place/Czech_Republic/Other/HavÃ­Å™ov/forecast.xml
http://www.yr.no/place/Czech_Republic/Moravia-Silesia/TÅ™inec/forecast.xml
http://www.yr.no/place/Czech_Republic/Other/OrlovÃ¡/forecast.xml
http://www.yr.no/place/Czech_Republic/Other/ÄŒeskÃ½_TÄ›Å¡Ã­n/forecast.xml
http://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Moravia/Brno/forecast.xml
http://www.yr.no/place/Czech_Republic/Moravia-Silesia/Ostrava/forecast.xml
http://www.yr.no/place/Czech_Republic/PlzeÅˆ/PlzeÅˆ/forecast.xml
http://www.yr.no/place/Czech_Republic/Olomouc/Olomouc/forecast.xml
http://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml
http://www.yr.no/place/Czech_Republic/Prague/ModÅ™any/forecast.xml
http://www.yr.no/place/Czech_Republic/Prague/LibeÅˆ/forecast.xml
http://www.yr.no/place/Czech_Republic/Prague/ÄŒernÃ½_Most/forecast.xml
http://www.yr.no/place/Czech_Republic/Prague/BranÃ­k/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Moravia/Brno/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Moravia/Znojmo/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Moravia/HodonÃ­n~3075654/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Moravia/BÅ™eclav/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Moravia/VyÅ¡kov/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Bohemia/Budweis/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Bohemia/TÃ¡bor/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Bohemia/PÃ­sek/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Bohemia/Strakonice/forecast.xml
http://www.yr.no/place/Czech_Republic/South_Bohemia/JindÅ™ichÅ¯v_Hradec/forecast.xml
http://www.yr.no/place/Czech_Republic/VysoÄina/Jihlava/forecast.xml
http://www.yr.no/place/Czech_Republic/VysoÄina/TÅ™ebÃ­Ä/forecast.xml
http://www.yr.no/place/Czech_Republic/VysoÄina/HavlÃ­ÄkÅ¯v_Brod/forecast.xml
http://www.yr.no/place/Czech_Republic/VysoÄina/Å½ÄÃ¡r_nad_SÃ¡zavou/forecast.xml
http://www.yr.no/place/Czech_Republic/VysoÄina/PelhÅ™imov/forecast.xml
http://www.yr.no/place/Czech_Republic/Karlovy_Vary/Karlovy_Vary/forecast.xml
http://www.yr.no/place/Czech_Republic/Karlovy_Vary/Cheb/forecast.xml
http://www.yr.no/place/Czech_Republic/Karlovy_Vary/Sokolov/forecast.xml
http://www.yr.no/place/Czech_Republic/Karlovy_Vary/Ostrov~3068766/forecast.xml
http://www.yr.no/place/Czech_Republic/Karlovy_Vary/Chodov~3077706/forecast.xml
http://www.yr.no/place/Czech_Republic/Hradec_KrÃ¡lovÃ©/Hradec_KrÃ¡lovÃ©/forecast.xml
http://www.yr.no/place/Czech_Republic/Hradec_KrÃ¡lovÃ©/Trutnov/forecast.xml
http://www.yr.no/place/Czech_Republic/Hradec_KrÃ¡lovÃ©/NÃ¡chod/forecast.xml
http://www.yr.no/place/Czech_Republic/Hradec_KrÃ¡lovÃ©/JiÄÃ­n/forecast.xml
http://www.yr.no/place/Czech_Republic/Hradec_KrÃ¡lovÃ©/Rychnov_nad_KnÄ›Å¾nou/forecast.xml
http://www.yr.no/place/Czech_Republic/Liberec/Liberec/forecast.xml
http://www.yr.no/place/Czech_Republic/Liberec/Jablonec_nad_Nisou/forecast.xml
http://www.yr.no/place/Czech_Republic/Liberec/ÄŒeskÃ¡_LÃ­pa/forecast.xml
http://www.yr.no/place/Czech_Republic/Liberec/Semily/forecast.xml
http://www.yr.no/place/Czech_Republic/Liberec/VelkÃ¡_JavorskÃ¡/forecast.xml
http://www.yr.no/place/Czech_Republic/Olomouc/Olomouc/forecast.xml
http://www.yr.no/place/Czech_Republic/Olomouc/ProstÄ›jov/forecast.xml
http://www.yr.no/place/Czech_Republic/Olomouc/PÅ™erov/forecast.xml
http://www.yr.no/place/Czech_Republic/Olomouc/Å umperk/forecast.xml
http://www.yr.no/place/Czech_Republic/Olomouc/Hranice/forecast.xml
http://www.yr.no/place/Czech_Republic/Moravia-Silesia/Ostrava/forecast.xml
http://www.yr.no/place/Czech_Republic/Moravia-Silesia/KarvinÃ¡~3073789/forecast.xml
http://www.yr.no/place/Czech_Republic/Moravia-Silesia/Opava/forecast.xml
http://www.yr.no/place/Czech_Republic/Moravia-Silesia/FrÃ½dek-MÃ­stek/forecast.xml
http://www.yr.no/place/Czech_Republic/Moravia-Silesia/NovÃ½_JiÄÃ­n/forecast.xml
http://www.yr.no/place/Czech_Republic/Pardubice/Pardubice/forecast.xml
http://www.yr.no/place/Czech_Republic/Pardubice/Chrudim/forecast.xml
http://www.yr.no/place/Czech_Republic/Pardubice/Svitavy/forecast.xml
http://www.yr.no/place/Czech_Republic/Pardubice/ÄŒeskÃ¡_TÅ™ebovÃ¡/forecast.xml
http://www.yr.no/place/Czech_Republic/Pardubice/ÃšstÃ­_nad_OrlicÃ­/forecast.xml
http://www.yr.no/place/Czech_Republic/PlzeÅˆ/PlzeÅˆ/forecast.xml
http://www.yr.no/place/Czech_Republic/PlzeÅˆ/Klatovy/forecast.xml
http://www.yr.no/place/Czech_Republic/PlzeÅˆ/Rokycany/forecast.xml
http://www.yr.no/place/Czech_Republic/PlzeÅˆ/Tachov/forecast.xml
http://www.yr.no/place/Czech_Republic/PlzeÅˆ/SuÅ¡ice/forecast.xml
http://www.yr.no/place/Czech_Republic/Central_Bohemia/Kladno/forecast.xml
http://www.yr.no/place/Czech_Republic/Central_Bohemia/MladÃ¡_Boleslav/forecast.xml
http://www.yr.no/place/Czech_Republic/Central_Bohemia/PÅ™Ã­bram/forecast.xml
http://www.yr.no/place/Czech_Republic/Central_Bohemia/KolÃ­n/forecast.xml
http://www.yr.no/place/Czech_Republic/Central_Bohemia/KutnÃ¡_Hora/forecast.xml
http://www.yr.no/place/Czech_Republic/ÃšstÃ­_nad_Labem/ÃšstÃ­_nad_Labem/forecast.xml
http://www.yr.no/place/Czech_Republic/ÃšstÃ­_nad_Labem/Most/forecast.xml
http://www.yr.no/place/Czech_Republic/ÃšstÃ­_nad_Labem/DÄ›ÄÃ­n/forecast.xml
http://www.yr.no/place/Czech_Republic/ÃšstÃ­_nad_Labem/Teplice/forecast.xml
http://www.yr.no/place/Czech_Republic/ÃšstÃ­_nad_Labem/Chomutov/forecast.xml
http://www.yr.no/place/Czech_Republic/ZlÃ­n/ZlÃ­n~3061369/forecast.xml
http://www.yr.no/place/Czech_Republic/ZlÃ­n/ZlÃ­n/forecast.xml
http://www.yr.no/place/Czech_Republic/ZlÃ­n/KromÄ›Å™Ã­Å¾/forecast.xml
http://www.yr.no/place/Czech_Republic/ZlÃ­n/VsetÃ­n/forecast.xml
http://www.yr.no/place/Czech_Republic/ZlÃ­n/ValaÅ¡skÃ©_MeziÅ™Ã­ÄÃ­/forecast.xml";

            string[] locations = links.Split("\r\n");   //jen \n nestačí a velká část linků neprojde, je potřeba použít \r\n

            foreach (string loc in locations)
            {

                Forecast f = GetForecastByTime(forTime, loc);

                if (f.Temperature == -1)    //neplatná předpověď, webová služba vrátila error místo XML dat
                {
                    //Console.WriteLine("N " + loc);
                    continue;
                }
                //else
                //    Console.WriteLine("A " + loc);

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

                DrawIntersectionCircle(20, x, y, forBitmap, f.GetPrecipitationColor());

            }


            //forBitmap.Save("XMLBitmap" + forTime.ToString("yyyyMMddHH") + ".bmp", ImageFormat.Bmp);

            return forBitmap;

        }

        
    }

}
