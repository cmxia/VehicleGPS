using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using VehicleGPS.Models.Login;
using System.Xml;
using System.IO;
using System.Data.Objects;
using VehicleGPS.Services;
using System.Windows.Threading;
using System.Timers;

namespace VehicleGPS.Models
{
    /*车辆或者用户的详细信息*/
    /// <summary>
    /// 车辆或用户的详细信息
    /// </summary>
    class StaticDetailInfo
    {
        private static StaticDetailInfo instance = null;
        private StaticDetailInfo()
        {
            this.ListVehicleDetailInfo = new List<CVDetailInfo>();
            this.ListClientDetailInfo = new List<CVDetailInfo>();
            this.ListVehicleWarnDetailInfo = new List<GPSWarnInfo>();
            if (this.webService == null)
            {
                this.webService = new BusinessDataServiceWEB();
            }
            //this.gpsTimer = new Timer();
            //InitDispatchTimer();
        }
        public static StaticDetailInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticDetailInfo();
            }
            return instance;
        }
        /*定时刷新最新GPS信息*/
        //public Timer gpsTimer;
        private BusinessDataServiceWEB webService = null;
        //private void InitDispatchTimer()
        //{
        //    this.gpsTimer.Interval = VehicleConfig.GetInstance().GETLATESTGPSINFOINTERVAL * 1000;
        //    this.gpsTimer.Enabled = true;
        //    this.gpsTimer.Elapsed += new ElapsedEventHandler(gpsTimer_Elapsed);
        //    this.gpsTimer.Start();
        //    //dispatchTimer.Interval = TimeSpan.FromSeconds(VehicleConfig.GetInstance().GETLATESTGPSINFOINTERVAL);
        //    //dispatchTimer.Tick += new EventHandler(this.webService.TimerTick);
        //    //dispatchTimer.Start();
        //}

        //void gpsTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    StaticTreeState.IsRefreshRealTimeData = true;
        //    this.webService.TimerTick(sender, e);
        //}

        public List<CVDetailInfo> ListVehicleDetailInfo;
        public List<CVDetailInfo> ListClientDetailInfo;
        public List<GPSWarnInfo> ListVehicleWarnDetailInfo;

        /// <summary>
        /// 获取GPS数据
        /// </summary>
        public void RefreshGPSInfo()
        {
            this.webService.GetLatestVehicleGPSInfoThread();
        }
        /// <summary>
        /// 客户端登陆心跳
        /// </summary>
        public void ClientLogin()
        {
            this.webService.ClientLogin();
        }
    }
}
