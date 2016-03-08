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
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch
{
    /// <summary>
    /// VehicleInfo.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleInfo : Window
    {
        public VehicleInfo(object parentVM)
        {
            InitializeComponent();
            this.DataContext = new VehicleInfoViewModel(parentVM);
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
