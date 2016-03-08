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
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;

namespace VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor
{
    /// <summary>
    /// KeyWordSearchView.xaml 的交互逻辑
    /// </summary>
    public partial class KeyWordSearchView : Window
    {
        public KeyWordSearchView()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(WarnInfo_MouseLeftButtonDown);
        }
        void WarnInfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void search_bt_Click(object sender, RoutedEventArgs e)
        {
            if (this.keyWord_tb.Text == null || this.keyWord_tb.Text.ToString().Trim() == "")
            {
                MessageBox.Show("请先输入关键字！");
                return;
            }
            string keyword = this.keyWord_tb.Text.ToString().Trim();
            RealTimeViewModel.GetInstance().MapService.RemoveAllMarkers();
            RealTimeViewModel.GetInstance().InitRegionInBaidu();
            RealTimeViewModel.GetInstance().InitBaiduMap();
            RealTimeViewModel.GetInstance().MapService.SearchByKeyWord(keyword);
        }

        private void cancel_bt_Click(object sender, RoutedEventArgs e)
        {
            RealTimeViewModel.GetInstance().MapService.RemoveAllMarkers();
            RealTimeViewModel.GetInstance().InitRegionInBaidu();
            RealTimeViewModel.GetInstance().InitBaiduMap();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //RealTimeViewModel.GetInstance().MapService.RemoveAllMarkers();
            //RealTimeViewModel.GetInstance().InitRegionInBaidu();
            //RealTimeViewModel.GetInstance().InitBaiduMap();
        }
    }
}
