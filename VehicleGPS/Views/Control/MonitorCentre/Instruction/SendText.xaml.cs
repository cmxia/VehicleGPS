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
    /// SendText.xaml 的交互逻辑
    /// </summary>
    public partial class SendText : Window
    {
        private string context_emergent_flag = "0", context_show_flag = "1", context_TTS_read_flag = "0", context_adshow_flag = "1", context_flag = "0";
        private string sendContext;
        private string termType;
        public bool isDisplay = true;
        public SendText()
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
            //termType = VBaseInfo.GetInstance().GPSType;
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

        //发送指令
        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            //获取指令参数
            //string cmd_setparam_str = "";

            //sendContext = context_emergent_flag * 1 + context_show_flag * 4 + context_TTS_read_flag * 8 + context_adshow_flag * 16 + context_flag * 32;
            sendContext = context_emergent_flag + context_show_flag + context_TTS_read_flag + context_adshow_flag + context_flag;
            if ((Context.Text.Length > 0))
            {
                Result.Text = Texttest();
                if (Result.Text == "指令已发出，正在处理！")
                {
                    States.Text = "已发送";
                    List<MessageInfo> tmp = new List<MessageInfo>();
                    tmp = StaticMessageInfo.GetInstance().SendMessageList;

                    MessageInfo msg = new MessageInfo();
                    msg.SimID = this.SIM.Text.ToString();
                    msg.Time = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                    msg.VehicleNum = this.VehicleId.Text.ToString();
                    msg.Content = GetTextContent();
                    tmp.Add(msg);
                    while (true)
                    {
                        if (StaticTreeState.SendText == LoadingState.LOADCOMPLETE)
                        {
                            StaticTreeState.SendText = LoadingState.LOADING;
                            StaticMessageInfo.GetInstance().SendMessageList = tmp;
                            StaticTreeState.SendText = LoadingState.LOADCOMPLETE;
                            break;
                        }
                    }
                    Socket.ExcuteSql("文本信息下发", StaticLoginInfo.GetInstance().UserName, sendContext + Context.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                }
            }
            else
            {
                MessageBox.Show("文本信息不能为空！");
            }
        }
        private string GetTextContent()
        {
            string content = "";
            if (context_lead.IsChecked == true)
            {
                content += "(中心导航信息)";
            }
            else
            {
                content += "(CAN故障码信息)";
            }
            if (Emergency.IsChecked == true)
            {
                content += "[紧急]";
            }
            if (Show.IsChecked == true)
            {
                content += "[终端显示]";
            }
            if (TTSShow.IsChecked == true)
            {
                content += "[终端TTS播读]";
            }
            if (Adshow.IsChecked == true)
            {
                content += "[广告屏显示]";
            }
            content += ":" + Context.Text.ToString().Trim();
            return content;
        }
        

        private string Texttest()
        {

            string  rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "TEXTSENDCMD_TYPE");
            jo.Add("simId", SIM.Text);
            jo.Add("termType", termType);
            jo.Add("textFlag", sendContext);
            jo.Add("textContent", Context.Text);
            rebool = Socket.zmqInstructionsPack(SIM.Text, jo);
            return rebool;
        }

        //文本信息下发
        private void Context_Info_Click(object sender, RoutedEventArgs e)
        {
            if (context_lead.IsChecked == true)
            {
                context_flag = "0";
            }
            else if (context_can.IsChecked == true)
            {
                context_flag = "1";
            }
        }

        private void context_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.Content.ToString())
            {
                case "紧急":
                    context_emergent_flag = "1";
                    break;
                case "终端显示器显示":
                    context_show_flag = "1";
                    break;
                case "终端TTS播读":
                    context_TTS_read_flag = "1";
                    break;
                case "广告屏显示":
                    context_adshow_flag = "1";
                    break;
                default:
                    break;
            }

        }
    }
}
