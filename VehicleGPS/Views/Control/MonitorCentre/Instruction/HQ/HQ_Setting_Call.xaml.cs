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
    /// HQ_Setting_Scope.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_Setting_Call : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string itemList = "";
        private string type;
        private string isdigitReply = "1";
        private int phoneNum = 0;
        public bool isDisplay = true;
        public HQ_Setting_Call(string e)
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
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (tcall_limit.Text.Length == 0 && trecvLimit.Text.Length == 0 &&
                       tsite0.Text.Length == 0 && tphone0.Text.Length == 0)
            {
                MessageBox.Show("输入框不能均为空！");
            }
            else {
                if (tcall_limit.Text.Length > 0)
                {
                    itemList = "1;" + tcall_limit.Text + ";";
                }
                if (trecvLimit.Text.Length > 0)
                {
                    itemList += "2;" + trecvLimit.Text + ";";
                }
                foreach (var tb in phoneList.Children)
                {
                    phoneNum++;
                    StackPanel sp = (StackPanel)tb;
                    TextBox tbx = (TextBox)sp.Children[1];
                    string count = (phoneNum + 2).ToString() + ";";
                    itemList += count + tbx.Text + ";";
                }
                itemList += (phoneNum + 3).ToString()+";" + isdigitReply + ";";
                sendInfo("G");
            }
        }

        private void sendInfo(string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype, itemList);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql(type, StaticLoginInfo.GetInstance().UserName, itemList, Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_HQSETTINGCMD_TYPE_" + cmdtype;
                cmd.cmdContent = type + ":" + itemList;
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

        public static string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd, string itemList)
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "HQSETTINGCMD_TYPE ");
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            jo.Add("itemList", itemList);
            jo.Add("cmdid", sim + "_HQSETTINGCMD_TYPE_" + subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void AddPhone_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = new StackPanel();
            sp.Width = 336;
            sp.Orientation = Orientation.Horizontal;
            sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tbl = new TextBlock();
            tbl.Text = "电话号码在sim卡中的位置";
            tbl.Margin = new Thickness(5,5,5,5);
            tbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbl.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbx = new TextBox();
            tbx.Name = "tsite" + phoneNum;
            tbx.Width = 100;
            tbx.Margin = new Thickness(5, 5, 5, 5);
            tbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbl.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            sp.Children.Add(tbl);
            sp.Children.Add(tbx);

            StackPanel spp = new StackPanel();
            spp.Width = 336;
            spp.Orientation = Orientation.Horizontal;
            spp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            spp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tblp = new TextBlock();
            tblp.Text = "电话号码";
            tblp.Margin = new Thickness(5, 5, 5, 5);
            tblp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tblp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbxp = new TextBox();
            tbxp.Name = "tphone" + phoneNum;
            tbxp.Width = 100;
            tbxp.Margin = new Thickness(5, 5, 5, 5);
            tblp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tblp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            spp.Children.Add(tblp);
            spp.Children.Add(tbxp);

            phoneList.Children.Add(sp);
            phoneList.Children.Add(spp);
        }
    }
}
