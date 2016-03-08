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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// SLChatMsg.xaml 的交互逻辑
    /// </summary>
    public partial class SLChatMsg : UserControl
    {
        public string cname;
        public SLChatMsg(int count, string name)
        {
            InitializeComponent();
            cname = name;
            this.NumCounts.Text = count.ToString();
            this.FromPart.Text = name;
        }
    }
}
