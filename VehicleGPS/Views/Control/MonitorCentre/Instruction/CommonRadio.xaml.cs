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
    /// CommonRadio.xaml 的交互逻辑
    /// </summary>
    public partial class CommonRadio : Window
    {
        private string termType;//终端类型
        private string context_flag = "0";
        private string CommonRadioType;
        public bool isDisplay = true;
        public CommonRadio(string e)
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            CommonRadioType = e;
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
            Title = e;
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
                case "位置汇报方案":
                    context_one.Content = "根据ACC状态";
                    context_two.Content = "根据登陆状态和ACC状态";
                    break;
                case "终端电话接听策略":
                    context_one.Content="自动接听";
                    context_two.Content="ACC ON时自动接听，OFF时手动接听";
                    break;
            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            string context_flag_add = "4;" + Common.toHex(uint.Parse(context_flag), 4);
            CommandInfo cmd = new CommandInfo();
            switch (CommonRadioType)
            {
                case "位置汇报方案":
                    Result.Text=Socket.Texttest(SIM.Text, termType, context_flag_add, "33", SIM.Text + "_SETTERMPARAMCMD_TYPE_33");
                    if (Result.Text == "指令已发出，正在处理！")
                    {
                        States.Text = "已发送";
                        Socket.ExcuteSql("位置汇报方案", StaticLoginInfo.GetInstance().UserName, context_flag, Result.Text, VBaseInfo.GetInstance().SIM);
                        cmd.cmdId = SIM.Text + "_SETTERMPARAMCMD_TYPE_33";
                        cmd.cmdContent = CommonRadioType + ":" + context_flag;
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
                case "终端电话接听策略":
                    Result.Text=Socket.Texttest(SIM.Text, termType, context_flag_add, "69", SIM.Text + "_SETTERMPARAMCMD_TYPE_69");
                    if (Result.Text == "指令已发出，正在处理！")
                    {
                        States.Text = "已发送";
                        Socket.ExcuteSql("终端电话接听策略", StaticLoginInfo.GetInstance().UserName, context_flag, Result.Text, VBaseInfo.GetInstance().SIM);
                        cmd.cmdId = SIM.Text + "_SETTERMPARAMCMD_TYPE_69";
                        cmd.cmdContent = CommonRadioType + ":" + context_flag;
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

        private void Context_Info_Click(object sender, RoutedEventArgs e)
        {
            if (context_one.IsChecked == true)
            {
                context_flag = "0";
            }
            else if (context_two.IsChecked == true)
            {
                context_flag = "1";
            }
        }
    }
}
