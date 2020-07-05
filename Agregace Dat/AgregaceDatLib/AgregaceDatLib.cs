using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace AgregaceDatLib
{

    //XML
    //Yr.no - https://www.yr.no/place/Norway/Vestfold_og_Telemark/Midt-Telemark/Gvarv/forecast.xml
    //https://www.yr.no/place/Czech_Republic/Olomouc/Olomouc/forecast.xml
    //http://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml

    //JSON
    //api.openweathermap.org/data/2.5/forecast/daily?lat=35&lon=139&cnt=10
    //API KEY - ea63080a4f8e99972630d2671e3ef805
    //pro.openweathermap.org/data/2.5/forecast/hourly?lat={lat}&lon={lon}&appid={your api key}
    //pro.openweathermap.org/data/2.5/forecast/hourly?lat=30&lon=160&appid=ea63080a4f8e99972630d2671e3ef805
    //https://samples.openweathermap.org/data/2.5/forecast/daily?lat=35&lon=139&cnt=10&appid=b1b15e88fa797225412429c1c50c122a1

    //BITMAP
    //https://www.in-pocasi.cz/radarove-snimky/
    //https://www.in-pocasi.cz/radarove-snimky/napoveda/
    //https://www.in-pocasi.cz/data/chmi_v2/20200531_0800_r.png                 //599 x 380
    //http://portal.chmi.cz/files/portal/docs/meteo/rad/img/banner-text.png     
    //http://radar.bourky.cz/data/pacz2gmaps.z_max3d.20200705.1020.0.png        //728 x 528
    //                                               ROK MĚSÍC DEN.HODINY MINUTY.0.png 

    public interface DataLoader
    {
        Forecast GetForecastByTime(DateTime t);

    }

    public class XMLDataLoader : DataLoader
    {
        public string UrlAdress { get; set; }

        public XMLDataLoader(string uA)
        {
            UrlAdress = uA;
        }



        public string GetXML()
        {
            string xmlText = "";

            using (var client = new WebClient())
            {
                xmlText = client.DownloadString(UrlAdress);
            }

            return xmlText;
        }

        public Forecast GetForecastByTime(DateTime t)
        {
            Forecast f = new Forecast();
            f.Time = t;

            TextReader tr = new StringReader(GetXML());


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



    }

    public class JSONDataLoader : DataLoader
    {
        public string UrlAdress { get; set; }

        public JSONDataLoader(string url)
        {
            UrlAdress = url;
        }

        public string GetJSON()
        {
            string JSONText = "";

            using (var client = new WebClient())
            {
                JSONText = client.DownloadString(UrlAdress);
            }

            return JSONText;
        }

        public Forecast GetForecastByTime(DateTime t)
        {
            Forecast f = new Forecast();
            f.Time = t;

            string JSONtext = GetJSON();

            //JSONForecast jF = JsonConvert.DeserializeObject<JSONForecast>(JSONtext);

            dynamic jsonForecast = JObject.Parse(JSONtext);

            f.City = jsonForecast.city.name;
            f.Country = jsonForecast.city.country;
            f.Latitude = jsonForecast.city.coord.lat;
            f.Longitude = jsonForecast.city.coord.lon;

            JArray jsonForecastArray = (JArray)jsonForecast["list"];

            int counter = 0;

            /*
            //zaokrouhlení na celou hodinu
            t.AddMinutes(30);
            t = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0);
            */

            foreach (var el in jsonForecastArray)
            {
                dynamic jsonElement = JObject.Parse(el.ToString());

                if (counter == 0)
                {
                    f.Temperature = Double.Parse(jsonElement.main.temp.ToString().Replace('.', ',')) - 273.15;  //Kelvin na celsius

                    try
                    {
                        f.Precipitation = Double.Parse(jsonElement.rain.GetValue("3h").ToString().Replace('.', ','));   //jsonElement.main.rain.3h ?
                    }
                    catch
                    {
                        f.Precipitation = 0;
                    }

                    counter++;
                    continue;
                }

                DateTime from = DateTime.Parse(jsonElement.dt_txt.ToString());
                DateTime to = from.AddHours(3); //předpověď v 3 hodinovém okně

                if (t >= from && t <= to)
                {
                    f.Temperature = Double.Parse(jsonElement.main.temp.ToString().Replace('.', ',')) - 273.15;

                    try
                    {
                        f.Precipitation = Double.Parse(jsonElement.rain.GetValue("3h").ToString().Replace('.', ','));
                    }
                    catch
                    {
                        f.Precipitation = 0;
                    }

                    break;
                }


            }


            return f;
        }
    }

    public class BitmapDataLoader : DataLoader
    {
        public string UrlAdress { get; set; }

        public double PixelLon { get; set; }
        public double PixelLat { get; set; }

        public BitmapDataLoader(string url)
        {
            UrlAdress = url;
        }

        public double GetPrecipitationFromPixel(Color pixel)
        {
            double precipitation = 0;

            /*
            srážky dle RGB barev

            index - R G B - srážky

            0 - 0 0 0 - 0 mm/h
            4 - 56 0 112 - 0 mm/h

            8 - 48 0 168 - 0.1 mm/h
            12 - 0 0 252 - 0.2 mm/h
            16 - 0 108 192 - 0.4 mm/h
            20 - 0 160 0 - 0.8 mm/h

            24 - 0 188 0 - 1 mm/h
            28 - 52 216 0 - 2 mm/h
            32 - 156 220 0 - 4 mm/h
            36 - 224 220 0 - 8 mm/h

            40 - 252 176 0 - 10 mm/h
            44 - 252 132 0 - 30 mm/h
            48 - 252 88 0 - 60 mm/h

            52 - 252 0 0 - 100 mm/h
            56 - 160 0 0 - 100+ mm/h
            60 - 255 255 255 - 100+ mm/h

            */

            if (ColInRange(0, 56, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(0, 167, pixel.B))
                precipitation = 0;
            else if (ColInRange(0, 48, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(168, 251, pixel.B))
                precipitation = 0.1;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(0, 107, pixel.G) && ColInRange(193, 252, pixel.B))
                precipitation = 0.2;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(108, 159, pixel.G) && ColInRange(1, 192, pixel.B))
                precipitation = 0.4;
            else if (ColInRange(0, 0, pixel.R) && ColInRange(160, 187, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 0.8;
            else if (ColInRange(0, 51, pixel.R) && ColInRange(188, 215, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 1;
            else if (ColInRange(52, 155, pixel.R) && ColInRange(216, 219, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 2;
            else if (ColInRange(156, 223, pixel.R) && ColInRange(220, 220, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 4;
            else if (ColInRange(224, 251, pixel.R) && ColInRange(177, 220, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 8;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(133, 176, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 10;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(89, 132, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 30;
            else if (ColInRange(252, 255, pixel.R) && ColInRange(1, 88, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 60;
            else if (ColInRange(150, 255, pixel.R) && ColInRange(0, 0, pixel.G) && ColInRange(0, 0, pixel.B))
                precipitation = 100;

            return precipitation;
        }

        private bool ColInRange(int min, int max, int val)
        {
            return (min <= val) && (val <= max);
        }

        private void SetPixelCoord(int w, int h)
        {
            //levý horní roh, 51.88 10.06
            //lavý doní roh, 47.09 10.6
            //pravý horní roh, 51.88 20.21
            //pravý doní roh, 47.09 20.21

            double lonDif = 20.21 - 10.06;
            double latDif = 51.88 - 47.09;


            PixelLon = lonDif / w;
            PixelLat = latDif / h;

        }

        public Bitmap GetBitmap()
        {

            Bitmap bitmap;

            using (WebClient client = new WebClient())
            {
                Stream stream = client.OpenRead(UrlAdress);
                bitmap = new Bitmap(stream);

                //stream.Flush();
                //stream.Close();
                //client.Dispose();
            }

            return bitmap;

        }

        public Forecast GetForecastByTime(DateTime t)
        {
            Bitmap bitmap = GetBitmap();

            SetPixelCoord(bitmap.Width, bitmap.Height);

            Forecast f = new Forecast();
            f.Time = t;

            //Praha
            double targetLat = 50.5; 
            double targetLon = 14.25;

            //pravý horní roh bitmapy
            double bY = 51.88;
            double bX = 10.06;

            for (int y = 0; y < bitmap.Height; y++)
            {
                bX = 10.06;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    if( bX <= targetLon && targetLon <= bX + PixelLon && bY - PixelLat <= targetLat && targetLat <= bY)
                    {
                        /*
                        Console.WriteLine("Bingo!");
                        Console.WriteLine("cyklus x: {0} y: {1}", x, y);
                        Console.WriteLine("bitmapa x: {0} y: {1}", bX, bY);
                        Console.WriteLine("praha x: {0} y: {1}", targetLon, targetLat);
                        */

                        f.Precipitation = GetPrecipitationFromPixel(bitmap.GetPixel(x,y));
                    }

                    bX += PixelLon;
                }

                bY -= PixelLat;

            }

            return f;
        }
    }

    public class Forecast
    {

        public string City { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }        //zeměpisná šířka "y"
        public string Longitude { get; set; }       //zeměpisná délka "x"
        public DateTime Time { get; set; }
        public double Temperature { get; set; }     //teplota celsius
        public double Precipitation { get; set; }   //srážky = slabá (0,1 – 2,5), mírná	(2,6 – 8), silná (8 – 40), velmi silná (> 40)


        public string ToString()
        {
            string output = "Country: " + Country + " City: " + City + " Time: " + Time + " Temperature: " + Temperature + " Precipitation: " + Precipitation;

            return output;
        }

        internal void AddForecast(Forecast newF)
        {
            Temperature += newF.Temperature;
            Precipitation += newF.Precipitation;
        }

        internal void SetAvgForecast(int numOfFcs)
        {
            Temperature = Temperature / numOfFcs;
            Precipitation = Precipitation / numOfFcs;
        }
    }

    public class AvgForecast
    {
        private List<DataLoader> dLs;

        public AvgForecast()
        {
            dLs = new List<DataLoader>();
        }

        public AvgForecast(List<DataLoader> defDLs)
        {
            dLs = defDLs;
        }

        public void Add(DataLoader newDl)
        {
            dLs.Add(newDl);
        }

        public void Remove(DataLoader remDl)
        {
            dLs.Remove(remDl);
        }

        public Forecast GetForecast(DateTime time, String place)
        {

            Forecast newF = new Forecast();

            for(int i = 0; i < dLs.Count - 1; i++)
            {
                if(i == 0)
                {
                    newF = dLs[0].GetForecastByTime(time);
                    continue;
                }

                newF.AddForecast(dLs[i].GetForecastByTime(time));
            }

            newF.SetAvgForecast(dLs.Count);

            return newF;
        }

    }

}
