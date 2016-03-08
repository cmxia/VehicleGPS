using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Navigation;
using VehicleGPS.Models;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Windows;

namespace VehicleGPS.Services.DispatchCentre.SiteManage
{
    class SiteMapService
    {
        public WebBrowser WebMap;
        public SiteMapService(WebBrowser webMap)
        {
            this.WebMap = webMap;
            this.InitBaiduMap();
        }
        public void InitBaiduMap()
        {
           
            Uri uri = new Uri(VehicleConfig.GetInstance().siteMapPath, UriKind.RelativeOrAbsolute);
            this.WebMap.Navigate(uri);
        }
        /*初始化实时地图*/
        public void InitSiteMap()
        {
            this.WebMap.InvokeScript("InitSiteMap");
        }
        /*划区域*/
        public void addRegionByOne(string name, string lng, string lat, string radius)
        {
            this.WebMap.InvokeScript("addCircleByOne", new object[] { lng, lat, radius, "#f00", name });
        }
        /*添加点击监听事件*/
        public void addClickListener()
        {
            this.WebMap.InvokeScript("addCircleListener");
        }
        /*获取区域的地理信息*/
        public string GetRegionGeoInfo()
        {
            return this.WebMap.InvokeScript("returnLngLatRadius").ToString();
        }
        /*清楚地图上的所有覆盖物*/
        public void RemoveAllMarkers() {
            this.WebMap.InvokeScript("RemoveAllOverlays");
        }
    }
}
