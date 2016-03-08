using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Windows.Controls;

namespace VehicleGPS.Services.MonitorCentre.ImageCheck
{
    class ImageCheckMapService
    {
        public WebBrowser WebMap;
        public ImageCheckMapService(WebBrowser webMap)
        {
            this.WebMap = webMap;
            this.InitBaiduMap();
        }
        public void InitBaiduMap()
        {
            Uri uri = new Uri(VehicleConfig.GetInstance().siteMapPath, UriKind.RelativeOrAbsolute);
            this.WebMap.Navigate(uri);
        }

        public void ShowInMap(string lng, string lat, string addr, string vehicleid, string sim, string recordtime)
        {
            this.WebMap.InvokeScript("showInMap", new object[] { lng, lat, addr, vehicleid, sim, recordtime });
        }

        public void removeAllMacker() {
            this.WebMap.InvokeScript("RemoveAllOverlays");
        }
    }
}
