﻿using System;
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

namespace VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch
{
    /// <summary>
    /// BackToStationTree.xaml 的交互逻辑
    /// </summary>
    public partial class BackToStationTree : UserControl
    {
        public BackToStationTree()
        {
            InitializeComponent();
            InitButton();
            this.DataContext = new BackToStationTreeViewModel();
        }
        /*设置“刷新按钮”样式*/
        private void InitButton()
        {
            SetButton(imgBtn_Refresh);
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
    }
}
