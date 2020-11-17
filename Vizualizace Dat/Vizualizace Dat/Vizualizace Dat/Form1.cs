using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
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
        private GMapMarker marker;
        private GMapMarker marker2;
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
            gMap.Overlays.Remove(markers);
            markers.Markers.Remove(marker);
            markers.Markers.Remove(marker2);
            gMap.Overlays.Remove(polygons);
            gMap.Overlays.Remove(routes);

            routeP.Clear();
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
                if(radioButton1.Checked)
                {
                    gMap.Overlays.Remove(markers);
                    markers.Markers.Remove(marker);
                    markers.Markers.Remove(marker2);
                    gMap.Overlays.Remove(routes);

                    PointLatLng point = new PointLatLng(latD, lonD);

                    marker = new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin);

                    gMap.Overlays.Add(markers);
                    markers.Markers.Add(marker);
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

                        marker = new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin);

                        gMap.Overlays.Add(markers);
                        markers.Markers.Add(marker);

                        routeP.Clear();

                        zoomReload();
                    }
                    else if(routeP.Count == 1)
                    {
                        gMap.Overlays.Remove(markers);
                        markers.Markers.Remove(marker);
                        markers.Markers.Remove(marker2);

                        PointLatLng point = routeP[0];

                        marker2 = new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin);

                        gMap.Overlays.Add(markers);
                        markers.Markers.Add(marker2);
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

                    
                    double distanceKm = 0;

                    for(int i = 0; i < routePoints.Count - 1; i++)
                    {
                        distanceKm += BitmapHandler.GetDistance(routePoints[i], routePoints[i + 1]);
                    }

                    distanceKm /= 1000;
                    distanceKm = Math.Round(distanceKm, 2);

                    int timeMin = (int)(distanceKm / kmPerMin);

                    gMap.Overlays.Remove(routes);
                    gMap.Overlays.Remove(markers);
                    markers.Markers.Remove(marker);
                    markers.Markers.Remove(marker2);

                    PointLatLng point1 = routePoints[0];

                    Bitmap bFor = BitmapHandler.GetBitmapFromServer("prec", selectedTime, loaders, bounds);
                    int x = BitmapHandler.GetX(point1.Lng, bFor.Width, bounds[0].Lng, bounds[1].Lng);
                    int y = BitmapHandler.GetY(point1.Lat, bFor.Height, bounds[0].Lat, bounds[1].Lat);

                    Color c = Color.Transparent;

                    try
                    {
                        c = bFor.GetPixel(x, y);
                    }
                    catch
                    { }

                    string precVal = BitmapHandler.GetPrecipitationFromPixel(c).ToString();

                    marker = new GMarkerGoogle(point1, GMarkerGoogleType.red_pushpin);
                    marker.ToolTipText = $"Start\nčas: {selectedTime.ToString("HH:mm - dd. MM.")}\nsrážky: {precVal} mm";

                    gMap.Overlays.Add(markers);
                    markers.Markers.Add(marker);

                    var r = new GMapRoute(routePoints, "route");
                    routes = new GMapOverlay("routes");

                    routes.Routes.Add(r);

                    gMap.Overlays.Add(routes);

                    PointLatLng point2 = routePoints[routePoints.Count - 1];

                    bFor = BitmapHandler.GetBitmapFromServer("prec", selectedTime.AddMinutes(timeMin), loaders, bounds);
                    x = BitmapHandler.GetX(point2.Lng, bFor.Width, bounds[0].Lng, bounds[1].Lng);
                    y = BitmapHandler.GetY(point2.Lat, bFor.Height, bounds[0].Lat, bounds[1].Lat);

                    c = Color.Transparent;

                    try
                    {
                        c = bFor.GetPixel(x, y);
                    }
                    catch
                    { }

                    precVal = BitmapHandler.GetPrecipitationFromPixel(c).ToString();

                    marker2 = new GMarkerGoogle(point2, GMarkerGoogleType.red_pushpin);
                    marker2.ToolTipText = $"Cíl\nčas: {selectedTime.AddMinutes(timeMin).ToString("HH: mm - dd.MM.")}\nsrážky: {precVal} mm\n\nvzdálenost: {distanceKm} km\nzabere: {timeMin} min";

                    gMap.Overlays.Add(markers);
                    markers.Markers.Add(marker2);

                    routeP.Clear();

                    gMap.Position = routePoints[routePoints.Count / 2];

                    //zoomReload();


                }

                if(fileContent == string.Empty)
                {
                    throw new Exception("Soubor nelze otevřít a zpracovat");
                }

                

            }
            catch
            {
                MessageBox.Show("Vlož gpx soubor!", "Chyba při zpracování gpx souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
