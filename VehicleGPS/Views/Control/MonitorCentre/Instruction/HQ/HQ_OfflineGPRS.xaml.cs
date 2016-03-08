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
using Newtonsoft.Json.Linq;
using VehicleGPS.Models.Login;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ
{
    /// <summary>
    /// HQ_OfflineGPRS.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_OfflineGPRS : Window
    {
       private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string type;
        public bool isDisplay = true;
        public HQ_OfflineGPRS(string e)
        {
            if (VBaseInfo.GetInstance().GPSType_id != "1")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            type = e;
            Title = e;
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
            label1.Text = e;
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case "终端掉线后重新建立GPRS连接的最大时间":
                    sendInfo(type, "C");
                    break;
                case "设置服务器UDP端口号":
                case "设置运行时定时拍摄的时间间隔":
                    sendInfo(type, "D");
                    break;
            }
        }

        private string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd,string type)
        {
            string rebool = "";
            JObject jo = new JObject();
            if (type == "设置运行时定时拍摄的时间间隔")
            {
                jo.Add("cmd", "HQMMEDIACMD_TYPE");
                jo.Add("cmdid", sim + "_HQMMEDIACMD_TYPE_" + subcmd);
            }
            else
            {
                jo.Add("cmd", "HQNETWORKCONFCMD_TYPE");
                jo.Add("cmdid", sim + "_HQNETWORKCONFCMD_TYPE_" + subcmd);
            }
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            if (type == "终端掉线后重新建立GPRS连接的最大时间" || type == "设置运行时定时拍摄的时间间隔")
            {
                jo.Add("interval", tb1.Text);
            }
            else if (type == "设置服务器UDP端口号")
            {
                jo.Add("udpport", tb1.Text);
            }
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void sendInfo(string cmdname, string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype,type);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(cmdname, StaticLoginInfo.GetInstance().UserName, "", Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                if (type == "设置运行时定时拍摄的时间间隔")
                {
                    cmd.cmdId = SIM.Text + "_HQMMEDIACMD_TYPE_" + cmdtype;
                }
                else
                {
                    cmd.cmdId = SIM.Text + "_HQNETWORKCONFCMD_TYPE_" + cmdtype;
                }
                cmd.cmdContent = cmdname + ":";
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

        private void Store_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                Store_checkBox_flag = "1";
            }
        }

        private void Reply_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == false)
            {
                Reply_checkBox_flag = "0";
            }
        }
    }
}
