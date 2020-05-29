using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                    if(counter == 0)
                    {
                        f.Country = location.Element("country").Value;
                        f.City = location.Element("name").Value;

                        counter++;
                    }else
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
                            if(counter == 0)    //nastavení první hodnoty pokud není žádná jiná známá
                            {
                                counter++;

                                f.Temperature = Double.Parse(time.Element("temperature").Attribute("value").Value.Replace('.', ','));
                                f.Precipitation = Double.Parse(time.Element("precipitation").Attribute("value").Value.Replace('.', ','));

                                continue;
                            }

                            DateTime from = DateTime.Parse(time.Attribute("from").Value);
                            DateTime to = DateTime.Parse(time.Attribute("to").Value);

                            if(t >= from && t <= to)
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

        public JSONDataLoader (string url)
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

    public class Forecast
    {

        public string City { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }        //zeměpisná šířka
        public string Longitude { get; set; }       //zeměpisná délka
        public DateTime Time { get; set; }
        public double Temperature { get; set; }     //teplota celsius
        public double Precipitation { get; set; }   //srážky = slabá (0,1 – 2,5), mírná	(2,6 – 8), silná (8 – 40), velmi silná (> 40)

    }

}
