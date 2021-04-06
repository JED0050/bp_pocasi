using System;
using System.Collections.Generic;

namespace Vizualizace_Dat
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

        public static List<string> GetListOfTypes()
        {
            List<string> typesList = new List<string>();

            typesList.Add(PRECIPITATION);
            typesList.Add(TEMPERATURE);
            typesList.Add(HUMIDITY);
            typesList.Add(PRESSURE);

            return typesList;
        }
    }
}
