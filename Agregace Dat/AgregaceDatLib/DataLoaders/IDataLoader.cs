using System;
using System.Drawing;

namespace AgregaceDatLib
{
    //XML
    //Yr.no - https://www.yr.no/place/Norway/Vestfold_og_Telemark/Midt-Telemark/Gvarv/forecast.xml
    //https://www.yr.no/place/Czech_Republic/Olomouc/Olomouc/forecast.xml
    //http://www.yr.no/place/Czech_Republic/Prague/Prague/forecast.xml
    //http://fil.nrk.no/yr/viktigestader/verda.txt

    //JSON
    //api.openweathermap.org/data/2.5/forecast/daily?lat=35&lon=139&cnt=10
    //API KEY - ea63080a4f8e99972630d2671e3ef805
    //pro.openweathermap.org/data/2.5/forecast/hourly?lat={lat}&lon={lon}&appid={your api key}
    //pro.openweathermap.org/data/2.5/forecast/hourly?lat=30&lon=160&appid=ea63080a4f8e99972630d2671e3ef805
    //https://samples.openweathermap.org/data/2.5/forecast/daily?lat=35&lon=139&cnt=10&appid=b1b15e88fa797225412429c1c50c122a1

    //BITMAP
    //https://www.in-pocasi.cz/radarove-snimky/
    //https://www.in-pocasi.cz/radarove-snimky/napoveda/
    //https://www.in-pocasi.cz/data/chmi_v2/20200531_0800_r.png                 //599 x 380
    //http://portal.chmi.cz/files/portal/docs/meteo/rad/img/banner-text.png     
    //http://radar.bourky.cz/data/pacz2gmaps.z_max3d.20200705.1020.0.png        //728 x 528
    //                                               ROK MĚSÍC DEN.HODINY MINUTY.0.png 

    //2. BITMAP
    //http://www.medard-online.cz/
    //http://www.medard-online.cz/apipreview?run=201108_18&forecast=precip&layer=eu
    //http://www.medard-online.cz/apiforecast?run=210124_06&forecast=precip&layer=eu&step=78    //1 - 78

    //XML
    //http://api.weatherunlocked.com/api/forecast/50.10,20.28?app_id=79ef9432&app_key=904d54d78eec41c2c55ed93cbaf7c7ca
    //key1 - 904d54d78eec41c2c55ed93cbaf7c7ca
    //key1 - 79ef9432
    //key2 - 71ef299d81c23d71cd36fcb8ee8691ba
    //id2 - c0e28ff1
    public interface DataLoader
    {
        Bitmap GetForecastBitmap(DateTime forTime, string type);
        Forecast GetForecastPoint(DateTime forTime, PointLonLat point);
        void SaveNewDeleteOldBmps();
    }
}
