using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vizualizace_Dat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            pBForecast.BackColor = Color.Transparent;

        }

        private void clickInMap(object sender, EventArgs e)
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
            lCity.Text = BitmapHandler.GetAdressFromLonLat(lon, lat);
            lPrec.Text = $"Srážky: {BitmapHandler.GetPrecipitationFromPixel(c)} [mm]";

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
    }
}
