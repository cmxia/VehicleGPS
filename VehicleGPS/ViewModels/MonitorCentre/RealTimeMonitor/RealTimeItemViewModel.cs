
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows;

namespace VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor
{
    class RealTimeItemViewModel : NotificationObject
    {

        public RealTimeItemViewModel()
        {
        }
        public CVDetailInfo VehicleInfo { get; set; }
        /*当前位置*/
        public string currentAddress;
        public string CurrentAddress
        {
            get { return currentAddress; }
            set
            {
                if (currentAddress != value)
                {
                    currentAddress = value;
                    this.RaisePropertyChanged("CurrentAddress");
                }
            }
        }
    }
}

