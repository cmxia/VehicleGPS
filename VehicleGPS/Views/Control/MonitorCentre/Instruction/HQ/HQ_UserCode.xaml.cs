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
using Newtonsoft.Json;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ
{
    /// <summary>
    /// HQ_UserCode.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_UserCode : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        List<itemInfo> itemList = new List<itemInfo>();
        public bool isDisplay = true;
        public HQ_UserCode()
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
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb1.Text) && string.IsNullOrEmpty(tb2.Text) && string.IsNullOrEmpty(tb3.Text)
                && string.IsNullOrEmpty(tb5.Text) && string.IsNullOrEmpty(tb6.Text)==true)
            {
                MessageBox.Show("请先设置再发送指令！");
                return;
            }
            if (tb1.Text.Length > 0)
            {
                itemInfo item1 = new itemInfo();
                item1.key = "1";
                item1.data = tb1.Text;
                itemList.Add(item1);
            }
            if (tb2.Text.Length > 0)
            {
                itemInfo item2 = new itemInfo();
                item2.key = "2";
                item2.data = tb2.Text;
                itemList.Add(item2);
            }
            if (tb3.Text.Length > 0)
            {
                itemInfo item3 = new itemInfo();
                item3.key = "3";
                item3.data = tb3.Text;
                itemList.Add(item3);
            }
            if (tb5.Text.Length > 0 || tb6.Text.Length > 0)
            {
                itemInfo item4 = new itemInfo();
                item4.key = "4";
                item4.cid = tb4.Text;
                item4.type = tb5.Text;
                item4.apn = tb6.Text;
                itemList.Add(item4);
            }
            sendInfo("更改用户名、密码、拨号号码、APN", "B");
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

        private string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd, List<itemInfo> list)
        {
            string stritem = JsonConvert.SerializeObject(list);
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "HQNETWORKCONFCMD_TYPE");
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            jo.Add("itemList", stritem);
            jo.Add("cmdid", sim + "_HQNETWORKCONFCMD_TYPE_" + subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void sendInfo(string cmdname, string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype,itemList);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(cmdname, StaticLoginInfo.GetInstance().UserName, JsonConvert.SerializeObject(itemList), Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_HQNETWORKCONFCMD_TYPE_" + cmdtype;
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

        public class itemInfo
        {
            public string key { get; set; }
            public string data { get; set; }
            public string cid { get; set; }
            public string type { get; set; }
            public string apn { get; set; }
        }
    }
}