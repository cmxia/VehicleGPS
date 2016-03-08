using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models.MonitorCentre
{
    public class CommandInfo : NotificationObject
    {
        public int sequence { get; set; }//序号
        public string cmdId { get; set; }//指令ID
        public string cmdName { get; set; }//指令名称
        public string cmdTime { get; set; }//发指令时间
        public string cmdContent { get; set; }//指令内容
        public string cmdResult { get; set; }//发指令结果
        public string cmdSim { get; set; }//被发指令的车辆SIM卡号
        public string memo { get; set; }//备注
        //add by 夏创铭
        public string VehicleID { get; set; }//车辆内部编号
        public string VehicleNum { get; set; }//车牌号

        private string returnmsg;//目标回传

        public string ReturnMsg
        {
            get { return returnmsg; }
            set
            {
                returnmsg = value;
                this.RaisePropertyChanged("ReturnMsg");
            }
        }

        private string sendstatus;//发送状态

        public string SendStatus
        {
            get { return sendstatus; }
            set
            {
                sendstatus = value;
                this.RaisePropertyChanged("SendStatus");
            }
        }
        
    }
}
