using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VehicleGPS.Models;

namespace VehicleGPS.Services.MonitorCentre.VehicleTrack
{
    class VehicleTrackMapService
    {
        private string ImageDir = Environment.CurrentDirectory + "/../../Images/TrackPlay/";
        public WebBrowser WebMap { get; set; }
        public VehicleTrackMapService(WebBrowser webMap)
        {
            this.WebMap = webMap;
            this.InitBaiduMap();
        }
        public void InitBaiduMap()
        {
            string currentUri = Environment.CurrentDirectory;
            //Uri uri = new Uri(currentUri + "/../../Views/Control/MonitorCentre/VehicleTrack/Map/VehicleTrackMap.htm", UriKind.RelativeOrAbsolute);
            Uri uri = new Uri(VehicleConfig.GetInstance().vehicleTrackMapPath, UriKind.RelativeOrAbsolute);
            this.WebMap.Navigate(uri);
        }
        public void SetMarker(string lng, string lat, string cont, string icon)
        {
            this.WebMap.InvokeScript("SetMarker", new object[] { lng, lat, cont, icon });
        }
        public void FocusMarker(string lng, string lat)
        {
            this.WebMap.InvokeScript("FocusMarker", new object[] { lng, lat });
        }
        public void RemoveAllOverlay()
        {
            this.WebMap.InvokeScript("RemoveAllOverlays");
        }
    }
}
