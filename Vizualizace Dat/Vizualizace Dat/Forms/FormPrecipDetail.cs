using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vizualizace_Dat.Properties;

namespace Vizualizace_Dat
{
    public partial class FormPrecipDetail : Form
    {
        private ForecType forecTypeTemp = new ForecType(ForecastTypes.TEMPERATURE);
        private ForecType forecTypePrec = new ForecType(ForecastTypes.PRECIPITATION);
        private ForecType forecTypeHumi = new ForecType(ForecastTypes.HUMIDITY);
        private ForecType forecTypePres = new ForecType(ForecastTypes.PRESSURE);

        public FormPrecipDetail(DateTime selectedTime, PointLatLng point, string validLoaders, List<PointLatLng> bounds, FormMain mainForm)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = "Detail počasí   -   " + selectedTime.ToString("HH:mm dd.MM.yyyy");

            List<int> hours = new List<int> { 7, 14, 21 };

            for (int day = 0; day < 7; day++)
            {
                bool canDraw = false;

                DateTime time = selectedTime.AddDays(day);

                string dateTime = time.ToString("ddd, dd.MM").ToUpper();

                Bitmap tempIc = Resources.thermometer;
                Bitmap precIc = Resources.umbrella;
                Bitmap nullIc = new Bitmap(1, 1);

                List<string> precs = new List<string>();
                List<string> temps = new List<string>();
                List<Bitmap> precIcons = new List<Bitmap>();
                List<Bitmap> tempIcons = new List<Bitmap>();

                foreach (int hour in hours)
                {
                    time = new DateTime(time.Year, time.Month, time.Day, hour, 0, 0);

                    try
                    {
                        mainForm.ProgressBarIncrement();

                        double precValue = BitmapHandler.GetFullPrecInPoint(time, point, validLoaders, bounds, forecTypePrec);

                        string precString = precValue + "mm";


                        precs.Add(precString);
                        precIcons.Add(precIc);

                        canDraw = true;
                    }
                    catch
                    {
                        precs.Add("");
                        precIcons.Add(nullIc);
                    }

                    try
                    {
                        mainForm.ProgressBarIncrement();

                        double tempValue = BitmapHandler.GetFullPrecInPoint(time, point, validLoaders, bounds, forecTypeTemp);

                        string tempString = tempValue + "°";

                        temps.Add(tempString);
                        tempIcons.Add(tempIc);

                        canDraw = true;
                    }
                    catch
                    {
                        temps.Add("");
                        tempIcons.Add(nullIc);
                    }
                }

                time = new DateTime(time.Year, time.Month, time.Day, 14, 0, 0);

                string humiString = "";
                try
                {
                    mainForm.ProgressBarIncrement();

                    double humiVal = BitmapHandler.GetFullPrecInPoint(time, point, validLoaders, bounds, forecTypeHumi);

                    humiString = humiVal + "%";

                    canDraw = true;
                }
                catch 
                {
                    try
                    {
                        time = new DateTime(time.Year, time.Month, time.Day, 7, 0, 0);

                        double humiVal = BitmapHandler.GetFullPrecInPoint(time, point, validLoaders, bounds, forecTypeHumi);

                        humiString = humiVal + "%";

                        canDraw = true;
                    }
                    catch
                    {
                        try
                        {
                            time = new DateTime(time.Year, time.Month, time.Day, 21, 0, 0);

                            double humiVal = BitmapHandler.GetFullPrecInPoint(time, point, validLoaders, bounds, forecTypeHumi);

                            humiString = humiVal + "%";

                            canDraw = true;
                        }
                        catch
                        { }
                    }
                }

                string presString = "";
                try
                {
                    mainForm.ProgressBarIncrement();

                    double presVal = BitmapHandler.GetFullPrecInPoint(time, point, validLoaders, bounds, forecTypePres);

                    presString = presVal + "hPa";

                    canDraw = true;
                }
                catch { }

                if (canDraw)
                {
                    dataGrid.Rows.Add(dateTime, tempIcons[0], temps[0], precIcons[0], precs[0], tempIcons[1], temps[1], precIcons[1], precs[1], tempIcons[2], temps[2], precIcons[2], precs[2], humiString, presString);
                }
                else
                {
                    return;
                }
                
            }
        }
    }
}
