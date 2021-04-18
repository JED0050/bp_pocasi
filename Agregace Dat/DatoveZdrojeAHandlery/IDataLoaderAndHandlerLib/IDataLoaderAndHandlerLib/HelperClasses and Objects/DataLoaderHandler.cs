using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace IDataLoaderAndHandlerLib.HandlersAndObjects
{
    abstract public class DataLoaderHandler
    {
        protected DataLoaderConfig dataLoaderConfig;

        //protected PointLonLat defaultTopLeftBound = new PointLonLat(4.1303633, 55.1995133);
        //protected PointLonLat defaultBotRightBound = new PointLonLat(37.9033283, 41.6999200);

        private static Bounds defaultBounds;
        protected static Bounds DefaultBounds
        {
            get
            {
                if(defaultBounds == null)
                {
                    defaultBounds = BoundsJsonHandler.LoadBoundsFromJsonFile();
                }

                return defaultBounds;
            }
        }


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
        
        protected double GetValueFromBitmapTypeAndBounds(Bitmap bmp, Bounds bounds, PointLonLat target, string type)
        {
            PointLonLat p1 = bounds.TopLeftCorner;
            PointLonLat p2 = bounds.BotRightCorner;

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

        protected Point GetPointFromBoundsAndTarget(Size bmpSize, Bounds bounds, PointLonLat target)
        {
            PointLonLat p1 = bounds.TopLeftCorner;
            PointLonLat p2 = bounds.BotRightCorner;

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
            //Debug.WriteLine(fullFileName + "\n" + DateTime.Now + "\n\n");
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

        protected Bitmap GetPartOfBitmap(Bitmap dataBitmap, Bounds loaderBounds)
        {

            if (dataBitmap.Width != 728 || dataBitmap.Height != 528)
            {
                Bitmap resizedB = new Bitmap(728, 528);
                using (Graphics g = Graphics.FromImage(resizedB))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(dataBitmap, 0, 0, 728, 528);
                };

                dataBitmap = resizedB;
            }

            if (loaderBounds.TopLeftCorner.Lon <= DefaultBounds.TopLeftCorner.Lon && loaderBounds.TopLeftCorner.Lat >= DefaultBounds.TopLeftCorner.Lat && loaderBounds.BotRightCorner.Lon >= DefaultBounds.TopLeftCorner.Lon && loaderBounds.BotRightCorner.Lat <= DefaultBounds.TopLeftCorner.Lat)
            {
                //hranice jsou menší než datová bitmapa => vyřežeme ze staré bitmapy část kterou následně roztáhneme

                Point p1 = new Point();
                Point p2 = new Point();

                double lonDif = loaderBounds.BotRightCorner.Lon - loaderBounds.TopLeftCorner.Lon;
                double latDif = loaderBounds.TopLeftCorner.Lat - loaderBounds.BotRightCorner.Lat;

                double PixelLon = lonDif / dataBitmap.Width;
                double PixelLat = latDif / dataBitmap.Height;

                double bY = loaderBounds.TopLeftCorner.Lat;
                double bX = loaderBounds.TopLeftCorner.Lon;

                int x;
                for (x = 0; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.TopLeftCorner.Lon && DefaultBounds.TopLeftCorner.Lon <= bX + DefaultBounds.TopLeftCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p1.X = x;

                int y;
                for (y = 0; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.TopLeftCorner.Lat && DefaultBounds.TopLeftCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p1.Y = y;

                for (; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.BotRightCorner.Lon && DefaultBounds.BotRightCorner.Lon <= bX + DefaultBounds.BotRightCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p2.X = x;

                for (; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.BotRightCorner.Lat && DefaultBounds.BotRightCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p2.Y = y;

                Bitmap smallBitmap = new Bitmap(p2.X - p1.X, p2.Y - p1.Y);

                for (x = 0; x < smallBitmap.Width; x++)
                {
                    for (y = 0; y < smallBitmap.Height; y++)
                    {
                        Color c = dataBitmap.GetPixel(x + p1.X, y + p1.Y);

                        smallBitmap.SetPixel(x, y, c);
                    }
                }

                Bitmap newBitmap = new Bitmap(728, 528);

                Graphics g = Graphics.FromImage(newBitmap);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                g.DrawImage(smallBitmap, 0, 0, 728, 528);

                smallBitmap.Dispose();

                return newBitmap;
            }
            else if (loaderBounds.TopLeftCorner.Lon >= DefaultBounds.TopLeftCorner.Lon && loaderBounds.TopLeftCorner.Lat <= DefaultBounds.TopLeftCorner.Lat && loaderBounds.BotRightCorner.Lon <= DefaultBounds.TopLeftCorner.Lon && loaderBounds.BotRightCorner.Lat >= DefaultBounds.TopLeftCorner.Lat)
            {
                //hranice jsou větší než bitmapa => změníme rozměry datové bitmapy a vložíme ji do nové bitmapy

                int xS = -1;
                int yS = -1;

                int counter = 0;

                Point p1 = new Point();
                Point p2 = new Point();

                double lonDif = DefaultBounds.BotRightCorner.Lon - DefaultBounds.TopLeftCorner.Lon;
                double latDif = DefaultBounds.TopLeftCorner.Lat - DefaultBounds.BotRightCorner.Lat;

                double PixelLon = lonDif / 728;
                double PixelLat = latDif / 528;

                double bY = DefaultBounds.TopLeftCorner.Lat;
                double bX = DefaultBounds.TopLeftCorner.Lon;

                while (bX < loaderBounds.BotRightCorner.Lon)
                {
                    if (bX >= loaderBounds.TopLeftCorner.Lon)
                    {
                        p2.X++;

                        if (xS == -1)
                        {
                            xS = counter;
                        }
                    }

                    if (counter <= 727)
                    {
                        p1.X = p2.X;
                    }

                    bX += PixelLon;
                    counter++;
                }

                counter = 0;

                while (bY > loaderBounds.BotRightCorner.Lat)
                {
                    if (bY <= loaderBounds.TopLeftCorner.Lat)
                    {
                        p2.Y++;

                        if (yS == -1)
                        {
                            yS = counter;
                        }
                    }

                    if (counter <= 527)
                    {
                        p1.Y = p2.Y;
                    }

                    bY -= PixelLat;
                    counter++;
                }

                Bitmap newBmp = new Bitmap(728, 528);

                Bitmap resizedBmp = new Bitmap(p1.X, p1.Y);
                using (Graphics g = Graphics.FromImage(resizedBmp))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(dataBitmap, 0, 0, p1.X, p1.Y);
                };

                for (int x = 0; x < p1.X; x++)
                {
                    for (int y = 0; y < p1.Y; y++)
                    {
                        newBmp.SetPixel(x + xS, y + yS, resizedBmp.GetPixel(x, y));
                    }
                }

                resizedBmp.Dispose();

                return newBmp;
            }
            else
            {
                //vyřezání části z data bitmapy

                Point p1 = new Point();
                Point p2 = new Point();

                double lonDif = loaderBounds.BotRightCorner.Lon - loaderBounds.TopLeftCorner.Lon;
                double latDif = loaderBounds.TopLeftCorner.Lat - loaderBounds.BotRightCorner.Lat;

                double PixelLon = lonDif / dataBitmap.Width;
                double PixelLat = latDif / dataBitmap.Height;

                double bY = loaderBounds.TopLeftCorner.Lat;
                double bX = loaderBounds.TopLeftCorner.Lon;

                int x;
                for (x = 0; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.TopLeftCorner.Lon && DefaultBounds.TopLeftCorner.Lon <= bX + DefaultBounds.TopLeftCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p1.X = x;

                int y;
                for (y = 0; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.TopLeftCorner.Lat && DefaultBounds.TopLeftCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p1.Y = y;

                for (; x < dataBitmap.Width; x++)
                {
                    if (bX >= DefaultBounds.BotRightCorner.Lon && DefaultBounds.BotRightCorner.Lon <= bX + DefaultBounds.BotRightCorner.Lon)
                        break;

                    bX += PixelLon;
                }

                p2.X = x;

                for (; y < dataBitmap.Height; y++)
                {
                    if (bY - PixelLat <= DefaultBounds.BotRightCorner.Lat && DefaultBounds.BotRightCorner.Lat <= bY)
                        break;

                    bY -= PixelLat;
                }

                p2.Y = y;

                int cutX = p2.X - p1.X;
                if (cutX == 0)
                {
                    cutX = 728;
                    p1.X = 0;
                }

                int cutY = p2.Y - p1.Y;
                if (cutY == 0)
                {
                    cutY = 528;
                    p1.Y = 0;
                }

                Bitmap smallBitmap = new Bitmap(cutX, cutY);

                for (x = 0; x < smallBitmap.Width; x++)
                {
                    for (y = 0; y < smallBitmap.Height; y++)
                    {
                        Color c = dataBitmap.GetPixel(x + p1.X, y + p1.Y);

                        smallBitmap.SetPixel(x, y, c);
                    }
                }

                //vložení této části do nové bitmapy

                int xS = -1;
                int yS = -1;

                int counter = 0;

                p1 = new Point();
                p2 = new Point();

                lonDif = DefaultBounds.BotRightCorner.Lon - DefaultBounds.TopLeftCorner.Lon;
                latDif = DefaultBounds.TopLeftCorner.Lat - DefaultBounds.BotRightCorner.Lat;

                PixelLon = lonDif / 728;
                PixelLat = latDif / 528;

                bY = DefaultBounds.TopLeftCorner.Lat;
                bX = DefaultBounds.TopLeftCorner.Lon;

                while (bX < loaderBounds.BotRightCorner.Lon)
                {
                    if (bX >= loaderBounds.TopLeftCorner.Lon)
                    {
                        p2.X++;

                        if (xS == -1)
                        {
                            xS = counter;
                        }
                    }

                    if (counter <= 727)
                    {
                        p1.X = p2.X;
                    }

                    bX += PixelLon;
                    counter++;
                }

                counter = 0;

                while (bY > loaderBounds.BotRightCorner.Lat)
                {
                    if (bY <= loaderBounds.TopLeftCorner.Lat)
                    {
                        p2.Y++;

                        if (yS == -1)
                        {
                            yS = counter;
                        }
                    }

                    if (counter <= 527)
                    {
                        p1.Y = p2.Y;
                    }

                    bY -= PixelLat;
                    counter++;
                }

                Bitmap newBmp = new Bitmap(728, 528);

                Bitmap resizedBmp = new Bitmap(p1.X, p1.Y);
                using (Graphics g = Graphics.FromImage(resizedBmp))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(smallBitmap, 0, 0, p1.X, p1.Y);
                };

                for (x = 0; x < p1.X; x++)
                {
                    for (y = 0; y < p1.Y; y++)
                    {
                        newBmp.SetPixel(x + xS, y + yS, resizedBmp.GetPixel(x, y));
                    }
                }

                smallBitmap.Dispose();
                resizedBmp.Dispose();

                return newBmp;
            }
        }
    }

}
