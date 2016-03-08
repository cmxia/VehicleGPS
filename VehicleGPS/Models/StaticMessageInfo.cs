using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Models
{
    class StaticMessageInfo : NotificationObject
    {
        public StaticMessageInfo()
        {
            this.CmdList = new List<CommandInfo>();
            this.SendMessageList = new List<MessageInfo>();
            this.ReceivedMessageList = new List<MessageInfo>();
        }

         //while (true)
         //   {
         //       if (StaticTreeState.MessageInfo==LoadingState.LOADCOMPLETE)
         //       {
         //           StaticTreeState.MessageInfo = LoadingState.NOLOADING;
         //           CommandInfo cmd = new CommandInfo();
         //           //这里要对cmd对象做相应的赋值
         //           StaticMessageInfo.GetInstance().CmdList.Add(cmd);
         //           List<CommandInfo> tmp = StaticMessageInfo.GetInstance().CmdList;
         //           StaticMessageInfo.GetInstance().CmdList = tmp;
         //           StaticTreeState.MessageInfo = LoadingState.LOADCOMPLETE;
         //           break;
         //       }
         //   }

        private static StaticMessageInfo instance = null;
        public static StaticMessageInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticMessageInfo();
            }
            return instance;
        }

        //发出短语
        private List<MessageInfo> sendmessage;

        public List<MessageInfo> SendMessageList
        {
            get { return sendmessage; }
            set
            {
                sendmessage = value;
                this.RaisePropertyChanged("SendMessageList");
            }
        }
        //收到的短语
        private List<MessageInfo> receivedmessage;

        public List<MessageInfo> ReceivedMessageList
        {
            get { return receivedmessage; }
            set
            {
                receivedmessage = value;
                this.RaisePropertyChanged("ReceivedMessageList");
            }
        }
        //指令列表
        private List<CommandInfo> commandlist;

        public List<CommandInfo> CmdList
        {
            get { return commandlist; }
            set
            {
                commandlist = value;
                this.RaisePropertyChanged("CmdList");
            }
        }
        
    }
}
