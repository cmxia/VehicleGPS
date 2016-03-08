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
using VehicleGPS.ViewModels.MonitorCentre;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;

namespace VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor
{
    /// <summary>
    /// VehicleInfoShowSetting.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleInfoShowSetting : Window
    {
        public VehicleInfoShowSetting()
        {
            InitializeComponent();
            this.DataContext = ShowSettingViewModel.GetInstance();
        }

        private void btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement item in sp_BasicInfo1.Children)
            {
                if (item is CheckBox)
                {
                    BindingExpression exp = ((CheckBox)item).GetBindingExpression(CheckBox.IsCheckedProperty);
                    exp.UpdateSource();
                }
            }
            foreach (UIElement item in sp_BasicInfo2.Children)
            {
                if (item is CheckBox)
                {
                    BindingExpression exp = ((CheckBox)item).GetBindingExpression(CheckBox.IsCheckedProperty);
                    exp.UpdateSource();
                }
            }
            foreach (UIElement item in sp_GpsInfo1.Children)
            {
                if (item is CheckBox)
                {
                    BindingExpression exp = ((CheckBox)item).GetBindingExpression(CheckBox.IsCheckedProperty);
                    exp.UpdateSource();
                }
            }
            foreach (UIElement item in sp_StateInfo1.Children)
            {
                if (item is CheckBox)
                {
                    BindingExpression exp = ((CheckBox)item).GetBindingExpression(CheckBox.IsCheckedProperty);
                    exp.UpdateSource();
                }
            }
            foreach (UIElement item in sp_StateInfo2.Children)
            {
                if (item is CheckBox)
                {
                    BindingExpression exp = ((CheckBox)item).GetBindingExpression(CheckBox.IsCheckedProperty);
                    exp.UpdateSource();
                }
            }
            foreach (UIElement item in sp_WarnInfo1.Children)
            {
                if (item is CheckBox)
                {
                    BindingExpression exp = ((CheckBox)item).GetBindingExpression(CheckBox.IsCheckedProperty);
                    exp.UpdateSource();
                }
            }
            foreach (UIElement item in sp_WarnInfo2.Children)
            {
                if (item is CheckBox)
                {
                    BindingExpression exp = ((CheckBox)item).GetBindingExpression(CheckBox.IsCheckedProperty);
                    exp.UpdateSource();
                }
            }
            this.Close();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
