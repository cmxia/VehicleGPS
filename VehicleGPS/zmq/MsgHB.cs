using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.zmq
{
    class MsgHB
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

    public class HeartBody
    {
        private string userid;

        public string Userid
        {
            get { return userid; }
            set { userid = value; }
        }
        private string guid;

        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        private string loginType;

        public string LoginType
        {
            get { return loginType; }
            set { loginType = value; }
        }


    }
}
