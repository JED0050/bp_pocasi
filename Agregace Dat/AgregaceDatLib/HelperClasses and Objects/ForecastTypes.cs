using System;
using System.Collections.Generic;
using System.Text;

namespace AgregaceDatLib
{
    public class ForecastTypes
    {
        public static string PRECIPITATION = "prec";
        public static string TEMPERATURE = "temp";
        public static string HUMIDITY = "humi";
        public static string PRESSURE = "pres";

        public static bool IsTypeKnown(string type)
        {
            return (type == PRECIPITATION || type == TEMPERATURE || type == HUMIDITY || type == PRESSURE);
        }
    }
}
