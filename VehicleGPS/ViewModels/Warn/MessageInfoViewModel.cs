using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Views;
using VehicleGPS.Models;
using System.Timers;

namespace VehicleGPS.ViewModels.Warn
{
    class MessageInfoViewModel : NotificationObject
    {
        MainView ParentWin = null;
        public MessageInfoViewModel(object parentWin)
        {
            ParentWin = (MainView)parentWin;
            this.MessageList = new List<WarnInfo>();
            this.MessageList = StaticWarnInfo.GetInstance().MessageList;
            //Timer timer = new Timer();
            //timer.Elapsed += new ElapsedEventHandler(Clearmessage);
            //timer.Interval = 30000;
            //timer.Enabled = true;
            //timer.AutoReset = true;
            //timer.Start();
        }
        private void Clearmessage(object sender, EventArgs e)
        {
            StaticTreeState.MessageInfo = LoadingState.NOLOADING;
            this.MessageList.Clear();
            StaticTreeState.MessageInfo = LoadingState.LOADCOMPLETE;
        }
        private static MessageInfoViewModel instance = null;

        public static MessageInfoViewModel GetInstance(object parentWin = null)
        {
            if (instance == null)
            {
                instance = new MessageInfoViewModel(parentWin);
            }
            return instance;
        }
        private List<WarnInfo> messagelist;

        public List<WarnInfo> MessageList
        {
            get { return messagelist; }
            set
            {
                messagelist = value;
                this.RaisePropertyChanged("MessageList");
            }
        }

    }
}
