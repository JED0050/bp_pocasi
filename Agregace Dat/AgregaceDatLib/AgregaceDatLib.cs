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

    public interface DataLoader
    {

        Forecast GetForecastByTimeInPrague(DateTime t);

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
            string text = "";

            using (var client = new WebClient())
            {
                text = client.DownloadString(UrlAdress);
            }

            return text;
        }

        public Forecast GetForecastByTimeInPrague(DateTime t)
        {
            Forecast f = new Forecast();

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
