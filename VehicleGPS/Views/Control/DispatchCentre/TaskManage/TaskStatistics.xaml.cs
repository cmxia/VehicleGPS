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
using VehicleGPS.ViewModels.DispatchCentre.TaskManage;

namespace VehicleGPS.Views.Control.DispatchCentre.TaskManage
{
    /// <summary>
    /// TaskStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class TaskStatistics : Window
    {
        public TaskStatistics()
        {
            
            InitializeComponent();
            this.DataContext = new TaskStatisticsViewModel();
        }
    }
}
