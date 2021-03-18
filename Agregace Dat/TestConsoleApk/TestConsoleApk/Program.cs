using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using AgregaceDatLib;
using IDataLoaderAndHandlerLib.Interface;

namespace TestConsoleApk
{
    class Program
    {

        static void Main(string[] args)
        {

            AvgForecast a = new AvgForecast();
            a = new AvgForecast("");
            a = new AvgForecast();
            a = new AvgForecast("");
            a = new AvgForecast("");
            a = new AvgForecast();
            a = new AvgForecast("");
            a = new AvgForecast("");
            a = new AvgForecast();
            a = new AvgForecast("");

            AvgForecast b = new AvgForecast();
            b = new AvgForecast("");
            b = new AvgForecast();
            b = new AvgForecast("");
            b = new AvgForecast("");
            b = new AvgForecast();
            b = new AvgForecast("");
            b = new AvgForecast("");
            b = new AvgForecast();
            b = new AvgForecast("");

            Console.ReadLine();

            b = new AvgForecast("");


            //string x = Environment.CurrentDirectory;

            //Console.WriteLine(x);

            //AvgForecast a = new AvgForecast("");



            /*
            string dllPath =
                @"D:\Škola - programy\Bakalářka - Předpověď počasí\bp_pocasi\Agregace Dat\DatoveZdrojeAHandlery\DLRadarBourkyLib\DLRadarBourkyLib\bin\Debug\netcoreapp2.1\DLRadarBourkyLib.dll";

            var assembly = Assembly.LoadFile(dllPath);

            var type = assembly.GetType("DLRadarBourkyLib.RadarBourkyDataLoader");

            DataLoader dL = (DataLoader)Activator.CreateInstance(type);

            Console.WriteLine(dL.GetLoaderConfigFile().DataLoaderName);*/
        }

        
    }
}
