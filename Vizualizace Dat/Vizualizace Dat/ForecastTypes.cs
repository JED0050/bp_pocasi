using System;

namespace Vizualizace_Dat
{
    public class ForecastTypes
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
