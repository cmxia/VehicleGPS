using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VehicleGPS.Models;
using System.Windows.Threading;
using System.Windows;
using VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Services.MonitorCentre.TrackPlayBack
{
    class TrackPlayMapService
    {
        public WebBrowser WebMap { get; set; }
        public TrackPlayMapService(WebBrowser webMap)
        {
            this.WebMap = webMap;
            this.InitBaiduMap();
            this.InitPlayTimer();
        }
        public void InitBaiduMap()
        {
            string currentUri = Environment.CurrentDirectory;
            //Uri uri = new Uri(currentUri + "/../../Views/Control/MonitorCentre/TrackPlayBack/Map/TrackPlayMap.htm", UriKind.RelativeOrAbsolute);
            Uri uri = new Uri(VehicleConfig.GetInstance().trackPlayMapPath, UriKind.RelativeOrAbsolute);
            this.WebMap.Navigate(uri);
        }
        /*初始化地图，清空*/
        public void InitTrackPlayMap()
        {
            this.WebMap.InvokeScript("InitTrackPlayMap");
        }
        /*初始化数据*/
        TrackPlayViewModel trackPlayVM;
        private string[] lngs;//经度
        private string[] lats;//纬度
        private string[] conts;//内容
        private string[] staticIcons;//静态的方向图标
        private string[] moveIcons;//移动时的方向图标
        private int playCounter;//播放计数
        private int playSpeed;//播放速度
        public void InitTrackPlayData(TrackPlayViewModel trackPlayVM)
        {
            if (trackPlayVM.ListVehicleInfo != null && trackPlayVM.ListVehicleInfo.Count != 0)
            {
                this.trackPlayVM = trackPlayVM;
                List<TrackBackGpsInfo> listVehicleInfo = trackPlayVM.ListVehicleInfo;
                string lngstr = listVehicleInfo[0].GpsInfo.Longitude;
                string latstr = listVehicleInfo[0].GpsInfo.Latitude;
                string staticIconstr = VehicleCommon.GetStaticDirectionImageUrl(listVehicleInfo[0].GpsInfo.Direction);
                string moveIconstr = VehicleCommon.GetDirectionImageUrl(listVehicleInfo[0].GpsInfo.Direction,VehicleCommon.VSOnlineRun);

                string vehicleid = trackPlayVM.SelectedVehicle.Name;
                string sim = trackPlayVM.SelectedVehicle.SIM;
                string uploadTime = listVehicleInfo[0].GpsInfo.Datetime;
                string speed = listVehicleInfo[0].GpsInfo.Speed;
                string direction = listVehicleInfo[0].GpsInfo.Direction;
                string curlocation = listVehicleInfo[0].GpsInfo.CurLocation;
                string contstr = VehicleCommon.GetHtml(vehicleid, sim, uploadTime, speed, direction, curlocation);
                for (int i = 1; i < listVehicleInfo.Count; i++)
                {
                    lngstr += "$" + listVehicleInfo[i].GpsInfo.Longitude;
                    latstr += "$" + listVehicleInfo[i].GpsInfo.Latitude;
                    staticIconstr += "$" + VehicleCommon.GetStaticDirectionImageUrl(listVehicleInfo[i].GpsInfo.Direction);
                    moveIconstr += "$" + VehicleCommon.GetDirectionImageUrl(listVehicleInfo[i].GpsInfo.Direction,VehicleCommon.VSOnlineRun);

                    uploadTime = listVehicleInfo[i].GpsInfo.Datetime;
                    speed = listVehicleInfo[i].GpsInfo.Speed;
                    direction = listVehicleInfo[i].GpsInfo.Direction;
                    curlocation = listVehicleInfo[i].GpsInfo.CurLocation;
                    contstr += "$" + VehicleCommon.GetHtml(vehicleid, sim, uploadTime, speed, direction, curlocation);
                }
                this.lngs = lngstr.Split('$');
                this.lats = latstr.Split('$');
                this.conts = contstr.Split('$');
                this.staticIcons = staticIconstr.Split('$');
                this.moveIcons = moveIconstr.Split('$');
                this.playCounter = 0;

                this.WebMap.InvokeScript("InitTrackPlayData", new object[] { lngstr, latstr, contstr, staticIconstr });
                /*起始点*/
                this.WebMap.InvokeScript("SetMoveMarker", new object[] { this.lngs[0], this.lats[0], this.conts[0], this.moveIcons[0] });
            }
        }
        /*定时播放*/
        public DispatcherTimer playTimer = null;
        private void InitPlayTimer()
        {
            this.playTimer = new DispatcherTimer();
            this.playTimer.Interval = TimeSpan.FromMilliseconds(this.playSpeed);
            this.playTimer.Tick += new EventHandler(SetMoveMarker);
        }
        /// <summary>
        /// 设置播放速度
        /// </summary>
        /// <param name="speed"></param>
        public void SetPlaySpeed(int speed)
        {
            this.playSpeed = (11 - speed) * 150;//一个刻度表示播放速度0.15秒播放一次，总共10个刻度
            this.playTimer.Interval = TimeSpan.FromMilliseconds(this.playSpeed);
        }
        private void SetMoveMarker(object sender, EventArgs e)
        {
            if (this.playCounter <= this.lngs.Length - 1)
            {
                this.WebMap.InvokeScript("SetMoveMarker", new object[] { this.lngs[this.playCounter], this.lats[this.playCounter], this.conts[this.playCounter], this.moveIcons[this.playCounter] });
                this.trackPlayVM.SelectedInfoIndex = playCounter;
                this.playCounter++;
            }
            else
            {
                MessageBox.Show("轨迹播放结束", "轨迹回放", MessageBoxButton.OKCancel);
                StopTrackPlay();
            }
        }
        public void SetMarker(string lng, string lat, string cont, string icon)
        {
            this.WebMap.InvokeScript("SetMoveMarkerAndShowInfoWindow", new object[] { lng, lat, cont, icon });
        }
        public void StartTrackPlay()
        {
            this.playTimer.Start();
        }
        public void PauseTrackPlay()
        {
            this.playTimer.Stop();
        }
        public void StopTrackPlay()
        {
            this.trackPlayVM.StartEnable = true;
            this.trackPlayVM.PauseEnable = false;
            this.trackPlayVM.StopEnable = false;

            this.playTimer.Stop();
            this.playCounter = 0;
            /*回到起点*/
            this.WebMap.InvokeScript("SetMoveMarker", new object[] { this.lngs[0], this.lats[0], this.conts[0], this.moveIcons[0] });
        }
    }
}
