using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Navigation;
using VehicleGPS.Models;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Windows;

namespace VehicleGPS.Services.MonitorCentre.RealTimeMonitor
{
    class RealTimeMapService
    {
        public WebBrowser WebMap;
        private bool isInitOk = false;
        public RealTimeMapService(WebBrowser webMap)
        {
            this.WebMap = webMap;
            this.InitBaiduMap();
        }
        public void InitBaiduMap()
        {
            isInitOk = false;
            //string currentUri = Environment.CurrentDirectory;
            //Uri uri = new Uri(currentUri + "/../../Views/Control/MonitorCentre/RealTimeMonitor/Map/RealTimeMap.htm", UriKind.RelativeOrAbsolute);
            Uri uri = new Uri(VehicleConfig.GetInstance().realTimeMapPath, UriKind.RelativeOrAbsolute);
            this.WebMap.Navigate(uri);
            this.WebMap.LoadCompleted += new LoadCompletedEventHandler(WebMap_LoadCompleted);
        }
        /*初始化区域信息*/
        private void WebMap_LoadCompleted(object sender, NavigationEventArgs e)
        {
            RealTimeViewModel.GetInstance().InitRegionInBaidu();
            StaticTreeState.RealTimeMapViewContruct = true;

        }

        /*初始化实时地图*/
        public void InitRealTimeMap()
        {
            this.WebMap.InvokeScript("InitRealTimeMap");
            isInitOk = true;
        }
        /*添加标注
         * <param name="lng">经度</param>
         * <param name="lat">纬度</param>
         * <param name="cont">内容</param>
         */
        public void SetMarker(string name, double lng, double lat, string cont, string icon)
        {
            this.WebMap.InvokeScript("SetMarker", new object[] { name, lng, lat, cont, icon });
        }
        /*设置点聚合*/
        public void SetMarkerCluster()
        {
            this.WebMap.InvokeScript("SetMarkerCluster");
        }
        public void AddMarkers()
        {
            this.WebMap.InvokeScript("AddMarkers");
        }
        /*获得一个marker的焦点*/
        public void FocusMarker(double lng, double lat)
        {
            this.WebMap.InvokeScript("FocusMarker", new object[] { lng, lat });
        }
        /*获得一个marker的焦点*/
        public void FocusMarker(string name)
        {
            this.WebMap.InvokeScript("FocusMarker", new object[] { name });
        }
        /*获得一个marker的焦点*/
        public void ModMarker(string name, double lng, double lat, string cont, string icon)
        {
            this.WebMap.InvokeScript("ModMarker", new object[] { name, lng, lat, cont, icon });
        }
        /*地理解析*/
        public void ParseAddress(string lngs, string lats)
        {
            this.WebMap.InvokeScript("ParseAddress", new object[] { lngs, lats });
        }

        public void SearchByKeyWord(string keyword)
        {
            this.WebMap.InvokeScript("SearchByKeyWord", keyword);
        }

        /*划区域*/
        public void addRegionByOne(string name, string lng, string lat, string radius, string color)
        {
            this.WebMap.InvokeScript("addCircleByOne", new object[] { lng, lat, radius, color, name });
        }
        /*打开区域测量*/
        public void OpenDrawingManager()
        {
            this.WebMap.InvokeScript("OpenDrawingManager");
        }
        /*关闭区域测量*/
        public void CloseDrawingManager()
        {
            this.WebMap.InvokeScript("CloseDrawingManager");
        }
        /*添加点击监听事件*/
        public void AddCilckListener()
        {
            this.WebMap.InvokeScript("addCircleListener");
        }
        /*获取区域信息*/
        public string GetCircleInfo()
        {
            return this.WebMap.InvokeScript("returnLngLatRadius").ToString();
        }
        /*移除地图所有覆盖物*/
        public void RemoveAllMarkers()
        {
            this.WebMap.InvokeScript("RemoveAllOverlays");
        }
        /*移除地图监听*/
        public void RemoveClickListener()
        {
            this.WebMap.InvokeScript("removeclicklistener");
        }
        public void RemoveCircleByAdd()
        {
            this.WebMap.InvokeScript("removeCircleByAdd");
        }
    }
}
