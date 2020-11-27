using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Vizualizace_Dat
{
    public partial class Form1 : Form
    {
        private double lonD = 0;
        private double latD = 0;
        private bool canDrawPoint = false;
        private int picIndex = 0;
        private GMapOverlay markers = new GMapOverlay("markers");
        private List<GMapMarker> listMarkers = new List<GMapMarker>();
        private GMapOverlay polygons;
        private List<PointLatLng> routeP = new List<PointLatLng>();
        private GMapOverlay routes;
        private DateTime selectedTime;
        private List<PointLatLng> bounds;

        public Form1()
        {
            InitializeComponent();

            pBForecast.BackColor = Color.Transparent;

            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.MinZoom = 2;
            gMap.MaxZoom = 15;
            gMap.Zoom = 6;
            gMap.Position = new PointLatLng(49.89, 15.16);
            gMap.ShowCenter = false;

            selectedTime = DateTime.Now;
            label1.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");

            bounds = BitmapHandler.GetBounds((int)gMap.Zoom, gMap.Position);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void canDrawPointChange(object sender, EventArgs e)
        {
            if (canDrawPoint)
            {
                button1.ForeColor = Color.Black;
            }
            else
            {
                button1.ForeColor = Color.Red;
            }

            canDrawPoint = !canDrawPoint;
        }

        private void clearAllPoints(object sender, EventArgs e)
        {
            clearMarkers();
            gMap.Overlays.Remove(polygons);
            gMap.Overlays.Remove(routes);

            routeP.Clear();

            pGraph.Refresh();
        }

        private void clearMarkers()
        {
            gMap.Overlays.Remove(markers);

            foreach (GMapMarker marker in listMarkers)
            {
                markers.Markers.Remove(marker);
            }

            listMarkers.Clear();
        }

        private void showDownloadedBitmap(object sender, EventArgs e)
        {
            gMap.Visible = !gMap.Visible;
        }

        private void clickInBitmap(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;

            int x = coordinates.X;
            int y = coordinates.Y;

            double lon = BitmapHandler.GetLon(x, pBForecast.Image.Width);
            double lat = BitmapHandler.GetLat(y, pBForecast.Image.Height);

            Color c = ((Bitmap)pBForecast.Image).GetPixel(x, y);

            lLonX.Text = $"[X: {x}] Lon: {lon}";
            lLatY.Text = $"[Y: {y}] Lat: {lat}";
            lPrec.Text = $"Srážky: {BitmapHandler.GetPrecipitationFromPixel(c)} [mm]\n" + c.ToString();
        }

        private void drawDownloadedBitmap(object sender, EventArgs e)
        {
            //změna bitmapy
            string filePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName + @"\Bitmaps";

            string[] files = Directory.GetFiles(filePath);
            picIndex++;
            if (picIndex > files.Length - 1)
            {
                picIndex = 0;
            }

            pBForecast.Image = Image.FromFile(files[picIndex]);

            //vykreslení bitmapy na mapu

            Bitmap bFor = (Bitmap)pBForecast.Image;

            double bX = 10.06;
            int bW = bFor.Width;
            int bH = bFor.Height;
            double pixelLon = (20.21 - 10.06) / bW;
            double pixelLat = (51.88 - 47.09) / bH;

            gMap.Overlays.Remove(polygons);
            polygons = new GMapOverlay("polygons");

            for (int x = 0; x < bW; x++)
            {
                double bY = 51.88;

                for (int y = 0; y < bH; y++)
                {

                    Color c = bFor.GetPixel(x, y);

                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        bY -= pixelLat;
                        continue;
                    }


                    List<PointLatLng> points = new List<PointLatLng>();
                    points.Add(new PointLatLng(bY, bX));
                    points.Add(new PointLatLng(bY - pixelLat, bX));
                    points.Add(new PointLatLng(bY - pixelLat, bX + pixelLon));
                    points.Add(new PointLatLng(bY, bX + pixelLon));

                    var polygon = new GMapPolygon(points, "pixel")
                    {
                        Stroke = new Pen(Color.Transparent, 0),
                        Fill = new SolidBrush(c)
                    };

                    polygons.Polygons.Add(polygon);


                    bY -= pixelLat;
                }

                bX += pixelLon;
            }

            gMap.Overlays.Add(polygons);

            zoomReload();

        }

        private void drawBitmapFromServer(object sender, EventArgs e)
        {
            string loaders = SetLoaders();

            bounds = BitmapHandler.GetBounds((int)gMap.Zoom, gMap.Position);

            Bitmap bFor = BitmapHandler.GetBitmapFromServer("prec", selectedTime, loaders, bounds);

            pBForecast.Image = bFor;

            int bW = bFor.Width;
            int bH = bFor.Height;
            double pixelLon = (bounds[1].Lng - bounds[0].Lng) / bW;
            double pixelLat = (bounds[0].Lat - bounds[1].Lat) / bH;

            gMap.Overlays.Remove(polygons);
            polygons = new GMapOverlay("polygons");

            double bX = bounds[0].Lng;
            for (int x = 0; x < bW; x++)
            {
                double bY = bounds[0].Lat;

                for (int y = 0; y < bH; y++)
                {

                    Color c = bFor.GetPixel(x, y);

                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        bY -= pixelLat;
                        continue;
                    }


                    List<PointLatLng> points = new List<PointLatLng>();
                    points.Add(new PointLatLng(bY, bX));
                    points.Add(new PointLatLng(bY - pixelLat, bX));
                    points.Add(new PointLatLng(bY - pixelLat, bX + pixelLon));
                    points.Add(new PointLatLng(bY, bX + pixelLon));

                    var polygon = new GMapPolygon(points, "pixel")
                    {
                        Stroke = new Pen(Color.Transparent, 0),
                        Fill = new SolidBrush(c)
                    };

                    polygons.Polygons.Add(polygon);


                    bY -= pixelLat;
                }

                bX += pixelLon;
            }

            gMap.Overlays.Add(polygons);

            zoomReload();
        }

        private void mouseMoveInMap(object sender, MouseEventArgs e)
        {
            double lat = gMap.FromLocalToLatLng(e.X, e.Y).Lat;
            double lon = gMap.FromLocalToLatLng(e.X, e.Y).Lng;

            lonD = lon;
            latD = lat;

            lLonX.Text = $"Lon: {Math.Round(lon, 6)}";
            lLatY.Text = $"Lat: {Math.Round(lat, 6)}";

            /*
            int i = 1;
            foreach(PointLatLng p in BitmapHandler.GetBounds((int)gMap.Zoom, gMap.Position))
            {
                Console.WriteLine(i + ": " + p.Lng + " " + p.Lat);
                i++;
            }
            */
        }

        private void mouseDoubleClickInMap(object sender, MouseEventArgs e)
        {

            int x = BitmapHandler.GetX(lonD, pBForecast.Image.Width, bounds[0].Lng, bounds[1].Lng);
            int y = BitmapHandler.GetY(latD, pBForecast.Image.Height, bounds[0].Lat, bounds[1].Lat);

            Color c = Color.Transparent;

            try
            {
                c = ((Bitmap)pBForecast.Image).GetPixel(x, y);
            }
            catch
            { }

            lCity.Text = BitmapHandler.GetAdressFromLonLat(lonD, latD);
            lPrec.Text = $"Srážky: {BitmapHandler.GetPrecipitationFromPixel(c)} [mm]";

            if (canDrawPoint)
            {
                if (radioButton1.Checked)
                {
                    clearMarkers();
                    gMap.Overlays.Remove(routes);

                    PointLatLng point = new PointLatLng(latD, lonD);

                    string precVal = ""; // = BitmapHandler.GetFullPrecInPoint(selectedTime, point, "prec", SetLoaders(), bounds).ToString();

                    List<double> precValues = new List<double>();

                    for (int i = 0; i < 10; i++)
                    {
                        double val = BitmapHandler.GetFullPrecInPoint(selectedTime.AddHours(i), point, "prec", SetLoaders(), bounds);

                        if (i == 0)
                            precVal = val.ToString();

                        precValues.Add(val);
                    }

                    DrawGraph(precValues);

                    listMarkers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin));
                    listMarkers[0].ToolTipText = $"čas: {selectedTime.ToString("HH:mm - dd.MM.")}\nsrážky: {precVal} mm";

                    gMap.Overlays.Add(markers);
                    markers.Markers.Add(listMarkers[0]);
                }
                else if(radioButton2.Checked)
                {
                    gMap.Overlays.Remove(routes);
                    routeP.Add(new PointLatLng(latD, lonD));

                    if (routeP.Count == 2)
                    {

                        List<PointLatLng> routePoints = BitmapHandler.GetRoutePoints(routeP[0], routeP[1]);

                        var r = new GMapRoute(routePoints, "route");
                        routes = new GMapOverlay("routes");

                        routes.Routes.Add(r);

                        gMap.Overlays.Add(routes);

                        PointLatLng point = routeP[1];

                        listMarkers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin));

                        gMap.Overlays.Add(markers);
                        markers.Markers.Add(listMarkers[1]);

                        routeP.Clear();

                        zoomReload();
                    }
                    else if(routeP.Count == 1)
                    {
                        clearMarkers();

                        PointLatLng point = routeP[0];

                        listMarkers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin));

                        gMap.Overlays.Add(markers);
                        markers.Markers.Add(listMarkers[0]);
                    }
                }
                
            }

        }

        private void zoomReload()
        {
            gMap.Zoom = gMap.Zoom + 1;
            gMap.Zoom = gMap.Zoom - 1;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            selectedTime = DateTime.Now.AddMinutes(trackBar1.Value * 10);

            label1.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");

        }

        private void getGPXPath(object sender, EventArgs e)
        {
            string loaders = SetLoaders();

            try
            {
                double kmPerMin;    //kolo = 0.25, auto = 1, chůze = 0,06

                FormGpxChooseTravelForm travelMenu = new FormGpxChooseTravelForm();
                var dialogResult = travelMenu.ShowDialog();

                if (dialogResult == DialogResult.OK)
                    kmPerMin = FormGpxChooseTravelForm.KmPerMinute;
                else
                    throw new Exception("Nebyla zvolena žádní varianta dopravy!");

                var fileContent = string.Empty;
                var filePath = string.Empty;

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "gpx files(*.gpx)| *.gpx";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;

                        var fileStream = openFileDialog.OpenFile();

                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                    }
                    else
                    {
                        throw new Exception("Soubor nelze otevřít a zpracovat");
                    }

                    List<PointLatLng> routePoints = BitmapHandler.GetRoutePoints(fileContent);

                    if(routePoints.Count < 2)
                    {
                        throw new Exception("Nebyl nalezen dostatečný počet bodů");
                    }

                    gMap.Overlays.Remove(routes);
                    clearMarkers();

                    int numberOfPoints = 5; //včetně začátku a cíle
                    int step = routePoints.Count / (numberOfPoints - 1);

                    double distanceKm = 0;

                    var r = new GMapRoute(routePoints, "route");
                    routes = new GMapOverlay("routes");

                    routes.Routes.Add(r);

                    gMap.Overlays.Add(routes);

                    for (int i = 0; i < routePoints.Count; i++)
                    {

                        if(i == 0)
                        {
                            PointLatLng pointStart = routePoints[i];

                            string precVal = BitmapHandler.GetFullPrecInPoint(selectedTime, pointStart, "prec", loaders, bounds).ToString();

                            listMarkers.Add(new GMarkerGoogle(pointStart, GMarkerGoogleType.red_pushpin));
                            listMarkers[0].ToolTipText = $"Start\nčas: {selectedTime.ToString("HH:mm - dd. MM.")}\nsrážky: {precVal} mm";

                            gMap.Overlays.Add(markers);
                            markers.Markers.Add(listMarkers[0]);
                        }
                        else if(i == routePoints.Count - 1)
                        {
                            PointLatLng pointEnd = routePoints[i];

                            int timeMin = (int)(distanceKm / kmPerMin);

                            string precVal = BitmapHandler.GetFullPrecInPoint(selectedTime.AddMinutes(timeMin), pointEnd, "prec", loaders, bounds).ToString();
                            double roundedDistanceKm = Math.Round(distanceKm, 2);

                            listMarkers.Add(new GMarkerGoogle(pointEnd, GMarkerGoogleType.red_pushpin));
                            listMarkers[listMarkers.Count - 1].ToolTipText = $"Cíl\nčas: {selectedTime.AddMinutes(timeMin).ToString("HH:mm - dd.MM.")}\nsrážky: {precVal} mm\n\nvzdálenost: {roundedDistanceKm} km\nzabere: {timeMin} min";

                            gMap.Overlays.Add(markers);
                            markers.Markers.Add(listMarkers[listMarkers.Count - 1]);

                            break;
                        }
                        else if (i % step == 0)
                        {
                            PointLatLng pointInner = routePoints[i];

                            int timeMin = (int)(distanceKm / kmPerMin);

                            string precVal = BitmapHandler.GetFullPrecInPoint(selectedTime.AddMinutes(timeMin), pointInner, "prec", loaders, bounds).ToString();
                            double roundedDistanceKm = Math.Round(distanceKm, 2);

                            listMarkers.Add(new GMarkerGoogle(pointInner, GMarkerGoogleType.red_pushpin));
                            listMarkers[listMarkers.Count - 1].ToolTipText = $"čas: {selectedTime.AddMinutes(timeMin).ToString("HH:mm - dd.MM.")}\nsrážky: {precVal} mm\n\nvzdálenost: {roundedDistanceKm} km\nzabere: {timeMin} min";

                            markers.Markers.Add(listMarkers[listMarkers.Count - 1]);
                        }

                        distanceKm += BitmapHandler.GetDistance(routePoints[i], routePoints[i + 1]) / 1000;
                    }

                    routeP.Clear();

                    gMap.Position = routePoints[routePoints.Count / 2];

                }

                if(fileContent == string.Empty)
                {
                    throw new Exception("Soubor nelze otevřít a zpracovat");
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při zpracování gpx souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private string SetLoaders()
        {
            string loaders = "";

            if (checkBox1.Checked)
                loaders += "b1";

            if (checkBox2.Checked)
                loaders += "x";

            if (checkBox3.Checked)
                loaders += "j";

            if (checkBox4.Checked)
                loaders += "b2";

            return loaders;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double[] ar = new[] { 8.2, 7.4, 3.1, 10, 0, 5.1, 9.2, 5.1, 2.3, 3.4 };
            List<double> ar2 = new List<double>(ar);

            DrawGraph(ar2);
        }

        private void DrawGraph(List<double> values)
        {
            pGraph.Refresh();

            int panelW = pGraph.Width;
            int panelH = pGraph.Height;

            Graphics g = pGraph.CreateGraphics();

            Pen p;
            p = new Pen(Color.Black, 2);

            g.DrawLine(p, 15, 5, 15, (float)panelH - 5);
            g.DrawLine(p, 15, (float)panelH - 5, (float)panelW - 5, (float)panelH - 5);

            double max = values[0];
            double min = 0; //values[0];

            foreach(double v in values)
            {
                if (max < v)
                    max = v;

                if (min > v)
                    min = v;
            }

            lGraphMax.Text = max.ToString();
            lGraphMin.Text = min.ToString();

            double recW = (panelW / (values.Count + 1));
            double recFullH = panelH - 10 - 10;

            int x = 20;
            int y = 10;

            for (int i = 0; i < values.Count; i++)
            {
                SolidBrush brush = new SolidBrush(Color.Blue);

                double perc = values[i] / (double)(max - min);

                double recH = recFullH * perc;

                Rectangle r;

                r = new Rectangle(x, (int)(y + recFullH - recH), (int)recW, (int)recH);
                g.FillRectangle(brush, r);

                x += (int)recW + 5;
            }
        }

        private void lGraphMax_Click(object sender, EventArgs e)
        {

        }
    }
}
