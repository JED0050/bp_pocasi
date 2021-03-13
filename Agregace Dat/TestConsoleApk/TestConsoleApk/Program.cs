using System;
using AgregaceDatLib;

namespace TestConsoleApk
{
    class Program
    {
        static void Main(string[] args)
        {
            AvgForecast a = new AvgForecast("");

            a.SaveForecastBitmaps();
        }
    }
}
