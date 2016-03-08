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
    /// HQ_EmegentPic.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_EmegentPic : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string type;
        public bool isDisplay = true;
        public HQ_EmegentPic(string e)
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
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (tb1.Text.Length == 0)
            {
                MessageBox.Show("请设置时间间隔！");
                return;
            }
            if (tb2.Text.Length == 0)
            {
                MessageBox.Show("请设置抓拍次数！");
                return;
            }
            if (type == "紧急情况下（盗警、劫警）图像抓拍的时间间隔以及抓拍次数")
            {
                sendInfo("紧急情况下（盗警、劫警）图像抓拍的时间间隔以及抓拍次数", "E");
            }
            else
            {
                sendInfo("要求上传多幅图像", "C");
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
            jo.Add("cmd", "HQMMEDIACMD_TYPE");
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            jo.Add("interval", tb1.Text);
            jo.Add("times", tb2.Text);
            jo.Add("cmdid", sim + "_HQMMEDIACMD_TYPE_" + subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void sendInfo(string cmdname, string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(cmdname, StaticLoginInfo.GetInstance().UserName, tb1.Text + tb2.Text , Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_HQMMEDIACMD_TYPE_" + cmdtype;
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