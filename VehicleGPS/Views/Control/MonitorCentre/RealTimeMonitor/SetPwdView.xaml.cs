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
using VehicleGPS.Models;
using System.IO;
using System.Xml;

namespace VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor
{
    /// <summary>
    /// SetPwdView.xaml 的交互逻辑
    /// </summary>
    public partial class SetPwdView : Window
    {
        private string path = VehicleConfig.GetInstance().warnsetPwdPath;
        public SetPwdView()
        {
            InitializeComponent();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("密码文件未找到！");
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode xn = xmlDoc.SelectSingleNode("Setting");
            string orginPwd = xn.ChildNodes[0].InnerText;

            // 使用一个IntPtr类型值来存储加密字符串的起始点  
            IntPtr p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.origin_pwd.SecurePassword);

            // 使用.NET内部算法把IntPtr指向处的字符集合转换成字符串  
            string password = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);
            if (password != orginPwd)
            {
                MessageBox.Show("原密码错误！请重填！");
                return;
            }
            p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.new_pwd.SecurePassword);
            string new_pwd = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);
            p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.confirm_pwd.SecurePassword);
            string confirm_pwd = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);
            if (new_pwd != confirm_pwd)
            {
                MessageBox.Show("新密码与确认密码不一致，请重填！");
                return;
            }
            xn.ChildNodes[0].InnerText = confirm_pwd;
            xmlDoc.Save(this.path);
            MessageBox.Show("配置成功！");
            this.Close();
        }
    }
}
