using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfApplication1.zmq;
using Newtonsoft.Json.Linq;

namespace VehicleGPS.zmq
{
    class JsonHelper
    {
        public static LatestGpsInfo DeserializeLatestGpsInfo(string strObj)
        {
            if (strObj == "" || strObj == null)
                return null;

            JObject jo = JObject.Parse(strObj);
            LatestGpsInfo temp = new LatestGpsInfo();
            temp.Altitude = jo["altitude"].ToString();
            temp.DevSpeed = jo["devSpeed"].ToString();
            temp.Direction = jo["direction"].ToString();
            temp.Distance = jo["distance"].ToString();
            temp.GpsDistance = jo["gpsDistance"].ToString();
            temp.Latitude = jo["latitude"].ToString();
            temp.Longitude = jo["longitude"].ToString();
            temp.Oil = jo["oil"].ToString();
            temp.SatelliteNum = jo["satelliteNum"].ToString();
            temp.SignalStrength = jo["signalStrength"].ToString();
            temp.SimId = jo["simId"].ToString();
            temp.Speed = jo["speed"].ToString();
            temp.UpTime = jo["upTime"].ToString();
            return temp;
        }
    }
}
