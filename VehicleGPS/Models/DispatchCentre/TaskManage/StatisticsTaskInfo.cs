using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.DispatchCentre.TaskManage
{
    public class StatisticsTaskInfo
    {
        public string taskId { get; set; }//任务单号
        public string UnitName { get; set; }//单位名称
        public string StartRegion { get; set; }//出车区域
        public string EndRegion { get; set; }//工地
        public string Position { get; set; }//施工部位
        public double PlanCount { get; set; }//计划方量
        public double CompleteCount { get; set; }//完成方量
        public double CompleteNum { get; set; }//完成趟次
        public string unitId { get; set; }//单位ID
    }
}
