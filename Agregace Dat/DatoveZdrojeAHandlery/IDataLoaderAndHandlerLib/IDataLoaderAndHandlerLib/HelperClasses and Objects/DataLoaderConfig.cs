using System;
using System.Collections.Generic;
using System.Text;

namespace IDataLoaderAndHandlerLib.HandlersAndObjects
{
    public class DataLoaderConfig
    {
        public DateTime LastUpdateDateTime { get; set; }
        public PointLonLat TopLeftCornerLonLat { get; set; }
        public PointLonLat BotRightCornerLonLat { get; set; }
        public int MaximumDownloadsPerMinute { get; set; }
        public double MinimumHoursToNewDownload { get; set; }
        public int MaximumHoursBack { get; set; }
        public string DataLoaderName { get; set; }
        public string DataLoaderShortcut { get; set; }
    }
}
