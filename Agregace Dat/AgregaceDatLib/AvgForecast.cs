using System;
using System.Collections.Generic;
using System.Text;

namespace AgregaceDatLib
{
    public class AvgForecast
    {
        private List<DataLoader> dLs;

        public AvgForecast()
        {
            dLs = new List<DataLoader>();
        }

        public AvgForecast(List<DataLoader> defDLs)
        {
            dLs = defDLs;
        }

        public void Add(DataLoader newDl)
        {
            dLs.Add(newDl);
        }

        public void Remove(DataLoader remDl)
        {
            dLs.Remove(remDl);
        }

        public Forecast GetForecast(DateTime time, String place)
        {

            Forecast newF = new Forecast();

            for (int i = 0; i < dLs.Count; i++)
            {

                if (i == 0)
                {
                    newF = dLs[0].GetForecastByTime(time);
                    continue;
                }

                newF.AddForecast(dLs[i].GetForecastByTime(time));

            }

            newF.SetAvgForecast(dLs.Count);

            return newF;
        }

    }
}
