using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models
{
    public class WarnInfo : NotificationObject
    {
        //推送过来的字段
        public string cmdId { get; set; }
        public string serialNum { get; set; }
        public string sequence { get; set; }//序号
        public string VehicleId { get; set; }//车辆内部编号
        public string VehicleNum { get; set; }//车牌号
        public string VehicleStatus { get; set; }//车辆状态
        public string ProcessStatus { get; set; }//处理状态
        public string Speed { get; set; }//速度
        public string UnitId { get; set; }//所属单位id
        public string UnitName { get; set; }//所属单位名称
        public string Long { get; set; }//经度
        public string Lat { get; set; }//纬度
        public string Time { get; set; }//报警时间
        public string AlarmType { get; set; }//报警编号
        public string AlarmContent { get; set; }//报警类型名称
        public string SimId { get; set; }//sim卡号
        public string Place { get; set; }//地址
        public string Driver { get; set; }//驾驶员
        private bool isselected;//是否选中

        public bool IsSelected
        {
            get { return isselected; }
            set
            {
                isselected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }
        //是否解除 
        private bool isrelieved;

        public bool IsRelieved
        {
            get { return isrelieved; }
            set
            {
                isrelieved = value;
                this.RaisePropertyChanged("IsRelieved");
            }
        }


    }
}
