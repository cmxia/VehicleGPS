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
using System.IO;
using System.Xml;
using VehicleGPS.Models;

namespace VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor
{
    /// <summary>
    /// VehicleInfoConfig.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleInfoConfig : Window
    {
        private string path = VehicleConfig.GetInstance().vehicleConfigPath;
        public VehicleInfoConfig()
        {
            InitializeComponent();
            InitWindow();
        }

        private void InitWindow()
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("配置文件未找到！初始化失败");
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("Setting");
            string orginMember = xn.ChildNodes[0].InnerText;
            if (orginMember.Equals("0"))
            {
                this.vehicleid.IsChecked = true;
            }
            else
            {
                this.innerid.IsChecked = true;
            }
        }


        //确定  按钮监听事件
        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("配置文件未找到！设置失败");
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("Setting");
            if (true)
	{
		 
	}
            xn.ChildNodes[0].InnerText = "";
            xmlDoc.Save(this.path);
            MessageBox.Show("配置成功！");
            this.Close();
            if (MessageBoxResult.OK == MessageBox.Show("本操作需要重新启动车辆管理系统，您确定要退出吗！", "退出系统", MessageBoxButton.OKCancel))
            {
                Application.Current.Shutdown();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        //取消  按钮监听事件
        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
