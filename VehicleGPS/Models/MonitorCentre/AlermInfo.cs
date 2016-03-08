using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.MonitorCentre
{
    class AlermInfo
    {
        public int id { get; set; }//序号
        public string simId { get; set; }//sim卡号
        public string vehicleId { get; set; }//车牌号
        public string lng { get; set; }//经度
        public string lat { get; set; }//纬度
        public string speed { get; set; }//速度
        public string warntype { get; set; }//报警类型
        public string warnstarttime { get; set; }//报警开始时间
        public string warnendtime { get; set; }//报警结束时间
    }
}
