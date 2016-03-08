using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.Report
{
    class OilCost
    {
        public string OilMounts { get; set; }//当前油量
        public string SIM { get; set; }// SIM卡号
        public string VehicleId { get; set; }//车牌号
        public string Vehiclenum { get; set; }//车辆编号
        public string LONG { get; set; }//经度
        public string LAT { get; set; }//纬度
        public string finsertTime { get; set; }//插入时间

        public string Address { get; set; }//地址
        public string Department { get; set; }//所属单位
    }
}
