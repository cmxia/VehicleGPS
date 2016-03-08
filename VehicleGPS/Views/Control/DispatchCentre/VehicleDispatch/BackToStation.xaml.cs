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
    /// BackToStation.xaml 的交互逻辑
    /// </summary>
    public partial class BackToStation : Window
    {
        public BackToStation()
        {
            InitializeComponent();
            this.DataContext = new BackToStationViewModel();
            this.Loaded += new RoutedEventHandler(BackToStation_Loaded);
        }

        /*将窗口的数据模型赋值给树形模型，方便操作*/
        void BackToStation_Loaded(object sender, RoutedEventArgs e)
        {
            ((BackToStationTreeViewModel)vechicleTree.DataContext).StationVM = (BackToStationViewModel)this.DataContext;
        }
    }
}
