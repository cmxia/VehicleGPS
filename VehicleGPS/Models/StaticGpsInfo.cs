using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Services;
using System.Windows.Threading;
using System.Timers;
using VehicleGPS.zmq;

namespace VehicleGPS.Models
{
    /*定时刷新gps数据*/
    class StaticGpsInfo
    {
        private static StaticGpsInfo instance = null;
        private StaticGpsInfo()
        {
            //this.webService = new BusinessDataServiceWEB();
            //this.dispatcherTimer = new Timer();
            //this.InitDispatchTimer();
            ClientObserver client = new ClientObserver();
        }
        public static StaticGpsInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticGpsInfo();
            }
            return instance;
        }
        /*定时刷新最新GPS信息*/
        public Timer dispatcherTimer;
        private BusinessDataServiceWEB webService;
        private void InitDispatchTimer()
        {
            this.dispatcherTimer.Interval = VehicleConfig.GetInstance().GETLATESTGPSINFOINTERVAL*1000;
            this.dispatcherTimer.Elapsed += new ElapsedEventHandler(dispatcherTimer_Tick);
            this.dispatcherTimer.Start();
        }
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
           // this.webService.TimerTick(sender, e);
        }
    }
}
