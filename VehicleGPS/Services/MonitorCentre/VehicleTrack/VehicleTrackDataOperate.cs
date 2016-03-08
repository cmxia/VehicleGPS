using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using VehicleGPS.Models;
using VehicleGPS.ViewModels.MonitorCentre.VehicleTrack;
using ZeroMQ;
using Newtonsoft.Json.Linq;

namespace VehicleGPS.Services.MonitorCentre.VehicleTrack
{
    class VehicleTrackDataOperate
    {
       
        
        private BusinessDataServiceWEB serviceWeb;
        private VehicleTrackViewModel vehicleTrackViewModel;
        public VehicleTrackDataOperate(VehicleTrackViewModel vehicleTrackViewModel)
        {
            this.vehicleTrackViewModel = vehicleTrackViewModel;
            this.serviceWeb = new BusinessDataServiceWEB();
        }

        /*获取历史轨迹信息*/
        public void GetVehicleTrackInfo()
        {
            serviceWeb.GetLatestVehicleGPSInfoBySimThread(vehicleTrackViewModel);
        }
        /*定时刷新最新GPS信息*/
        public DispatcherTimer dispatchTimer;
        public void InitDispatchTimer()
        {
            dispatchTimer = new DispatcherTimer();
            dispatchTimer.Interval = TimeSpan.FromSeconds(VehicleConfig.GetInstance().VEHICLETRACKTIMEINTERVAL);//20
            //dispatchTimer.Interval = TimeSpan.FromSeconds(5);
            dispatchTimer.Tick += new EventHandler(this.VehicleTrackTimerTick);
            dispatchTimer.Start();
        }
        public void StopDispatchTimer()
        {
            this.dispatchTimer.Stop();
        }
        /*定时刷新最新GPS信息*/
        public void VehicleTrackTimerTick(object sender, EventArgs e)
        {
            this.serviceWeb.GetLatestVehicleGPSInfoBySimThread(vehicleTrackViewModel);
        }
        /// <summary>
        /// 开始获取实时数据
        /// 2014-06-10
        /// </summary>
        public void VehicleTrackInfoStart()
        {
            this.serviceWeb.GetLatestVehicleGPSInfoBySimThread(vehicleTrackViewModel);
        }
        /// <summary>
        /// 停止获取实时数据
        /// 2014-06-10
        /// </summary>
        public void VehicleTrackInfoStop()
        {
            this.serviceWeb.PushThreadRunning = false;
            System.Threading.Thread.Sleep(1000);
        }
    }
}
