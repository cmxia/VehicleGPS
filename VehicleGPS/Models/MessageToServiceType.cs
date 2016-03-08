using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models
{
    class MessageToServiceType
    {
        private string gType;//包头

        public string GType
        {
            get { return gType; }
            set { gType = value; }
        }
        private string gpsBasic;//包体

        public string GpsBasic
        {
            get { return gpsBasic; }
            set { gpsBasic = value; }
        }
        private string gpsAttatch;//扩展位

        public string GpsAttatch
        {
            get { return gpsAttatch; }
            set { gpsAttatch = value; }
        }
    }
}
