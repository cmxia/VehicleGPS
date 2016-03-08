using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models.DispatchCentre.VehicleDispatch
{
    public class BackToStationInfo : NotificationObject
    {
        public string fPlanId { get; set; } //任务单编号
        public string PlanDetailId { get; set; }//任务详细清单编号
        public string vehicleID { get; set; } //车牌号
        public string vehicleNum { get; set; }//车辆编号 
        public string sim { get; set; } //SIM
        public string vehicleTypeName { get; set; }//车辆类型
        public string startPoint { get; set; }//发车区域
        public string endPoint { get; set; }//运往工地
        public string transCapPer{ get; set; }//运输方量
        public string loadAmount { get; set; }//核载方量
        public string concreteName { get; set; }//混凝土标号（包含水）
        public string carsStatus { get; set; }//车辆出行状态
        public string offTypeName { get; set; }//卸料方式
        public string driverId { get; set; }//驾驶员编号
        public string driverName { get; set; }//驾驶员姓名
        public string isassigncircle { get; set; }//是否循环派车 
        private bool ischecked;//是否选中

        public bool IsChecked
        {
            get { return ischecked; }
            set
            {
                ischecked = value;
                this.RaisePropertyChanged("IsChecked");
            }
        }
        private bool isAddCount;//是否增加方量

        public bool IsAddCount
        {
            get { return isAddCount; }
            set
            {
                isAddCount = value;
                this.RaisePropertyChanged("IsAddCount");
            }
        }

    }
}
