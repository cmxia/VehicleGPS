using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.DispatchCentre.TaskManage
{
    class DispatchTaskInfo
    {
        //public int Sequence { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FPlanId { get; set; }//任务单号
        public string ConcreteId { get; set; }//混凝土标号
        //public string StartPoint { get; set; }//起始区域
        //public string TransCap { get; set; }//已运输方量
        //public string TransDistance { get; set; }//运输距离
        //public string Count { get; set; }//已发车次统计
        public string site { get; set; }//施工部位
        //public string PourType { get; set; }//浇筑方式
        //public string StartTime { get; set; }//开始时间
        public string EndTime { get; set; }//结束时间
        //public string CarsPlanStatus { get; set; }//车辆执行任务状态
        //public string Slump { get; set; }
        //public string ImpGrade { get; set; }
        //public string FAgreementid { get; set; }//合同编号
        //public string EndPoint { get; set; }//结束工地
        //public string TransedCap { get; set; }//运输总方量
        //public string GapTime { get; set; }//派车间隔
        //public string StartID { get; set; }//开始区域编号
        //public string EndID { get; set; }//结束工地编号
        public string ConcreteName { get; set; }//标号名称
        public string concretNum { get; set; }// 标号 新的
        //public string RegionID { get; set; }//站点编号
        //public string RegionName { get; set; }//站点名称
        //public string FMemo { get; set; }//备注

        public int Sequence { get; set; }
        public string TaskListId { get; set; }//任务单号
        public string UnitId { get; set; }//单位编号
        public string UnitName { get; set; }//单位名称
        public string TransGoods { get; set; }//运输物品
        public string StartRegId { get; set; }//开始区域编号
        public string StartRegName { get; set; }//起始区域
        public string EndRegId { get; set; }//结束工地编号
        public string EndRegName { get; set; }//结束工地
        public string TransTotalCube { get; set; }//运输总方量
        public string TransedCube { get; set; }//已运输方量
        public string TransDistance { get; set; }//运输距离
        public string CarTranCount { get; set; }//已发车次统计
        public string InsertTime { get; set; }//下任务时间
        public string TaskStatus { get; set; }//车辆执行任务状态
        public string TaskLevel { get; set; }//任务级别
        public string viscosity { get; set; }//塌落度
        public string taskStatus { get; set; }//完成情况
        public string startTime { get; set; }//计划开始时间
        public string firstOverTime { get; set; }//首次提醒时间
        public string secondOverTime { get; set; }//再次提醒时间
        
        

    }
}
