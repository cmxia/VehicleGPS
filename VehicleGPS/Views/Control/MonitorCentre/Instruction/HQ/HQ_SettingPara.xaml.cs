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
    /// HQ_SettingPara.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_SettingPara : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        List<itemInfo> itemList = new List<itemInfo>();
        private string isdigitReply = "1";
        private int phoneNum = 0;
        //StackPanel sp = new StackPanel();
        public bool isDisplay = true;
        public HQ_SettingPara()
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
            //if (tcall_limit.Text.Length == 0 && trecvLimit.Text.Length == 0 &&
            //           tsite0.Text.Length == 0 && tphone0.Text.Length == 0)
            //{
            //    MessageBox.Show("输入框不能均为空！");
            //}
            //else
            //{
            //    if (tcall_limit.Text.Length > 0)
            //    {
            //        itemList = "1;" + tcall_limit.Text + ";";
            //    }
            //    if (trecvLimit.Text.Length > 0)
            //    {
            //        itemList += "2;" + trecvLimit.Text + ";";
            //    }
            //    foreach (var tb in paraList.Children)
            //    {
            //        phoneNum++;
            //        StackPanel sp = (StackPanel)tb;
            //        TextBox tbx = (TextBox)sp.Children[1];
            //        string count = (phoneNum + 2).ToString() + ";";
            //        itemList += count + tbx.Text + ";";
            //    }
            //    itemList += (phoneNum + 3).ToString() + ";" + isdigitReply + ";";
            //    sendInfo("G");
            //}
            itemInfo item = new itemInfo();
            item.id = tb_key.Text;
            item.len = tb_length.Text;
            item.data = tb_content.Text;
            itemList.Add(item);
            if (phoneNum==0)
            {
                sendInfo("K");
            }
            else
            {
                for (int i = 1; i < phoneNum+1; i++)
                {
                    itemInfo additem = new itemInfo();
                    StackPanel sp = paraList.FindName("newSp" + phoneNum.ToString()) as StackPanel;
                    TextBox key = sp.FindName("tb_key" + phoneNum.ToString()) as TextBox;
                    TextBox length = sp.FindName("tb_length" + phoneNum.ToString()) as TextBox;
                    TextBox content = sp.FindName("tb_content" + phoneNum.ToString()) as TextBox;
                    additem.id = key.Text;
                    additem.len = length.Text;
                    additem.data = content.Text;
                    itemList.Add(additem);
                }
                sendInfo("K");
            }
            
        }

        private void sendInfo(string cmdtype)
        {
            Result.Text=Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype, itemList);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                Socket.ExcuteSql("设置终端参数", StaticLoginInfo.GetInstance().UserName, JsonConvert.SerializeObject(itemList), Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_HQSETTINGCMD_TYPE_" + cmdtype;
                cmd.cmdContent = "设置终端参数" + ":" + itemList;
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

        private string Texttest(string sim, string Store_checkBox_flag, string Reply_checkBox_flag, string subcmd, List<itemInfo> list)
        {
            string stritem = JsonConvert.SerializeObject(list);
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "HQSETTINGCMD_TYPE ");
            jo.Add("simId", sim);
            jo.Add("store", Store_checkBox_flag);
            jo.Add("reply", Reply_checkBox_flag);
            jo.Add("subcmd", subcmd);
            jo.Add("paramList", stritem);
            jo.Add("cmdid", sim + "_HQSETTINGCMD_TYPE_" + subcmd);
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void AddPara_Click(object sender, RoutedEventArgs e)
        {
            phoneNum++;
            StackPanel sp = new StackPanel();
            //sp.Name = "sp" + phoneNum.ToString();
            sp.Width = 470;
            sp.Orientation = Orientation.Horizontal;
            sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tbl = new TextBlock();
            tbl.Text = "参数关键字";
            tbl.Margin = new Thickness(1,5,0,5);
            tbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbl.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbx1 = new TextBox();
            //tbx1.Name = "tb_key" + phoneNum;
            tbx1.Width = 100;
            tbx1.Margin = new Thickness(0, 5, 0, 5);
            tbx1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbx1.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tb2 = new TextBlock();
            tb2.Text = "参数长度";
            tb2.Margin = new Thickness(5, 5, 0, 5);
            tb2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tb2.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbx2 = new TextBox();
            //tbx2.Name = "tb_length" + phoneNum;
            tbx2.Width = 100;
            tbx2.Margin = new Thickness(0, 5, 0, 5);
            tbx2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbx2.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tb3 = new TextBlock();
            tb3.Text = "参数内容";
            tb3.Margin = new Thickness(5, 5, 0, 5);
            tb3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tb3.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbx3 = new TextBox();
            //tbx3.Name = "tb_content" + phoneNum;
            tbx3.Width = 100;
            tbx3.Margin = new Thickness(0, 5, 0, 5);
            tbx3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbx3.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            sp.Children.Add(tbl);
            sp.Children.Add(tbx1);
            
            sp.Children.Add(tb2);
            sp.Children.Add(tbx2);
            
            sp.Children.Add(tb3);
            sp.Children.Add(tbx3);
            

            paraList.Children.Add(sp);
            paraList.RegisterName("newSp" + phoneNum.ToString(), sp);
            sp.RegisterName("tb_key" + phoneNum.ToString(), tbx1);
            sp.RegisterName("tb_length" + phoneNum.ToString(), tbx2);
            sp.RegisterName("tb_content" + phoneNum.ToString(), tbx3);
        }

        private void DeletePara_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = paraList.FindName("newSp" + phoneNum.ToString()) as StackPanel;
            //TextBox key = sp.FindName("tb_key" + phoneNum.ToString()) as TextBox;
            //TextBox length = sp.FindName("tb_length" + phoneNum.ToString()) as TextBox;
            //TextBox content = sp.FindName("tb_content" + phoneNum.ToString()) as TextBox;
            if (phoneNum==0)
            {
                MessageBox.Show("至少设置一个参数！");
                return;
            }
            if (sp!=null)
            {
                sp.UnregisterName("tb_key" + phoneNum.ToString());
                sp.UnregisterName("tb_length" + phoneNum.ToString());
                sp.UnregisterName("tb_content" + phoneNum.ToString());
                paraList.Children.Remove(sp);
                paraList.UnregisterName("newSp" + phoneNum.ToString());
                
                phoneNum--;
            }
        }

        private class itemInfo
        {
            public string id { get; set; }
            public string len { get; set; }
            public string data { get; set; }
        }
    }
}
