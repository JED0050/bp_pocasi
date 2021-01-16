﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private GMapOverlay markers = new GMapOverlay("markers");
        private List<GMapMarker> listMarkers = new List<GMapMarker>();
        private GMapOverlay polygons;
        private List<PointLatLng> routeP = new List<PointLatLng>();
        private GMapOverlay routes;
        private DateTime selectedTime;
        private List<PointLatLng> bounds;
        private Bitmap dataBitmap = new Bitmap(728,528);
        private string loaders = ApkConfig.Loaders;
        private FormWindowState lastWindowState = FormWindowState.Normal;
        private Size appSize = new Size(800, 600);
        private List<double> graphValues = new List<double>();

        public Form1()
        {
            InitializeComponent();

            this.MinimumSize = new Size(800, 600);

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

        private void clearAllPoints(object sender, EventArgs e)
        {
            clearMarkers();
            gMap.Overlays.Remove(polygons);
            gMap.Overlays.Remove(routes);

            routeP.Clear();

            GraphClear();
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

        private void drawBitmapFromServer()
        {

            bounds = BitmapHandler.GetBounds((int)gMap.Zoom, gMap.Position);

            Bitmap bFor = BitmapHandler.GetBitmapFromServer("prec", selectedTime, loaders, bounds);

            dataBitmap = bFor;

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

            //gMap.Refresh();

            zoomReload();
        }

        private void mouseMoveInMap(object sender, MouseEventArgs e)
        {
            double lat = gMap.FromLocalToLatLng(e.X, e.Y).Lat;
            double lon = gMap.FromLocalToLatLng(e.X, e.Y).Lng;

            lonD = lon;
            latD = lat;
        }

        private void mouseDoubleClickInMap(object sender, MouseEventArgs e)
        {

            int x = BitmapHandler.GetX(lonD, dataBitmap.Width, bounds[0].Lng, bounds[1].Lng);
            int y = BitmapHandler.GetY(latD, dataBitmap.Height, bounds[0].Lat, bounds[1].Lat);

            Color c = Color.Transparent;

            try
            {
                c = dataBitmap.GetPixel(x, y);
            }
            catch
            { }

            clearMarkers();
            gMap.Overlays.Remove(routes);

            PointLatLng point = new PointLatLng(latD, lonD);

            string precVal = ""; // = BitmapHandler.GetFullPrecInPoint(selectedTime, point, "prec", SetLoaders(), bounds).ToString();

            List<double> precValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                double val = BitmapHandler.GetFullPrecInPoint(selectedTime.AddHours(i), point, "prec", loaders, bounds);

                if (i == 0)
                    precVal = val.ToString();

                precValues.Add(val);
            }

            graphValues = precValues;

            DrawGraph();

            listMarkers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin));
            listMarkers[0].ToolTipText = $"čas: {selectedTime.ToString("HH:mm - dd.MM.")}\nsrážky: {precVal} mm";

            gMap.Overlays.Add(markers);
            markers.Markers.Add(listMarkers[0]);

        }

        private void zoomReload()
        {
            gMap.Zoom = gMap.Zoom + 0.001;
            gMap.Zoom = gMap.Zoom - 0.001;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            selectedTime = DateTime.Now.AddMinutes(trackBar1.Value * 10);

            label1.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");
            
        }

        private void DrawGraph()
        {
            
            if (graphValues.Count == 0)
            {
                GraphClear();
                return;
            }

            pGraph.Refresh();

            //int panelW = pGraph.Width;
            //int panelH = pGraph.Height;

            Graphics g = pGraph.CreateGraphics();

            Pen p;
            p = new Pen(Color.Black, 2);

            int recGap = 5;
            int startSpaceX = 40;
            int endSpaxeX = 10;
            int startSpaceY = 10;
            int endSpaceY = 10;
            int botLineH = 100;
            int botLineW = pGraph.Width - 40;

            g.DrawLine(p, startSpaceX, 5, startSpaceX, (float)botLineH - 5);
            g.DrawLine(p, startSpaceX, (float)botLineH - 5, (float)botLineW - 5, (float)botLineH - 5);

            double max = 30;
            double min = 0;

            double recW = (double)(botLineW - startSpaceX - endSpaxeX - (graphValues.Count - 1) * recGap) / (double)(graphValues.Count);
            double recFullH = botLineH - startSpaceY - endSpaceY;

            int x = startSpaceX + 5;
            int y = startSpaceY;

            Font valueFont = new Font("Arial", 10);
            SolidBrush valueBrush = new SolidBrush(Color.Black);

            DateTime firstDate = selectedTime;

            for (int i = 0; i < graphValues.Count; i++)
            {
                SolidBrush brush = new SolidBrush(Color.Blue);

                double perc = (graphValues[i]) / (double)(max - min);

                //if (perc == 0)
                //    perc = 0.01;

                double recH = recFullH * perc;

                Rectangle r;

                r = new Rectangle(x, (int)(y + recFullH - recH), (int)recW, (int)recH);
                
                g.FillRectangle(brush, r);
                
                
                Point[] point = new Point[] { new Point(x + 7, botLineH) };

                string dateText = firstDate.ToString("dd.MM.yyyy HH:mm");

                g.Transform.TransformPoints(point);
                g.RotateTransform(20, MatrixOrder.Append);
                g.TranslateTransform(point[0].X, point[0].Y, MatrixOrder.Append);
                g.DrawString(dateText, valueFont, valueBrush, 0, 0);

                g.ResetTransform();

                x += (int)recW + recGap;

                firstDate = firstDate.AddHours(1);
            }

            g.DrawString("[°C]", valueFont, valueBrush, startSpaceX + 5, startSpaceY - 10);
            g.DrawString("[datetime]", valueFont, valueBrush, botLineW - 30, botLineH);

            g.DrawString(max.ToString(), valueFont, valueBrush, 1, startSpaceY - 10);
            g.DrawString(min.ToString(), valueFont, valueBrush, 1, botLineH - endSpaceY - 10);
        }

        private void GraphClear()
        {
            pGraph.Refresh();
            graphValues = new List<double>();
        }

        private void nahrátCestuZGPXSouboruToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string loaders = SetLoaders();

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

                    if (routePoints.Count < 2)
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

                        if (i == 0)
                        {
                            PointLatLng pointStart = routePoints[i];

                            string precVal = BitmapHandler.GetFullPrecInPoint(selectedTime, pointStart, "prec", loaders, bounds).ToString();

                            listMarkers.Add(new GMarkerGoogle(pointStart, GMarkerGoogleType.red_pushpin));
                            listMarkers[0].ToolTipText = $"Start\nčas: {selectedTime.ToString("HH:mm - dd. MM.")}\nsrážky: {precVal} mm";

                            gMap.Overlays.Add(markers);
                            markers.Markers.Add(listMarkers[0]);
                        }
                        else if (i == routePoints.Count - 1)
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

                if (fileContent == string.Empty)
                {
                    throw new Exception("Soubor nelze otevřít a zpracovat");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při zpracování gpx souboru", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void přemazatMapuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearMarkers();
            gMap.Overlays.Remove(polygons);
            gMap.Overlays.Remove(routes);

            routeP.Clear();

            GraphClear();
        }

        private void nastavitVýchozíAdresuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.ServerAddress = "https://localhost:44336/";
        }

        private void datovéZdrojeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormChooseDataLoaders loadersForm = new FormChooseDataLoaders();
            loadersForm.ShowDialog();
            loaders = FormChooseDataLoaders.loaders;
        }

        private void vlastníAdresaToolStripMenuItem_TextChanged(object sender, EventArgs e)
        {
            ApkConfig.ServerAddress = vlastníAdresaToolStripMenuItem.Text;
        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            drawBitmapFromServer();
        }

        private void bAnim_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < 10; i++)
            {
                drawBitmapFromServer();

                label1.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");
                label1.Update();

                selectedTime = selectedTime.AddHours(1);

                if(i != 0)
                    trackBar1.Value += 6;

            }

        }

        private void nastavitVlastníAdresuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vlastníAdresaToolStripMenuItem.Text = ApkConfig.ServerAddress;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

            if (this.Size != appSize)
            {
                appSize = this.Size;

                DrawGraph();
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {           
            
            if (WindowState != lastWindowState)
            {
                lastWindowState = WindowState;

                if (WindowState == FormWindowState.Maximized)   //kliknutí na fullscreen talčítko
                {
                    DrawGraph();
                }

                else if (WindowState == FormWindowState.Normal)   //vrácení z fullscreenu do menšího okna
                {
                    DrawGraph();
                }
            }
            
        }
    }
}
