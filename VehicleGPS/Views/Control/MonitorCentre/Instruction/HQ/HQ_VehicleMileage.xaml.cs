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
    /// HQ_VehicleMileage.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_VehicleMileage : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string inDist_flag;
        private string clearDist_flag;
        public bool isDisplay = true;
        public HQ_VehicleMileage()
        {
            if (VBaseInfo.GetInstance().GPSType_id != "1")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (context_1.IsChecked==false&&context_2.IsChecked==false)
            {
                MessageBox.Show("请设置是否附加里程信息");
                return;
            }
            if (context_3.IsChecked == false && context_4.IsChecked == false)
            {
                MessageBox.Show("请设置里程信息是否清零");
                return;
            }
            sendInfo("查询车辆里程信息", "H");
        }

        private void Context_Info_Click1(object sender, RoutedEventArgs e)
        {
            if (context_1.IsChecked == true)
            {
                inDist_flag = "1";
            }
            else if (context_2.IsChecked == true)
            {
                inDist_flag = "0";
            }
        }

        private void Context_Info_Click2(object sender, RoutedEventArgs e)
        {
            if (context_3.IsChecked == true)
            {
                clearDist_flag = "0";
            }
            else if (context_4.IsChecked == true)
            {
                clearDist_flag = "1";
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

        private string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd, string inDist_flag,string clearDist_flag)
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "HQCTRLCMD_TYPE");
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            jo.Add("inDist", inDist_flag);
            jo.Add("clearDist", clearDist_flag);
            jo.Add("cmdid", sim + "_HQCTRLCMD_TYPE_" + subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void sendInfo(string cmdname, string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype, inDist_flag,clearDist_flag);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(cmdname, StaticLoginInfo.GetInstance().UserName, inDist_flag + clearDist_flag, Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_HQCTRLCMD_TYPE_" + cmdtype;
                cmd.cmdContent = cmdname + ":" + inDist_flag + clearDist_flag;
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
