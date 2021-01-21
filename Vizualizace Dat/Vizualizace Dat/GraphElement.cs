using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vizualizace_Dat
{
    public class GraphElement
    {
        public GraphElement(double v, DateTime t, double? d = null)
        {
            Value = v;
            Time = t;
            Distance = d;
        }

        public double Value { get; set; }
        public DateTime Time { get; set; }
        public double? Distance { get; set; }

        public string TimeInfo
        {
            get
            {
                if(Distance == null)
                {
                    return Time.ToString("dd.MM.yyyy HH:mm");
                }
                else
                {
                    return Time.ToString("dd.MM.yyyy HH:mm") + "\n" + Distance + " [km]";
                }

            }
        }
    }
}
