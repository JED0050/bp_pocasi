using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace AgregaceDatLib
{
    abstract public class DataLoaderHandler
    {
        protected DataLoaderConfig dataLoaderConfig;

        protected PointLonLat defaultTopLeftBound = new PointLonLat(4.1303633, 55.1995133);
        protected PointLonLat defaultBotRightBound = new PointLonLat(37.9033283, 41.6999200);

        private float sign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
        protected bool PointInTriangle(Point pt, Point v1, Point v2, Point v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        protected Color GetCollorInTriangle(Point p, Point v1, Point v2, Point v3, double val1, double val2, double val3, string type)
        {
            double dis1 = Math.Sqrt(Math.Pow(v1.X - p.X, 2) + Math.Pow(v1.Y - p.Y, 2));
            double dis2 = Math.Sqrt(Math.Pow(v2.X - p.X, 2) + Math.Pow(v2.Y - p.Y, 2));
            double dis3 = Math.Sqrt(Math.Pow(v3.X - p.X, 2) + Math.Pow(v3.Y - p.Y, 2));

            double w1, w2, w3;

            if (dis1 > 0)
                w1 = 1 / dis1;
            else
                w1 = 0;

            if (dis2 > 0)
                w2 = 1 / dis2;
            else
                w2 = 0;

            if (dis3 > 0)
                w3 = 1 / dis3;
            else
                w3 = 0;

            double avgVal = (w1 * val1 + w2 * val2 + w3 * val3) / (w1 + w2 + w3);

            return ColorValueHandler.GetColorForValueAndType(avgVal, type);
        }
        
        protected double GetValueFromBitmapTypeAndPoints(Bitmap bmp, PointLonLat p1, PointLonLat p2, PointLonLat target, string type)
        {
            double lonDif = Math.Abs(p1.Lon - p2.Lon);
            double latDif = Math.Abs(p1.Lat - p2.Lat);

            double PixelLon = lonDif / bmp.Width;
            double PixelLat = latDif / bmp.Height;

            double bY = p1.Lat;
            double bX = p1.Lon;

            double locLon = target.Lon;
            double locLat = target.Lat;

            int x;
            for (x = 0; x < bmp.Width - 1; x++)
            {
                if (bX >= locLon && locLon <= bX + PixelLon)
                    break;

                bX += PixelLon;
            }

            int y;
            for (y = 0; y < bmp.Height - 1; y++)
            {
                if (bY - PixelLat <= locLat && locLat <= bY)
                    break;

                bY -= PixelLat;
            }

            Color pixel = bmp.GetPixel(x, y);

            return ColorValueHandler.GetValueForColorAndType(pixel, type);
        }

        protected Point GetPointFromBoundsAndTarget(Size bmpSize, PointLonLat p1, PointLonLat p2, PointLonLat target)
        {
            Point point = new Point();

            double lonDif = Math.Abs(p1.Lon - p2.Lon);
            double latDif = Math.Abs(p1.Lat - p2.Lat);

            double PixelLon = lonDif / bmpSize.Width;
            double PixelLat = latDif / bmpSize.Height;

            double bY = p1.Lat;
            double bX = p1.Lon;

            double locLon = target.Lon;
            double locLat = target.Lat;

            int x;
            for (x = 0; x < bmpSize.Width - 1; x++)
            {
                if (bX >= locLon && locLon <= bX + PixelLon)
                    break;

                bX += PixelLon;
            }

            int y;
            for (y = 0; y < bmpSize.Height - 1; y++)
            {
                if (bY - PixelLat <= locLat && locLat <= bY)
                    break;

                bY -= PixelLat;
            }

            point.X = x;
            point.Y = y;

            return point;
        }

        protected double GetValueFromBitmapAndPoint(Bitmap bmp, Point target, string type)
        {
            Color pixel = bmp.GetPixel(target.X, target.Y);

            return ColorValueHandler.GetValueForColorAndType(pixel, type);
        }

        protected string GetBitmapName(string type, DateTime time)
        {
            return type + "-" + time.ToString("yyyy-MM-dd-HH") + ".bmp";
        }

        protected DateTime GetDateTimeFromBitmapName(string name)
        {
            string onlyDateName = name.Split('.')[0];

            string[] timeParts = onlyDateName.Split("-");

            DateTime dateTime = new DateTime(int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]), int.Parse(timeParts[4]), 0, 0);

            return dateTime;
        }

        protected bool IsReadyToDownloadData(DataLoaderConfig configFile)
        {
            DateTime lastUpdateDT = configFile.LastUpdateDateTime;

            TimeSpan timeSpan = DateTime.Now - lastUpdateDT;

            if (Math.Abs(timeSpan.TotalHours) >= configFile.MinimumHoursToNewDownload)
            {
                return true;
            }

            return false;
        }

        protected void CreateNewConfigFile(DataLoaderConfig configFile)
        {
            string fullFileName = GetPathToDataDirectory("loaderConfig.json");

            string jsonContent = JsonConvert.SerializeObject(configFile);

            File.WriteAllText(fullFileName, jsonContent);
        }

        protected DataLoaderConfig GetDataLoaderConfigFile()
        {
            string fullFileName = GetPathToDataDirectory("loaderConfig.json");

            using (StreamReader r = new StreamReader(fullFileName))
            {
                string jsonString = r.ReadToEnd();

                DataLoaderConfig configFile = JsonConvert.DeserializeObject<DataLoaderConfig>(jsonString);

                return configFile;
            }
        }

        protected abstract string GetPathToDataDirectory(string fileName);
    }

}
