using GMap.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vizualizace_Dat
{
    public class CustomRoute
    {

        private List<CustomRoutePart> listRouteParts = new List<CustomRoutePart>();
        private List<CustomRoutePoint> listRoutePoints = new List<CustomRoutePoint>();
        private List<PointLatLng> listLanLngPoints = new List<PointLatLng>();

        private CustomRoutePoint lastPoint = new CustomRoutePoint();
        private CustomRoutePart lastPart = new CustomRoutePart();

        public void AddPoint(CustomRoutePoint point, bool last = false)
        {
            listRoutePoints.Add(point);
            listLanLngPoints.Add(point.Point);

            if (last)
            {
                lastPart.Add(point);
                listRouteParts.Add(lastPart);
            }
            else
            {
                if (lastPart.Points.Count == 0)
                {
                    lastPart.Add(point);
                }
                else if (point.Value != lastPoint.Value)
                {
                    lastPart.Add(point);
                    listRouteParts.Add(lastPart);

                    lastPart = new CustomRoutePart();
                    lastPart.Add(point);
                }
                else if (point.Value == lastPoint.Value)
                {
                    lastPart.Add(point);
                }
            }

            lastPoint = point;
        }

        public void PrintInfo()
        {
            int c = 0;

            foreach(var rPart in listRouteParts)
            {
                Debug.WriteLine($"Part: {c} Points: {rPart.Points.Count} Value: {rPart.Value}");
                c++;
            }

        }

        public List<CustomRoutePart> GetRouteParts()
        {
            return listRouteParts;
        }

        public List<CustomRoutePoint> GetAllRoutePoints()
        {
            return listRoutePoints;
        }

        public List<PointLatLng> GetAllLatLngPoints()
        {
            return listLanLngPoints;
        }
    }

    public class CustomRoutePoint
    {
        public PointLatLng Point { get; set; }
        public double Value { get; set; }
        public DateTime Time { get; set; }
    }

    public class CustomRoutePart
    {
        public List<PointLatLng> Points = new List<PointLatLng>();
        public double Value { get; set; }

        public void Add(CustomRoutePoint point)
        {
            if(Points.Count == 0)
            {
                Value = point.Value;
            }

            Points.Add(point.Point);
        }
    }
}
