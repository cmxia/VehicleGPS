using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Models
{
    /// <summary>
    /// author 夏创铭
    /// 实时信息
    /// 包括发送短语  接受短语  指令
    /// </summary>
    class StaticRealTimeInfo : NotificationObject
    {

        public StaticRealTimeInfo()
        {
            this.CmdList = new List<CommandInfo>();
            this.SendMessageList = new List<MessageInfo>();
            this.ReceiveMessageList = new List<MessageInfo>();
        }
        private static StaticRealTimeInfo instance = null;
        public static StaticRealTimeInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticRealTimeInfo();
            }
            return instance;
        }
        //发出短语
        private List<MessageInfo> sendmessagelist;

        public List<MessageInfo> SendMessageList
        {
            get { return sendmessagelist; }
            set
            {
                sendmessagelist = value;
                this.RaisePropertyChanged("SendMessageList");
            }
        }
        //接收短语
        private List<MessageInfo> receivemessagelist;

        public List<MessageInfo> ReceiveMessageList
        {
            get { return receivemessagelist; }
            set
            {
                receivemessagelist = value;
                this.RaisePropertyChanged("ReceiveMessageList");
            }
        }
        //指令
        private List<CommandInfo> cmdlist;

        public List<CommandInfo> CmdList
        {
            get { return cmdlist; }
            set
            {
                cmdlist = value;
                this.RaisePropertyChanged("CmdList");
            }
        }

        public bool iscluster = false;//是否聚类显示车辆  默认不聚类
    }
}
