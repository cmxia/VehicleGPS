using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.DispatchCentre.VehicleDispatch
{
    public class DriverInfo
    {
        public string sequence { get; set; }//序号
        public string workNum { get; set; }//内部编号
        public string workID { get; set; }//职工编号
        public string DriverName { get; set; }//姓名
        public string sex { get; set; }//性别
        public string zoneID { get; set; }//所属站点ID
    }
}
