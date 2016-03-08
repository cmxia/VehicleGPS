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

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// ChildAlert.xaml 的交互逻辑
    /// 消息中心提示窗口
    /// </summary>
    public partial class ChildAlert : Window
    {
        public ChildAlert()
        {
            InitializeComponent();
        }
        public ChildAlert(string info)
            : this()
        {
            this.info.Text = info;
        }
    }
}
