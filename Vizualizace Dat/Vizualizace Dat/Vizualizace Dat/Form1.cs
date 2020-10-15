using MapWinGIS;
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
        private double lonD = 0;
        private double latD = 0;
        private bool canDrawPoint = false;

        public Form1()
        {
            InitializeComponent();

            pBForecast.BackColor = Color.Transparent;

            axMap1.Projection = tkMapProjection.PROJECTION_WGS84;
            axMap1.TileProvider = tkTileProvider.OpenStreetMap;
            axMap1.SendMouseMove = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void mouseMoveInMap(object sender, AxMapWinGIS._DMapEvents_MouseMoveEvent e)
        {

            double mapX = 0, mapY = 0;
            axMap1.PixelToProj(e.x, e.y, ref mapX, ref mapY);

            lonD = mapX;
            latD = mapY;

            lLonX.Text = $"Lon: {Math.Round(mapX,6)}";
            lLatY.Text = $"Lat: {Math.Round(mapY,6)}";
        }

        private void doubleclickInMap(object sender, EventArgs e)
        {
            int x = BitmapHandler.GetX(lonD, pBForecast.Image.Width, 10.06, 20.21);
            int y = BitmapHandler.GetY(latD, pBForecast.Image.Height, 51.88, 47.09);

            Color c = ((Bitmap)pBForecast.Image).GetPixel(x, y);

            lCity.Text = BitmapHandler.GetAdressFromLonLat(lonD, latD);
            lPrec.Text = $"Srážky: {BitmapHandler.GetPrecipitationFromPixel(c)} [mm]";

            if(canDrawPoint)
            {
                axMap1.NewDrawing(tkDrawReferenceList.dlSpatiallyReferencedList);
                axMap1.DrawCircle(lonD, latD, 5, new Utils().ColorByName(tkMapColor.Red), true);
            }
            
        }

        private void canDrawPointChange(object sender, EventArgs e)
        { 
            if(canDrawPoint)
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
            axMap1.ClearDrawings();
        }
    }
}
