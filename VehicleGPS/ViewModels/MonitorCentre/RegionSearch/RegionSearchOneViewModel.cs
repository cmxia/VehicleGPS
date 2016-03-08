using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models;

namespace VehicleGPS.ViewModels.MonitorCentre.RegionSearch
{
    class RegionSearchOneViewModel : NotificationObject
    {
        public RegionSearchOneViewModel()
        {
            this.TracePlayBackCommand = new DelegateCommand(new Action(this.TracePlayBackCommandExecute));
        }
        public string Sequence { get; set; }
        public string VehicleId { get; set; }
        public string SIM { get; set; }
        public string vehicleNum { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public DelegateCommand TracePlayBackCommand { get; set; }
        private void TracePlayBackCommandExecute()
        {
            CVBasicInfo basic = new CVBasicInfo();
            basic.SIM = this.SIM;
            basic.ID = this.vehicleNum;
            basic.Name = this.VehicleId;
            VehicleGPS.Views.Control.MonitorCentre.TrackPlayBack.TrackPlayBack win = new VehicleGPS.Views.Control.MonitorCentre.TrackPlayBack.TrackPlayBack(basic, starttime, endtime);
            win.ShowDialog();
        }
    }
}
