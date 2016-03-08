using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models.MonitorCentre
{
    public class PointOfLine : NotificationObject
    {
        private string lng;

        public string Lng
        {
            get { return lng; }
            set
            {
                lng = value;
                this.RaisePropertyChanged("Lng");
            }
        }
        private string lat;

        public string Lat
        {
            get { return lat; }
            set
            {
                lat = value;
                this.RaisePropertyChanged("Lat");
            }
        }

        private string inflectionid;

        public string InflectinID
        {
            get { return inflectionid; }
            set
            {
                inflectionid = value;
                this.RaisePropertyChanged("InflectinID");
            }
        }

        private string roadid;
        public string RoadID
        {
            get { return roadid; }
            set
            {
                roadid = value;
                this.RaisePropertyChanged("RoadID");
            }
        }

        private string roadwidth;
        public string RoadWidth
        {
            get { return roadwidth; }
            set
            {
                roadwidth = value;
                this.RaisePropertyChanged("RoadWidth");
            }
        }

        /*行驶时间是否被选中*/
        private bool isSelectedTime;
        public bool IsSelectedTime
        {
            get { return isSelectedTime; }
            set
            {
                isSelectedTime = value;
                if (IsSelectedTime)
                {
                    TimeEnable = true;
                }
                else
                {
                    TimeEnable = false;
                }
                this.RaisePropertyChanged("IsSelectedTime");
            }
        }

        private bool timeenable = false;

        public bool TimeEnable
        {
            get { return timeenable; }
            set
            {
                timeenable = value;
                this.RaisePropertyChanged("TimeEnable");
            }
        }


        /*限速是否被选中*/
        private bool isSelectedSpeed;
        public bool IsSelectedSpeed
        {
            get { return isSelectedSpeed; }
            set
            {
                isSelectedSpeed = value;
                if (IsSelectedSpeed)
                {
                    SpeedEnable = true;
                }
                else
                {
                    SpeedEnable = false;
                }
                this.RaisePropertyChanged("IsSelectedSpeed");
            }
        }

        private bool speedenable = false;

        public bool SpeedEnable
        {
            get { return speedenable; }
            set
            {
                speedenable = value;
                this.RaisePropertyChanged("SpeedEnable");
            }
        }

        private List<string> nslatitude = new List<string>() { "北纬", "南纬" };
        public List<string> NSLatitude
        {
            get { return nslatitude; }
            set
            {
                nslatitude = value;
                this.RaisePropertyChanged("NSLatitude");
            }
        }
        private int nslatselectedindex;

        public int NSLatSelectedIndex
        {
            get { return nslatselectedindex; }
            set
            {
                nslatselectedindex = value;
                this.RaisePropertyChanged("NSLatSelectedIndex");
            }
        }
        private List<string> nslongitude = new List<string>() { "东经", "西经" };
        public List<string> NSLongitude
        {
            get { return nslongitude; }
            set
            {
                nslongitude = value;
                this.RaisePropertyChanged("NSLongitude");
            }
        }
        private int nslngselectedindex;

        public int NSLngSelectedIndex
        {
            get { return nslngselectedindex; }
            set
            {
                nslngselectedindex = value;
                this.RaisePropertyChanged("NSLngSelectedIndex");
            }
        }
        private string longlimit;
        public string LongLimit
        {
            get { return longlimit; }
            set
            {
                longlimit = value;
                this.RaisePropertyChanged("LongLimit");
            }
        }

        private string lesslimit;
        public string LessLimit
        {
            get { return lesslimit; }
            set
            {
                lesslimit = value;
                this.RaisePropertyChanged("LessLimit");
            }
        }

        private string highspeed;
        public string HighSpeed
        {
            get { return highspeed; }
            set
            {
                highspeed = value;
                this.RaisePropertyChanged("HighSpeed");
            }
        }

        private string hightime;
        public string HighTime
        {
            get { return hightime; }
            set
            {
                hightime = value;
                this.RaisePropertyChanged("HighTime");
            }
        }
    }
}
