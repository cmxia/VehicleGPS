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
    /// WarnRelievePwd.xaml 的交互逻辑
    /// </summary>
    public partial class WarnRelievePwd : Window
    {
        private string path = VehicleConfig.GetInstance().warnsetPwdPath;
        public static bool isRight = false;
        public WarnRelievePwd()
        {
            InitializeComponent();
            //this.Top = 380;
            //this.Left = (SystemParameters.FullPrimaryScreenWidth - this.Width) / 2;
        }

        private void ConFirm_btn_Click(object sender, RoutedEventArgs e)
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
            IntPtr p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.pwd.SecurePassword);

            // 使用.NET内部算法把IntPtr指向处的字符集合转换成字符串  
            string password = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);
            if (orginPwd != password)
            {
                MessageBox.Show("密码错误！请重新输入！");
                return;
            }
            isRight = true;
            this.Close();
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
