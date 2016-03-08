using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;

namespace VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack
{
    /// <summary>
    /// 轨迹回放播放条目
    /// </summary>
    class TrackPlayItemViewModel : NotificationObject
    {
        public TrackPlayItemViewModel()
        {
        }
        public CVDetailInfo VehicleInfo { get; set; }
        public bool? isSelected;
        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    this.RaisePropertyChanged("IsSelected");
                }
            }
        }
    }
}
