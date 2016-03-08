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
using VehicleGPS.Models;
using VehicleGPS.ViewModels.MonitorCentre.Instruction;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// SetCirLineRectPoly.xaml 的交互逻辑
    /// </summary>
    public partial class SetCirLineRectPoly : Window
    {
        public bool isDisplay = true;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">需要设置的覆盖物类型 "circle":圆形，"line"：路线，"rect"：矩形，"poly"：多边形</param>
        public SetCirLineRectPoly(string type)
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            InitWindow(type);
            this.DataContext = new InstructionViewModel(this.InsMap, type);
        }
        //初始化窗口
        private void InitWindow(string type)
        {
            switch (type)
            {
                case "circle":
                    this.Title = "设置圆形区域";
                    this.CircleGeo.Visibility = Visibility.Visible;
                    break;
                case "line":
                    this.Title = "设置路线";
                    this.LineGeo.Visibility = Visibility.Visible;
                    this.SettingRegion.Visibility = Visibility.Collapsed;
                    this.Speed.Visibility = Visibility.Collapsed;
                    this.SpLatiLong.Visibility = Visibility.Collapsed;
                    this.SpGnss.Visibility = Visibility.Collapsed;
                    this.SpGprs.Visibility = Visibility.Collapsed;
                    break;
                case "rect":
                    this.Title = "设置矩形区域";
                    this.RectGeo.Visibility = Visibility.Visible;
                    break;
                case "poly":
                    this.Title = "设置多边形区域";
                    this.PolyGeo.Visibility = Visibility.Visible;
                    this.SettingRegion.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }
    }
}
