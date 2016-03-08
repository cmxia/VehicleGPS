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
    /// Common.xaml 的交互逻辑
    /// </summary>
    public partial class Common : Window
    {
        private string termType;//终端类型
        private string commonType;
        private string[] cmdData;//[0]:命令ID，[1]：命令长度，[2]：命令数据类型
        public bool isDisplay = true;
        public Common(string e)
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            string[] splitArr = e.Split('|');
            cmdData = splitArr[0].Split(';');
            commonType = splitArr[1];
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
            Title = commonType;
            lb_Common.Text = commonType;
            switch (commonType)
            {
                case "图像/视频质量":
                    lb_CommonAdd.Text = "(1-10,1最好)";
                    break;
                case "亮度":
                case "色度":
                    lb_CommonAdd.Text = "(0-255)";
                    break;
                case "对比度":
                case "饱和度":
                    lb_CommonAdd.Text = "(0-127)";
                    break;
                default:
                    lb_CommonAdd.IsEnabled = false;
                    break;
            }
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

        public static string toHex(uint num, int len)
        {
            string str = num.ToString("X");
            len *= 2;
            while (str.Length < len) str = "0" + str;
            return str;
        }
        private string toHex(string str)
        {
            string hexStr = "";
            //Unicode->Ascii
            byte[] asciiBytes = Encoding.GetEncoding("GB2312").GetBytes(str.ToCharArray());
            cmdData[1] = asciiBytes.Length.ToString();
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
        private void sendInfo(string cmdnum)
        {
            //根据命令数据长度转换数据
            string str = tb_Common.Text;
            switch (cmdData[2])
            {
                case "0":   //DWORD
                    {
                        try
                        {
                            str = toHex(uint.Parse(str), 4);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("请输入数字！");
                            return;
                        }
                    }
                    break;
                case "1":
                    {
                        try
                        {
                            str = toHex(uint.Parse(str), 2);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("请输入数字！");
                            return;
                        }
                    }
                    break;
                case "2":
                    {
                        try
                        {
                            str = toHex(uint.Parse(str), 1);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("请输入数字！");
                            return;
                        }
                    }
                    break;
                case "3":
                    {
                        str = toHex(str);
                    }
                    break;
            }
            str = cmdData[1] + ";" + str;
            string cmdid = SIM.Text + "_SETTERMPARAMCMD_TYPE_" + cmdnum;
            Result.Text = Socket.Texttest(SIM.Text, termType, str, cmdnum, cmdid);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(commonType, StaticLoginInfo.GetInstance().UserName, tb_Common.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = cmdid;
                cmd.cmdContent = commonType + ":" + tb_Common.Text.ToString();
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

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if ((tb_Common.Text.Length > 0))
            {
                switch (commonType)
                {
                    case "终端心跳发送时间间隔(s)":
                        sendInfo("1");
                        break;
                    case "TCP消息应答超时时间(s)":
                        sendInfo("2");
                        break;
                    case "TCP消息重传次数":
                        sendInfo("3");
                        break;
                    case "UDP消息应答超时时间(s)":
                        sendInfo("4");
                        break;
                    case "UDP消息重传次数":
                        sendInfo("5");
                        break;
                    case "SMS消息应答超时时间(s)":
                        sendInfo("6");
                        break;
                    case "SMS消息重传次数":
                        sendInfo("7");
                        break;
                    case "主服务器APN":
                        sendInfo("16");
                        break;
                    case "主服务器无线通信拨号用户名":
                        sendInfo("17");
                        break;
                    case "主服务器无线通信拨号密码":
                        sendInfo("18");
                        break;
                    case "主服务器地址，IP或域名":
                        sendInfo("19");
                        break;
                    case "备份服务器APN":
                        sendInfo("20");
                        break;
                    case "备份服务器无线通信拨号用户名":
                        sendInfo("21");
                        break;
                    case "备份服务器无线通信拨号密码":
                        sendInfo("22");
                        break;
                    case "备份服务器地址，IP或域名":
                        sendInfo("23");
                        break;
                    case "TCP端口":
                        sendInfo("24");
                        break;
                    case "UDP端口":
                        sendInfo("25");
                        break;
                    case "驾驶员未登录汇报时间间隔(s)":
                        sendInfo("34");
                        break;
                    case "休眠时汇报时间间隔(s)":
                        sendInfo("39");
                        break;
                    case "紧急报警时汇报时间间隔(s)":
                        sendInfo("40");
                        break;
                    case "缺省时间汇报间隔(s)":
                        sendInfo("41");
                        break;
                    case "驾驶员未登录汇报距离间隔(m)":
                        sendInfo("45");
                        break;
                    case "休眠时汇报距离间隔(m)":
                        sendInfo("46");
                        break;
                    case "紧急报警时汇报距离间隔(m)":
                        sendInfo("47");
                        break;
                    case "缺省距离汇报间隔(m)":
                        sendInfo("44");
                        break;
                    case "拐点补传角度(小于180)":
                        sendInfo("48");
                        break;
                    case "监控平台电话号码":
                        sendInfo("64");
                        break;
                    case "复位电话号码":
                        sendInfo("65");
                        break;
                    case "恢复出厂设置电话号码":
                        sendInfo("66");
                        break;
                    case "监控平台SMS电话号码":
                        sendInfo("67");
                        break;
                    case "接收终端SMS文本报警号码":
                        sendInfo("68");
                        break;
                    case "每次最长通话时间(s)":
                        sendInfo("70");
                        break;
                    case "当月最长通话时间(s)":
                        sendInfo("71");
                        break;
                    case "监听电话号码":
                        sendInfo("72");
                        break;
                    case "监管平台特权短信号码":
                        sendInfo("73");
                        break;
                    case "最高速度(km/h)":
                        sendInfo("85");
                        break;
                    case "超速持续时间(s)":
                        sendInfo("86");
                        break;
                    case "连续驾驶时间门限(s)":
                        sendInfo("87");
                        break;
                    case "当天累计驾驶时间门限(s)":
                        sendInfo("88");
                        break;
                    case "最小休息时间(s)":
                        sendInfo("89");
                        break;
                    case "最长停车时间(s)":
                        sendInfo("90");
                        break;
                    case "图像/视频质量":
                        sendInfo("112");
                        break;
                    case "亮度":
                        sendInfo("113");
                        break;
                    case "对比度":
                        sendInfo("114");
                        break;
                    case "饱和度":
                        sendInfo("115");
                        break;
                    case "色度":
                        sendInfo("116");
                        break;
                    case "车辆里程表读数":
                        sendInfo("128");
                        break;
                    case "车辆所在省域ID":
                        sendInfo("129");
                        break;
                    case "车辆所在市域ID":
                        sendInfo("130");
                        break;
                    case "机动车号牌":
                        sendInfo("131");
                        break;
                }
            }
            else
            {
                MessageBox.Show("输入框不能为空！");
            }
        }
    }
}
