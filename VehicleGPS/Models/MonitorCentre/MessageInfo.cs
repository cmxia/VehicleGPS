using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.MonitorCentre
{
    /// <summary>
    /// author 夏创铭
    /// 短语信息
    /// </summary>
    public class MessageInfo
    {
        public string VehicleID { get; set; } //内部编号
        public string SimID { get; set; }//sim卡号
        public string VehicleNum { get; set; }//车牌号
        public string Time { get; set; }//时间
        public string Content { get; set; }//内容
    }
}
