using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models
{
    class GPSStopInfo:NotificationObject
    {
        public int Sequence { get; set; }//序号
        public string StartTime { get; set; }//开始时间
        public string EndTime { get; set; }//结束时间
        public string LastTime { get; set; }//持续时间
        //停车地址
        private string address;

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                this.RaisePropertyChanged("Address");
            }
        }

        public string lng { get; set; }
        public string lat { get; set; }
    }
}
