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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;

namespace VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch
{
    /// <summary>
    /// VehicleDispatch.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleDispatch : UserControl
    {
        //private List<AlarmMsgInfo> alarmMsglist = null;

        public VehicleDispatch()
        {
            InitializeComponent();
            InitButton();
            this.DataContext = VehicleDispatchViewModel.GetInstance();
            ///推送6-11
            StaticTreeState.VehicleDispatchViewContruct = true;
        }
        /*设置“查询”“显示所有”“时间提醒设置”样式*/
        private void InitButton()
        {
            SetButton(imgBtn_Query);
        }
        private void SetButton(ImageButton imgBtn)
        {
            imgBtn.Margin = new Thickness(2, 0, 2, 0);
            imgBtn.VerticalAlignment = VerticalAlignment.Center;
            imgBtn.HorizontalAlignment = HorizontalAlignment.Center;

            imgBtn.TextFontColor = new SolidColorBrush(Colors.Black);
            imgBtn.TextMargin = new Thickness(3);

            imgBtn.MouseOverBorderBackground = new SolidColorBrush(Color.FromRgb(255, 243, 206));
            imgBtn.MouseOverBorderCorner = new CornerRadius(3);
        }

        /*点击时间：时间提醒设置*/
        private void imgBtn_TimeSetting_Click(object sender, RoutedEventArgs e)
        {
            Window win = new TimeRemindSetting(null);
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
        }
        /*点击强制回站*/
        private void imgBtn_Back_Click(object sender, RoutedEventArgs e)
        {
            Window win = new BackToStation();
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DispatchVehicle infoWin = new DispatchVehicle(this.DataContext);
            infoWin.Owner = Window.GetWindow(this);
            infoWin.ShowDialog();
        }

        private void DestImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VehicleInfo vehicleInfoWin = new VehicleInfo(this.DataContext);
            vehicleInfoWin.Owner = Window.GetWindow(this);
            vehicleInfoWin.ShowDialog();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }
    }

}
