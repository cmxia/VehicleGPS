using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.DispatchCentre.VehicleDispatch
{
    class SiteInfo
    {
        public string ZoneID { get; set; }//工地id
        public string ZoneName { get; set; }//工地名称
        public string Long { get; set; }//工地经度
        public string Lat { get; set; }//工地纬度
        public string Radio { get; set; }//工地半径
        public string TransTime { get; set; }//运输时间
        public string OffTime { get; set; }//待卸时间
        public string StartFdate { get; set; }//开盘时间
        public string IsTransAlarm { get; set; }//是否开启运输时间报警
        public string IsOffAlarm { get; set; }//是否开启卸载时间报警
        public string IsStartFAlarm { get; set; }//是否开启开盘时间报警
    }
}
