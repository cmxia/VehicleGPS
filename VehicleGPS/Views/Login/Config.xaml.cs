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
using System.Text.RegularExpressions;
using System.Xml;

namespace VehicleGPS.Views.Login
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class Config : Window
    {
        public Config()
        {
            InitializeComponent();
            this.InitConfig();
        }

        private void InitConfig()
        {
            VehicleConfig vconfig = VehicleConfig.GetInstance();
            this.webip.Text = vconfig.WEBIP;
            this.webport.Text = vconfig.WEBPORT;
            this.busip.Text = vconfig.BUSINESSIP;
            this.busport.Text = vconfig.BUSINESSPORT;
            this.insip.Text = vconfig.INSTRUCTIONIP;
            this.insport.Text = vconfig.INSTRUCTIONPORT;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定保存吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                VehicleConfig vconfig = VehicleConfig.GetInstance();
                string webip = this.webip.Text;
                string webport = this.webport.Text;
                string busip = this.busip.Text;
                string busport = this.busport.Text;
                string insip = this.insip.Text;
                string insport = this.insport.Text;
                if (IsPortValidate(webport) && IsPortValidate(busport)
                    && IsPortValidate(insport))
                {
                    if (IsIpValidate(webip) && IsIpValidate(busip) && IsIpValidate(insip))
                    {
                        vconfig.WEBIP = webip;
                        vconfig.WEBPORT = webport;
                        vconfig.BUSINESSIP = busip;
                        vconfig.BUSINESSPORT = busport;
                        vconfig.INSTRUCTIONIP = insip;
                        vconfig.INSTRUCTIONPORT = insport;
                        vconfig.RefreshConfig();
                        vconfig.WriteConfig();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("请输入正确的ip地址", "提示", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("端口号必须在1~65534之间", "提示", MessageBoxButton.OK);
                }
                
            }
        }
        private bool IsPortValidate(string port)
        {
            try
            {
                int iport = Convert.ToInt32(port);
                if (iport > 1 && iport < 65534)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;
        }
        private bool IsIpValidate(string ip)
        {
            string reg = @"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))";
            if (!Regex.Match(ip, reg).Success)
            {
                return false;
            }
            return true;
        }
    }
}
