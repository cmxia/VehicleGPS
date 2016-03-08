using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using System.Windows;
using Newtonsoft.Json;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class TimeRemindSettingViewModel : NotificationObject
    {
        private VehicleDispatchDataOperate DataOperate { get; set; }
        public TimeRemindSettingViewModel(string taskId)
        {
            this.ConditionChangedCommand = new DelegateCommand<object>(new Action<object>(this.ConditionChangedCommandExecute));
            this.CloseCommand = new DelegateCommand<Window>(new Action<Window>(this.CloseCommandExecute));
            this.ConfirmCommand = new DelegateCommand<Window>(new Action<Window>(this.ConfirmCommandExecute));
            this.DataOperate = new VehicleDispatchDataOperate();
            this.ListSiteInfo = this.DataOperate.GetSiteInfo();
            this.taskListId = taskId;
        }
        private List<SiteInfo> listSiteInfo = new List<SiteInfo>();
        public List<SiteInfo> ListSiteInfo
        {
            get { return listSiteInfo; }
            set
            {
                if (listSiteInfo != value)
                {
                    listSiteInfo = value;
                    this.RaisePropertyChanged("ListSiteInfo");
                }
            }
        }

        private SiteInfo SelectedSiteInfo { get; set; }

        private string transTime;
        public string TransTime//运输时间
        {
            get { return transTime; }
            set
            {
                if (transTime != value)
                {
                    transTime = value;
                    this.RaisePropertyChanged("TransTime");
                }
            }
        }

        private string offTime;
        public string OffTime//待卸时间
        {
            get { return offTime; }
            set
            {
                if (offTime != value)
                {
                    offTime = value;
                    this.RaisePropertyChanged("OffTime");
                }
            }
        }

        private DateTime startFdate = new DateTime();
        public DateTime StartFdate//开盘时间
        {
            get { return startFdate; }
            set
            {
                if (startFdate != value)
                {
                    startFdate = value;
                    this.RaisePropertyChanged("StartFdate");
                }
            }
        }

        private bool isTransAlarm;
        public bool IsTransAlarm//是否开启运输时间报警
        {
            get { return isTransAlarm; }
            set
            {
                if (isTransAlarm != value)
                {
                    isTransAlarm = value;
                    this.RaisePropertyChanged("IsTransAlarm");
                }
            }
        }

        private bool isOffAlarm;
        public bool IsOffAlarm//是否卸载时间报警
        {
            get { return isOffAlarm; }
            set
            {
                if (isOffAlarm != value)
                {
                    isOffAlarm = value;
                    this.RaisePropertyChanged("IsOffAlarm");
                }
            }
        }

        private bool isStartFAlarm;
        public bool IsStartFAlarm//是否开启开盘时间报警
        {
            get { return isStartFAlarm; }
            set
            {
                if (isStartFAlarm != value)
                {
                    isStartFAlarm = value;
                    this.RaisePropertyChanged("IsStartFAlarm");
                }
            }
        }

        public DelegateCommand<object> ConditionChangedCommand { get; set; }
        private void ConditionChangedCommandExecute(object selectedItem)
        {
            if (selectedItem != null)
            {
                SiteInfo selectedInfo = ((SiteInfo)selectedItem);
                this.SelectedSiteInfo = selectedInfo;
                if (selectedInfo != null && this.listSiteInfo != null && this.listSiteInfo.Count != 0)
                {
                    foreach (SiteInfo info in this.listSiteInfo)
                    {
                        if (selectedInfo.ZoneID == info.ZoneID)
                        {
                            this.OffTime = info.OffTime;
                            if (info.StartFdate != "")
                            {
                                this.StartFdate = Convert.ToDateTime(info.StartFdate.Replace('-', ':'));
                            }
                            this.TransTime = info.TransTime;
                            this.IsOffAlarm = info.IsOffAlarm == "true" ? true : false;
                            this.IsStartFAlarm = info.IsStartFAlarm == "true" ? true : false;
                            this.IsTransAlarm = info.IsTransAlarm == "true" ? true : false;
                            break;
                        }
                    }
                }
            }
        }
        public DelegateCommand<Window> CloseCommand { get; set; }
        private void CloseCommandExecute(Window win)
        {
            win.Close();
        }

        private string taskListId = null;
        public DelegateCommand<Window> ConfirmCommand { get; set; }
        private void ConfirmCommandExecute(Window win)
        {
            //if (this.DataOperate.UpdateTimeRemindSetting(this.taskListId , this.transTime, this.isTransAlarm
            //    , this.offTime, this.isOffAlarm, this.startFdate.ToShortTimeString(), this.isStartFAlarm))
            //{
            //    MessageBox.Show("设置时间提醒成功", "时间设置", MessageBoxButton.OK);
            //}
            //else
            //{
            //    MessageBox.Show("设置时间提醒失败", "时间设置", MessageBoxButton.OK);
            //}

            if (this.IsOffAlarm || this.IsStartFAlarm || this.IsTransAlarm)
            {
                string sql = "update TranTaskList set transGoods='' ";
                Dictionary<string, string> instruction = new Dictionary<string, string>();
                instruction.Add("cmd", "TIMEREMIND_TYPE");
                instruction.Add("cmdid", taskListId + "_TIMEREMIND_TYPE");
                instruction.Add("taskListId", taskListId);
                if (this.IsStartFAlarm)
                {
                    sql += ", StartFdate='" + this.StartFdate.ToString("HH:mm:ss") + "' ";
                    instruction.Add("startTaskTime", this.StartFdate.ToString("HH:mm:ss"));
                }
                else
                {
                    instruction.Add("startTaskTime", "");
                }
                if (this.IsTransAlarm)
                {
                    sql += ", TransTime='" + this.TransTime + "' ";
                    instruction.Add("startTransTime", this.TransTime);
                }
                else
                {
                    instruction.Add("startTransTime", "");
                }
                if (this.IsOffAlarm)
                {
                    sql += ", OffTime='" + this.OffTime + "' ";
                    instruction.Add("startUnloadTime", this.OffTime);
                }
                else
                {
                    instruction.Add("startUnloadTime", "");
                }
                sql += "where taskListId='" + this.taskListId + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (jsonStr == "error")
                {
                    MessageBox.Show("设置失败！");
                }
                else
                {
                    MessageBox.Show("设置成功！");
                    string insstring = JsonConvert.SerializeObject(instruction);
                    zmq.zmqPackHelper.zmqInstructionsPack("", insstring);
                }
            }
            win.Close();
        }
    }
}
