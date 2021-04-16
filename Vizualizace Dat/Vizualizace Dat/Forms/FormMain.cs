using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
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
    public partial class FormMain : Form
    {
        private double lonD = 0;
        private double latD = 0;
        private GMapOverlay markers = new GMapOverlay("markers");
        private List<GMapMarker> listMarkers = new List<GMapMarker>();
        private List<GMapMarker> routePointsTooltipMarkers = new List<GMapMarker>();
        private GMapOverlay routes;
        private DateTime selectedTime;
        private List<PointLatLng> bounds;
        private Bitmap dataBitmap = new Bitmap(728, 528);
        private FormWindowState lastWindowState = FormWindowState.Normal;
        private Size appMinSize = new Size(800, 700);
        private List<GraphElement> graphCols = new List<GraphElement>();
        private ForecType forecTypeTemp = new ForecType(ForecastTypes.TEMPERATURE);
        private ForecType forecTypePrec = new ForecType(ForecastTypes.PRECIPITATION);
        private ForecType forecTypeHumi = new ForecType(ForecastTypes.HUMIDITY);
        private ForecType forecTypePres = new ForecType(ForecastTypes.PRESSURE);
        private ForecType forecType = new ForecType(ForecastTypes.PRECIPITATION);
        private GMapOverlay bitmapOverlay = new GMapOverlay("bitmapMarker");
        private GMarkerGoogle bitmapMarker;
        private bool isBitmapShown = false;
        private bool areDataSendByUser = false;
        private string validLoaders = ApkConfig.Loaders;
        private PointLatLng markPoint = new PointLatLng();
        private List<PointLatLng> routePoints = new List<PointLatLng>();
        private CustomRoute customRoute = new CustomRoute();
        private PointLatLng doubleclickedPoint = new PointLatLng();
        private int zoomLevel = 6;
        private Bitmap bigDataBitmap = new Bitmap(1,1);

        public FormMain()
        {
            InitializeComponent();

            this.MinimumSize = appMinSize;
            this.WindowState = FormWindowState.Maximized;

            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.MinZoom = 4;
            gMap.MaxZoom = 12;
            gMap.Zoom = zoomLevel;
            gMap.Position = new PointLatLng(49.89, 18.16);
            gMap.ShowCenter = false;

            bClearRoute.Visible = false;
            bRouteNewTime.Visible = false;

            ValidetaTrackbarTimeMarks();

            selectedTime = DateTime.Now;

            bounds = BitmapHandler.GetBounds(gMap);

            string[] defaultLoaders = ApkConfig.Loaders.Split(',');

            foreach (string loader in defaultLoaders)
            {
                if (loader == "rb")
                {
                    checkBox1.Checked = true;
                }

                if (loader == "mdrd")
                {
                    checkBox2.Checked = true;
                }

                if (loader == "yrno")
                {
                    checkBox3.Checked = true;
                }

                if (loader == "owm")
                {
                    checkBox4.Checked = true;
                }

                if (loader == "weun")
                {
                    checkBox5.Checked = true;
                }
            }

            nUDAnimNumOfAnims.Value = ApkConfig.AnimMaxMove;

            switch (ApkConfig.AnimStepMinutes)
            {
                case 10:
                    cBAnimStep.SelectedItem = cBAnimStep.Items[0];
                    break;
                case 30:
                    cBAnimStep.SelectedItem = cBAnimStep.Items[1];
                    break;
                case 60:
                    cBAnimStep.SelectedItem = cBAnimStep.Items[2];
                    break;
                case 3 * 60:
                    cBAnimStep.SelectedItem = cBAnimStep.Items[3];
                    break;
                case 6 * 60:
                    cBAnimStep.SelectedItem = cBAnimStep.Items[4];
                    break;
            }

            if (ApkConfig.ForecastType == ForecastTypes.PRECIPITATION)
            {
                rBPrec.Checked = true;
            }
            else if (ApkConfig.ForecastType == ForecastTypes.TEMPERATURE)
            {
                rBTemp.Checked = true;
            }
            else if (ApkConfig.ForecastType == ForecastTypes.PRESSURE)
            {
                rBPres.Checked = true;
            }
            else if (ApkConfig.ForecastType == ForecastTypes.HUMIDITY)
            {
                rBHumi.Checked = true;
            }

            areDataSendByUser = true;

            int hoursBack = 6;

            selectedTime = selectedTime.AddMinutes(-selectedTime.Minute % 10).AddHours(-hoursBack);

            trackBar1.Value = hoursBack * 6;
            selectedTime = selectedTime.AddHours(hoursBack);
            lDateTimeForecast.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void clearAllPoints(object sender, EventArgs e)
        {
            clearMarkers();
            isBitmapShown = false;
            bitmapOverlay.Markers.Remove(bitmapMarker);
            gMap.Overlays.Remove(bitmapOverlay);
            routePoints.Clear();
            GraphClear();
        }

        private void clearMarkers()
        {
            gMap.Overlays.Remove(markers);

            foreach (GMapMarker marker in listMarkers)
            {
                markers.Markers.Remove(marker);
            }

            foreach (var routeMarker in routePointsTooltipMarkers)
            {
                markers.Markers.Remove(routeMarker);
            }

            listMarkers.Clear();

            customRoute = new CustomRoute();

            bClearRoute.Visible = false;
            bRouteNewTime.Visible = false;

            gMap.Overlays.Remove(routes);
        }

        private int drawBitmapFromServer(Bitmap serverBitmap = null)
        {
            if (customRoute.GetAllRoutePoints().Count > 0)
            {
                return 1;
            }
            else if (serverBitmap == null)
            {
                bounds = BitmapHandler.GetBounds(gMap);

                try
                {
                    serverBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, selectedTime, validLoaders, bounds);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Chyba, ze serveru se nepodařilo stáhnout potřebná data! Zkuste změnit datové zdroje, čas či typ předpovědi.\n\nOdpověď serveru: {ex.Message}", "Chyba při získávání dat", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return 1;
                }
            }

            bitmapOverlay.Markers.Remove(bitmapMarker);
            gMap.Overlays.Remove(bitmapOverlay);

            int xDif = (int)(gMap.FromLatLngToLocal(bounds[1]).X - gMap.FromLatLngToLocal(bounds[0]).X);
            int yDif = (int)(gMap.FromLatLngToLocal(bounds[1]).Y - gMap.FromLatLngToLocal(bounds[0]).Y);

            Bitmap transparentBitmap = new Bitmap(serverBitmap.Width, serverBitmap.Height);

            for (int xP = 0; xP < serverBitmap.Width; xP++)
            {
                for (int yP = 0; yP < serverBitmap.Height; yP++)
                {
                    Color oldP = serverBitmap.GetPixel(xP, yP);

                    if ((oldP.R == 0 && oldP.G == 0 && oldP.B == 0) || (oldP.R == 255 && oldP.G == 255 && oldP.B == 255))
                        continue;

                    transparentBitmap.SetPixel(xP, yP, Color.FromArgb(ApkConfig.BitmapAlpha, oldP.R, oldP.G, oldP.B));
                }
            }

            dataBitmap = transparentBitmap;

            Bitmap resizedBitmap = new Bitmap(transparentBitmap, new Size(xDif, yDif));

            bitmapMarker = new GMarkerGoogle(new PointLatLng(bounds[1].Lat, (bounds[0].Lng + bounds[1].Lng) / 2), resizedBitmap) { IsHitTestVisible = false };
            bitmapOverlay.Markers.Add(bitmapMarker);

            gMap.Overlays.Add(bitmapOverlay);

            zoomReload();

            isBitmapShown = true;

            return 0;
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

            markPoint.Lat = latD;
            markPoint.Lng = lonD;

            Color c = Color.Transparent;

            try
            {
                c = dataBitmap.GetPixel(x, y);
            }
            catch
            { }

            //int routePoints = customRoute.GetFullRoute().Count;

            clearMarkers();
            gMap.Overlays.Remove(routes);

            if(!isBitmapShown)
            {
                drawBitmapFromServer();
            }

            doubleclickedPoint = new PointLatLng(latD, lonD);

            string precVal = "";
            string exMsg = "";

            graphCols = new List<GraphElement>();

            for (int i = 0; i < ApkConfig.DblclickMaxData; i++)
            {
                double val;

                try
                {
                    val = BitmapHandler.GetFullPrecInPoint(selectedTime.AddHours(i), doubleclickedPoint, validLoaders, bounds, forecType);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Chyba, ze serveru se nepodařilo stáhnout potřebná data! Zkuste změnit datové zdroje, čas či typ předpovědi.\n\nOdpověď serveru: {ex.Message}", "Chyba při získávání dat", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    exMsg = ex.Message;

                    break;
                }

                if (i == 0)
                    precVal = val.ToString();

                graphCols.Add(new GraphElement(val, selectedTime.AddHours(i)));
            }

            if (graphCols.Count > 0)
            {
                DrawGraph();

                listMarkers.Add(new GMarkerGoogle(doubleclickedPoint, GMarkerGoogleType.red_dot));
                listMarkers[0].ToolTipText = $"čas: {selectedTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {precVal} {forecType.Unit}";

                gMap.Overlays.Add(markers);
                markers.Markers.Add(listMarkers[0]);

                ShowWeatherWindow();
            }
            else
            {
                MessageBox.Show($"Chyba, ze serveru se nepodařilo stáhnout potřebná data! Zkuste změnit datové zdroje, čas či typ předpovědi.\n\nOdpověď serveru: {exMsg}", "Chyba při získávání dat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DrawNewGraphForSamePoint(bool dontRedrawRoute = false)
        {
            if (listMarkers.Count == 1)
            {
                graphCols = new List<GraphElement>();

                string text = null;

                for (int i = 0; i < ApkConfig.DblclickMaxData; i++)
                {
                    double val;

                    try
                    {
                        val = BitmapHandler.GetFullPrecInPoint(selectedTime.AddHours(i), markPoint, validLoaders, bounds, forecType);
                    }
                    catch
                    {
                        break;
                    }

                    if (text == null)
                        text = $"čas: {selectedTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {val} {forecType.Unit}";

                    graphCols.Add(new GraphElement(val, selectedTime.AddHours(i)));
                }
                if (graphCols.Count > 0)
                {
                    listMarkers[0].ToolTipText = text;

                    DrawGraph();
                    RedrawMark();
                }
                else
                {
                    GraphClear();
                    clearMarkers();
                }
            }
            else if (listMarkers.Count >= 2)
            {
                if(dontRedrawRoute)
                {
                    return;
                }

                try
                {
                    string oldType = listMarkers[0].ToolTipText.Split('\n')[2];

                    Debug.WriteLine($"oT: {oldType} aT: {forecType.CzForecType}");

                    if (oldType.StartsWith(forecType.CzForecType))
                    {
                        RedrawMark();
                        return;
                    }
                }
                catch
                { }

                foreach(var col in graphCols)
                {
                    try
                    {
                        double val = BitmapHandler.GetFullPrecInPoint(col.Time, col.Point, validLoaders, bounds, forecType);
                        col.Value = val;
                    }
                    catch
                    {
                        graphCols.Clear();

                        break;
                    }
                }

                if (graphCols.Count > 0)
                {
                    DrawGraph();
                    RedrawMark();
                }
                else
                {
                    GraphClear();
                    clearMarkers();
                }
            }
            else
            {
                GraphClear();
                clearMarkers();
            }
        }

        private void RedrawMark()
        {
            if (isBitmapShown && listMarkers.Count == 1)
            {
                GMapMarker oldMark = listMarkers[0];
                string oldMarkText = listMarkers[0].ToolTipText;

                clearMarkers();

                listMarkers.Add(oldMark);
                listMarkers[0].ToolTipText = oldMarkText;

                gMap.Overlays.Add(markers);
                markers.Markers.Add(listMarkers[0]);
            }
            else if (customRoute.GetAllRoutePoints().Count > 0)
            {
                List<GMapMarker> newMarkers = new List<GMapMarker>(listMarkers);

                CustomRoute newCustomRoute = customRoute;

                clearMarkers();
                gMap.Overlays.Remove(routes);
                listMarkers.Clear();

                foreach (var mark in newMarkers)
                {
                    listMarkers.Add(mark);
                    markers.Markers.Add(mark);
                }

                customRoute = newCustomRoute;

                DrawNewRoute();

                //gMap.Overlays.Add(routes);
                gMap.Overlays.Add(markers);
            }
        }

        public void DrawNewRoute()
        {
            bClearRoute.Visible = true;
            bRouteNewTime.Visible = true;

            CustomRoute newRoute = new CustomRoute();

            DateTime lastTime = DateTime.Now;
            Bitmap lastBitmap = new Bitmap(1, 1);

            int c = 0;

            foreach (var point in customRoute.GetAllRoutePoints())
            {
                if(c == 0)
                {
                    lastTime = point.Time;
                    lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, lastTime, validLoaders, bounds);
                }
                else if ((point.Time - lastTime).TotalMinutes >= 30 || (point.Time.Minute > 30 && lastTime.Minute <= 30))
                {
                    lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, point.Time, validLoaders, bounds);
                    lastTime = point.Time;
                }

                double value = 0;

                try
                {
                    value = BitmapHandler.GetFullPrecInPoint(lastTime, point.Point, validLoaders, bounds, forecType, lastBitmap);
                }
                catch{ }

                CustomRoutePoint newPoint = point;
                //newPoint.Time = point.Time;
                //newPoint.Point = point.Point;
                newPoint.Value = value;

                if (c == customRoute.GetAllRoutePoints().Count - 1)
                    newRoute.AddPoint(newPoint, true);
                else
                    newRoute.AddPoint(newPoint);

                if(c == 0)
                {
                    listMarkers.Add(new GMarkerGoogle(point.Point, GMarkerGoogleType.red_small));
                    listMarkers[0].ToolTipText = $"Start\nčas: {point.Time.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}";
                    markers.Markers.Add(listMarkers[0]);
                }
                else if (c == customRoute.GetAllRoutePoints().Count - 1)
                {
                    listMarkers.Add(new GMarkerGoogle(point.Point, GMarkerGoogleType.red_small));
                    listMarkers[listMarkers.Count - 1].ToolTipText = $"Cíl\nčas: {point.Time.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}\n\nvzdálenost: {point.DistanceKm} km\nzabere: {point.GetTakesTimeString()}";
                    markers.Markers.Add(listMarkers[listMarkers.Count - 1]);
                }
                else if (c % 20 == 0)
                {
                    routePointsTooltipMarkers.Add(new GMarkerGoogle(point.Point, new Bitmap(10, 10)));
                    routePointsTooltipMarkers[routePointsTooltipMarkers.Count - 1].ToolTipText = $"čas: {point.Time.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}\n\nvzdálenost: {point.DistanceKm} km\nzabere: {point.GetTakesTimeString()}";
                    markers.Markers.Add(routePointsTooltipMarkers[routePointsTooltipMarkers.Count - 1]);
                }

                c++;
            }

            routes = new GMapOverlay("routes");

            customRoute = newRoute;

            foreach (var rPart in customRoute.GetRouteParts())
            {
                Color col = forecType.GetColorFromValue(rPart.Value);

                var route = new GMapRoute(rPart.Points, "route");
                route.Stroke = new Pen(Color.FromArgb(255, col));
                route.Stroke.Width = 4;
                routes.Routes.Add(route);
            }

            gMap.Overlays.Add(routes);
            zoomReload();

            //customRoute.PrintInfo();
        }

        private void zoomReload()
        {
            gMap.Zoom = gMap.Zoom + 0.01;
            gMap.Zoom = gMap.Zoom - 0.01;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            selectedTime = DateTime.Now;
            selectedTime = selectedTime.AddMinutes(-selectedTime.Minute % 10).AddHours(-6);
            selectedTime = selectedTime.AddMinutes(trackBar1.Value * 10);

            lDateTimeForecast.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");

            ValidateDataLoaders();
        }

        private void DrawGraph()
        {

            if (graphCols.Count == 0)
            {
                GraphClear();
                return;
            }

            pGraph.Refresh();

            //int panelW = pGraph.Width;
            //int panelH = pGraph.Height;

            Graphics graphGraphics = pGraph.CreateGraphics();

            Pen p;
            p = new Pen(Color.Black, 2);

            int recGap = 5;
            int startSpaceX = 40;
            int endSpaxeX = 10;
            int startSpaceY = 10;
            int endSpaceY = 10;
            int botLineH = 100;
            int botLineW = pGraph.Width - 40;

            graphGraphics.DrawLine(p, startSpaceX, 5, startSpaceX, (float)botLineH - 5);
            graphGraphics.DrawLine(p, startSpaceX, (float)botLineH - 5, (float)botLineW - 5, (float)botLineH - 5);

            double max = forecType.GraphMaxValue;
            double min = forecType.GraphMinValue;

            double recW = (double)(botLineW - startSpaceX - endSpaxeX - (graphCols.Count - 1) * recGap) / (double)(graphCols.Count);
            double recFullH = botLineH - startSpaceY - endSpaceY;

            int x = startSpaceX + 5;
            int y = startSpaceY;

            Color columnColor = Color.FromArgb(ApkConfig.GraphColumnColor);

            Font valueFont = new Font("Arial", 10);
            SolidBrush valueBrush = new SolidBrush(Color.Black);
            Pen zeroValuePen = new Pen(Color.Red, 1);

            if (columnColor.ToArgb() == Color.Red.ToArgb())
            {
                zeroValuePen = new Pen(Color.Blue, 1);
            }

            SolidBrush zeroValueBrush = new SolidBrush(Color.Red);

            DateTime firstDate = selectedTime;

            double incVal = 0;

            if (min != 0)
            {
                incVal = -min;
            }

            for (int i = 0; i < graphCols.Count; i++)
            {
                SolidBrush brush = new SolidBrush(columnColor);

                double perc = (incVal + graphCols[i].Value) / (double)(max - min);

                if (perc > 0 && perc < 0.009)
                    perc = 0.01;

                double recH = Math.Round(recFullH * perc);

                Rectangle r;

                r = new Rectangle(x, (int)(y + recFullH - recH), (int)recW, (int)recH);

                graphGraphics.FillRectangle(brush, r);

                graphGraphics.DrawString(graphCols[i].Value.ToString(), valueFont, valueBrush, x + (int)(recW / 2) - 5, startSpaceY - 10);

                Point[] point = new Point[] { new Point(x + 7, botLineH) };

                string dateText = graphCols[i].TimeInfo;

                graphGraphics.Transform.TransformPoints(point);
                graphGraphics.RotateTransform(20, MatrixOrder.Append);
                graphGraphics.TranslateTransform(point[0].X, point[0].Y, MatrixOrder.Append);
                graphGraphics.DrawString(dateText, valueFont, valueBrush, 0, 0);

                graphGraphics.ResetTransform();

                x += (int)recW + recGap;

                firstDate = firstDate.AddHours(1);
            }

            if(min < 0)
            {
                double perc = (incVal + 0) / (double)(max - min);

                if (perc == 0)
                    perc = 0.01;

                double recH = Math.Round(recFullH * perc);

                int zeroY = (int)(y + recFullH - recH);

                graphGraphics.DrawLine(zeroValuePen, startSpaceX + 2, zeroY, (float)botLineW - 25, zeroY);

                graphGraphics.DrawString("0", valueFont, zeroValueBrush, 1, zeroY - 5);
            }

            graphGraphics.DrawString(forecType.Unit, valueFont, valueBrush, startSpaceX + 5, startSpaceY - 10);
            graphGraphics.DrawString("[datetime]", valueFont, valueBrush, botLineW - 30, botLineH);

            graphGraphics.DrawString(max.ToString(), valueFont, valueBrush, 1, startSpaceY - 10);
            graphGraphics.DrawString(min.ToString(), valueFont, valueBrush, 1, botLineH - endSpaceY - 10);
        }

        private void GraphClear()
        {
            graphCols = new List<GraphElement>();

            pGraph.Invalidate();
        }

        private void nahrátCestuZGPXSouboruToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string loaders = SetLoaders();

            try
            {
                DialogResult dialogResult = MessageBox.Show("Přejete si zadat počáteční a cílový čas cesty? (Ne = zadáte pouze počáteční čas)",
                      "Volba zadání časů", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                bool hasBeginAndEndTimes = false;
                DateTime beginTime;
                DateTime endTime = DateTime.Now;
                double kmPerMin = 0;
                double hourDif = 0;

                if (dialogResult == DialogResult.Yes)
                {
                    FormGpxBeginTime gpxBeginTimeForm = new FormGpxBeginTime("Zvol počáteční čas cesty", "vybrat čas konce");
                    dialogResult = gpxBeginTimeForm.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        beginTime = FormGpxBeginTime.BeginTime;
                    else
                        throw new Exception("Nebyl zvolen platný čas začátku cesty!");

                    FormGpxBeginTime gpxEndTimeForm = new FormGpxBeginTime("Zvol koncový čas cesty", "vybrat GPX soubor");
                    dialogResult = gpxEndTimeForm.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        endTime = FormGpxBeginTime.BeginTime;
                    else
                        throw new Exception("Nebyl zvolen platný čas začátku cesty!");

                    TimeSpan dTSpan = endTime - beginTime;
                    hourDif = dTSpan.TotalHours;

                    if(hourDif < 0)
                    {
                        throw new Exception("Čas počátku cesty musí předcházet času konce cesty.");
                    }

                    hasBeginAndEndTimes = true;
                }
                else if(dialogResult == DialogResult.No)
                {
                    FormGpxBeginTime gpxBeginTimeForm = new FormGpxBeginTime("Zvol počáteční čas cesty", null);
                    dialogResult = gpxBeginTimeForm.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        beginTime = FormGpxBeginTime.BeginTime;
                    else
                        throw new Exception("Nebyl zvolen platný čas začátku cesty!");

                    FormGpxChooseTravelForm travelMenu = new FormGpxChooseTravelForm();
                    dialogResult = travelMenu.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        kmPerMin = FormGpxChooseTravelForm.KmPerMinute;
                    else
                        throw new Exception("Nebyla zvolena žádní varianta dopravy!");
                }
                else
                {
                    return;
                }

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

                    gMap.Overlays.Remove(routes);
                    clearAllPoints(null, null);

                    bClearRoute.Visible = true;
                    bRouteNewTime.Visible = true;

                    routePoints = BitmapHandler.GetRoutePoints(fileContent);

                    if (routePoints.Count < 2)
                    {
                        throw new Exception("Nebyl nalezen dostatečný počet bodů");
                    }

                    int numOfGraphCols = 10; //včetně začátku a cíle
                    int step = routePoints.Count / (numOfGraphCols - 1);

                    double distanceKm = 0;

                    if (hasBeginAndEndTimes)
                    {
                        double fullDistanceKm = 0;
                        
                        for(int i = 0; i < routePoints.Count - 1; i++)
                        {
                            fullDistanceKm += BitmapHandler.GetDistance(routePoints[i], routePoints[i + 1]);
                        }

                        fullDistanceKm /= 1000;

                        kmPerMin = fullDistanceKm / (hourDif * 60);
                    }

                    DateTime lastTime = beginTime;
                    Bitmap lastBitmap = new Bitmap(1, 1);

                    for (int i = 0; i < routePoints.Count; i++)
                    {
                        double value = 0;
                        double timeMin = 0;

                        if (i == 0)
                        {
                            lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, beginTime, validLoaders, bounds);

                            PointLatLng pointStart = routePoints[i];

                            value = BitmapHandler.GetFullPrecInPoint(beginTime, pointStart, validLoaders, bounds, forecType, lastBitmap);

                            graphCols.Add(new GraphElement(value, beginTime, routePoints[i], 0));

                            listMarkers.Add(new GMarkerGoogle(pointStart, GMarkerGoogleType.red_small));
                            listMarkers[0].ToolTipText = $"Start\nčas: {beginTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}";

                            markers.Markers.Add(listMarkers[0]);
                        }
                        else if (i == routePoints.Count - 1)
                        {
                            PointLatLng pointEnd = routePoints[i];
                            timeMin = hourDif * 60;

                            if (!hasBeginAndEndTimes)
                            {
                                timeMin = distanceKm / kmPerMin;
                                endTime = beginTime.AddMinutes(timeMin);
                            }

                            if((endTime - lastTime).TotalMinutes >= 30 || (endTime.Minute > 30 && lastTime.Minute <= 30))
                            {
                                lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, endTime, validLoaders, bounds);
                            }

                            value = BitmapHandler.GetFullPrecInPoint(endTime, pointEnd, validLoaders, bounds, forecType, lastBitmap);
                            double roundedDistanceKm = Math.Round(distanceKm, 2);

                            CustomRoutePoint customPointLast = new CustomRoutePoint();
                            customPointLast.Value = value;
                            customPointLast.Point = routePoints[i];
                            customPointLast.Time = endTime;
                            customPointLast.DistanceKm = roundedDistanceKm;
                            customPointLast.TakesTime = (endTime - beginTime).TotalMinutes;
                            customRoute.AddPoint(customPointLast, true);

                            graphCols.Add(new GraphElement(value, endTime, routePoints[i], roundedDistanceKm));

                            listMarkers.Add(new GMarkerGoogle(pointEnd, GMarkerGoogleType.red_small));
                            listMarkers[listMarkers.Count - 1].ToolTipText = $"Cíl\nčas: {endTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}\n\nvzdálenost: {roundedDistanceKm} km\nzabere: {customPointLast.GetTakesTimeString()}";

                            //gMap.Overlays.Add(markers);
                            markers.Markers.Add(listMarkers[listMarkers.Count - 1]);

                            break;
                        }
                        else //body mezi začátkem a koncem
                        {
                            PointLatLng pointInner = routePoints[i];

                            timeMin = distanceKm / kmPerMin;

                            DateTime timeNow = beginTime.AddMinutes(timeMin);

                            if ((timeNow - lastTime).TotalMinutes >= 30 || (timeNow.Minute > 30 && lastTime.Minute <= 30))
                            {
                                lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, timeNow, validLoaders, bounds);
                            }

                            lastTime = timeNow;

                            value = BitmapHandler.GetFullPrecInPoint(timeNow, pointInner, validLoaders, bounds, forecType, lastBitmap);

                            if (i % step == 0) //sloupce pro graf
                            {
                                double roundedDistanceKm = Math.Round(distanceKm, 2);

                                graphCols.Add(new GraphElement(value, timeNow, pointInner, roundedDistanceKm));
                            }
                        }

                        CustomRoutePoint customPoint = new CustomRoutePoint();
                        customPoint.Value = value;
                        customPoint.Point = routePoints[i];
                        customPoint.Time = beginTime.AddMinutes(timeMin);
                        customPoint.DistanceKm = Math.Round(distanceKm, 2);
                        customPoint.TakesTime = distanceKm / kmPerMin;
                        customRoute.AddPoint(customPoint);

                        distanceKm += BitmapHandler.GetDistance(routePoints[i], routePoints[i + 1]) / 1000;

                        if(i % 20 == 0 && i != 0 && i != routePoints.Count - 1) //přidávání tooltipů na trase
                        {
                            string takesTime = "";

                            if (customPoint.TakesTime <= 180)
                            {
                                takesTime = Math.Round(customPoint.TakesTime) + "min";
                            }
                            else
                            {
                                takesTime = Math.Floor(customPoint.TakesTime / 60) + "h " + Math.Round(customPoint.TakesTime % 60) + "min";
                            }

                            routePointsTooltipMarkers.Add(new GMarkerGoogle(routePoints[i], new Bitmap(10,10)));
                            routePointsTooltipMarkers[routePointsTooltipMarkers.Count - 1].ToolTipText = $"čas: {customPoint.Time.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}\n\nvzdálenost: {customPoint.DistanceKm} km\nzabere: {customPoint.GetTakesTimeString()}";
                            markers.Markers.Add(routePointsTooltipMarkers[routePointsTooltipMarkers.Count - 1]);
                        }
                    }
                    //var r = new GMapRoute(routePoints, "route");
                    routes = new GMapOverlay("routes");

                    foreach (var rPart in customRoute.GetRouteParts())
                    {
                        Color col = forecType.GetColorFromValue(rPart.Value);

                        var route = new GMapRoute(rPart.Points, "route");
                        route.Stroke = new Pen(Color.FromArgb(255, col));
                        route.Stroke.Width = 4;
                        routes.Routes.Add(route);
                    }

                    //routes.Routes.Add(r);
                    gMap.Overlays.Add(routes);
                    gMap.Overlays.Add(markers);
                    //customRoute.PrintInfo();

                    gMap.Position = routePoints[routePoints.Count / 2];

                    DrawGraph();

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
            isBitmapShown = false;
            bitmapOverlay.Markers.Remove(bitmapMarker);
            gMap.Overlays.Remove(bitmapOverlay);
            gMap.Overlays.Remove(routes);

            routePoints.Clear();

            GraphClear();
        }

        private void nastavitVýchozíAdresuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.ServerAddress = "https://localhost:44336/";
        }

        private void vlastníAdresaToolStripMenuItem_TextChanged(object sender, EventArgs e)
        {
            ApkConfig.ServerAddress = vlastníAdresaToolStripMenuItem.Text;
        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            drawBitmapFromServer();

            ValidetaTrackbarTimeMarks();

            DrawNewGraphForSamePoint(true);
        }

        private void ValidetaTrackbarTimeMarks()
        {
            double stepVal = trackBar1.Maximum / (tLPTimeMarks.ColumnCount - 2);
            DateTime dateTimeNow = DateTime.Now;
            dateTimeNow = dateTimeNow.AddMinutes(- dateTimeNow.Minute % 10).AddHours(-6);

            for (int i = 1; i < tLPTimeMarks.ColumnCount -1; i++)
                tLPTimeMarks.Controls[i].Text = dateTimeNow.AddMinutes(Math.Ceiling(stepVal / 2 + (i - 1) * stepVal) * 10).ToString("dd.MM. HH:mm");

        }

        private void bAnim_Click(object sender, EventArgs e)
        {
            clearMarkers();
            GraphClear();

            for (int i = 0; i <= ApkConfig.AnimMaxMove; i++)
            {
                int res = drawBitmapFromServer();

                lDateTimeForecast.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");
                lDateTimeForecast.Update();

                lAnimProgress.Text = $"{i}/{ApkConfig.AnimMaxMove}";
                lAnimProgress.Update();

                if (res == 1)
                {
                    selectedTime = selectedTime.AddMinutes(- ApkConfig.AnimStepMinutes);
                    trackBar1.Value -= (ApkConfig.AnimStepMinutes / 10);

                    break;
                }

                if (i != 0)
                    if (trackBar1.Value + (ApkConfig.AnimStepMinutes / 10) <= trackBar1.Maximum)
                        trackBar1.Value += (ApkConfig.AnimStepMinutes / 10);
                    else
                        break;

                selectedTime = selectedTime.AddMinutes(ApkConfig.AnimStepMinutes);
            }

            Thread.Sleep(250);

            lAnimProgress.Text = "";
            lAnimProgress.Update();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

            if (this.Size != appMinSize)
            {
                appMinSize = this.Size;

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

        private void gMap_OnMapZoomChanged()
        {

            if (isBitmapShown)
            {
                try
                {
                    //Debug.WriteLine(gMap.Zoom + " " + zoomLevel);

                    if ((gMap.Zoom > 8 && zoomLevel <= 8) || (gMap.Zoom <= 8 && zoomLevel > 8) || zoomLevel == -1)
                    {
                        bounds = BitmapHandler.GetBounds(gMap);

                        Bitmap serverBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, selectedTime, validLoaders, bounds);

                        Bitmap transparentBitmap = new Bitmap(serverBitmap.Width, serverBitmap.Height);

                        for (int xP = 0; xP < serverBitmap.Width; xP++)
                        {
                            for (int yP = 0; yP < serverBitmap.Height; yP++)
                            {
                                Color oldP = serverBitmap.GetPixel(xP, yP);

                                if ((oldP.R == 0 && oldP.G == 0 && oldP.B == 0) || (oldP.R == 255 && oldP.G == 255 && oldP.B == 255))
                                    continue;

                                transparentBitmap.SetPixel(xP, yP, Color.FromArgb(ApkConfig.BitmapAlpha, oldP.R, oldP.G, oldP.B));
                            }
                        }

                        dataBitmap = transparentBitmap;

                        if((zoomLevel == -1 && (int)gMap.Zoom == 9) || zoomLevel != -1)
                        {
                            zoomLevel = (int)gMap.Zoom;
                        }
                        
                    }

                    bitmapOverlay.Markers.Remove(bitmapMarker);
                    gMap.Overlays.Remove(bitmapOverlay);

                    int xDif = (int)(gMap.FromLatLngToLocal(bounds[1]).X - gMap.FromLatLngToLocal(bounds[0]).X);
                    int yDif = (int)(gMap.FromLatLngToLocal(bounds[1]).Y - gMap.FromLatLngToLocal(bounds[0]).Y);

                    Bitmap resizedBitmap = new Bitmap(dataBitmap, new Size(xDif, yDif));

                    bitmapMarker = new GMarkerGoogle(new PointLatLng(bounds[1].Lat, (bounds[0].Lng + bounds[1].Lng) / 2), resizedBitmap) { IsHitTestVisible = false };
                    bitmapOverlay.Markers.Add(bitmapMarker);

                    gMap.Overlays.Add(bitmapOverlay);

                    RedrawMark();
                }
                catch
                {
                    //isBitmapShown = false;
                }

                //zoomReload();
            }
        }

        private void minimálníToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.BitmapAlpha = 255;
            drawBitmapFromServer(dataBitmap);
        }

        private void maximálníToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.BitmapAlpha = 0;
            drawBitmapFromServer(dataBitmap);
        }

        private void vlastníToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            menuOwnTransparent.Text = ApkConfig.BitmapAlpha.ToString();
        }

        private void nastavitVlastníAdresuToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            vlastníAdresaToolStripMenuItem.Text = ApkConfig.ServerAddress;
        }

        private void checkBoxLoaders_CheckedChanged(object sender, EventArgs e)
        {
            if (areDataSendByUser)
            {
                string jsonLoaders = "";

                if (checkBox1.Checked)
                {
                    jsonLoaders += ",rb";
                }

                if (checkBox2.Checked)
                {
                    jsonLoaders += ",mdrd";
                }

                if (checkBox3.Checked)
                {
                    jsonLoaders += ",yrno";
                }

                if (checkBox4.Checked)
                {
                    jsonLoaders += ",owm";
                }

                if (checkBox5.Checked)
                {
                    jsonLoaders += ",weun";
                }

                if (jsonLoaders.Length > 0 && jsonLoaders[0] == ',')
                {
                    jsonLoaders = jsonLoaders.Remove(0, 1);
                }

                ApkConfig.Loaders = jsonLoaders;

                ValidateDataLoaders();
                drawBitmapFromServer();

                DrawNewGraphForSamePoint();
            }
        }

        private void menuOwnTransparent_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                ApkConfig.BitmapAlpha = int.Parse(menuOwnTransparent.Text);

                if (ApkConfig.BitmapAlpha < 0)
                    ApkConfig.BitmapAlpha = 0;
                else if (ApkConfig.BitmapAlpha > 255)
                    ApkConfig.BitmapAlpha = 255;

                menuOwnTransparent.Text = ApkConfig.BitmapAlpha.ToString();

                drawBitmapFromServer(dataBitmap);
            }
            catch
            {
                menuOwnTransparent.Text = ApkConfig.BitmapAlpha.ToString();

                //MessageBox.Show("Hodnota průhlednosti musí být celé číslo v rozmezí od 0 do 255!", "Chyba při změně průhlednosti srážek", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void rBTemp_CheckedChanged(object sender, EventArgs e)
        {
            
            if(rBPrec.Checked)
            {
                forecType = forecTypePrec;
            }
            else if(rBTemp.Checked)
            {
                forecType = forecTypeTemp;
            }
            else if (rBPres.Checked)
            {
                forecType = forecTypePres;
            }
            else if (rBHumi.Checked)
            {
                forecType = forecTypeHumi;
            }

            RadioButton rB = (RadioButton)sender;

            if(rB.Checked)
            {
                ValidateDataLoaders();

                drawBitmapFromServer();

                pBScale.Image = forecType.ScaleBitmap;
                lScaleMin.Text = forecType.GraphMinValue.ToString();
                lScaleMax.Text = forecType.GraphMaxValue.ToString();

                if(customRoute.GetAllRoutePoints().Count == 0)
                    GraphClear();

                if (areDataSendByUser)
                {
                    DrawNewGraphForSamePoint();
                    ApkConfig.ForecastType = forecType.Type;
                }
            }
        }

        private void ValidateDataLoaders()
        {
            //rb
            if (selectedTime > DateTime.Now.AddHours(6))
            {
                checkBox1.Enabled = false;
            }
            else if(rBTemp.Checked || rBPres.Checked || rBHumi.Checked)
            {
                checkBox1.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
            }

            //mdrd
            if (selectedTime > DateTime.Now.AddDays(3))
            {
                checkBox2.Enabled = false;
            }
            else if(rBPres.Checked || rBHumi.Checked)
            {
                checkBox2.Enabled = false;
            }
            else
            {
                checkBox2.Enabled = true;
            }


            validLoaders = "";

            if (checkBox1.Checked && checkBox1.Enabled)
            {
                validLoaders += ",rb";
            }

            if (checkBox2.Checked && checkBox2.Enabled)
            {
                validLoaders += ",mdrd";
            }

            if (checkBox3.Checked && checkBox3.Enabled)
            {
                validLoaders += ",yrno";
            }

            if (checkBox4.Checked && checkBox4.Enabled)
            {
                validLoaders += ",owm";
            }

            if (checkBox5.Checked && checkBox5.Enabled)
            {
                validLoaders += ",weun";
            }

            if (validLoaders.Length > 0 && validLoaders[0] == ',')
            {
                validLoaders = validLoaders.Remove(0, 1);
            }
        }

        private void pGraph_Paint(object sender, PaintEventArgs e)
        {
            if(graphCols.Count == 0)
            {
                //base.OnPaint(e);
                using (Graphics g = e.Graphics)
                {
                    Font textFont = new Font("Arial", 15);
                    SolidBrush textBrush = new SolidBrush(Color.Black);

                    string textContent = $"Dvojitým kliknutím do mapy zde vykreslíte graf počasí pro následujících {ApkConfig.DblclickMaxData} hodin.\nNásledujícím kliknutím na značku dojde k jejímu zrušení.";

                    Size textSize = TextRenderer.MeasureText(g, textContent, textFont);

                    int xPos = (pGraph.Width / 2) - (textSize.Width / 2);

                    //Debug.WriteLine($"x {xPos} w {pGraph.Width} t {size.Width}");

                    g.DrawString(textContent, textFont, textBrush, xPos, 55);

                    pGraph.Update();
                }
            }
        }

        private void dblclickCustomData_MouseLeave(object sender, EventArgs e)
        {
            int val = ApkConfig.DblclickMaxData;

            try
            {
                val = int.Parse(dblclickCustomDataText.Text);

                ApkConfig.DblclickMaxData = val;

                val = ApkConfig.DblclickMaxData;

                if(graphCols.Count == 0)
                    GraphClear();
            }
            catch
            { }

            dblclickCustomDataText.Text = val.ToString();
        }

        private void dblclickMinData_Click(object sender, EventArgs e)
        {
            ApkConfig.DblclickMaxData = 1;

            if (graphCols.Count == 0)
                GraphClear();
        }

        private void dblclickMaxData_Click(object sender, EventArgs e)
        {
            ApkConfig.DblclickMaxData = 40;

            if (graphCols.Count == 0)
                GraphClear();
        }

        private void dblclickCustomData_MouseEnter(object sender, EventArgs e)
        {
            dblclickCustomDataText.Text = ApkConfig.DblclickMaxData.ToString();
        }

        private void setDefSettings_Click(object sender, EventArgs e)
        {
            ApkConfig.ServerAddress = "https://localhost:44336/";
            ApkConfig.BitmapAlpha = 150;
            ApkConfig.AnimMaxMove = 10;
            ApkConfig.DblclickMaxData = 10;

            drawBitmapFromServer(dataBitmap);
            GraphClear();
            clearMarkers();
        }

        private void gMap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if(listMarkers.Count == 1)
            {
                if (item == listMarkers[0])
                {
                    clearMarkers();
                    GraphClear();
                }
            }
        }

        private void aktualizovatŠkályToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string envirDir = Environment.CurrentDirectory;
            string resourcesDir = Path.GetFullPath(Path.Combine(envirDir, @"..\..\")) + @"Resources\";

            foreach (string type in ForecastTypes.GetListOfTypes())
            {
                string url = ApkConfig.ServerAddress + @"\scale?type=" + type;

                Bitmap scale;

                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        using (Stream s = wc.OpenRead(url))
                        {
                            scale = new Bitmap(s);
                        }

                        string scaleName = resourcesDir + @"\" + $"scale_{type}.png";

                        scale.Save(scaleName, ImageFormat.Png);
                        scale.Dispose();

                        if(type == ForecastTypes.TEMPERATURE)
                        {
                            forecTypeTemp = new ForecType(type);
                        }
                        else if (type == ForecastTypes.PRECIPITATION)
                        {
                            forecTypePrec = new ForecType(type);
                        }
                        else if (type == ForecastTypes.HUMIDITY)
                        {
                            forecTypeHumi = new ForecType(type);
                        }
                        else if (type == ForecastTypes.PRESSURE)
                        {
                            forecTypePres = new ForecType(type);
                        }

                    }
                    catch
                    { }
                }
            }

            forecType = new ForecType(forecType.Type);
            pBScale.Image = forecType.ScaleBitmap;
        }

        private void cBAnimStep_SelectedIndexChanged(object sender, EventArgs e)
        {

            string[] textParts = cBAnimStep.Text.Split(' ');

            if (textParts[1] == "min")
            {
                ApkConfig.AnimStepMinutes = int.Parse(textParts[0]);
            }
            else
            {
                ApkConfig.AnimStepMinutes = int.Parse(textParts[0]) * 60;
            }

        }

        private void nUDAnimNumOfAnims_ValueChanged(object sender, EventArgs e)
        {
            ApkConfig.AnimMaxMove = (int)nUDAnimNumOfAnims.Value;
        }

        private void modráToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.GraphColumnColor = Color.Blue.ToArgb();

            DrawGraph();
        }

        private void červenáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.GraphColumnColor = Color.Red.ToArgb();

            DrawGraph();
        }

        private void zelenáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.GraphColumnColor = Color.Green.ToArgb();

            DrawGraph();
        }

        private void bClearRoute_Click(object sender, EventArgs e)
        {
            clearMarkers();

            gMap.Overlays.Remove(routes);
            routePoints.Clear();

            GraphClear();

            drawBitmapFromServer();
        }

        private void bRouteNewTime_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Přejete si zadat počáteční a cílový čas cesty? (Ne = zadáte pouze počáteční čas)",
                      "Volba zadání časů", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                bool hasBeginAndEndTimes = false;
                DateTime beginTime;
                DateTime endTime = DateTime.Now;
                double kmPerMin = 0;
                double hourDif = 0;

                if (dialogResult == DialogResult.Yes)
                {
                    FormGpxBeginTime gpxBeginTimeForm = new FormGpxBeginTime("Zvol počáteční čas cesty", "vybrat čas konce");
                    dialogResult = gpxBeginTimeForm.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        beginTime = FormGpxBeginTime.BeginTime;
                    else
                        throw new Exception("Nebyl zvolen platný čas začátku cesty!");

                    FormGpxBeginTime gpxEndTimeForm = new FormGpxBeginTime("Zvol koncový čas cesty", "ok");
                    dialogResult = gpxEndTimeForm.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        endTime = FormGpxBeginTime.BeginTime;
                    else
                        throw new Exception("Nebyl zvolen platný čas začátku cesty!");

                    TimeSpan dTSpan = endTime - beginTime;
                    hourDif = dTSpan.TotalHours;

                    if (hourDif < 0)
                    {
                        throw new Exception("Čas počátku cesty musí předcházet času konce cesty.");
                    }

                    hasBeginAndEndTimes = true;
                }
                else if (dialogResult == DialogResult.No)
                {
                    FormGpxBeginTime gpxBeginTimeForm = new FormGpxBeginTime("Zvol počáteční čas cesty", null);
                    dialogResult = gpxBeginTimeForm.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        beginTime = FormGpxBeginTime.BeginTime;
                    else
                        throw new Exception("Nebyl zvolen platný čas začátku cesty!");

                    FormGpxChooseTravelForm travelMenu = new FormGpxChooseTravelForm();
                    dialogResult = travelMenu.ShowDialog();

                    if (dialogResult == DialogResult.OK)
                        kmPerMin = FormGpxChooseTravelForm.KmPerMinute;
                    else
                        throw new Exception("Nebyla zvolena žádní varianta dopravy!");
                }
                else
                {
                    return;
                }


                routePoints = customRoute.GetAllLatLngPoints();

                gMap.Overlays.Remove(markers);

                foreach (GMapMarker marker in listMarkers)
                {
                    markers.Markers.Remove(marker);
                }

                foreach (var routeMarker in routePointsTooltipMarkers)
                {
                    markers.Markers.Remove(routeMarker);
                }

                listMarkers.Clear();

                customRoute = new CustomRoute();

                gMap.Overlays.Remove(routes);
                GraphClear();

                if (routePoints.Count < 2)
                {
                    throw new Exception("Nebyl nalezen dostatečný počet bodů");
                }

                int numOfGraphCols = 10; //včetně začátku a cíle
                int step = routePoints.Count / (numOfGraphCols - 1);

                double distanceKm = 0;

                if (hasBeginAndEndTimes)
                {
                    double fullDistanceKm = 0;

                    for (int i = 0; i < routePoints.Count - 1; i++)
                    {
                        fullDistanceKm += BitmapHandler.GetDistance(routePoints[i], routePoints[i + 1]);
                    }

                    fullDistanceKm /= 1000;

                    kmPerMin = fullDistanceKm / (hourDif * 60);
                }

                DateTime lastTime = beginTime;
                Bitmap lastBitmap = new Bitmap(1, 1);

                for (int i = 0; i < routePoints.Count; i++)
                {
                    double value = 0;
                    double timeMin = 0;

                    if (i == 0)
                    {
                        lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, beginTime, validLoaders, bounds);

                        PointLatLng pointStart = routePoints[i];

                        value = BitmapHandler.GetFullPrecInPoint(beginTime, pointStart, validLoaders, bounds, forecType, lastBitmap);

                        graphCols.Add(new GraphElement(value, beginTime, routePoints[i], 0));

                        listMarkers.Add(new GMarkerGoogle(pointStart, GMarkerGoogleType.red_small));
                        listMarkers[0].ToolTipText = $"Start\nčas: {beginTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}";

                        markers.Markers.Add(listMarkers[0]);
                    }
                    else if (i == routePoints.Count - 1)
                    {
                        PointLatLng pointEnd = routePoints[i];
                        timeMin = hourDif * 60;

                        if (!hasBeginAndEndTimes)
                        {
                            timeMin = distanceKm / kmPerMin;
                            endTime = beginTime.AddMinutes(timeMin);
                        }

                        if ((endTime - lastTime).TotalMinutes >= 30 || (endTime.Minute > 30 && lastTime.Minute <= 30))
                        {
                            lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, endTime, validLoaders, bounds);
                        }

                        value = BitmapHandler.GetFullPrecInPoint(endTime, pointEnd, validLoaders, bounds, forecType, lastBitmap);
                        double roundedDistanceKm = Math.Round(distanceKm, 2);

                        CustomRoutePoint customPointLast = new CustomRoutePoint();
                        customPointLast.Value = value;
                        customPointLast.Point = routePoints[i];
                        customPointLast.Time = endTime;
                        customPointLast.DistanceKm = roundedDistanceKm;
                        customPointLast.TakesTime = (endTime - beginTime).TotalMinutes;
                        customRoute.AddPoint(customPointLast, true);

                        graphCols.Add(new GraphElement(value, endTime, routePoints[i], roundedDistanceKm));

                        listMarkers.Add(new GMarkerGoogle(pointEnd, GMarkerGoogleType.red_small));
                        listMarkers[listMarkers.Count - 1].ToolTipText = $"Cíl\nčas: {endTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}\n\nvzdálenost: {roundedDistanceKm} km\nzabere: {customPointLast.GetTakesTimeString()}";

                        //gMap.Overlays.Add(markers);
                        markers.Markers.Add(listMarkers[listMarkers.Count - 1]);

                        break;
                    }
                    else //body mezi začátkem a koncem
                    {
                        PointLatLng pointInner = routePoints[i];

                        timeMin = distanceKm / kmPerMin;

                        DateTime timeNow = beginTime.AddMinutes(timeMin);

                        if ((timeNow - lastTime).TotalMinutes >= 30 || (timeNow.Minute > 30 && lastTime.Minute <= 30))
                        {
                            lastBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, timeNow, validLoaders, bounds);
                        }

                        lastTime = timeNow;

                        value = BitmapHandler.GetFullPrecInPoint(timeNow, pointInner, validLoaders, bounds, forecType, lastBitmap);

                        if (i % step == 0) //sloupce pro graf
                        {
                            double roundedDistanceKm = Math.Round(distanceKm, 2);

                            graphCols.Add(new GraphElement(value, timeNow, pointInner, roundedDistanceKm));
                        }
                    }

                    CustomRoutePoint customPoint = new CustomRoutePoint();
                    customPoint.Value = value;
                    customPoint.Point = routePoints[i];
                    customPoint.Time = beginTime.AddMinutes(timeMin);
                    customPoint.DistanceKm = Math.Round(distanceKm, 2);
                    customPoint.TakesTime = distanceKm / kmPerMin;
                    customRoute.AddPoint(customPoint);

                    distanceKm += BitmapHandler.GetDistance(routePoints[i], routePoints[i + 1]) / 1000;

                    if (i % 20 == 0 && i != 0 && i != routePoints.Count - 1) //přidávání tooltipů na trase
                    {
                        string takesTime = "";

                        if (customPoint.TakesTime <= 180)
                        {
                            takesTime = Math.Round(customPoint.TakesTime) + "min";
                        }
                        else
                        {
                            takesTime = Math.Floor(customPoint.TakesTime / 60) + "h " + Math.Round(customPoint.TakesTime % 60) + "min";
                        }

                        routePointsTooltipMarkers.Add(new GMarkerGoogle(routePoints[i], new Bitmap(10, 10)));
                        routePointsTooltipMarkers[routePointsTooltipMarkers.Count - 1].ToolTipText = $"čas: {customPoint.Time.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {value} {forecType.Unit}\n\nvzdálenost: {customPoint.DistanceKm} km\nzabere: {customPoint.GetTakesTimeString()}";
                        markers.Markers.Add(routePointsTooltipMarkers[routePointsTooltipMarkers.Count - 1]);
                    }
                }

                //var r = new GMapRoute(routePoints, "route");
                routes = new GMapOverlay("routes");

                foreach (var rPart in customRoute.GetRouteParts())
                {
                    Color col = forecType.GetColorFromValue(rPart.Value);

                    var route = new GMapRoute(rPart.Points, "route");
                    route.Stroke = new Pen(Color.FromArgb(255, col));
                    route.Stroke.Width = 4;
                    routes.Routes.Add(route);
                }

                //routes.Routes.Add(r);
                gMap.Overlays.Add(routes);
                gMap.Overlays.Add(markers);
                //customRoute.PrintInfo();

                gMap.Position = routePoints[routePoints.Count / 2];

                DrawGraph();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba při zadávání nového času trasy", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if(customRoute.GetAllRoutePoints().Count == 0)
                {
                    clearMarkers();
                    drawBitmapFromServer();
                }
            }
        }

        private void bCityNameSearch_Click(object sender, EventArgs e)
        {
            if(tBPointName.Text.Length > 0 && tBPointName.ForeColor == Color.Black)
            {
                //Debug.WriteLine(tBPointName.Text);
                gMap.Focus();

                doubleclickedPoint = BitmapHandler.GetPointCoordFromCityName(tBPointName.Text);

                int x = BitmapHandler.GetX(doubleclickedPoint.Lng, dataBitmap.Width, bounds[0].Lng, bounds[1].Lng);
                int y = BitmapHandler.GetY(doubleclickedPoint.Lat, dataBitmap.Height, bounds[0].Lat, bounds[1].Lat);

                markPoint = doubleclickedPoint;

                Color c = Color.Transparent;

                try
                {
                    c = dataBitmap.GetPixel(x, y);
                }
                catch
                { }

                clearMarkers();
                gMap.Overlays.Remove(routes);

                if (!isBitmapShown)
                {
                    drawBitmapFromServer();
                }

                string precVal = "";
                string exMsg = "";

                graphCols = new List<GraphElement>();

                //tBPointName.Foc

                for (int i = 0; i < ApkConfig.DblclickMaxData; i++)
                {
                    double val;

                    try
                    {
                        val = BitmapHandler.GetFullPrecInPoint(selectedTime.AddHours(i), doubleclickedPoint, validLoaders, bounds, forecType);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show($"Chyba, ze serveru se nepodařilo stáhnout potřebná data! Zkuste změnit datové zdroje, čas či typ předpovědi.\n\nOdpověď serveru: {ex.Message}", "Chyba při získávání dat", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        exMsg = ex.Message;

                        break;
                    }

                    if (i == 0)
                        precVal = val.ToString();

                    graphCols.Add(new GraphElement(val, selectedTime.AddHours(i)));
                }

                if (graphCols.Count > 0)
                {
                    DrawGraph();
                    ShowWeatherWindow();

                    listMarkers.Add(new GMarkerGoogle(doubleclickedPoint, GMarkerGoogleType.red_dot));
                    listMarkers[0].ToolTipText = $"čas: {selectedTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {precVal} {forecType.Unit}";

                    gMap.Overlays.Add(markers);
                    markers.Markers.Add(listMarkers[0]);
                }
                else
                {
                    MessageBox.Show($"Chyba, ze serveru se nepodařilo stáhnout potřebná data! Zkuste změnit datové zdroje, čas či typ předpovědi.\n\nOdpověď serveru: {exMsg}", "Chyba při získávání dat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tBPointName_Enter(object sender, EventArgs e)
        {
            if (tBPointName.ForeColor == Color.Gray)
            {
                tBPointName.ForeColor = Color.Black;
                tBPointName.Text = "";
            }
        }

        private void tBPointName_Leave(object sender, EventArgs e)
        {
            if (tBPointName.Text.Length == 0)
            {
                tBPointName.ForeColor = Color.Gray;
                tBPointName.Text = "Praha, Česko";
            }
        }

        private void tBPointName_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyNumber = (int)e.KeyChar;

            if (keyNumber == 13) //13 = enter
            {
                bCityNameSearch_Click(sender, null);
            }
        }

        private void ShowWeatherWindow()
        {
            FormPrecipDetail formPrecipDetail = new FormPrecipDetail(selectedTime, doubleclickedPoint, validLoaders, bounds);
            formPrecipDetail.Show();
        }

        private void gMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (isBitmapShown && gMap.Zoom > 8)
            {
                List<PointLatLng> actBounds = BitmapHandler.GetBounds(gMap);

                if (!(bounds[0].Lng == actBounds[0].Lng && bounds[0].Lat == actBounds[0].Lat && bounds[1].Lng == actBounds[1].Lng && bounds[1].Lat == actBounds[1].Lat))
                {
                    try
                    {
                        Bitmap serverBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, selectedTime, validLoaders, actBounds);

                        Bitmap transparentBitmap = new Bitmap(serverBitmap.Width, serverBitmap.Height);

                        for (int xP = 0; xP < serverBitmap.Width; xP++)
                        {
                            for (int yP = 0; yP < serverBitmap.Height; yP++)
                            {
                                Color oldP = serverBitmap.GetPixel(xP, yP);

                                if ((oldP.R == 0 && oldP.G == 0 && oldP.B == 0) || (oldP.R == 255 && oldP.G == 255 && oldP.B == 255))
                                    continue;

                                transparentBitmap.SetPixel(xP, yP, Color.FromArgb(ApkConfig.BitmapAlpha, oldP.R, oldP.G, oldP.B));
                            }
                        }

                        dataBitmap = transparentBitmap;
                    }
                    catch
                    {
                        return;
                    }

                    zoomLevel = -1;

                    bounds = actBounds;

                    bitmapOverlay.Markers.Remove(bitmapMarker);
                    gMap.Overlays.Remove(bitmapOverlay);

                    int xDif = (int)(gMap.FromLatLngToLocal(bounds[1]).X - gMap.FromLatLngToLocal(bounds[0]).X);
                    int yDif = (int)(gMap.FromLatLngToLocal(bounds[1]).Y - gMap.FromLatLngToLocal(bounds[0]).Y);

                    Bitmap resizedBitmap = new Bitmap(dataBitmap, new Size(xDif, yDif));

                    bitmapMarker = new GMarkerGoogle(new PointLatLng(bounds[1].Lat, (bounds[0].Lng + bounds[1].Lng) / 2), resizedBitmap) { IsHitTestVisible = false };
                    bitmapOverlay.Markers.Add(bitmapMarker);

                    gMap.Overlays.Add(bitmapOverlay);

                    RedrawMark();
                }

            }
                
        }
    }
}
