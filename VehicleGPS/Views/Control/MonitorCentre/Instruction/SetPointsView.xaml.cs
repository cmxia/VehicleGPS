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
using VehicleGPS.ViewModels.MonitorCentre.Instruction;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// SetPointsView.xaml 的交互逻辑
    /// </summary>
    public partial class SetPointsView : Window
    {
        public SetPointsView(List<PointOfLine> list)
        {
            InitializeComponent();
            this.DataContext = new SetPointsViewModel(list);
        }

        private void btn_Cel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
