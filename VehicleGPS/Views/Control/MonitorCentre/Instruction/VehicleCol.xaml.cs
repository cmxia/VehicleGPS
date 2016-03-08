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
using ZeroMQ;
using Newtonsoft.Json.Linq;
using VehicleGPS.Models.Login;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// VehicleCol.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleCol : Window
    {
        private string context_flag = "蓝色";
        private string termType;//终端类型
        public bool isDisplay = true;
        public VehicleCol()
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
            switch (VBaseInfo.GetInstance().GPSType)
            {
                case "华强协议":
                    termType = "1";
                    break;
                case "部标协议":
                    termType = "2";
                    break;
                case "Queclink协议":
                    termType = "3";
                    break;
                case "康凯斯协议":
                    termType = "4";
                    break;
                case "世通协议":
                    termType = "5";
                    break;
                case "交大神州协议":
                    termType = "6";
                    break;
                default:
                    termType = "0";
                    break;
            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            context_flag = "1;" + Common.toHex(uint.Parse(context_flag), 1);
            string cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_132";
            Result.Text=Socket.Texttest(SIM.Text, termType, context_flag, "132", cmdid);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql("车牌颜色", StaticLoginInfo.GetInstance().UserName, context_flag, Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = cmdid;
                cmd.cmdContent = "车牌颜色" + ":" + context_flag;
                cmd.cmdSim = SIM.Text.ToString();
                cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                cmd.SendStatus = Result.Text.ToString();
                cmd.VehicleNum = VehicleId.Text.ToString();
                while (true)
                {
                    if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
                    {
                        StaticTreeState.CmdStatus = LoadingState.LOADING;
                        StaticMessageInfo.GetInstance().CmdList.Add(cmd);
                        StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
                        break;
                    }
                }
            }   
        }

        private void Context_Info_Click(object sender, RoutedEventArgs e)
        {
            if (context_blue.IsChecked == true)
            {
                context_flag = "1";
            }
            else if (context_yellow.IsChecked == true)
            {
                context_flag = "2";
            }
            else if (context_black.IsChecked == true)
            {
                context_flag = "3";
            }
            else if (context_white.IsChecked == true)
            {
                context_flag = "4";
            }
            else 
            {
                context_flag = "9";
            }
        }
    }
}
