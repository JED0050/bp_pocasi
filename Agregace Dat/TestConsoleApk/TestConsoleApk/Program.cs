using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using AgregaceDatLib;
using DLRadarBourkyLib;
using IDataLoaderAndHandlerLib.HandlersAndObjects;
using IDataLoaderAndHandlerLib.Interface;

namespace TestConsoleApk
{
    class Program
    {

        static void Main(string[] args)
        {

            AvgForecast a = new AvgForecast();
            a.Add(new RadarBourkyDataLoader());

            a.GetAvgForecBitmap(DateTime.Now.AddHours(-5), ForecastTypes.PRECIPITATION);
        }

        
    }
}
