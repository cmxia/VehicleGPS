using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VehicleGPS.Models;
using System.Windows.Threading;
using System.Windows;
using VehicleGPS.ViewModels.MonitorCentre.AbnormalStatictic;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Services.MonitorCentre.AbnormalStatictic
{
    class AbnormalStatictcMapService
    {
         public WebBrowser WebMap;
         public AbnormalStatictcMapService(WebBrowser webMap)
        {
            this.WebMap = webMap;
            this.InitBaiduMap();
        }
        public void InitBaiduMap()
        {
            //string currentUri = Environment.CurrentDirectory;
            //Uri uri = new Uri(currentUri + "/../../Views/Control/MonitorCentre/RealTimeMonitor/Map/RealTimeMap.htm", UriKind.RelativeOrAbsolute);
            Uri uri = new Uri(VehicleConfig.GetInstance().realTimeMapPath, UriKind.RelativeOrAbsolute);
            //MessageBox.Show(VehicleConfig.GetInstance().realTimeMapPath);
            this.WebMap.Navigate(uri);
            //this.WebMap.ObjectForScripting = new JsCommunicate();
        }

        /*初始化实时地图*/
        public void InitRealTimeMap()
        {
                this.WebMap.InvokeScript("InitRealTimeMap");
        }
        /*添加标注
         * <param name="lng">经度</param>
         * <param name="lat">纬度</param>
         * <param name="cont">内容</param>
         */
        public void SetMarker(string name,double lng,double lat,string cont,string icon)
        {
            this.WebMap.InvokeScript("SetMarker", new object[] {name, lng, lat, cont, icon });
        }
        /*设置点聚合*/
        public void SetMarkerCluster()
        {
            this.WebMap.InvokeScript("SetMarkerCluster");
        }
        
        /*获得一个marker的焦点*/
        public void FocusMarker(double lng, double lat)
        {
            this.WebMap.InvokeScript("FocusMarker", new object[] { lng, lat });
        }
        /*地理解析*/
        public void ParseAddress(string lngs, string lats)
        {
            this.WebMap.InvokeScript("ParseAddress", new object[] { lngs, lats });
        }
        /*划区域*/
        public void addRegionByOne(string name,string lng, string lat, string radius, string color)
        {
            this.WebMap.InvokeScript("addCircleByOne",new object[] {lng, lat, radius, color, name});
        }
    }
}
