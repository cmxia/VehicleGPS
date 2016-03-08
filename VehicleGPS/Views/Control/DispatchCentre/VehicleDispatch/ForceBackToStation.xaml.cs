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
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch
{
    /// <summary>
    /// ForceBackToStation.xaml 的交互逻辑
    /// </summary>
    public partial class ForceBackToStation : Window
    {
        public ForceBackToStation(TaskInfo taskinfo)
        {
            InitializeComponent();
            this.DataContext = new ForceBackToStationViewModel(taskinfo);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
