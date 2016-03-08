//WarnInfo.xaml.cs
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
using System.Net;
using System.IO;
using VehicleGPS.Models.Login;
using System.Runtime.Serialization.Json;
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;
using System.Windows.Threading;
using Microsoft.Practices.Prism.ViewModel;
using System.Threading;
using System.Media;
using System.ComponentModel;
using Newtonsoft.Json;
using VehicleGPS.ViewModels.Warn;

namespace VehicleGPS.Views.Warn
{
    /// <summary>
    /// WarnInfo.xaml 的交互逻辑
    /// </summary>
    public partial class WarnInfo : Window
    {
        private bool iswinshow = false;
        public WarnInfo()
        {
            InitializeComponent();
            this.Top = 5;
            this.Left = (SystemParameters.FullPrimaryScreenWidth - this.Width) / 2;
            this.DataContext = WarnInfoViewModel.GetInstance();
            this.expand.Content = "展开";
            this.Height = 30;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(WarnInfo_MouseLeftButtonDown);
        }

        void WarnInfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        //隐藏窗口
        public void HideWin()
        {
            this.Hide();
            iswinshow = false;
        }
        //显示窗口
        public void ShowWin()
        {
            this.Show();
            iswinshow = true;
        }
        public bool IsWinShow()
        {
            return this.iswinshow;
        }
        //单例
        private static WarnInfo instance = null;
        public static WarnInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new WarnInfo();
                StaticWarnInfo.isInitialed = true;
            }
            return instance;
        }
        public static void SetInstanceNull() {
            instance = null;
        }
        private void expand_Click(object sender, RoutedEventArgs e)
        {
            if (string.Compare(this.expand.Content.ToString(), "展开") == 0)
            {
                WarnInfoViewModel.GetInstance().RefreshCommandExecute();
                this.Height = 330;
                this.expand.Content = "收起";
                this.AlermInfo.Visibility = Visibility.Visible;
            }
            else
            {
                this.Height = 30;
                this.expand.Content = "展开";
                this.AlermInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            iswinshow = false;
        }
    }
}
