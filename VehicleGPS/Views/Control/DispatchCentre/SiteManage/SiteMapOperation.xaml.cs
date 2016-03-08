using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VehicleGPS.ViewModels.DispatchCentre.SiteManage;
using System.ComponentModel;
using VehicleGPS.Services;
using System.Windows.Navigation;
using VehicleGPS.Models;

namespace VehicleGPS.Views.Control.DispatchCentre.SiteManage
{
    /// <summary>
    /// SiteMapOperation.xaml 的交互逻辑
    /// 标注区域类
    /// </summary>
    public partial class SiteMapOperation : Window
    {
        private SiteAddModViewModel parentVM;
       // private SiteAddModDelViewModel parentVM;
        public SiteMapOperation(object parentVM)
        {
            InitializeComponent();
            //this.parentVM = (SiteAddModDelViewModel)parentVM;
            this.parentVM = (SiteAddModViewModel)parentVM;
            this.InitBaiduMap();
        }
        /*初始化百度地图*/
        private void InitBaiduMap()
        {
            //string currentUri = Environment.CurrentDirectory;
            Uri uri = new Uri(VehicleConfig.GetInstance().regionMapPath, UriKind.RelativeOrAbsolute);
            this.MyWeb.Navigate(uri);
        }

        #region 地图操作
        //添加表区监听事件
        public void addCircleListener(string color)
        {
            MyWeb.InvokeScript("addCircleListener", color);
        }
        //返回标区中心经纬度、区域半径和地址 ："lng;lat;radius"
        public string returnLngLatRadius()
        {
            return MyWeb.InvokeScript("returnLngLatRadius").ToString();
        }
        public void removeRegion()
        {
            MyWeb.InvokeScript("removeCircle");
        }
        public void addRegionByOne(string lng, string lat, string radius, string color, bool isEnableEdit)
        {
            MyWeb.InvokeScript("addCircleByOne", lng, lat, radius, color, isEnableEdit);
        }
        #endregion

        public void InitMap()
        {
            
            if (!string.IsNullOrEmpty(this.parentVM.regLongitude) &&
                !string.IsNullOrEmpty(this.parentVM.regLatitude) &&
                !string.IsNullOrEmpty(this.parentVM.RegRadius))
            {
                this.addRegionByOne(this.parentVM.regLongitude, this.parentVM.regLatitude, this.parentVM.RegRadius, "#f00", true);
            }
            else {
                this.addCircleListener("#f00");
            }
            
        }
        private void DispatchRegion_btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            string ret = this.returnLngLatRadius();
            if(string.IsNullOrEmpty(ret))
            {
                if (MessageBox.Show("未标区退出？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.Close();
                    return;
                }
                else
                {
                    return;
                }
            }
            string[] loglatradius = (ret).Split(';');//获取标区经纬度、圆心和地址："lng;lat;radius"
            string lng = loglatradius[0];
            string lat = loglatradius[1];
            string radius = loglatradius[2];
            string addr = "";
            if (!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat)) 
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    addr = (new BusinessDataServiceWEB()).ParseOneAddress(lng,lat);
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    this.parentVM.regLongitude = lng;
                    this.parentVM.regLatitude = lat;
                    this.parentVM.RegRadius= radius;
                    this.parentVM.regAddress = addr;

                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        this.Indicator.IsBusy = false;
                        this.MyWeb.Visibility = Visibility.Visible;
                        this.Close();
                    });
                };
                this.Indicator.IsBusy = true;
                this.MyWeb.Visibility = Visibility.Hidden;
                worker.RunWorkerAsync();
            }            
        }

        private void DispatchRegion_regionMarker_Click(object sender, RoutedEventArgs e)
        {
            this.removeRegion();
            this.addCircleListener("#f00");
        }

        private void MyWeb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            this.InitMap();
        }
    }
}
