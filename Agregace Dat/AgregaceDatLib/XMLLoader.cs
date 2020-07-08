using System;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace AgregaceDatLib
{

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

}
