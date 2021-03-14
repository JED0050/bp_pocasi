using AgregaceDatLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IDataLoaderAndHandlerLib.HandlersAndObjects;

namespace Webova_Sluzba
{
    public class ForecastJSONSerializeObject
    {
        public ForecastJSONSerializeObject(Forecast forecast)
        {
            DataSources = forecast.DataSources;

            Coord = new PointLonLat(forecast.DLongitude, forecast.DLatitude);

            ForecastJsonObject f = new ForecastJsonObject(forecast);

            Forecasts.Add(f);

        }

        public ForecastJSONSerializeObject(List<Forecast> forecasts)
        {
            Forecast forecast = forecasts[0];

            DataSources = forecast.DataSources;

            Coord = new PointLonLat(forecast.DLongitude, forecast.DLatitude);

            foreach (Forecast f in forecasts)
            {
                ForecastJsonObject fO = new ForecastJsonObject(f);
                Forecasts.Add(fO);
            }
        }
        public List<string> DataSources = new List<string>();
        public PointLonLat Coord { get; set; }
        public List<ForecastJsonObject> Forecasts = new List<ForecastJsonObject>();
    }


    public class ForecastJsonObject
    {

        public ForecastJsonObject(Forecast forecast)
        {
            Time = forecast.Time;
            Temperature = forecast.Temperature;
            Precipitation = forecast.Precipitation;
            Pressure = forecast.Pressure;
            Humidity = forecast.Humidity;
        }

        public DateTime Time { get; set; }

        public double? Temperature { get; set; }     //teplota
        public double? Precipitation { get; set; }   //srážky
        public double? Humidity { get; set; }        //vlhkost
        public double? Pressure { get; set; }        //tlak
    }


}
