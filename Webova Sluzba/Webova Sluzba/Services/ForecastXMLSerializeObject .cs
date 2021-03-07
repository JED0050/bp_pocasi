using AgregaceDatLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Webova_Sluzba
{
    [XmlRoot("XMLForecasts")]
    public class ForecastXMLSerializeObject
    {
        public ForecastXMLSerializeObject()
        {

        }

        public ForecastXMLSerializeObject(Forecast forecast)
        {
            DataSources = forecast.DataSources;

            Coord = new  PointLonLat(double.Parse(forecast.Latitude, CultureInfo.InvariantCulture), double.Parse(forecast.Longitude, CultureInfo.InvariantCulture));

            ForecastXMLObject f = new ForecastXMLObject(forecast);

            Forecasts.Add(f);

        }

        public ForecastXMLSerializeObject(List<Forecast> forecasts)
        {
            Forecast forecast = forecasts[0];

            DataSources = forecast.DataSources;

            Coord = new PointLonLat(double.Parse(forecast.Latitude, CultureInfo.InvariantCulture), double.Parse(forecast.Longitude, CultureInfo.InvariantCulture));

            foreach(Forecast f in forecasts)
            {
                ForecastXMLObject fO = new ForecastXMLObject(f);
                Forecasts.Add(fO);
            }
        }
        [XmlArrayItem("DataSource")]
        public List<string> DataSources = new List<string>();

        public PointLonLat Coord;

        [XmlArrayItem("Forecast")]
        public List<ForecastXMLObject> Forecasts = new List<ForecastXMLObject>();
    }


    public class ForecastXMLObject
    {
        public ForecastXMLObject()
        {

        }

        public ForecastXMLObject(Forecast forecast)
        {
            DateTime.Value = forecast.Time;
            DateTime.Format = "ISO 8601";

            if (forecast.Temperature.HasValue)
            {
                Temperature = new ForecastXMLAttribute<double>();
                Temperature.Value = forecast.Temperature.Value;
                Temperature.Unit = "°C";
            }

            if (forecast.Precipitation.HasValue)
            {
                Precipitation = new ForecastXMLAttribute<double>();
                Precipitation.Value = forecast.Precipitation.Value;
                Precipitation.Unit = "mm";
            }

            if (forecast.Humidity.HasValue)
            {
                Humidity = new ForecastXMLAttribute<double>();
                Humidity.Value = forecast.Humidity.Value;
                Humidity.Unit = "%";
            }

            if (forecast.Pressure.HasValue)
            {
                Pressure = new ForecastXMLAttribute<double>();
                Pressure.Value = forecast.Pressure.Value;
                Pressure.Unit = "hPa";
            }
        }

        public ForecastXMLAttribute<DateTime> DateTime = new ForecastXMLAttribute<DateTime>();

        public ForecastXMLAttribute<double> Temperature = null;     //teplota
        public ForecastXMLAttribute<double> Precipitation = null;   //srážky
        public ForecastXMLAttribute<double> Humidity = null;        //vlhkost
        public ForecastXMLAttribute<double> Pressure = null;        //tlak
    }

    public class ForecastXMLAttribute<T>
    {
        [XmlAttribute]
        public T Value { get; set; }

        [XmlAttribute]
        public string Unit { get; set; }

        [XmlAttribute]
        public string Format { get; set; }
    }


}
