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
    /// HQ_NoneData.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_NoneData : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string type;
        public bool isDisplay = true;
        public HQ_NoneData(string e)
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
            if ("不带范围的点名信息"==e)
            {
                ck_Reply.IsEnabled = false;
            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case "不带范围的点名信息":
                    sendInfo(type, "E");
                    break;
                case "解除终端报警":
                    sendInfo(type, "C");
                    break;
                case "查询车辆电瓶电压信息":
                    sendInfo(type, "G");
                    break;
                case "要求上传一幅图像":
                    sendInfo(type, "A");
                    break;
                case "查询终端参数1":
                    sendInfo(type,"J");
                    break;
                case "查询终端参数2":
                    sendInfo(type,"N");
                    break;
                case "查询终端产品ID":
                    sendInfo(type,"L");
                    break;
            }
        }

        private string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd)
        {
            string rebool = "";
            JObject jo = new JObject();
            if (type == "要求上传一幅图像")
            {
                jo.Add("cmd", "HQMMEDIACMD_TYPE");
                jo.Add("cmdid", sim + "_HQMMEDIACMD_TYPE_" + subcmd);
            }
            else if (type == "查询终端参数1" || type == "查询终端参数2" || type == "查询终端产品ID")
            {
                jo.Add("cmd", "HQSETTINGCMD_TYPE");
                jo.Add("cmdid", sim + "_HQSETTINGCMD_TYPE_" + subcmd);
            }
            else
            {
                jo.Add("cmd", "HQCTRLCMD_TYPE");
                jo.Add("cmdid", sim + "_HQCTRLCMD_TYPE_" + subcmd);
            }
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void sendInfo(string cmdname, string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(cmdname, StaticLoginInfo.GetInstance().UserName, "", Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                if (type == "要求上传一幅图像")
                {
                    cmd.cmdId = SIM.Text + "_HQMMEDIACMD_TYPE_" + cmdtype;
                }
                else if (type == "查询终端参数1" || type == "查询终端参数2" || type == "查询终端产品ID")
                {
                    cmd.cmdId = SIM.Text + "_HQSETTINGCMD_TYPE_" + cmdtype;
                }
                else
                {
                    cmd.cmdId = SIM.Text + "_HQCTRLCMD_TYPE_" + cmdtype;
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
