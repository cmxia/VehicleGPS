using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.Login
{
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
