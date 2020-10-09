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

            lLonY.Text = "[Y] Lon: " + coordinates.Y;
            lLatX.Text = "[X] Lat: " + coordinates.X;

            Color c = ((Bitmap)pBForecast.Image).GetPixel(coordinates.X, coordinates.Y);

            lPrec.Text = $"Srážky: {new BitmapHandler().GetPrecipitationFromPixel(c)} [mm]";

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
    }
}
