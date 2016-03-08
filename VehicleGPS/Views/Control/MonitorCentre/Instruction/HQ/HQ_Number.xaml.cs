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
using ZeroMQ;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ
{
    /// <summary>
    /// HQ_Number.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_Number : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string type;
        //private string itemList = "";
        List<itemInfo> itemList = new List<itemInfo>();
        public bool isDisplay = true;
        public HQ_Number(string e)
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
            type = e;
            Title = e;
            switch (e)
            {
                case "设置号码":
                    lb_Number1.Text = "设置第一服务中心号码";
                    lb_Number2.Text = "设置第二服务中心号码";
                    lb_Number3.Text = "设置求助电话号码";
                    lb_Number4.Text = "设置DTMF报警电话号码";
                    lb_Number5.Text = "设置整机重起电话号码";
                    lb_Number6.Text = "设置短信息服务中心号码";
                    lb_Number7.Text = "设置终端SIM卡号码";
                    break;
                case "设置密码":
                    lb_Number1.Text = "设置第一司机登录密码";
                    lb_Number2.Text = "设置第二司机登录密码";
                    sp_2.Visibility = Visibility.Collapsed;
                    sp_3.Visibility = Visibility.Collapsed;
                    sp_4.Visibility = Visibility.Collapsed;
                    break;
                case "其他设置":
                    lb_Number1.Text = "设置速度报警的报警值";
                    lb_Number2.Text = "设置关ACC进入省电模式的时间";
                    lb_Number3.Text = "设置存储历史数据时间间隔";
                    lb_Number4.Text = "设置短信标识";
                    sp_3.Visibility = Visibility.Collapsed;
                    sp_4.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case "设置号码":
                    if (tb_Number1.Text.Length==0&&tb_Number2.Text.Length==0&&
                        tb_Number3.Text.Length==0&&tb_Number4.Text.Length==0&&
                        tb_Number5.Text.Length==0&&tb_Number6.Text.Length==0&&
                        tb_Number7.Text.Length==0)
                    {
                        MessageBox.Show("输入框不能均为空！");
                    }
                    else
                    {
                        if (tb_Number1.Text.Length>0)
                        {
                            //itemList = "1" + tb_Number1.Text + ";";
                            itemInfo item1 = new itemInfo();
                            item1.key = "1";
                            item1.value = tb_Number1.Text;
                            itemList.Add(item1);
                        }
                        if (tb_Number2.Text.Length>0)
                        {
                            //itemList += "2" + tb_Number2.Text + ";";
                            itemInfo item2 = new itemInfo();
                            item2.key = "2";
                            item2.value = tb_Number2.Text;
                            itemList.Add(item2);
                        }
                        if (tb_Number3.Text.Length > 0)
                        {
                            //itemList += "4" + tb_Number3.Text + ";";
                            itemInfo item3 = new itemInfo();
                            item3.key = "4";
                            item3.value = tb_Number3.Text;
                            itemList.Add(item3);
                        }
                        if (tb_Number4.Text.Length > 0)
                        {
                            //itemList += "5" + tb_Number4.Text + ";";
                            itemInfo item4 = new itemInfo();
                            item4.key = "5";
                            item4.value = tb_Number4.Text;
                            itemList.Add(item4);
                        }
                        if (tb_Number5.Text.Length > 0)
                        {
                            //itemList += "6" + tb_Number5.Text + ";";
                            itemInfo item5 = new itemInfo();
                            item5.key = "6";
                            item5.value = tb_Number5.Text;
                            itemList.Add(item5);
                        }
                        if (tb_Number6.Text.Length > 0)
                        {
                            //itemList += "7" + tb_Number6.Text + ";";
                            itemInfo item6 = new itemInfo();
                            item6.key = "7";
                            item6.value = tb_Number6.Text;
                            itemList.Add(item6);
                        }
                        if (tb_Number7.Text.Length > 0)
                        {
                            //itemList += "8" + tb_Number7.Text + ";";
                            itemInfo item7 = new itemInfo();
                            item7.key = "8";
                            item7.value = tb_Number7.Text;
                            itemList.Add(item7);
                        }
                        sendInfo("A");
                    }
                    break;
                case "设置密码":
                    if (tb_Number1.Text.Length==0&&tb_Number2.Text.Length==0)
                    {
                        MessageBox.Show("输入框不能均为空！");
                    }
                    else
                    {
                        if (tb_Number1.Text.Length > 0)
                        {
                            //itemList = "1" + tb_Number1.Text + ";";
                            itemInfo item1 = new itemInfo();
                            item1.key = "1";
                            item1.value = tb_Number1.Text;
                            itemList.Add(item1);
                        }
                        if (tb_Number2.Text.Length > 0)
                        {
                            //itemList += "2" + tb_Number2.Text + ";";
                            itemInfo item2 = new itemInfo();
                            item2.key = "2";
                            item2.value = tb_Number2.Text;
                            itemList.Add(item2);
                        }
                        sendInfo("B");
                    }
                    
                    break;
                case "其他设置":
                    if (tb_Number1.Text.Length==0&&tb_Number2.Text.Length==0&&
                        tb_Number3.Text.Length==0&&tb_Number4.Text.Length==0)
                    {
                        MessageBox.Show("输入框不能均为空！");
                    }
                    else
                    {
                        if (tb_Number1.Text.Length > 0)
                        {
                            //itemList = "1" + tb_Number1.Text + ";";
                            itemInfo item1 = new itemInfo();
                            item1.key = "1";
                            item1.value = tb_Number1.Text;
                            itemList.Add(item1);
                        }
                        if (tb_Number2.Text.Length > 0)
                        {
                            //itemList += "2" + tb_Number2.Text + ";";
                            itemInfo item2 = new itemInfo();
                            item2.key = "2";
                            item2.value = tb_Number2.Text;
                            itemList.Add(item2);
                        }
                        if (tb_Number3.Text.Length > 0)
                        {
                            //itemList += "3" + tb_Number3.Text + ";";
                            itemInfo item3 = new itemInfo();
                            item3.key = "3";
                            item3.value = tb_Number3.Text;
                            itemList.Add(item3);
                        }
                        if (tb_Number4.Text.Length > 0)
                        {
                            //itemList += "4" + tb_Number4.Text + ";";
                            itemInfo item4 = new itemInfo();
                            item4.key = "4";
                            item4.value = tb_Number4.Text;
                            itemList.Add(item4);
                        }
                        sendInfo("H");
                    }
                    break;
            }
        }

        private void sendInfo(string cmdtype)
        {
            Result.Text = Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype, itemList);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(type, StaticLoginInfo.GetInstance().UserName, JsonConvert.SerializeObject(itemList), Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_HQSETTINGCMD_TYPE_" + cmdtype;
                cmd.cmdContent = type + ":" + JsonConvert.SerializeObject(itemList);
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
            else
            {
                States.Text = "未发送";
                Result.Text = "发送失败";
                //Socket.ExcuteSql(type, StaticLoginInfo.GetInstance().UserName, itemList, Result.Text, VBaseInfo.GetInstance().SIM);
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

        public static string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd, List<itemInfo> list)
        {
            string stritem = JsonConvert.SerializeObject(list);
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "HQSETTINGCMD_TYPE ");
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            jo.Add("itemList", stritem);
            jo.Add("cmdid", sim + "_HQSETTINGCMD_TYPE_"+subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        public class itemInfo
        {
            public string key { get; set; }
            public string value { get; set; }
        }
    }
}
