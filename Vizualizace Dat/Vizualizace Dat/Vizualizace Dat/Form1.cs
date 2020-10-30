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
        private GMapOverlay polygons;

        public Form1()
        {
            InitializeComponent();

            pBForecast.BackColor = Color.Transparent;

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "MM/dd/yyyy hh:mm:ss";

            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.Position = new PointLatLng(0, 0);
            gMap.MinZoom = 1;
            gMap.MaxZoom = 10;
            gMap.Zoom = 5;
            gMap.Position = new PointLatLng(49.89, 15.16);
            gMap.ShowCenter = false;

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
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gMap.Visible = !gMap.Visible;
        }

        private void clickInBitmap(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            System.Drawing.Point coordinates = me.Location;

            int x = coordinates.X;
            int y = coordinates.Y;

            double lon = BitmapHandler.GetLon(x, pBForecast.Image.Width);
            double lat = BitmapHandler.GetLat(y, pBForecast.Image.Height);

            Color c = ((Bitmap)pBForecast.Image).GetPixel(x, y);

            lLonX.Text = $"[X: {x}] Lon: {lon}";
            lLatY.Text = $"[Y: {y}] Lat: {lat}";
            lPrec.Text = $"Srážky: {BitmapHandler.GetPrecipitationFromPixel(c)} [mm]\n" + c.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
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

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string loaders = null;

            if (checkBox1.Checked)
                loaders += "b";

            if (checkBox2.Checked)
                loaders += "x";

            if (checkBox3.Checked)
                loaders += "j";

            DateTime selectedTime = dateTimePicker1.Value;

            Bitmap bFor = BitmapHandler.GetBitmapFromServer("prec", selectedTime, loaders);

            pBForecast.Image = bFor;

            int bW = bFor.Width;
            int bH = bFor.Height;
            double pixelLon = (20.21 - 10.06) / bW;
            double pixelLat = (51.88 - 47.09) / bH;

            gMap.Overlays.Remove(polygons);
            polygons = new GMapOverlay("polygons");

            double bX = 10.06;
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
        }

        private void mouseMoveInMap(object sender, MouseEventArgs e)
        {
            double lat = gMap.FromLocalToLatLng(e.X, e.Y).Lat;
            double lon = gMap.FromLocalToLatLng(e.X, e.Y).Lng;

            lonD = lon;
            latD = lat;

            lLonX.Text = $"Lon: {Math.Round(lon, 6)}";
            lLatY.Text = $"Lat: {Math.Round(lat, 6)}";
        }

        private void mouseDoubleClickInMap(object sender, MouseEventArgs e)
        {

            int x = BitmapHandler.GetX(lonD, pBForecast.Image.Width, 10.06, 20.21);
            int y = BitmapHandler.GetY(latD, pBForecast.Image.Height, 51.88, 47.09);

            Color c = ((Bitmap)pBForecast.Image).GetPixel(x, y);

            lCity.Text = BitmapHandler.GetAdressFromLonLat(lonD, latD);
            lPrec.Text = $"Srážky: {BitmapHandler.GetPrecipitationFromPixel(c)} [mm]";

            if (canDrawPoint)
            {
                gMap.Overlays.Remove(markers);
                markers.Markers.Remove(marker);

                Console.WriteLine("lat: " + latD + " lon: " + lonD);

                PointLatLng point = new PointLatLng(latD, lonD);

                marker = new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin);

                gMap.Overlays.Add(markers);
                markers.Markers.Add(marker);
                
            }

        }
    }
}
