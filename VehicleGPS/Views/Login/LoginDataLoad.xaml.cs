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
using System.Windows.Threading;
using VehicleGPS.Models;
using VehicleGPS.ViewModels.Login;

namespace VehicleGPS.Views.Login
{
    /// <summary>
    /// LoginDataLoad.xaml 的交互逻辑
    /// </summary>
    public partial class LoginDataLoad : Window
    {
        public LoginDataLoad()
        {
            InitializeComponent();
            this.DataContext = new LoginDataLoadViewModel(this);
        }
    }
}
