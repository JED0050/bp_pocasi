using System;
using System.Collections.Generic;
using System.Text;

namespace IDataLoaderAndHandlerLib.HandlersAndObjects
{
    public struct ForecastTypes
    {
        public static readonly string PRECIPITATION = "prec";
        public static readonly string TEMPERATURE = "temp";
        public static readonly string HUMIDITY = "humi";
        public static readonly string PRESSURE = "pres";

        public static bool IsTypeKnown(string type)
        {
            return (type == PRECIPITATION || type == TEMPERATURE || type == HUMIDITY || type == PRESSURE);
        }
    }
}
