using IDataLoaderAndHandlerLib.HandlersAndObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDataLoaderAndHandlerLib.HandlersAndObjects
{
    public class Bounds
    {
        public Bounds() { }

        public Bounds(PointLonLat tLC, PointLonLat bRC)
        {
            TopLeftCorner = tLC;
            BotRightCorner = bRC;
        }

        [JsonProperty("TopLeftCornerLonLat")]
        public PointLonLat TopLeftCorner { get; set; }
        [JsonProperty("BotRightCornerLonLat")]
        public PointLonLat BotRightCorner { get; set; }
    }
}
