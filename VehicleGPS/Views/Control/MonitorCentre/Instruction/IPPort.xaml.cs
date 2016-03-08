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
    /// IPPort.xaml 的交互逻辑
    /// </summary>
    public partial class IPPort : Window
    {
        private string termType;//终端类型
        private string ipportType;
        private string dataLength;
        public bool isDisplay = true;
        public IPPort(string e)
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            ipportType = e;
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
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            switch (ipportType)
            {
                case "主服务器IP和端口号":
                    sendInfo("19");
                    break;
                case "备份服务器IP和端口号":
                    sendInfo("23");
                    break;
            }
        }

        private void sendInfo(string cmdnum)
        {
            string data;
            //IP string类型
            string strip = tb_IP.Text;
            strip = toHex(strip);
            strip = dataLength + ";" + strip;
            //TCP端口 dword
            string strtcp = tb_TCP.Text;
            try
            {
                strtcp = toHex(uint.Parse(strtcp), 4);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请输入数字！");
                return;
            }
            strtcp = "4;" + strtcp;
            //UDP端口 dword
            string strudp = tb_UDP.Text;
            try
            {
                strtcp = toHex(uint.Parse(strudp), 4);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("请输入数字！");
                return;
            }
            strudp = "4;" + strudp;
            string cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_" + cmdnum;
            if (cmdnum=="19")
            {
                data = "19;" + strip + ";24;" + strtcp + ";25;" + strudp;
            }
            else
            {
                data = "23;" + strip + ";24;" + strtcp + ";25;" + strudp;
            }
            Result.Text = Texttest(SIM.Text, termType, data, cmdid);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(ipportType, StaticLoginInfo.GetInstance().UserName, tb_IP.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = cmdid;
                cmd.cmdContent = ipportType + ":" + tb_IP.Text.ToString();
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

        private string Texttest(string sim, string termType, string data ,string cmdid)
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "SETTERMPARAMCMD_TYPE");
            jo.Add("simId", sim);
            jo.Add("termType", termType);
            jo.Add("paramNum", "3");
            jo.Add("data", data);
            jo.Add("cmdid", cmdid);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private string toHex(string str)
        {
            string hexStr = "";
            //Unicode->Ascii
            byte[] asciiBytes = Encoding.GetEncoding("GB2312").GetBytes(str.ToCharArray());
            dataLength = asciiBytes.Length.ToString();
            //转换为16进制
            for (int i = 0; i < asciiBytes.Length; ++i)
            {
                int num = (int)(asciiBytes[i]);
                string tmpStr = num.ToString("X");
                if (tmpStr.Length != 2) hexStr += "0" + tmpStr;
                else hexStr += tmpStr;
            }
            return hexStr;
        }

        public static string toHex(uint num, int len)
        {
            string str = num.ToString("X");
            len *= 2;
            while (str.Length < len) str = "0" + str;
            return str;
        }
    }
}
