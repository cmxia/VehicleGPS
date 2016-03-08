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
using Newtonsoft.Json;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction.HQ
{
    /// <summary>
    /// HQ_Setting_CI.xaml 的交互逻辑
    /// </summary>
    public partial class HQ_Setting_CI : Window
    {
        private string Store_checkBox_flag = "0";
        private string Reply_checkBox_flag = "1";
        private string context1_flag;
        private string context2_flag;
        private string context3_flag="0";
        private string context4_flag;
        private string context5_flag;
        private string context6_flag;
        private string context7_flag="0";
        private string context8_flag="0";
        private string context9_flag="0";
        private string context10_flag;
        private string context11_flag;
        private string context12_flag;
        private string context13_flag;
        private string type;
        public bool isDisplay = true;
        List<HQ_Number.itemInfo> itemList = new List<HQ_Number.itemInfo>();
        public HQ_Setting_CI(string e)
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
            if ("设置终端上传后需要中心回复的项目" == e)
            {
                this.Height = Double.Parse("450");
                this.Width = Double.Parse("500");
                ck_Name1.Content = "登陆";
                ck_Name2.Content = "脱网";
                ck_Name3.Content = "劫警";
                ck_Name4.Content = "盗警";
                context_1.Content = "不需要中心确认终端信息";
                context_2.Content = "需要";
                context_3.Content = "不需要中心确认终端信息";
                context_4.Content = "需要";
                context_5.Content = "不需要中心确认终端信息";
                context_6.Content = "需要";
                context_7.Content = "不需要中心确认终端信息";
                context_8.Content = "需要";
                sp_5.Visibility = Visibility.Collapsed;
                sp_6.Visibility = Visibility.Collapsed;
                sp_7.Visibility = Visibility.Collapsed;
                sp_8.Visibility = Visibility.Collapsed;
                sp_9.Visibility = Visibility.Collapsed;
                sp_10.Visibility = Visibility.Collapsed;
                sp_11.Visibility = Visibility.Collapsed;
                sp_12.Visibility = Visibility.Collapsed;
                sp_13.Visibility = Visibility.Collapsed;
            }
        }


        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (type == "设置功能开关")
            {
                if (ck_Name1.IsChecked == false&&ck_Name2.IsChecked==false&&ck_Name3.IsChecked == false&&ck_Name4.IsChecked==false&&
                    ck_Name5.IsChecked == false&&ck_Name6.IsChecked==false&&ck_Name7.IsChecked == false&&ck_Name8.IsChecked==false&&
                    ck_Name9.IsChecked == false&&ck_Name10.IsChecked==false&&ck_Name11.IsChecked == false&&ck_Name12.IsChecked==false&&
                    ck_Name13.IsChecked == false)
                {
                    MessageBox.Show("设置不能为空！");
                    return;
                }
                if (ck_Name1.IsChecked==true)
                {
                    if (context_1.IsChecked==false&&context_2.IsChecked==false)
                    {
                        MessageBox.Show("请设置登陆信息上传！");
                        return;
                    }
                    else
                    {
                        //itemList = "1;" + context1_flag + ";";
                        HQ_Number.itemInfo item1 = new HQ_Number.itemInfo();
                        item1.key = "1";
                        item1.value = context1_flag;
                        itemList.Add(item1);
                    }
                }
                if (ck_Name2.IsChecked == true)
                {
                    if (context_3.IsChecked == false && context_4.IsChecked == false)
                    {
                        MessageBox.Show("请设置脱网信息上传！");
                        return;
                    }
                    else
                    {
                        //itemList += "2;" + context2_flag + ";";
                        HQ_Number.itemInfo item2 = new HQ_Number.itemInfo();
                        item2.key = "2";
                        item2.value = context2_flag;
                        itemList.Add(item2);
                    }
                }
                if (ck_Name3.IsChecked == true)
                {
                    //itemList += "3;" + context3_flag + ";";
                    HQ_Number.itemInfo item3 = new HQ_Number.itemInfo();
                    item3.key = "3";
                    item3.value = context3_flag;
                    itemList.Add(item3);
                }
                if (ck_Name4.IsChecked == true)
                {
                    if (context_7.IsChecked == false && context_8.IsChecked == false)
                    {
                        MessageBox.Show("请设置传感器上传功能！");
                        return;
                    }
                    else
                    {
                        //itemList += "4;" + context4_flag + ";";
                        HQ_Number.itemInfo item4 = new HQ_Number.itemInfo();
                        item4.key = "4";
                        item4.value = context4_flag;
                        itemList.Add(item4);
                    }
                }
                if (ck_Name5.IsChecked == true)
                {
                    if (context_9.IsChecked == false && context_10.IsChecked == false)
                    {
                        MessageBox.Show("请设置终端默认通信方式！");
                        return;
                    }
                    else
                    {
                        //itemList += "5;" + context5_flag + ";";
                        HQ_Number.itemInfo item5 = new HQ_Number.itemInfo();
                        item5.key = "5";
                        item5.value = context5_flag;
                        itemList.Add(item5);
                    }
                }
                if (ck_Name6.IsChecked == true)
                {
                    if (context_11.IsChecked == false && context_12.IsChecked == false)
                    {
                        MessageBox.Show("请设置是否允许短息上传信息！");
                        return;
                    }
                    else
                    {
                        //itemList += "6;" + context6_flag + ";";
                        HQ_Number.itemInfo item6 = new HQ_Number.itemInfo();
                        item6.key = "6";
                        item6.value = context6_flag;
                        itemList.Add(item6);
                    }
                }
                if (ck_Name7.IsChecked == true)
                {
                    //itemList += "7;" + context8_flag + ";";
                    HQ_Number.itemInfo item7 = new HQ_Number.itemInfo();
                    item7.key = "7";
                    item7.value = context7_flag;
                    itemList.Add(item7);
                }
                if (ck_Name8.IsChecked == true)
                {
                    //itemList += "8;" + context8_flag + ";";
                    HQ_Number.itemInfo item8 = new HQ_Number.itemInfo();
                    item8.key = "8";
                    item8.value = context8_flag;
                    itemList.Add(item8);
                }
                if (ck_Name9.IsChecked == true)
                {
                    //itemList += "9;" + context9_flag + ";";
                    HQ_Number.itemInfo item9 = new HQ_Number.itemInfo();
                    item9.key = "9";
                    item9.value = context9_flag;
                    itemList.Add(item9);
                }
                if (ck_Name10.IsChecked == true)
                {
                    if (context_19.IsChecked == false && context_20.IsChecked == false)
                    {
                        MessageBox.Show("请设置是否上传定时回传信息！");
                        return;
                    }
                    else
                    {
                        //itemList += "A;" + context10_flag + ";";
                        HQ_Number.itemInfo item10 = new HQ_Number.itemInfo();
                        item10.key = "A";
                        item10.value = context10_flag;
                        itemList.Add(item10);
                    }
                }
                if (ck_Name11.IsChecked == true)
                {
                    if (context_21.IsChecked == false && context_22.IsChecked == false)
                    {
                        MessageBox.Show("请设置是否需要语音提示！");
                        return;
                    }
                    else
                    {
                        //itemList += "B;" + context11_flag + ";";
                        HQ_Number.itemInfo item11 = new HQ_Number.itemInfo();
                        item11.key = "B";
                        item11.value = context11_flag;
                        itemList.Add(item11);
                    }
                }
                if (ck_Name12.IsChecked == true)
                {
                    if (context_23.IsChecked == false && context_24.IsChecked == false)
                    {
                        MessageBox.Show("请设置是否打开行车记录功能！");
                        return;
                    }
                    else
                    {
                        //itemList += "C;" + context12_flag + ";";
                        HQ_Number.itemInfo item12 = new HQ_Number.itemInfo();
                        item12.key = "C";
                        item12.value = context12_flag;
                        itemList.Add(item12);
                    }
                }
                if (ck_Name13.IsChecked == true)
                {
                    if (context_25.IsChecked == false && context_26.IsChecked == false&&context_27.IsChecked==false)
                    {
                        MessageBox.Show("请选择开机界面！");
                        return;
                    }
                    else
                    {
                        //itemList += "D;" + context13_flag + ";";
                        HQ_Number.itemInfo item13 = new HQ_Number.itemInfo();
                        item13.key = "D";
                        item13.value = context13_flag;
                        itemList.Add(item13);
                    }
                }
                sendinfo("I");
            }
            else
            {
                if (ck_Name1.IsChecked == false && ck_Name2.IsChecked == false && ck_Name3.IsChecked == false && ck_Name4.IsChecked == false)
                {
                    MessageBox.Show("设置不能为空！");
                    return;
                }
                if (ck_Name1.IsChecked == true)
                {
                    if (context_1.IsChecked == false && context_2.IsChecked == false)
                    {
                        MessageBox.Show("请设置登陆是否需要中心确认终端信息！");
                        return;
                    }
                    else
                    {
                        //itemList = "1;" + context1_flag + ";";
                        HQ_Number.itemInfo item1 = new HQ_Number.itemInfo();
                        item1.key = "1";
                        item1.value = context1_flag;
                        itemList.Add(item1);
                    }
                }
                if (ck_Name2.IsChecked == true)
                {
                    if (context_3.IsChecked == false && context_4.IsChecked == false)
                    {
                        MessageBox.Show("请设置脱网是否需要中心确认终端信息！");
                        return;
                    }
                    else
                    {
                        //itemList += "2;" + context2_flag + ";";
                        HQ_Number.itemInfo item2 = new HQ_Number.itemInfo();
                        item2.key = "2";
                        item2.value = context2_flag;
                        itemList.Add(item2);
                    }
                }
                if (ck_Name3.IsChecked == true)
                {
                    if (context_5.IsChecked == false && context_6.IsChecked == false)
                    {
                        MessageBox.Show("请设置劫警是否需要中心确认终端信息！");
                        return;
                    }
                    else
                    {
                        //itemList += "3;" + context3_flag + ";";
                        HQ_Number.itemInfo item3 = new HQ_Number.itemInfo();
                        item3.key = "3";
                        item3.value = context3_flag;
                        itemList.Add(item3);
                    }
                }
                if (ck_Name4.IsChecked == true)
                {
                    if (context_7.IsChecked == false && context_8.IsChecked == false)
                    {
                        MessageBox.Show("请设置盗警是否需要中心确认终端信息！");
                        return;
                    }
                    else
                    {
                        //itemList += "4;" + context4_flag + ";";
                        HQ_Number.itemInfo item4 = new HQ_Number.itemInfo();
                        item4.key = "4";
                        item4.value = context4_flag;
                        itemList.Add(item4);
                    }
                }
                sendinfo("C");
            }
        }

        private void sendinfo(string cmdtype)
        {
            Result.Text=HQ_Number.Texttest(SIM.Text, Store_checkBox_flag, Reply_checkBox_flag, cmdtype, itemList);
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

        #region radio操作
        private void Context_Info_Click1(object sender, RoutedEventArgs e)
        {
            if (context_1.IsChecked == true)
            {
                context1_flag = "0";
            }
            else
            {
                context1_flag = "1";
            }
        }

        private void Context_Info_Click2(object sender, RoutedEventArgs e)
        {
            if (context_3.IsChecked == true)
            {
                context2_flag = "0";
            }
            else
            {
                context2_flag = "1";
            }
        }

        private void Context_Info_Click3(object sender, RoutedEventArgs e)
        {
            if (context_6.IsChecked == true)
            {
                context3_flag = "1";
            }
        }

        private void Context_Info_Click4(object sender, RoutedEventArgs e)
        {
            if (context_7.IsChecked == true)
            {
                context4_flag = "0";
            }
            else
            {
                context4_flag = "1";
            }
        }

        private void Context_Info_Click5(object sender, RoutedEventArgs e)
        {
            if (context_10.IsChecked == true)
            {
                sp_6.Visibility = Visibility.Visible;
                context5_flag = "1";
            }
            else
            {
                sp_6.Visibility = Visibility.Collapsed;
                context5_flag = "0";
            }
        }

        private void Context_Info_Click6(object sender, RoutedEventArgs e)
        {
            if (context_11.IsChecked == true)
            {
                context6_flag = "0";
            }
            else
            {
                context6_flag = "1";
            }
        }

        private void Context_Info_Click7(object sender, RoutedEventArgs e)
        {
            if (context_14.IsChecked == true)
            {
                context7_flag = "1";
            }
        }

        private void Context_Info_Click8(object sender, RoutedEventArgs e)
        {
            if (context_16.IsChecked == true)
            {
                context8_flag = "1";
            }
        }

        private void Context_Info_Click9(object sender, RoutedEventArgs e)
        {
            if (context_18.IsChecked == true)
            {
                context9_flag = "1";
            }
        }

        private void Context_Info_Click10(object sender, RoutedEventArgs e)
        {
            if (context_19.IsChecked == true)
            {
                context10_flag = "0";
            }
            else
            {
                context10_flag = "1";
            }
        }

        private void Context_Info_Click11(object sender, RoutedEventArgs e)
        {
            if (context_21.IsChecked == true)
            {
                context11_flag = "0";
            }
            else
            {
                context11_flag = "1";
            }
        }

        private void Context_Info_Click12(object sender, RoutedEventArgs e)
        {
            if (context_23.IsChecked == true)
            {
                context12_flag = "0";
            }
            else
            {
                context12_flag = "1";
            }
        }

        private void Context_Info_Click13(object sender, RoutedEventArgs e)
        {
            if (context_25.IsChecked == true)
            {
                context13_flag = "0";
            }
            if (context_26.IsChecked==true)
            {
                context13_flag = "1";
            }
            else
            {
                context13_flag = "2";
            }
        }
        #endregion

        #region 控制显隐
        private void context_CheckBox_Click1(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_1.Visibility = Visibility.Visible;
                context_2.Visibility = Visibility.Visible;
            }
            else
            {
                context_1.Visibility = Visibility.Collapsed;
                context_2.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click2(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_3.Visibility = Visibility.Visible;
                context_4.Visibility = Visibility.Visible;
            }
            else
            {
                context_3.Visibility = Visibility.Collapsed;
                context_4.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click3(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_5.Visibility = Visibility.Visible;
                context_6.Visibility = Visibility.Visible;
                context_5.IsChecked = true;
                if (type == "设置终端上传后需要中心回复的项目")
                {
                    context_5.IsChecked = false;
                }
                
            }
            else
            {
                context_5.Visibility = Visibility.Collapsed;
                context_6.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click4(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_7.Visibility = Visibility.Visible;
                context_8.Visibility = Visibility.Visible;
            }
            else
            {
                context_7.Visibility = Visibility.Collapsed;
                context_8.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click5(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_9.Visibility = Visibility.Visible;
                context_10.Visibility = Visibility.Visible;
            }
            else
            {
                context_9.Visibility = Visibility.Collapsed;
                context_10.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click6(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_11.Visibility = Visibility.Visible;
                context_12.Visibility = Visibility.Visible;
            }
            else
            {
                context_11.Visibility = Visibility.Collapsed;
                context_12.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click7(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_13.Visibility = Visibility.Visible;
                context_14.Visibility = Visibility.Visible;
                context_13.IsChecked = true;
            }
            else
            {
                context_13.Visibility = Visibility.Collapsed;
                context_14.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click8(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_15.Visibility = Visibility.Visible;
                context_16.Visibility = Visibility.Visible;
                context_15.IsChecked = true;
            }
            else
            {
                context_15.Visibility = Visibility.Collapsed;
                context_16.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click9(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_17.Visibility = Visibility.Visible;
                context_18.Visibility = Visibility.Visible;
                context_17.IsChecked = true;
            }
            else
            {
                context_17.Visibility = Visibility.Collapsed;
                context_18.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click10(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_19.Visibility = Visibility.Visible;
                context_20.Visibility = Visibility.Visible;
            }
            else
            {
                context_19.Visibility = Visibility.Collapsed;
                context_20.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click11(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_21.Visibility = Visibility.Visible;
                context_22.Visibility = Visibility.Visible;
            }
            else
            {
                context_21.Visibility = Visibility.Collapsed;
                context_22.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click12(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_23.Visibility = Visibility.Visible;
                context_24.Visibility = Visibility.Visible;
            }
            else
            {
                context_23.Visibility = Visibility.Collapsed;
                context_24.Visibility = Visibility.Collapsed;
            }
        }
        private void context_CheckBox_Click13(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                //context_checkBox_flag = "1";
                context_25.Visibility = Visibility.Visible;
                context_26.Visibility = Visibility.Visible;
                context_27.Visibility = Visibility.Visible;
            }
            else
            {
                context_25.Visibility = Visibility.Collapsed;
                context_26.Visibility = Visibility.Collapsed;
                context_27.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

    }
}
