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
    /// HQ_FixSpeed.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_FixSpeed : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string type;
        public bool isDisplay = true;
        public HQ_FixSpeed(string e)
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
            if (e == "设置终端产品ID")
            {
                label1.Text = "30字节产品ID";
                label2.Text = "司机身份识别码";
            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (type == "设置定速回传参数")
            {
                if (tb1.Text.Length == 0)
                {
                    MessageBox.Show("请设置长度间隔！");
                    return;
                }
                if (tb2.Text.Length == 0)
                {
                    MessageBox.Show("请设置发送次数！");
                    return;
                }
                sendInfo("设置定速回传参数", "N");
            }
            else
            {
                if (tb1.Text.Length == 0)
                {
                    MessageBox.Show("请设置产品ID！");
                    return;
                }
                if (tb2.Text.Length == 0)
                {
                    MessageBox.Show("请设置司机身份识别码！");
                    return;
                }
                sendInfo("设置终端产品ID", "M");
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

        private string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd)
        {
            string rebool = "";
            JObject jo = new JObject();
            if (type == "设置定速回传参数")
            {
                jo.Add("cmd", "HQCTRLCMD_TYPE");
                jo.Add("cmdid", sim + "_HQCTRLCMD_TYPE_" + subcmd);
            }
            else
            {
                jo.Add("cmd", "HQSETTINGCMD_TYPE");
                jo.Add("cmdid", sim + "_HQSETTINGCMD_TYPEE_" + subcmd);
            }
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            if (type == "设置定速回传参数")
            {
                jo.Add("meter", tb1.Text);
                jo.Add("times", tb2.Text);
            }
            else
            {
                jo.Add("termId", tb1.Text);
                jo.Add("driverId", tb2.Text);
            }
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void sendInfo(string cmdname, string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(cmdname, StaticLoginInfo.GetInstance().UserName, tb1.Text + tb2.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                if (type == "设置定速回传参数")
                {
                    cmd.cmdId = SIM.Text + "_HQCTRLCMD_TYPE_" + cmdtype;
                }
                else
                {
                    cmd.cmdId = SIM.Text + "_HQSETTINGCMD_TYPEE_" + cmdtype;
                }
                cmd.cmdContent = cmdname + ":" + tb1.Text + tb2.Text;
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