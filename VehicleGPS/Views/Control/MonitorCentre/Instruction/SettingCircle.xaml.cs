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
using VehicleGPS.ViewModels.MonitorCentre.Instruction;
using Newtonsoft.Json.Linq;
using ZeroMQ;
using Newtonsoft.Json;
using VehicleGPS.Models.Login;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// SettingCircle.xaml 的交互逻辑
    /// </summary>
    public partial class SettingCircle : Window
    {
        private string timeFlag="0";
        private string speedFlag = "0";
        private string InAlarmFlag = "0";
        private string InPlatformFlag = "0";
        private string OutAlamFlag = "0";
        private string OutPlatformFlag = "0";
        private string latitudeFlag = "0";
        private string longitudeFlag = "0";
        private string doorFlag = "0";
        private string socketFlag = "0";
        private string GNSSFlag = "0";
        private string setType = "0";
        List<region> regionList = new List<region>();
        public SettingCircle()
        {
            InitializeComponent();
            InitRegionGeoInfo();
            //areaType = e;
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
            //Title = e;
            //switch (e)
            //{
            //    case "设置矩形区域":
            //        TB_9.Visibility = Visibility.Visible;
            //        tb_9.Visibility = Visibility.Visible;
            //        TB_2.Text = "左上点纬度";
            //        TB_3.Text = "左上点经度";
            //        TB_4.Text = "右下点纬度";
            //        break;
            //}
        }

        /// <summary>
        /// 初始化区域地理信息
        /// </summary>
        private void InitRegionGeoInfo()
        {
            //string regionInfo = InstructionViewModel.GetInstance().MapService.GetCircleInfo();
            //string[] regionInfoMat = regionInfo.Split(';');
            //this.tb_2.Text = regionInfoMat[1];
            //this.tb_3.Text = regionInfoMat[0];
            //this.tb_4.Text = regionInfoMat[2];
        }

        private void Context_Info_Click(object sender, RoutedEventArgs e)
        {
            latitudeFlag=(context_5.IsChecked == true) ?  "1" : "0";
            longitudeFlag = (context_7.IsChecked == true) ? "1" : "0";
            doorFlag = (context_9.IsChecked == true) ? "1" : "0";
            socketFlag = (context_11.IsChecked == true) ? "1" : "0";
            GNSSFlag = (context_13.IsChecked == true) ? "1" : "0";
            if (context_2.IsChecked==true)
            {
                setType = "1";
            }
            else if (context_3.IsChecked==true)
            {
                setType="2";
            }
        }

        private void context_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch (cb.Content.ToString())
            {
                case "根据时间":
                    if (Time.IsChecked==false)
                    {
                        timeFlag = "0";
                        sp_time.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        timeFlag = "1";
                        sp_time.Visibility = Visibility.Visible;
                    }
                    break;
                case "限速":
                    if (Speed.IsChecked == false)
                    {
                        speedFlag = "0";
                        sp_speed.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        speedFlag = "1";
                        sp_speed.Visibility = Visibility.Visible;
                    }
                    break;
                case "进区域报警给驾驶员":
                    InAlarmFlag = "1";
                    break;
                case "进区域报警给平台":
                    InPlatformFlag = "1";
                    break;
                case "出区域报警给驾驶员":
                    OutAlamFlag = "1";
                    break;
                case "出区域报警给平台":
                    OutPlatformFlag = "1";
                    break;
                default:
                    break;
            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            region reg = new region();
            if (string.IsNullOrEmpty(tb_1.Text))
            {
                MessageBox.Show("请输入区域ID", "提示", MessageBoxButton.OK);
                return;
            }
            reg.id = tb_1.Text;
            reg.property = (int.Parse(timeFlag) * 1 + int.Parse(speedFlag) * 2 + int.Parse(InAlarmFlag)*4
                +int.Parse(InPlatformFlag)*8+int.Parse(OutAlamFlag)*16+int.Parse(OutPlatformFlag)*32
                +int.Parse(latitudeFlag)*64+int.Parse(longitudeFlag)*128+int.Parse(doorFlag)*256
                +int.Parse(socketFlag)*16384+int.Parse(GNSSFlag)*32768).ToString();
            reg.latitude = tb_2.Text;
            reg.longitude = tb_3.Text;
            reg.radius = tb_4.Text;
            reg.start = dtp_BeginTime.Value.ToString();
            reg.end = dtp_EndTime.Value.ToString();
            reg.maxSpeed = tb_7.Text;
            reg.duration = tb_8.Text;
            regionList.Add(reg);
            if (Texttest())
            {
                States.Text = "已发送";
                Result.Text = "发送成功";
                Socket.ExcuteSql("设置圆形区域", StaticLoginInfo.GetInstance().UserName, tb_1.Text, Result.Text, VBaseInfo.GetInstance().SIM);
            }
            else
            {
                States.Text = "未发送";
                Result.Text = "发送失败";
            }
            CommandInfo cmd = new CommandInfo();
            cmd.cmdContent = "设置圆形区域" + ":" + tb_1.Text;
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

        /// </summary>
        /// <param name="sim">sim卡号</param>
        /// <param name="insBody">指令包体</param>
        /// <returns>返回值 true 代表发送成功 false 代表发送失败
        /// </returns>
        private bool zmqInstructionsPack(string sim, JObject insBody)
        {
            bool reFlag = false;
            if (sim != null && insBody != null)
            {
                using (var InsCTX = ZmqContext.Create())
                {
                    using (var InsSockt = InsCTX.CreateSocket(SocketType.REQ))
                    {
                        InsSockt.Connect(Socket.DoubleInfoRemoutEndPoint);
                        //Log.WriteLog("ins", sim);
                        JObject jo = new JObject();
                        jo.Add("GType", "ins");
                        jo.Add("GpsBasic", insBody);
                        jo.Add("GpsAttatch", sim);
                        ZmqMessage msg = new ZmqMessage();
                        TimeSpan timeOut = new TimeSpan(0, 0, 0, 20, 0);//接收等待超时为20秒
                        msg.Append(Encoding.UTF8.GetBytes(jo.ToString()));
                        String str = jo.ToString();
                        InsSockt.SendMessage(msg);
                        string callBack = InsSockt.Receive(Encoding.UTF8, timeOut);

                        if (callBack == null)
                            reFlag = false;
                        else
                            if (callBack.Equals("1"))
                                reFlag = true;
                            else
                                reFlag = false;

                        InsSockt.Disconnect(Socket.DoubleInfoRemoutEndPoint);
                    }
                }
            }
            return reFlag;
        }

        private bool Texttest()
        {

            bool rebool = false;
            JObject jo = new JObject();
            jo.Add("cmd", "SETCIRCLEREGIONCMD_TYPE");
            jo.Add("simId", SIM.Text);
            jo.Add("type", setType);
            jo.Add("regionList", JsonConvert.SerializeObject(regionList));
            rebool = zmqInstructionsPack(SIM.Text, jo);
            return rebool;
        }

        public class region
        {
            public string id { get; set; }
            public string property { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string radius { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string maxSpeed { get; set; }
            public string duration { get; set; }
        }
    }
}
