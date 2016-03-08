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
using VehicleGPS.ViewModels.MonitorCentre.ImageCheck;

namespace VehicleGPS.Views.Control.MonitorCentre.ImageCheck
{
    /// <summary>
    /// ImageCheck.xaml 的交互逻辑
    /// </summary>
    public partial class ImageCheck : Window
    {
        public ImageCheck()
        {
            InitializeComponent();
            this.DataContext = new ImageCheckViewModel(this.MyWeb);
        }
    }
}
