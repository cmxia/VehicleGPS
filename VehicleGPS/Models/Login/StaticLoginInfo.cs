using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Models.Login
{
    /// <summary>
    /// 登陆单例类
    /// </summary>
    class StaticLoginInfo
    {
        private static StaticLoginInfo instance = null;
        private StaticLoginInfo()
        {

        }
        public static StaticLoginInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticLoginInfo();
            }
            return instance;
        }
        public string UserName { get; set; }
        public string Passwd { get; set; }
        public string LoginTime { get; set; }
        
    }
}
