using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models
{
    /*Client和Vehicel的基本信息*/
    /// <summary>
    /// 车辆和用户基本信息表（树形结构形成用到的节点）（权限内）
    /// </summary>
    public class CVBasicInfo
    {
        public string ID { get; set; }//节点ID
        public string InnerID { get; set; }//内部编号ID（针对车辆节点）
        public string Name { get; set; }//节点名字，车辆为车牌号，用户为用户名
        public string ParentID { get; set; }//父节点ID
        public string TypeID { get; set; }//节点类型
        public string SIM { get; set; }//SIM卡号，针对车辆
        public int OnlineCount { get; set; }//在线数量，针对用户;
        public int Count { get; set; }//总数量，针对用户
        public string TaskState { get; set; }//任务状态，针对车辆
        public string OnlineStatus { get; set; }//在线状态，针对车辆
    }
}
