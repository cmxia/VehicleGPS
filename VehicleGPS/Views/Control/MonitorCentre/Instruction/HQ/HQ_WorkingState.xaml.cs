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
    /// HQ_WorkingState.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_WorkingState : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string context_flag;
        private string type;
        public bool isDisplay = true;
        public HQ_WorkingState(string e)
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
            switch (e)
            {
                case "遥控智能锁车功能":
                    context_1.Content = "表示恢复车辆油路";
                    context_2.Content = "表示锁定车辆油路";
                    context_3.Visibility = Visibility.Collapsed;
                    context_4.Visibility = Visibility.Collapsed;
                    Height = double.Parse("360");
                    break;
                case "终端设置允许":
                    context_1.Content = "允许终端进行重要设置";
                    context_2.Content = "禁止终端进行重要设置";
                    context_3.Visibility = Visibility.Collapsed;
                    context_4.Visibility = Visibility.Collapsed;
                    Height = double.Parse("360");
                    break;

            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case "控制终端工作状态":
                    if (context_1.IsChecked == false && context_2.IsChecked == false && context_3.IsChecked == false && context_4.IsChecked == false)
                    {
                        MessageBox.Show("请设置控制终端工作状态！");
                    }
                    else
                    {
                        sendInfo("控制终端工作状态", "A");
                    }
                    break;
                case "遥控智能锁车功能":
                    if (context_1.IsChecked == false && context_2.IsChecked == false)
                    {
                        MessageBox.Show("请设置遥控智能锁车功能！");
                    }
                    else
                    {
                        sendInfo("遥控智能锁车功能", "B");
                    }
                    break;
                case "终端设置允许":
                    if (context_1.IsChecked == false && context_2.IsChecked == false)
                    {
                        MessageBox.Show("请设置终端设置允许功能！");
                    }
                    else
                    {
                        sendInfo("终端设置允许", "O");
                    }
                    break;
            }
        }

        private void Context_Info_Click(object sender, RoutedEventArgs e)
        {
            if (context_1.IsChecked == true)
            {
                context_flag = "0";
            }
            else if (context_2.IsChecked == true)
            {
                context_flag = "1";
            }
            else if (context_3.IsChecked == true)
            {
                context_flag = "2";
            }
            else if (context_4.IsChecked == true)
            {
                context_flag = "3";
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

        public static string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd, string context_flag)
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "HQCTRLCMD_TYPE");
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            jo.Add("data", context_flag);
            jo.Add("cmdid", sim + "_HQCTRLCMD_TYPE_" + subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void sendInfo(string cmdname,string cmdtype)
        {
            Result.Text =Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype, context_flag);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(cmdname, StaticLoginInfo.GetInstance().UserName, context_flag, Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_HQCTRLCMD_TYPE_" + cmdtype;
                cmd.cmdContent = cmdname + ":" + context_flag;
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
    }
}
