using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models
{
    class GpsOnlineInfo
    {
        public int Sequence { get; set; }//序号
        public string OnlineTime { get; set; }//上线时间
        public string OfflineTime { get; set; }//下线时间
        public string OnlineAddr { get; set; }//上线地点
        public string OfflineAddr { get; set; }//下线地点
        public string Slng { get; set; }//上线地点经度
        public string Slat { get; set; }//上线地点纬度
        public string Elng { get; set; }//下线地点经度
        public string Elat { get; set; }//下线地点纬度
    }
}
