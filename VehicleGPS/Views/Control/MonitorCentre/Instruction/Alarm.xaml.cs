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
    /// Alarm.xaml 的交互逻辑
    /// </summary>
    public partial class Alarm : Window
    {
        private string context_checkBox_flag = "0";
        private string termType;
        private string AlarmType;
        public bool isDisplay = true;
        public Alarm(string e)
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            this.DataContext = VBaseInfo.GetInstance();
            Title=e;
            AlarmType = e;
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
            switch (e)
            {
                case "报警屏蔽字":
                    ck_Name.Content = "报警屏蔽字，与位置信息汇报消息中的报警标志相对应";
                    break;
                case "报警发送文本SMS开关":
                    ck_Name.Content = "报警发送文本SMS 开关，与位置信息汇报消息中的报警标志相对应";
                    break;
                case "报警拍摄开关":
                    ck_Name.Content = "报警拍摄开关，与位置信息汇报消息中的报警标志相对应";
                    break;
                case "报警拍摄存储标志":
                    ck_Name.Content = "报警拍摄存储标志，与位置信息汇报消息中的报警标志相对应";
                    break;
                case "关键标志":
                    ck_Name.Content = "关键标志，与位置信息汇报消息中的报警标志相对应";
                    break;
            }
        }

        //发送指令
        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            context_checkBox_flag = "4;" + Common.toHex(uint.Parse(context_checkBox_flag), 4);
            string cmdid;
            CommandInfo cmd = new CommandInfo();
            switch (AlarmType)
            {
                case "报警屏蔽字":
                    cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_80";
                    Result.Text =Socket.Texttest(SIM.Text, termType, context_checkBox_flag, "80", cmdid);
                    if (Result.Text =="指令已发出，正在处理！")
                    {
                        States.Text = "已发送";
                        Socket.ExcuteSql(AlarmType, StaticLoginInfo.GetInstance().UserName, ck_Name.Content.ToString(), Result.Text, VBaseInfo.GetInstance().SIM);
                        cmd.cmdId = cmdid;                        
                        cmd.cmdContent = AlarmType + ":" + ck_Name.Content.ToString();
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
                    break;
                case "报警发送文本SMS开关":
                    cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_81";
                    Result.Text = Socket.Texttest(SIM.Text, termType, context_checkBox_flag, "81", cmdid);
                    if (Result.Text =="指令已发出，正在处理！")
                    {
                        States.Text = "已发送";
                        Socket.ExcuteSql(AlarmType, StaticLoginInfo.GetInstance().UserName, ck_Name.Content.ToString(), Result.Text, VBaseInfo.GetInstance().SIM);
                        cmd.cmdId = cmdid;
                        cmd.cmdContent = AlarmType + ":" + ck_Name.Content.ToString();
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
                    break;
                case "报警拍摄开关":
                    cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_82";
                    Result.Text=Socket.Texttest(SIM.Text, termType, context_checkBox_flag, "82",cmdid);
                    if (Result.Text == "指令已发出，正在处理！")
                    {
                        States.Text = "已发送";
                        Socket.ExcuteSql(AlarmType, StaticLoginInfo.GetInstance().UserName, ck_Name.Content.ToString(), Result.Text, VBaseInfo.GetInstance().SIM);
                        cmd.cmdId = cmdid;
                        cmd.cmdContent = AlarmType + ":" + ck_Name.Content.ToString();
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
                    break;
                case "报警拍摄存储标志":
                    cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_83";
                    Result.Text = Socket.Texttest(SIM.Text, termType, context_checkBox_flag, "83", cmdid);
                    if (Result.Text =="指令已发出，正在处理！")
                    {
                        States.Text = "已发送";
                        Socket.ExcuteSql(AlarmType, StaticLoginInfo.GetInstance().UserName, ck_Name.Content.ToString(), Result.Text, VBaseInfo.GetInstance().SIM);
                        cmd.cmdId = cmdid;
                        cmd.cmdContent = AlarmType + ":" + ck_Name.Content.ToString();
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
                    break;
                case "关键标志":
                    cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_84";
                    Result.Text = Socket.Texttest(SIM.Text, termType, context_checkBox_flag, "84", cmdid);
                    if (Result.Text =="指令已发出，正在处理！")
                    {
                        States.Text = "已发送";
                        Socket.ExcuteSql(AlarmType, StaticLoginInfo.GetInstance().UserName, ck_Name.Content.ToString(), Result.Text, VBaseInfo.GetInstance().SIM);
                        cmd.cmdId = cmdid;
                        cmd.cmdContent = AlarmType + ":" + ck_Name.Content.ToString();
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
                    break;
            }  
        }


        private void context_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked==true)
            {
                context_checkBox_flag="1";
            }
            
        }
    }
}
