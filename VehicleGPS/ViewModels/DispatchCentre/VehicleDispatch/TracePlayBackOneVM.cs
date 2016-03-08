using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Views.Control.MonitorCentre.TrackPlayBack;
using VehicleGPS.Models;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class TracePlayBackOneVM : NotificationObject
    {
        public TracePlayBackOneVM()
        {
            this.PlayBackCommand = new DelegateCommand(this.PlayBackCommandExecute);
        }
        public string tripTime { get; set; }//趟次
        public string transCount { get; set; }//运输方量
        public string vehiclenum { get; set; }//车辆编号 
        public string SIM { get; set; }//sim卡号
        private string vehicleid;

        public string vehicleID
        {
            get { return vehicleid; }
            set
            {
                vehicleid = value;
                this.RaisePropertyChanged("vehicleID");
            }
        }

        private string drivername;

        public string driverName
        {
            get { return drivername; }
            set
            {
                drivername = value;
                this.RaisePropertyChanged("driverName");
            }
        }
        private string assigntime;

        public string AssignTime
        {
            get { return assigntime; }
            set
            {
                assigntime = value;
                this.RaisePropertyChanged("AssignTime");
            }
        }
        private string finishtime;

        public string FinishTime
        {
            get { return finishtime; }
            set
            {
                finishtime = value;
                this.RaisePropertyChanged("FinishTime");
            }
        }
        public DelegateCommand PlayBackCommand { get; set; }
        void PlayBackCommandExecute()
        {
            DateTime starttime = DateTime.Parse(this.AssignTime);
            DateTime endtime = string.IsNullOrEmpty(this.FinishTime) ? DateTime.Now : DateTime.Parse(this.FinishTime);
            CVBasicInfo basic = new CVBasicInfo();
            basic.SIM = this.SIM;
            basic.ID = this.vehiclenum;
            basic.Name = this.vehicleID;
            TrackPlayBack win = new TrackPlayBack(basic, starttime, endtime);
            win.ShowDialog();
        }
    }
}
