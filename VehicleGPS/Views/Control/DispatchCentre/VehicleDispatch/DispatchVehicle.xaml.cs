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
    /// DispatchVehicle.xaml 的交互逻辑
    /// </summary>
    public partial class DispatchVehicle : Window
    {
        public DispatchVehicle(object parentVM)
        {
            InitializeComponent();
            this.DataContext = new MDispatchVehicleViewModel(this,parentVM);
        }

        private void btn_Cel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        public static DispatchVehicle window { get; set; }
    }
}
