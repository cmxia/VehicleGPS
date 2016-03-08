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
using VehicleGPS.Services;
using VehicleGPS.Services.Dialog;
using System.Globalization;
using VehicleGPS.ViewModels.Login;

namespace VehicleGPS.Views.Login
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(this);
        }

        private void CloseImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void SetImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Config config = new Config();
            config.Owner = this;
            config.ShowDialog();
        }
    }
}
