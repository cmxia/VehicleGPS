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

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// Heartbeat.xaml 的交互逻辑
    /// </summary>
    public partial class Heartbeat : Window
    {
        private string termType;//终端类型
        public Heartbeat()
        {
            InitializeComponent();
            this.DataContext = VBaseInfo.GetInstance();
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
            if ((HeartBeat.Text.Length > 0))
            {

                if (Texttest())
                {
                    States.Text = "已发送";
                    Result.Text = "发送成功";

                }
                else
                {
                    States.Text = "未发送";
                    Result.Text = "发送失败";
                }
            }
            else
            {
                MessageBox.Show("时间间隔不能为空！");
            }
        }

        public static string PubSubRemoutEndPoint = "tcp://" + VehicleConfig.GetInstance().BUSINESSIP + ":" + VehicleConfig.GetInstance().BUSINESSPORT;//推送监听地址
        public static string DoubleInfoRemoutEndPoint = "tcp://" + VehicleConfig.GetInstance().INSTRUCTIONIP + ":" + VehicleConfig.GetInstance().INSTRUCTIONPORT;//双向通信连接地址
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sim">sim卡号</param>
        /// <param name="insBody">指令包体</param>
        /// <returns>返回值 true 代表发送成功 false 代表发送失败
        /// </returns>
        public static bool zmqInstructionsPack(string sim, string insBody)
        {
            bool reFlag = false;
            if (sim != null && insBody != null)
            {
                using (var InsCTX = ZmqContext.Create())
                {
                    using (var InsSockt = InsCTX.CreateSocket(SocketType.REQ))
                    {
                        InsSockt.Connect(DoubleInfoRemoutEndPoint);
                        //Log.WriteLog("ins", sim);
                        JObject jo = new JObject();
                        jo.Add("GType", "ins");
                        jo.Add("GpsBasic", insBody);
                        jo.Add("GpsAttatch", sim);
                        ZmqMessage msg = new ZmqMessage();
                        TimeSpan timeOut = new TimeSpan(0, 0, 1, 0, 0);//接收等待超时为1分钟
                        msg.Append(Encoding.UTF8.GetBytes(jo.ToString()));
                        InsSockt.SendMessage(msg);
                        string callBack = InsSockt.Receive(Encoding.UTF8, timeOut);

                        if (callBack == null)
                            reFlag = false;
                        else
                            if (callBack.Equals("1"))
                                reFlag = true;
                            else
                                reFlag = false;

                        InsSockt.Disconnect(DoubleInfoRemoutEndPoint);
                    }
                }
            }
            return reFlag;
        }

        private bool Texttest()
        {

            bool rebool = false;
            JObject jo = new JObject();
            jo.Add("cmd", "SETTERMPARAMCMD_TYPE");
            jo.Add("simId", SIM.Text);
            jo.Add("termType", termType);
            jo.Add("paramNum", "1");
            jo.Add("data", "1;" + HeartBeat.Text);
            rebool = zmqInstructionsPack(SIM.Text, jo.ToString());
            return rebool;
        }
    }
}
