using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VehicleGPS.Models.DispatchCentre.VehicleDispatch
{
    public class DispatchVehicleInfo
    {
        public string sequence { get; set; }//序号
        public string FPlanId { get; set; }//任务单号
        public string InnerId { get; set; }//内部编号
        public string DriverId { get; set; }//驾驶员编号
        public string DriverName { get; set; }//驾驶员姓名
        public string VehicleID { get; set; }//车牌号
        public string VehicleNum { get; set; }//车辆编号
        public string VehicleType { get; set; }//车辆类型编号
        public string VehicleTypeName { get; set; }//车辆类型
        public string offtype { get; set; }//卸料方式
        public string offTypeName { get; set; }//卸料方式
        private string transCapPer;
        public string TransCapPer
        {
            get { return transCapPer; }
            set
            {
                if (string.IsNullOrEmpty(transCapPer))
                {
                    transCapPer = value;
                }
                else
                {
                    transCapPer = Convert.ToDouble(value).ToString("0.00");
                }
            }
        }//单次运输方量
        public string Holiday { get; set; }//节假日
        public string CarsStatus { get; set; }//车辆出行状态
        public string TowerID { get; set; }//塔楼ID
        public string TowerName { get; set; }//塔楼名
        public string ConcreteID { get; set; }//混凝土标号（包含水）
        public string ConcreteName { get; set; }//混凝土标号（包含水）
        public string LeaveStartPointTime { get; set; }//离开区域时间
        public string transDistance { get; set; }//运输距离
        public string Position { get; set; }//施工部位

        public string UnitName { get; set; }//客户名称
        public string StartRegion { get; set; }//厂区
        public string EndRegion { get; set; }//工地
        public string InsertTime { get; set; }//派车时间
        public string TripTime { get; set; }//趟次

        public string OutStartRegionTime { get; set; }//出站时间
        public string InEndRegionTime { get; set; }//到场时间
        public string StartUnloadTime { get; set; }//开始卸料时间
        public string EndUnloadTime { get; set; }//结束卸料时间

        public DateTime Now { get; set; }
        public DispatchVehicleInfo()
        {
            this.Now = DateTime.Now;
        }
    }
}
