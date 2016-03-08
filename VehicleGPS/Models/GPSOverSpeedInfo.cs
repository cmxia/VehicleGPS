using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models
{
    class GPSOverSpeedInfo
    {
        public string Sequence { get; set; }//序号
        public string StartTime { get; set; }//开始时间
        public string EndTime { get; set; }//结束时间
        public string LastTime { get; set; }//持续时间
        public string StartAddress { get; set; }//开始位置
        public string EndAddress { get; set; }//结束位置
        public string Slng { get; set; }//开始位置经度
        public string Slat { get; set; }//开始位置纬度
        public string Elng { get; set; }//结束位置经度
        public string Elat { get; set; }//结束位置纬度

    }
}
