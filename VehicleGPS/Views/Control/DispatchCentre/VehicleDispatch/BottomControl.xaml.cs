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
using System.Net;
using VehicleGPS.Models;
using VehicleGPS.Views.Control.DispatchCentre;

namespace VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch
{
   
    /// <summary>
    /// BottomControl.xaml 的交互逻辑
    /// </summary>
    public partial class BottomControl : Window
    {
        public delegate void MouseClick(object sender, MouseButtonEventArgs e);

        private MouseEnter onMouseEnter;
        private MouseMove onMouseMove;
        private MouseLeave onMouseLeave;

        private string inFactoryCount;  //工厂内车辆数
        private string startid;  //出发站点ID
        private string endid;  //目的地ID
        private string endname;  //目的地名称
        private double wholedistance;  //总运输距离
        private string tasknumber;  //任务号

        #region 属性

        public MouseEnter OnMouseEnter
        {
            get
            {
                return onMouseEnter;
            }
            set
            {
                onMouseEnter = value;
            }
        }

        public MouseMove OnMouseMove
        {
            get
            {
                return onMouseMove;
            }
            set
            {
                onMouseMove = value;
            }
        }

        public MouseLeave OnMouseLeave
        {
            get
            {
                return onMouseLeave;
            }
            set
            {
                onMouseLeave = value;
            }
        }

        public string StartID
        {
            get
            {
                return startid;
            }
            set
            {
                startid = value;
            }
        }

        public string InFactoryCount
        {
            get
            {
                return inFactoryCount;
            }
            set
            {
                inFactoryCount = value;
            }
        }

        public string EndID
        {
            get
            {
                return endid;
            }
            set
            {
                endid = value;
            }
        }

        public string EndName
        {
            get
            {
                return endname;
            }
            set
            {
                endname = value;
            }
        }

        public double WholeDistance
        {
            get
            {
                return wholedistance;
            }
            set
            {
                wholedistance = value;
            }
        }

        public string TaskNumber
        {
            get
            {
                return tasknumber;
            }
            set
            {
                tasknumber = value;
            }
        }

        #endregion

        public BottomControl(double WholeDistance, string TaskNumber, string StartID, string EndID, string EndName)
        {
            // 为初始化变量所必需
            InitializeComponent();

            this.WholeDistance = WholeDistance;
            this.TaskNumber = TaskNumber;
            this.StartID = StartID;
            this.EndID = EndID;
            this.EndName = EndName;
            this.txt_InnerStationNum.MouseLeftButtonDown += new MouseButtonEventHandler(txt_InnerStationNum_MouseLeftButtonDown);
        }

        private void txt_InnerStationNum_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouseclick(this, e);
        }

        private const double EARTH_RADIUS = 6378.137;  //地球半径
        private double latRegion;  //纬度(横)
        private double longRegion;  //经度(纵)
        private double radius;  //半径
        private Random urlRandom = new Random();
        public MouseClick Mouseclick;

        //王益大大调用方法
        public void getVehicleGpsInfoBySim(string sim, double latRegion, double longRegion, double radius)
        {
            this.latRegion = latRegion;
            this.longRegion = longRegion;
            this.radius = radius;

            string randomStr = urlRandom.Next().ToString() + DateTime.Now.ToString() + urlRandom.Next().ToString();
            Uri endpoint = new Uri(VehicleConfig.GetInstance().CONCRETE_SIM_ONLINE_WEB_URL + "?Sim=" + sim + "&time=" + randomStr);

            WebClient client = new WebClient();

            //异步下载资源
            client.DownloadStringAsync(endpoint);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadGpsInfoStringCompleted);
        }

        private void client_DownloadGpsInfoStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if ((null == e.Error) && (!String.IsNullOrWhiteSpace(e.Result)))
            {
                string xmlStr = e.Result;
                XmlUtilSL xmlUtil = new XmlUtilSL();
                GPSInfo vehGps = new GPSInfo();
                vehGps = xmlUtil.DeserializeGpsInfo(xmlStr);  //解析XML

                double distance = get_two_points_distance(this.latRegion, this.longRegion, double.Parse(vehGps.Latitude),
                    double.Parse(vehGps.Longitude)) - (this.radius / 1000);

                addCarsControls(this.WholeDistance, this.Slider.ActualWidth, distance, vehGps.Sim);
            }
        }

        private double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        //获取地图中两点间距离
        private double get_two_points_distance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        private void addCarsControls(double actualLength, double zoomLength, double transedLength, string sim)
        {
            CarPositionControlDown cpc = new CarPositionControlDown(this.OnMouseEnter, this.OnMouseMove, this.OnMouseLeave, sim);
            cpc.StartPoint = this.txt_StartPointName.Text.Trim();
            cpc.EndPoint = this.txt_EndPointName.Text.Trim();
            cpc.WholeDistance = Math.Round(actualLength, 2).ToString().Trim() + " km";
            cpc.TransedDistance = Math.Round(transedLength, 2).ToString().Trim() + " km";

            double precent = transedLength / actualLength;
            if (precent >= 1.0)
            {
                precent = 1.0;
            }
            cpc.Margin = new Thickness(precent * zoomLength, 1, 0, 0);
            this.oddCarsInfo.Children.Add(cpc);
        }
    }
}
