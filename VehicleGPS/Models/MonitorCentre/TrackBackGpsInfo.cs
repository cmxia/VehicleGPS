using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models.MonitorCentre
{
    class TrackBackGpsInfo : NotificationObject
    {
        public GPSInfo GpsInfo { get; set; }
        private string currentLocation;
        public string CurrentLocation
        {
            get { return currentLocation; }
            set
            {
                if (currentLocation != value)
                {
                    currentLocation = value;
                    this.RaisePropertyChanged("CurrentLocation");
                }
            }
        }
        private string vehicleid;

        public string VehicleId
        {
            get { return vehicleid; }
            set
            {
                vehicleid = value;
                this.RaisePropertyChanged("VehicleId");
            }
        }
        private string finnerid;

        public string FInnerId
        {
            get { return finnerid; }
            set
            {
                finnerid = value;
                this.RaisePropertyChanged("FInnerId");
            }
        }
        private string customername;

        public string CustomerName
        {
            get { return customername; }
            set
            {
                customername = value;
                this.RaisePropertyChanged("CustomerName");
            }
        }
        private string vehicletypename;

        public string VehicleTypeName
        {
            get { return vehicletypename; }
            set
            {
                vehicletypename = value;
                this.RaisePropertyChanged("VehicleTypeName");
            }
        }
        private string sim;

        public string SIM
        {
            get { return sim; }
            set
            {
                sim = value;
                this.RaisePropertyChanged("SIM");
            }
        }
        private int sequence;

        public int Sequence
        {
            get { return sequence; }
            set
            {
                sequence = value;
                this.RaisePropertyChanged("Sequence");
            }
        }

    }
}
