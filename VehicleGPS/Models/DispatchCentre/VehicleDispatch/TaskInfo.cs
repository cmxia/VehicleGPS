using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.DispatchCentre.VehicleDispatch
{
    public class TaskInfo
    {
        public string FAgreementid { get; set; }//合同编号
        public string FProjName { get; set; }//工程名称
        public string FPlanId { get; set; }//任务单号
        public string ConcreteId { get; set; }//混凝土标号
        public string ConcreteName { get; set; }//标号名称
        public string StartPoint { get; set; }//起始区域名称
        public string EndPoint { get; set; }//结束工地名称
        public string TransCap { get; set; }//运输总方量
        public string TransedCap { get; set; }//已运输方量
        public string NoTransCap { get; set; }//未运输方量
        public string TransDistance { get; set; }//运输距离
        public double RealTransDistance { get; set; }//真正的运输距离，根据起点终点经纬度计算
        public string CarsPlanStatus { get; set; }//车辆执行任务状态
        public string Count { get; set; }//已发车次统计
        public string Site { get; set; }//施工部位
        public string PourType { get; set; }//浇筑方式
        public string StartTime { get; set; }//开始时间
        public string EndTime { get; set; }//结束时间
        public string FinsertTime { get; set; }//插入时间
        public string FMemo { get; set; }//备注
        public string StartID { get; set; }//起始区域编号
        public string EndID { get; set; }//终止工地编号
        public string EndLng { get; set; }//工地经度
        public string EndLat { get; set; }//工地纬度
        public string EndRadius { get; set; }//工地半径
        public string StartLng { get; set; }//区域经度
        public string StartLat { get; set; }//区域纬度
        public string StartRadius { get; set; }//区域半径
        //public string TransTime { get; set; }//运输时间
        //public string IsStartFAlarm { get; set; }//是否开启开盘时间报警
        //public string IsTransAlarm { get; set; }//是否开启运输时间报警
        //public string IsoffAlarm { get; set; }//是否开启卸料时间报警
        //public string IsCountVAlarm { get; set; }//是否开启计划车数报警
        //public string IsGapAlarm { get; set; }//是否开启派车间隔报警
        //public string IsAlreadyVAlarm { get; set; }//是否开启累计方量报警
        //public string IsLeftVAlarm { get; set; }//是否开启尾数供应报警
        //public string IsFinishAlarm { get; set; }//是否开启结束任务报警
        //public string EarlyOffAlarm { get; set; }//是否卸料提醒
        //public string taskLevel { get; set; }//任务级别
        //public string LeftV { get; set; }//供应尾数
        //public string AlreadyV { get; set; }//累计方量
        //public string CountV { get; set; }//计划车数
        public string GapTime { get; set; }//派车间隔
        //public string offtime { get; set; }//待卸时间
        //public string StartFdate { get; set; }//开盘时间
        //public string FinishTime { get; set; }//结束任务时间
        public string RegionID { get; set; }//站点编号
        public string RegionName { get; set; }//站点名称
        public string UnitId { get; set; }//所属单位ID
        public string UnitName { get; set; }//所属单位名称
        public int tripcount { get; set; }//趟次数
    }
}
