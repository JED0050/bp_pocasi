using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
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
        private List<PointLatLng> routeP = new List<PointLatLng>();
        private GMapOverlay routes;
        private DateTime selectedTime;
        private List<PointLatLng> bounds;
        private Bitmap dataBitmap = new Bitmap(728, 528);
        private string loaders = ApkConfig.Loaders;
        private FormWindowState lastWindowState = FormWindowState.Normal;
        private Size appSize = new Size(800, 600);
        private List<GraphElement> graphCols = new List<GraphElement>();
        private ForecType forecTypeTemp = new ForecType("temp");
        private ForecType forecTypePrec = new ForecType("prec");
        private ForecType forecType = new ForecType("prec");
        private GMapOverlay bitmapOverlay = new GMapOverlay("bitmapMarker");
        private GMarkerGoogle bitmapMarker;
        private bool isBitmapShown = false;
        private int bitmapAlpha = ApkConfig.BitmapAlpha;

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

            string defaultLoaders = loaders;

            if(defaultLoaders.Contains("b1"))
            {
                checkBox1.Checked = true;
            }

            if (defaultLoaders.Contains("b2"))
            {
                checkBox2.Checked = true;
            }

            if (defaultLoaders.Contains("x"))
            {
                checkBox3.Checked = true;
            }

            if (defaultLoaders.Contains("j"))
            {
                checkBox4.Checked = true;
            }
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

        private int drawBitmapFromServer()
        {

            bounds = BitmapHandler.GetBounds((int)gMap.Zoom, gMap.Position);

            Bitmap serverBitmap;

            try
            {
                serverBitmap = BitmapHandler.GetBitmapFromServer(forecType.Type, selectedTime, loaders, bounds);
            }
            catch
            {
                MessageBox.Show("Chyba, ze serveru se nepodařilo stáhnout potřebná data! Zkuste změnit datové zdroje, čas či typ předpovědi.", "Chyba při získávání dat", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 1;
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

                    transparentBitmap.SetPixel(xP, yP, Color.FromArgb(bitmapAlpha, oldP.R, oldP.G, oldP.B));
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

            string precVal = "";

            graphCols = new List<GraphElement>();

            for (int i = 0; i < 10; i++)
            {
                double val;

                try
                {
                    val = BitmapHandler.GetFullPrecInPoint(selectedTime.AddHours(i), point, loaders, bounds, forecType);
                }
                catch
                {
                    MessageBox.Show("Chyba, ze serveru se nepodařilo stáhnout potřebná data! Zkuste změnit datové zdroje, čas či typ předpovědi.", "Chyba při získávání dat", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (i == 0)
                    precVal = val.ToString();

                graphCols.Add(new GraphElement(val, selectedTime.AddHours(i)));
            }

            DrawGraph();

            listMarkers.Add(new GMarkerGoogle(point, GMarkerGoogleType.red_dot));
            listMarkers[0].ToolTipText = $"čas: {selectedTime.ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {precVal} {forecType.Unit}";

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
            selectedTime = DateTime.Now.AddMinutes(trackBar1.Value);

            label1.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");

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

            double max = forecType.GraphMaxValue;
            double min = forecType.GraphMinValue;

            double recW = (double)(botLineW - startSpaceX - endSpaxeX - (graphCols.Count - 1) * recGap) / (double)(graphCols.Count);
            double recFullH = botLineH - startSpaceY - endSpaceY;

            int x = startSpaceX + 5;
            int y = startSpaceY;

            Font valueFont = new Font("Arial", 10);
            SolidBrush valueBrush = new SolidBrush(Color.Black);
            Pen zeroValuePen = new Pen(Color.Red, 1);
            SolidBrush zeroValueBrush = new SolidBrush(Color.Red);

            DateTime firstDate = selectedTime;

            double incVal = 0;

            if (min < 0)
            {
                incVal = min * -1;

                double perc = (incVal + 0) / (double)(max - min);

                if (perc == 0)
                    perc = 0.01;

                double recH = recFullH * perc;

                int zeroY = (int)(y + recFullH - recH);

                g.DrawLine(zeroValuePen, startSpaceX + 2, zeroY, (float)botLineW - 25, zeroY);

                g.DrawString("0", valueFont, zeroValueBrush, 1, zeroY - 5);
            }

            for (int i = 0; i < graphCols.Count; i++)
            {
                SolidBrush brush = new SolidBrush(Color.Blue);

                double perc = (incVal + graphCols[i].Value) / (double)(max - min);

                //if (perc == 0)
                //    perc = 0.01;

                double recH = recFullH * perc;

                Rectangle r;

                r = new Rectangle(x, (int)(y + recFullH - recH), (int)recW, (int)recH);

                g.FillRectangle(brush, r);

                g.DrawString(graphCols[i].Value.ToString(), valueFont, valueBrush, x + (int)(recW / 2) - 5, startSpaceY - 10);

                Point[] point = new Point[] { new Point(x + 7, botLineH) };

                string dateText = graphCols[i].TimeInfo;

                g.Transform.TransformPoints(point);
                g.RotateTransform(20, MatrixOrder.Append);
                g.TranslateTransform(point[0].X, point[0].Y, MatrixOrder.Append);
                g.DrawString(dateText, valueFont, valueBrush, 0, 0);

                g.ResetTransform();

                x += (int)recW + recGap;

                firstDate = firstDate.AddHours(1);
            }

            g.DrawString(forecType.Unit, valueFont, valueBrush, startSpaceX + 5, startSpaceY - 10);
            g.DrawString("[datetime]", valueFont, valueBrush, botLineW - 30, botLineH);

            g.DrawString(max.ToString(), valueFont, valueBrush, 1, startSpaceY - 10);
            g.DrawString(min.ToString(), valueFont, valueBrush, 1, botLineH - endSpaceY - 10);
        }

        private void GraphClear()
        {
            pGraph.Refresh();
            graphCols = new List<GraphElement>();
        }

        private void nahrátCestuZGPXSouboruToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string loaders = SetLoaders();

            try
            {
                DateTime beginTime;

                FormGpxBeginTime gpxBeginTimeForm = new FormGpxBeginTime();
                var dialogResult = gpxBeginTimeForm.ShowDialog();

                if (dialogResult == DialogResult.OK)
                    beginTime = FormGpxBeginTime.BeginTime;
                else
                    throw new Exception("Nebyl zvolen platný čas začátku cesty!");

                double kmPerMin;    //kolo = 0.25, auto = 1, chůze = 0,06

                FormGpxChooseTravelForm travelMenu = new FormGpxChooseTravelForm();
                dialogResult = travelMenu.ShowDialog();

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

                    int numberOfPoints = 10; //včetně začátku a cíle
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

                            double precVal = BitmapHandler.GetFullPrecInPoint(beginTime, pointStart, loaders, bounds, forecType);

                            graphCols.Add(new GraphElement(precVal, beginTime, 0));

                            listMarkers.Add(new GMarkerGoogle(pointStart, GMarkerGoogleType.red_small));
                            listMarkers[0].ToolTipText = $"Start\nčas: {beginTime.ToString("HH:mm - dd. MM.")}\n{forecType.CzForecType}: {precVal} {forecType.Unit}";

                            gMap.Overlays.Add(markers);
                            markers.Markers.Add(listMarkers[0]);
                        }
                        else if (i == routePoints.Count - 1)
                        {
                            PointLatLng pointEnd = routePoints[i];

                            int timeMin = (int)(distanceKm / kmPerMin);

                            double precVal = BitmapHandler.GetFullPrecInPoint(beginTime.AddMinutes(timeMin), pointEnd, loaders, bounds, forecType);
                            double roundedDistanceKm = Math.Round(distanceKm, 2);

                            graphCols.Add(new GraphElement(precVal, beginTime.AddMinutes(timeMin), roundedDistanceKm));

                            listMarkers.Add(new GMarkerGoogle(pointEnd, GMarkerGoogleType.red_small));
                            listMarkers[listMarkers.Count - 1].ToolTipText = $"Cíl\nčas: {beginTime.AddMinutes(timeMin).ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {precVal} {forecType.Unit}\n\nvzdálenost: {roundedDistanceKm} km\nzabere: {timeMin} min";

                            gMap.Overlays.Add(markers);
                            markers.Markers.Add(listMarkers[listMarkers.Count - 1]);

                            break;
                        }
                        else if (i % step == 0)
                        {
                            PointLatLng pointInner = routePoints[i];

                            int timeMin = (int)(distanceKm / kmPerMin);

                            double precVal = BitmapHandler.GetFullPrecInPoint(beginTime.AddMinutes(timeMin), pointInner, loaders, bounds, forecType);
                            double roundedDistanceKm = Math.Round(distanceKm, 2);

                            graphCols.Add(new GraphElement(precVal, beginTime.AddMinutes(timeMin), roundedDistanceKm));

                            listMarkers.Add(new GMarkerGoogle(pointInner, GMarkerGoogleType.red_small));
                            listMarkers[listMarkers.Count - 1].ToolTipText = $"čas: {beginTime.AddMinutes(timeMin).ToString("HH:mm - dd.MM.")}\n{forecType.CzForecType}: {precVal} {forecType.Unit}\n\nvzdálenost: {roundedDistanceKm} km\nzabere: {timeMin} min";

                            markers.Markers.Add(listMarkers[listMarkers.Count - 1]);
                        }

                        distanceKm += BitmapHandler.GetDistance(routePoints[i], routePoints[i + 1]) / 1000;
                    }

                    routeP.Clear();

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

            routeP.Clear();

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
        }

        private void bAnim_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < 10; i++)
            {
                int res = drawBitmapFromServer();

                if (res == 1)
                {
                    break;
                }

                label1.Text = selectedTime.ToString("dd. MM. yyyy - HH:mm");
                label1.Update();

                selectedTime = selectedTime.AddHours(1);

                if (i != 0)
                    trackBar1.Value += 60;

            }

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

        private void gMap_OnMapZoomChanged()
        {

            if (isBitmapShown)
            {
                bitmapOverlay.Markers.Remove(bitmapMarker);
                gMap.Overlays.Remove(bitmapOverlay);

                try
                {
                    int xDif = (int)(gMap.FromLatLngToLocal(bounds[1]).X - gMap.FromLatLngToLocal(bounds[0]).X);
                    int yDif = (int)(gMap.FromLatLngToLocal(bounds[1]).Y - gMap.FromLatLngToLocal(bounds[0]).Y);

                    Bitmap resizedBitmap = new Bitmap(dataBitmap, new Size(xDif, yDif));

                    bitmapMarker = new GMarkerGoogle(new PointLatLng(bounds[1].Lat, (bounds[0].Lng + bounds[1].Lng) / 2), resizedBitmap) { IsHitTestVisible = false };
                    bitmapOverlay.Markers.Add(bitmapMarker);

                    gMap.Overlays.Add(bitmapOverlay);
                }
                catch
                {
                    isBitmapShown = false;
                }

                //zoomReload();
            }
        }

        private void minimálníToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.BitmapAlpha = 255;
            bitmapAlpha = 255;
            gMap_OnMapZoomChanged();
        }

        private void maximálníToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApkConfig.BitmapAlpha = 0;
            bitmapAlpha = 0;
            gMap_OnMapZoomChanged();
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

            loaders = "";
            string jsonLoaders = "";

            if (checkBox1.Checked)
            {

                jsonLoaders += "b1";

                if (checkBox1.Enabled)
                {
                    loaders += "b1";
                }
            }

            if (checkBox2.Checked)
            {

                jsonLoaders += "b2";

                if (checkBox2.Enabled)
                {
                    loaders += "b2";
                }
            }

            if (checkBox3.Checked)
            {

                jsonLoaders += "x";

                if (checkBox3.Enabled)
                {
                    loaders += "x";
                }
            }

            if (checkBox4.Checked)
            {

                jsonLoaders += "j";

                if (checkBox4.Enabled)
                {
                    loaders += "j";
                }
            }

            ApkConfig.Loaders = jsonLoaders;
        }

        private void menuOwnTransparent_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                bitmapAlpha = int.Parse(menuOwnTransparent.Text);

                if (bitmapAlpha < 0)
                    bitmapAlpha = 0;
                else if (bitmapAlpha > 255)
                    bitmapAlpha = 255;

                menuOwnTransparent.Text = bitmapAlpha.ToString();

                ApkConfig.BitmapAlpha = bitmapAlpha;

                if (isBitmapShown)
                {
                    for (int x = 0; x < dataBitmap.Width; x++)
                    {
                        for (int y = 0; y < dataBitmap.Height; y++)
                        {
                            Color oldPixel = dataBitmap.GetPixel(x, y);

                            if ((oldPixel.R == 0 && oldPixel.G == 0 && oldPixel.B == 0) || (oldPixel.R == 255 && oldPixel.G == 255 && oldPixel.B == 255))
                                continue;

                            Color newPixel = Color.FromArgb(bitmapAlpha, oldPixel.R, oldPixel.G, oldPixel.B);

                            dataBitmap.SetPixel(x, y, newPixel);
                        }
                    }

                    gMap_OnMapZoomChanged();
                }
            }
            catch
            {
                menuOwnTransparent.Text = bitmapAlpha.ToString();

                //MessageBox.Show("Hodnota průhlednosti musí být celé číslo v rozmezí od 0 do 255!", "Chyba při změně průhlednosti srážek", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rBTemp_CheckedChanged(object sender, EventArgs e)
        {
            
            if(rBPrec.Checked)
            {
                forecType = forecTypePrec;
            }
            else
            {
                forecType = forecTypeTemp;
            }

            ValidateDataLoaders();
        }

        private void ValidateDataLoaders()
        {
            if (selectedTime > DateTime.Now.AddHours(6))
            {
                checkBox1.Enabled = false;
            }
            else if(rBTemp.Checked)
            {
                checkBox1.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
            }

            if (selectedTime > DateTime.Now.AddDays(4))
            {
                checkBox2.Enabled = false;
            }
            else
            {
                checkBox2.Enabled = true;
            }
        }
    }
}
