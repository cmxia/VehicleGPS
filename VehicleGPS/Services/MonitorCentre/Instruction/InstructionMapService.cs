using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Navigation;
using VehicleGPS.Models;
using System.Windows;
using VehicleGPS.ViewModels.MonitorCentre.Instruction;

namespace VehicleGPS.Services.MonitorCentre.Instruction
{
    class InstructionMapService
    {
        public WebBrowser WebMap;
        private bool isInitOk = false;
        private InstructionViewModel instance = null;
        public InstructionMapService(InstructionViewModel instanceP)
        {
            instance = instanceP;
            this.WebMap = instanceP.WebMap;
            this.InitBaiduMap();
        }
        public void InitBaiduMap()
        {
            isInitOk = false;
            Uri uri = new Uri(VehicleConfig.GetInstance().instructionMapPath, UriKind.RelativeOrAbsolute);
            this.WebMap.Navigate(uri);
            this.WebMap.LoadCompleted += new LoadCompletedEventHandler(WebMap_LoadCompleted);
        }
        /*初始化区域信息*/
        private void WebMap_LoadCompleted(object sender, NavigationEventArgs e)
        {
            instance.InitRegionInBaidu();
            OpenDrawTool();
        }
        public void OpenDrawTool()
        {
            switch (instance.InsType)
            {
                case "circle":
                    this.OpenDrawCircleManager();
                    break;
                case "line":
                    this.OpenDrawPolyLineManager();
                    break;
                case "rect":
                    this.OpenDrawRectManager();
                    break;
                case "poly":
                    this.OpenDrawPolyGonManager();
                    break;
                default:
                    break;
            }
        }
        /*划区域*/
        public void addRegionByOne(string name, string lng, string lat, string radius, string color)
        {
            this.WebMap.InvokeScript("addCircleByOne", new object[] { lng, lat, radius, color, name });
        }
        /*打开圆形测量*/
        public void OpenDrawCircleManager()
        {
            this.WebMap.InvokeScript("OpenDrawCircleManager");
        }
        /*打开折线测量*/
        public void OpenDrawPolyLineManager()
        {
            this.WebMap.InvokeScript("OpenDrawPolyLineManager");
        }
        /*打开多边形测量*/
        public void OpenDrawPolyGonManager()
        {
            this.WebMap.InvokeScript("OpenDrawPolyGonManager");
        }
        /*打开矩形测量*/
        public void OpenDrawRectManager()
        {
            this.WebMap.InvokeScript("OpenDrawRectManager");
        }
        /*关闭区域测量*/
        public void CloseDrawingManager()
        {
            this.WebMap.InvokeScript("CloseDrawingManager");
        }
        /*移除地图所有覆盖物*/
        public void RemoveAllMarkers()
        {
            this.WebMap.InvokeScript("RemoveAllOverlays");
        }
        public void RemoveCircleByAdd()
        {
            this.WebMap.InvokeScript("removeCircleByAdd");
        }


        public string GetCircle()
        {
            return this.WebMap.InvokeScript("getCircle").ToString();
        }
        public string GetLine()
        {
            return this.WebMap.InvokeScript("getLine").ToString();
        }
        public string GetRect()
        {
            return this.WebMap.InvokeScript("getRect").ToString();
        }
        public string GetPoly()
        {
            return this.WebMap.InvokeScript("getPoly").ToString();
        }
    }
}
